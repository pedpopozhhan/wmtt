using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Validator;
using AutoMapper;
using WCDS.WebFuncions.Core.Services;
using System.Linq;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp;
using System.Text;
using System.Net;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace WCDS.WebFuncions
{
    public class InvoiceStatusSyncMessageHandler
    {
        private readonly ILogger _log;
        public InvoiceStatusSyncMessageHandler(ILogger log)
        {
            _log = log;
        }
        public async Task SendInvoiceStatusSyncMessage(InvoiceStatusSyncMessageDto data, string invoiceNumber)
        {
            ServiceBusClient client = null;
            ServiceBusSender sender = null;
            try
            {
                client = new ServiceBusClient(Environment.GetEnvironmentVariable("InvoiceStatusSyncTopicConnectionString"), new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets });
                sender = client.CreateSender(Environment.GetEnvironmentVariable("InvoiceStatusSyncTopicName"));
                _log.LogInformation("SendInvoiceStatusSyncMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                var message = new ServiceBusMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = "",
                    Body = new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)))
                };

                _log.LogDebug("SendInvoiceStatusSyncMessage - Message: {0}", message.MessageId + "::" + message.Body);
                
                const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _log.LogError("SendInvoiceStatusSyncMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                throw new Exception("SendInvoiceStatusSyncMessage - Error Sending a message: " + ex.Message + ex.InnerException);
            }
            finally
            {
                if (sender != null) await sender.DisposeAsync();
                if (client != null) await client.DisposeAsync();
            }

            _log.LogInformation("SendInvoiceStatusSyncMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
        }

    }
}

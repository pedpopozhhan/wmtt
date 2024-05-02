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
    public class InvoiceDataSyncMessageHandler
    {
        private readonly ILogger _log;
        public InvoiceDataSyncMessageHandler(ILogger log)
        {
            _log = log;
        }

        public async Task SendCreateInvoiceMessage(InvoiceDataSyncMessageDto invoice, string invoiceNumber)
        {
            ServiceBusClient client = null;
            ServiceBusSender sender = null;
            try
            {
                client = new ServiceBusClient(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicConnectionString"), new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets });
                sender = client.CreateSender(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicName"));
                _log.LogInformation("SendCreateInvoiceMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                _log.LogError("SendCreateInvoiceMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                _log.LogDebug("SendCreateInvoiceMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                var message = new ServiceBusMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = "insert",
                    Body = new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(invoice)))
                };

                _log.LogInformation("SendCreateInvoiceMessage - Message: {0}", message.MessageId + "::" + message.Body);
                _log.LogError("SendCreateInvoiceMessage - Message: {0}", message.MessageId + "::" + message.Body);
                _log.LogDebug("SendCreateInvoiceMessage - Message: {0}", message.MessageId + "::" + message.Body);

                const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _log.LogInformation("SendCreateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                _log.LogError("SendCreateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                _log.LogDebug("SendCreateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                throw new Exception("SendCreateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
            }
            finally
            {
                if(sender != null) await sender.DisposeAsync();
                if(client != null) await client.DisposeAsync();                
            }

            _log.LogInformation("SendCreateInvoiceMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
            _log.LogError("SendCreateInvoiceMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
            _log.LogDebug("SendCreateInvoiceMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
        }

        public async Task SendUpdateInvoiceMessage(InvoiceDataSyncMessageDto invoice, string invoiceNumber)
        {
            ServiceBusClient client = null;
            ServiceBusSender sender = null;
            try
            {
                client = new ServiceBusClient(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicConnectionString"), new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets });
                sender = client.CreateSender(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicName"));
                _log.LogInformation("SendUpdateInvoiceMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                _log.LogError("SendUpdateInvoiceMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                _log.LogDebug("SendUpdateInvoiceMessage - started at:  {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
                var message = new ServiceBusMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = "update",
                    Body = new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(invoice)))
                };

                _log.LogInformation("SendUpdateInvoiceMessage - Message: {0}", message.MessageId + "::" + message.Body);
                _log.LogError("SendUpdateInvoiceMessage - Message: {0}", message.MessageId + "::" + message.Body);
                _log.LogDebug("SendUpdateInvoiceMessage - Message: {0}", message.MessageId + "::" + message.Body);

                const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _log.LogInformation("SendUpdateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                _log.LogError("SendUpdateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                _log.LogDebug("SendUpdateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
                throw new Exception("SendUpdateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
            }
            finally
            {
                if (sender != null) await sender.DisposeAsync();
                if (client != null) await client.DisposeAsync();
            }

            _log.LogInformation("SendUpdateInvoiceMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
            _log.LogError("SendUpdateInvoiceMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
            _log.LogDebug("SendUpdateInvoiceMessage - finished at: {0} for invoice {1}", DateTime.UtcNow, invoiceNumber);
        }

    }
}

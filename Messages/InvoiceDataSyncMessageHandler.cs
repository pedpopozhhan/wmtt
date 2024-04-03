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
        public async Task SendCreateInvoiceMessage(InvoiceDataSyncMessageDto invoice)
        {
            ServiceBusClient client = null;
            ServiceBusSender sender = null;
            try
            {
                client = new ServiceBusClient(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicConnectionString"), new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets });
                sender = client.CreateSender(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicName"));
                var message = new ServiceBusMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = "insert",
                    Body = new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(invoice)))
                };

                const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception("SendCreateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
            }
            finally
            {
                if(sender != null) await sender.DisposeAsync();
                if(client != null) await client.DisposeAsync();
            }
        }

        public async Task SendUpdateInvoiceMessage(InvoiceDataSyncMessageDto invoice)
        {
            ServiceBusClient client = null;
            ServiceBusSender sender = null;
            try
            {
                client = new ServiceBusClient(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicConnectionString"), new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets });
                sender = client.CreateSender(Environment.GetEnvironmentVariable("InvoiceDataSyncTopicName"));
                var message = new ServiceBusMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = "update",
                    Body = new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(invoice)))
                };

                const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception("SendUpdateInvoiceMessage - Error Sending a message: " + ex.Message + ex.InnerException);
            }
            finally
            {
                if (sender != null) await sender.DisposeAsync();
                if (client != null) await client.DisposeAsync();
            }
        }

    }
}
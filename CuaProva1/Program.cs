using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using CuaShared;

namespace CuaProva1
{
    public static class MessageSenderExtension
    {
        /// <summary>
        /// Converteix l'objecte a Json i l'envia a la cua
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cua"></param>
        /// <param name="objectToAdd">Objecte a convertir</param>
        /// <param name="caducitat">Opcional. Quan caduca</param>
        /// <returns></returns>
        public static async Task SendMessageAsJsonAsync<T>(this ServiceBusSender cua, string table, T objectToAdd)
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(objectToAdd)))
            {
                ContentType = "application/json",
                Subject = table,                
            };

            message.ApplicationProperties.Add("accio", Shared.GetRandomAction());

            await cua.SendMessageAsync(message);
        }
    }

    class Program
    {

        static async Task SendMessageAsync(string connection, string queueName)
        {
            await using var client = new ServiceBusClient(connection);
            ServiceBusSender sender = client.CreateSender(queueName);
            await sender.SendMessageAsJsonAsync(Shared.usuaris[0].Item1, Shared.usuaris[0].Item2);
        }

        static Queue<ServiceBusMessage> CreateMessages()
        {
            Queue<ServiceBusMessage> messages = new Queue<ServiceBusMessage>();
            foreach (var usuari in Shared.usuaris)
            {
                var usuariJson = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(usuari.Item2));
                var message = new ServiceBusMessage(usuariJson)
                {                    
                    ContentType = "application/json",
                    Subject = usuari.Item1
                };
                message.ApplicationProperties.Add("accio", Shared.GetRandomAction());
                messages.Enqueue(message);
            }
            return messages;
        }

        static async Task SendMessageBatchAsync(string connection, string queueName)
        {
            await using ServiceBusClient client = new ServiceBusClient(connection);
            ServiceBusSender sender = client.CreateSender(queueName);
            Queue<ServiceBusMessage> messages = CreateMessages();
            int messageCount = messages.Count;

            while (messages.Count > 0)
            {
                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                if (messageBatch.TryAddMessage(messages.Peek()))
                {
                    messages.Dequeue();
                }
                else
                {
                    throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
                }

                while (messages.Count > 0 && messageBatch.TryAddMessage(messages.Peek()))
                {
                    messages.Dequeue();
                }
                await sender.SendMessagesAsync(messageBatch);
            }
        }

        /// <summary>
        /// Enviar missatges a la cua
        /// </summary>
        /// <returns></returns>
        static async Task Main()
        {
            var (connection, queueName) = Shared.GetUserSecrets();

            // Un sol missatge
            await SendMessageAsync(connection, queueName);

            // Un grup de missatges
            await SendMessageBatchAsync(connection, queueName);
        }
    }
}

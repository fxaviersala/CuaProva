using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using CuaShared;
using Microsoft.Azure.ServiceBus;

namespace CuaProva1Rebre
{
    class Program
    {
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = args.Message.Body;

            Usuari usuari = JsonSerializer.Deserialize<Usuari>(Encoding.UTF8.GetString(body));            

            Console.WriteLine($"{message.ApplicationProperties["accio"]} de la taula {message.Subject} --> {usuari.Nom} {usuari.Cognom}");            

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }


        static async Task ReceiveMessagesAsync(string cua)
        {
            var options = new ServiceBusProcessorOptions
            {
                // AutoCompleteMessages = false,
                // MaxConcurrentCalls = 20                
            };
            ServiceBusProcessor processor = client.CreateProcessor(cua, options);

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();

            Console.WriteLine("Enter per acabar");
            Console.ReadKey();

            Console.WriteLine("Stop receiving");
            await processor.StopProcessingAsync();
            Console.WriteLine("processor killed");
        }

        private static async Task ReceiveDeadLetterMessages(string queueName)
        {
            ServiceBusReceivedMessage receivedMessage;

            var deadLetterPath = EntityNameHelper.FormatDeadLetterPath(queueName);
            var receiver = client.CreateReceiver(deadLetterPath);
            do
            {
                receivedMessage = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
                if (receivedMessage == null)
                {
                    continue;
                }
                Console.WriteLine($"{receivedMessage.Body} : {receivedMessage.DeadLetterReason} {receivedMessage.DeadLetterErrorDescription}");
                await receiver.CompleteMessageAsync(receivedMessage);

            } while (receivedMessage != null);

        }

        private static ServiceBusClient client;
        public static async Task Main()
        {
            var (connection, queueName) = Shared.GetUserSecrets();

            client = new ServiceBusClient(connection);

            await ReceiveMessagesAsync(queueName);


            // Obtenir la cua Dead-Letter
            lock (Console.Out)
            {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("--- DEAD LETTER --- ");
                    Console.ResetColor();
            }

            await ReceiveDeadLetterMessages(queueName);
            await client.DisposeAsync();
        }


    }
}

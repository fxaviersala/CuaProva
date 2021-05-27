using System;
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
            var body = args.Message.Body.ToString();
            Console.WriteLine($"{message.ApplicationProperties["accio"]} de la taula {message.Subject} --> {body}");            

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

        private static async Task ReceiveDeadLetterMessages()
        {
            ServiceBusReceivedMessage receivedMessage;

            var deadLetterPath = EntityNameHelper.FormatDeadLetterPath(Shared.QueueName);
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
            client = new ServiceBusClient(Shared.ConnectionString);

            await ReceiveMessagesAsync(Shared.QueueName);


            // Obtenir la cua Dead-Letter
            lock (Console.Out)
            {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("--- DEAD LETTER --- ");
                    Console.ResetColor();
            }

            await ReceiveDeadLetterMessages();
            await client.DisposeAsync();
        }


    }
}

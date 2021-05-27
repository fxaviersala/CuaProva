using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using CuaShared;

namespace CuaProva2Rebre
{
    public class Program
    {
        
        QueueClient receiveClient;

        public async Task Run(string connection, string queuename)
        {

            // Preview
            await this.PeekMessagesAsync(connection, queuename);
            // Get

            this.receiveClient = new QueueClient(connection, queuename, ReceiveMode.PeekLock);
            this.InitializeReceiver();

            await Task.WhenAny(
                Task.Run(() => Console.ReadKey()),
                Task.Delay(TimeSpan.FromSeconds(10))
            );

            await this.receiveClient.CloseAsync();
        }

        private Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {0}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }

        private void InitializeReceiver()
        {

            receiveClient.RegisterMessageHandler(
                async (message, cancellationToken) =>
                {
                    if (message.Label != null &&
                        message.ContentType != null &&                        
                        message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var body = message.Body;

                        Usuari usuari = JsonConvert.DeserializeObject<Usuari>(Encoding.UTF8.GetString(body));

                        lock (Console.Out)
                        {
                            Console.WriteLine($"{message.MessageId}/{message.SystemProperties.SequenceNumber} -  {message.UserProperties["accio"]} de la taula {message.Label} : {usuari.Nom} {usuari.Cognom}");                            
                        }
                    }
                    await receiveClient.CompleteAsync(message.SystemProperties.LockToken);
                },
                new MessageHandlerOptions((e) => LogMessageHandlerException(e)) { AutoComplete = false, MaxConcurrentCalls = 1 });

        }



        async Task PeekMessagesAsync(string connectionString, string queueName)
        {
            var receiver = new MessageReceiver(connectionString, queueName, ReceiveMode.PeekLock);

            Console.WriteLine("Veure missatges de la cua ...");
            while (true)
            {
                try
                {

                    var message = await receiver.PeekAsync();
                    if (message != null)
                    {
                        // print the message
                        var body = Encoding.UTF8.GetString(message.Body);
                        lock (Console.Out)
                        {
                            Console.WriteLine($"{message.MessageId} - {message.SystemProperties.SequenceNumber} - {message.ContentType} - {message.Label}");
                        }
                    }
                    else
                    {
                        // end.
                        break;
                    }
                }
                catch (ServiceBusException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
            }

            Console.WriteLine("----------------- Missatges acabats ----------------");
            await receiver.CloseAsync();
        }

        public static async Task<int> Main(string[] args)
        {
            var (connection, queueName) = Shared.GetUserSecrets();

            try
            {
                var app = new Program();
                await app.Run(connection, queueName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
            return 0;
        }
    }
}



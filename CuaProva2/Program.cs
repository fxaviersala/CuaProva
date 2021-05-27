using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using CuaShared;

namespace CuaProva2
{
    public class Program
    {
        public async Task Run()
        {
            await SendJsonMessagesAsync(Shared.ConnectionString, Shared.QueueName);
        }

        async Task SendJsonMessagesAsync(string connectionString, string queueName)
        {
            var sender = new MessageSender(connectionString, queueName);

            Console.WriteLine("Enviant ...");

            // send a message for each entry in the above array
            for (int i = 0; i < Shared.usuaris.Length; i++)
            {
                await sender.SendMessageAsJsonAsync(
                    Shared.usuaris[i].Item1,
                    i.ToString(),
                    Shared.usuaris[i].Item2,
                    TimeSpan.FromMinutes(10));

                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enviat: Id = {0}", i.ToString());
                    Console.ResetColor();
                }
            }
        }


        public static async Task<int> Main(string[] args)
        {
            try
            {
                var app = new Program();
                await app.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
            return 0;
        }
    }



    public static class MessageSenderExtension
    {
        /// <summary>
        /// Converteix l'objecte a Json i l'envia a la cua
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cua"></param>
        /// <param name="label">Etiqueta</param>
        /// <param name="id">Identificador</param>
        /// <param name="objectToAdd">Objecte a convertir</param>
        /// <param name="caducitat">Opcional. Quan caduca</param>
        /// <returns></returns>
        public static async Task SendMessageAsJsonAsync<T>(this MessageSender cua, string label, string id, T objectToAdd, TimeSpan? caducitat = null)
        {

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objectToAdd)))
            {
                ContentType = "application/json",
                Label = label,
                MessageId = id,
                TimeToLive = caducitat ?? new TimeSpan(0, 0, 0, 0, -1),                
            };

            message.UserProperties.Add("accio", Shared.GetRandomAction());

            await cua.SendAsync(message);
        }
    }
}



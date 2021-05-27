using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CuaShared
{
    public class Usuari
    {
        public string Nom { get; set; }
        public string Cognom { get; set; }
    }

    public static class Shared
    {
        static Random random;
        static Shared()
        {
            random = new Random();
        }

        public const string ConnectionString = "Endpoint=sb://cuaproves.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=u0k+SNS2Oi89NCAlioH9NU9zKaVUZ8yGzNZvtIv0s/8=";
        public const string QueueName = "fantastiqueue";
        
        private static string[] Accions = new[] { "create", "update", "remove" };
        public static string GetRandomAction()
        {
            return Accions[random.Next(Accions.Length)];
        }


        public static (string, Usuari)[] usuaris = new (string tipus, Usuari usuari)[]
        {
            ("usuari", new Usuari {Nom = "Frederic", Cognom = "Pi"}),
            ("usuari", new Usuari {Nom = "Filomenu", Cognom = "Garcia"}),
            ("usuari", new Usuari {Nom = "Manel", Cognom = "Puig"}),
            ("client", new Usuari {Nom = "Mohamed", Cognom = "Rius"}),
            ("usuari", new Usuari {Nom = "Newton", Cognom = "González"}),
            ("usuari", new Usuari {Nom = "Bernat", Cognom = "Puigpelat"}),
            ("client", new Usuari {Nom = "Ignasi", Cognom = "Roura"}),
            ("usuari", new Usuari {Nom = "Maria", Cognom = "Pinta"}),
            ("usuari", new Usuari {Nom = "Ramona", Cognom = "Reig"}),
            ("usuari", new Usuari {Nom = "Maria de la Asunción", Cognom = "Serra"})
        };

        public static (string connection, string queueName) GetUserSecrets()
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";
            //Determines the working environment as IHostingEnvironment is unavailable in a console app

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<SecretStuff>()
                .Build();
            


            var secret = configuration.GetSection("SecretStuff");
                        
            return (secret["ConnectionString"], secret["QueueName"]);
        }

        
    }
}

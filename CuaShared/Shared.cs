using System;

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

        public const string ConnectionString = "Endpoint=sb://proves.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bSINdC+fyIZd/xT+FBYoZLBb56OChR1Psb00PPjTM0g=";
        public const string QueueName = "entra";
        
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

        
    }
}

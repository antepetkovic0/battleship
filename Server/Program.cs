using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = (AkkaConfigurationSection)ConfigurationManager.GetSection("akka");
            var port = args.Length == 0 ? 0 : int.Parse(args[0]);
            var fullConfig = ConfigurationFactory.ParseString($"akka.remote.dot-netty.port={port}").WithFallback(config.AkkaConfig);

            using (var system = ActorSystem.Create("ClusterSystem", fullConfig))
            {
                Props props = Props.Create(() => new ServerActor());
                system.ActorOf(props, "Server");

                Console.ReadKey();
                system.WhenTerminated.Wait();
            }
        }
    }
}

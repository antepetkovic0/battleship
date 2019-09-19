using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using SharedMess;

namespace Server
{
    class ServerActor : ReceiveActor
    {
        List<IActorRef> actors;
        private Cluster cluster = Cluster.Get(Context.System);

        public ServerActor()
        {
            Console.WriteLine($"{Self.Path.Name} is up... {Self.Path}");

            actors = new List<IActorRef>();

            Receive<ClusterEvent.MemberUp>(x => HandleMemberUp(x));
            Receive<ActorIdentity>(x => HandleActorUp());

            Receive<Lobby>(x => WaitingForPlayers());
        }

        private void WaitingForPlayers()
        {
            if (actors.Count >= 2)
            {
                var player1 = actors[0];
                var player2 = actors[1];
                player1.Tell(new NewPlayer(1), player2);
                player2.Tell(new NewPlayer(2), player1);

                Become(Players);
            }
        }

        private void Players()
        {
            int k = 0;

            Console.WriteLine($"{actors.Count} players connected...");
            foreach (var el in actors)
            {
                Console.WriteLine($"{k} => {el.Path}");
                k++;
            }
            Console.WriteLine("GAME CAN START");
        }

        private void HandleActorUp()
        {
            if (Sender.Path.Name != "Server")
            {
                actors.Add(Sender);
            }               
        }

        private void HandleMemberUp(ClusterEvent.MemberUp x)
        {
            var rootPath = new RootActorPath(x.Member.Address);
            var selection = Context.ActorSelection($"{rootPath}/user/*");

            selection.Tell(new Identify("1"));
        }

        protected override void PreStart()
        {
            cluster.Subscribe(Self,
                new[] {
                    typeof(ClusterEvent.IMemberEvent),
                    typeof(ClusterEvent.UnreachableMember)
                });
            Context.System.Scheduler.ScheduleTellRepeatedly(1000, 2000, Self, new Lobby(), Self);
            
            base.PreStart();
        }

        protected override void PostStop()
        {
            cluster.Unsubscribe(Self);
            base.PostStop();
        }
    }
}

using System;
using Akka.Actor;


namespace Heartbeat
{
    namespace Msg
    {
        public class Heartbeat 
        {}

        public class SetInterval 
        {
            public int Interval { get; private set; }

            public SetInterval(int interval)
            {
                Interval = interval;
            }
        }

        public class SetActive 
        {
            public bool Active { get; private set; }

            public SetActive(bool active)
            {
                Active = active;
                
            }
        }
    }

    public class HeartbeatActor : ReceiveActor
    {
        protected IScheduler Scheduler { get; private set; }

        protected int Interval { get; private set; }

        protected bool Active { get; private set; }

        private readonly Msg.Heartbeat heartBeat = new Msg.Heartbeat();

        public HeartbeatActor(IScheduler scheduler)
        {
            Scheduler = scheduler;

            Receive<Msg.Heartbeat>(_ => 
            {
                if (Active)
                {
                    Execute();
                    ScheduleNextHeartbeat();
                }
            });

            Receive<Msg.SetActive>(m => 
            {
                Active = m.Active;
                Self.Tell(heartBeat);
            });

            Receive<Msg.SetInterval>(m => Interval = m.Interval);
        }

        protected virtual void Execute()
        {
            Console.Write(".");
        }

        void ScheduleNextHeartbeat()
        {
            Scheduler.ScheduleTellOnce(Interval, Self, heartBeat, Self);
        }
    }

    namespace Msg
    {
        public class HeartbeatTx {}
    }

    public class HeartbeatRxActor : ReceiveActor
    {
        public HeartbeatRxActor()
        {
            Receive<Msg.HeartbeatTx>(m => Console.Write("!"));
        }
    }

    public class HeartbeatTxActor : HeartbeatActor
    {
        IActorRef recipient;

        readonly Msg.HeartbeatTx tx = new Msg.HeartbeatTx();

        public HeartbeatTxActor(IScheduler scheduler, IActorRef recipient) : base(scheduler)
        {
            this.recipient = recipient;
        }

        protected override void Execute()
        {
            base.Execute();
            recipient.Tell(tx);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("AppRoot");
            var scheduler = system.Scheduler;
            var receiver = system.ActorOf(Props.Create(() => new HeartbeatRxActor()));
            var heartBeat = system.ActorOf(Props.Create(() => new HeartbeatTxActor(scheduler, receiver)));
            heartBeat.Tell(new Msg.SetInterval(500));
            scheduler.ScheduleTellOnce(new TimeSpan(0,0,0,10), heartBeat, new Msg.SetActive(false), null);
            heartBeat.Tell(new Msg.SetActive(true));

            Console.WriteLine("Hello World!");

            Console.ReadKey();
            //system.WhenTerminated.Wait();
        }
    }
}

using System;
using Akka.Actor;

namespace WinTail
{
    public class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";

        private readonly IActorRef consoleWriter;

        public ConsoleReaderActor(IActorRef consoleWriter)
        {
            this.consoleWriter = consoleWriter;
        }

        protected override void OnReceive(object message)
        {
            Console.WriteLine($"Reader received:{message?.ToString()}");

            var read = Console.ReadLine();
            if (!string.IsNullOrEmpty(read) && String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                Context.System.Terminate();
                
                return;
            }

            // send input to the console writer to process and print
            consoleWriter.Tell(read);

            // continue reading messages from the console
            Self.Tell("continue");
        }
    }
}
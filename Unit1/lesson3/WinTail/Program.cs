using System;
using Akka.Actor;

namespace WinTail
{
    class Program
    {
        public static ActorSystem TheSystem;
        static void Main(string[] args)
        {
            PrintInstructions();

            TheSystem = ActorSystem.Create("AppRoot");

            Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
            IActorRef consoleWriter = TheSystem.ActorOf(consoleWriterProps, "ConsoleWriter");

            Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriter));
            IActorRef validation = TheSystem.ActorOf(validationActorProps);

            Props consoleReaderProps = Props.Create(() => new ConsoleReaderActor(validation));
            IActorRef consoleReader = TheSystem.ActorOf(consoleReaderProps, "ConsoleReader");

            consoleReader.Tell(ConsoleReaderActor.StartCommand);

            TheSystem.WhenTerminated.Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }
    }
}

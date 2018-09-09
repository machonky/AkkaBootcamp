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

            IActorRef consoleWriter = TheSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            IActorRef consoleReader = TheSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriter)));

            consoleReader.Tell("start");

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

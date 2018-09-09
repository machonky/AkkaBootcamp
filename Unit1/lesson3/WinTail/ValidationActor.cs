using System;
using Akka.Actor;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriter;

        public ValidationActor(IActorRef consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input
                _consoleWriter.Tell(
                  new Messages.NullInputError("No input received."));
            }
            else
            {
                if (IsValid(msg))
                {
                    // send success to console writer
                    _consoleWriter.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    // signal that input was bad
                    _consoleWriter.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            // tell sender to continue doing its thing
            // (whatever that may be, this actor doesn't care)
            Sender.Tell(new Messages.ContinueProcessing());            
        }

        /// <summary>
        /// Determines if the message received is valid.
        /// Checks if number of chars in message received is even.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static bool IsValid(string msg)
        {
            var valid = msg.Length % 2 == 0;
            return valid;
        }
    }
}
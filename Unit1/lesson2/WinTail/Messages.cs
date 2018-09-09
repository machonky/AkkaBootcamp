using System;

namespace Messages
{
    public class ContinueProcessing
    {}

    public class InputSuccess
    {
        public string Reason { get;private set;}
        public InputSuccess(string reason)
        {
            Reason = reason;
        }
    }

    public class InputError
    {
        protected InputError(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; private set; }
    }

    public class NullInputError : InputError
    {
        public NullInputError(string reason) : base(reason)
        {}
    }

    public class ValidationError : InputError
    {
        public ValidationError(string reason) : base(reason)
        {}
    }
}
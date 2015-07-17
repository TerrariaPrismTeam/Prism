using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Prism
{
    [Serializable]
    public class RethrownException : Exception
    {
        readonly static string EXN_MSG = "An exception was rethrown";

        public RethrownException()
            : this(EXN_MSG + ".", null)
        {

        }
        public RethrownException(string message)
            : this(message, null)
        {

        }
        public RethrownException(Exception inner)
            : this(EXN_MSG + ": " + inner.Message, inner)
        {

        }
        public RethrownException(string message, Exception inner)
            : base(message, GetRealInner(inner))
        {

        }

        protected RethrownException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        static Exception GetRealInner(Exception inner)
        {
            Debugger.Break();

            if (inner is RethrownException)
                return inner.InnerException;

            return inner;
        }
    }
}

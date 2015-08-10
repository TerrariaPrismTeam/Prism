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

        [DebuggerStepThrough]
        public RethrownException()
            : this(EXN_MSG + ".", null)
        {

        }
        [DebuggerStepThrough]
        public RethrownException(string message)
            : this(message, null)
        {

        }
        [DebuggerStepThrough]
        public RethrownException(Exception inner)
            : this(EXN_MSG + ": " + inner.Message, inner)
        {

        }
        [DebuggerStepThrough]
        public RethrownException(string message, Exception inner)
            : base(message, GetRealInner(inner))
        {

        }
        [DebuggerStepThrough]
        protected RethrownException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        [DebuggerStepThrough]
        static Exception GetRealInner(Exception inner)
        {
            Debug.WriteLine("Uncaught exception is thrown!\n" + inner);
            Debugger.Break();

            if (inner is RethrownException)
                return inner.InnerException;

            return inner;
        }
    }
}

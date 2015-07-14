using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Injector;

namespace Prism
{
    static class Program
    {
        static void Main(string[] args)
        {
            InjectionExample.Load();

            InjectionExample.Example  ();
            InjectionExample.Publicify();

            InjectionExample.Emit();
        }
    }
}

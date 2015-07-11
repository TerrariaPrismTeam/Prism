using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Injector.ILAsm
{
    public class ILProvider : CodeDomProvider
    {
        readonly static string EXT = ".il";

        public override string FileExtension
        {
            get
            {
                return EXT;
            }
        }
        public override LanguageOptions LanguageOptions
        {
            get
            {
                return 0;
            }
        }

        [Obsolete]
        public override ICodeCompiler CreateCompiler()
        {
            return new ILCompiler();
        }
        [Obsolete]
        public override ICodeGenerator CreateGenerator()
        {
            throw new NotImplementedException();
        }
    }
}

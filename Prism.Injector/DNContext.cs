using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

using TypeAttributes = dnlib.DotNet.TypeAttributes;

namespace Prism.Injector
{
    struct AsmInfo : IEquatable<AsmInfo>
    {
        readonly static string MODULE = "<Module>";
        readonly static string COMPILER_GENERATED = typeof(CompilerGeneratedAttribute).FullName;

        public readonly AssemblyDef assembly;
        public readonly List<TypeDef> types;

        public AsmInfo(AssemblyDef def)
        {
            assembly = def;

            types = def.Modules.Select(m =>
            {
                var t = FilterCompilerGenerated(m.Types);

                return t.SafeConcat(t.Select(GetNestedTypesRec).Flatten());
            }).Flatten().OrderBy(td => td.FullName).ToList();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AsmInfo))
                return false;

            return Equals((AsmInfo)obj);
        }
        public override int GetHashCode()
        {
            return assembly.GetHashCode() | types.GetHashCode();
        }
        public override string ToString()
        {
            return assembly.ToString();
        }

        public bool Equals(AsmInfo other)
        {
            return assembly == other.assembly && types == other.types;
        }

        static IEnumerable<TypeDef> FilterCompilerGenerated(IEnumerable<TypeDef> coll)
        {
            return coll.Where(td => (td.Attributes & (TypeAttributes.RTSpecialName | TypeAttributes.SpecialName)) == 0
                    && td.Name != MODULE && !td.CustomAttributes.Any(ca => ca.AttributeType.FullName == COMPILER_GENERATED));
        }
        static IEnumerable<TypeDef> GetNestedTypesRec(TypeDef d)
        {
            if (!d.HasNestedTypes)
                return null;

            var n = FilterCompilerGenerated(d.NestedTypes);
            return n.SafeConcat(n.Select(GetNestedTypesRec).Flatten());
        }

        public static bool operator ==(AsmInfo a, AsmInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(AsmInfo a, AsmInfo b)
        {
            return a.assembly != b.assembly || a.types != b.types;
        }
    }

    public class DNContext
    {
        internal AsmInfo primaryAssembly;
        Assembly reflectionOnlyAsm;

        public readonly SigComparer          SigComparer;
        public readonly DNReflectionComparer RefComparer;

        public AssemblyDef PrimaryAssembly
        {
            get
            {
                return primaryAssembly.assembly;
            }
        }

        public AssemblyNameInfo[] References
        {
            //get
            //{
            //    return referencedAssemblies;
            //}
            get;
            private set;
        }
        
        public MemberResolver Resolver
        {
            get;
            private set;
        }

        public DNContext(string asmToLoad)
        {
            var pa = AssemblyDef.Load(asmToLoad);

            reflectionOnlyAsm = Assembly.ReflectionOnlyLoadFrom(asmToLoad);

            var refs = reflectionOnlyAsm.GetReferencedAssemblies();
            References = refs.Select(r => new AssemblyNameInfo(r)).ToArray();

            //stdLibAsms = refs.Where(n =>
            //{
            //    try
            //    {
            //        return Assembly.ReflectionOnlyLoad(n.FullName).GlobalAssemblyCache;
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}).Select(TranslateReference).Select(n => new AsmInfo(pa.MainModule.AssemblyResolver.Resolve(n))).ToList();

            primaryAssembly = new AsmInfo(pa); // load types after stdlib/gac references are loaded

            SigComparer = new SigComparer(SigComparerOptions.PrivateScopeIsComparable);
            RefComparer = new DNReflectionComparer(this);
            Resolver    = new MemberResolver      (this);
        }
    }
}

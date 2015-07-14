using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Collections.Generic;

using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Prism.Injector
{
    public class CecilContext
    {
        struct AsmInfo
        {
            public AssemblyDefinition assembly;
            public List<TypeDefinition> types;

            public AsmInfo(AssemblyDefinition def)
            {
                assembly = def;

                types = def.Modules.Select(m =>
                {
                    var t = FilterCompilerGenerated(m.Types);

                    return t.SafeConcat(t.Select(GetNestedTypesRec).Flatten());
                }).Flatten().OrderBy(td => td.FullName).ToList();
            }

            public override string ToString()
            {
                return assembly.ToString();
            }

            static IEnumerable<TypeDefinition> FilterCompilerGenerated(IEnumerable<TypeDefinition> coll)
            {
                return coll.Where(td => (td.Attributes & (TypeAttributes.RTSpecialName | TypeAttributes.SpecialName)) == 0
                        && td.Name != MODULE && !td.CustomAttributes.Any(ca => ca.AttributeType.FullName == COMPILER_GENERATED));
            }

            static IEnumerable<TypeDefinition> GetNestedTypesRec(TypeDefinition d)
            {
                if (!d.HasNestedTypes)
                    return null;

                var n = FilterCompilerGenerated(d.NestedTypes);
                return n.SafeConcat(n.Select(GetNestedTypesRec).Flatten());
            }
        }

        readonly static string MODULE = "<Module>";
        readonly static string COMPILER_GENERATED = typeof(CompilerGeneratedAttribute).FullName;

        AsmInfo primaryAssembly;
        Assembly reflectionOnlyAsm;

        List<AsmInfo> stdLibAsms = new List<AsmInfo>();

        List<AssemblyNameReference> referencedAssemblies;
        List<TypeReference> loadedRefTypes = new List<TypeReference>();

        public AssemblyDefinition PrimaryAssembly
        {
            get
            {
                return primaryAssembly.assembly;
            }
        }
        public IEnumerable<AssemblyNameReference> References
        {
            get
            {
                return referencedAssemblies;
            }
        }
        public IEnumerable<AssemblyDefinition> StdLibReferences
        {
            get
            {
                return stdLibAsms.Select(ai => ai.assembly);
            }
        }
        public IEnumerable<AssemblyDefinition> AllDefinedAssemblies
        {
            get
            {
                return StdLibReferences.Concat(new[] { primaryAssembly.assembly });
            }
        }

        public CecilContext(string asmToLoad)
        {
            var pa = AssemblyDefinition.ReadAssembly(asmToLoad);

            reflectionOnlyAsm = Assembly.ReflectionOnlyLoadFrom(asmToLoad);

            var refs = reflectionOnlyAsm.GetReferencedAssemblies();
            referencedAssemblies = refs.Select(TranslateReference).ToList();

            stdLibAsms = refs.Where(n =>
            {
                try
                {
                    return Assembly.ReflectionOnlyLoad(n.FullName).GlobalAssemblyCache;
                }
                catch
                {
                    return false;
                }
            }).Select(TranslateReference).Select(n => new AsmInfo(pa.MainModule.AssemblyResolver.Resolve(n))).ToList();

            primaryAssembly = new AsmInfo(pa); // load types after stdlib/gac references are loaded
        }

        AsmInfo InfoOf(AssemblyDefinition def)
        {
            if (def == primaryAssembly.assembly)
                return primaryAssembly;

            return stdLibAsms.FirstOrDefault(ai => ai.assembly == def);
        }

        public AssemblyDefinition GetAssembly(string displayName)
        {
            if (displayName == primaryAssembly.assembly.Name.Name)
                return primaryAssembly.assembly;

            return stdLibAsms.FirstOrDefault(ai => ai.assembly.Name.Name == displayName).assembly;
        }

        public TypeDefinition GetType(string fullName, string asmDisplayName = null)
        {
            return GetType(fullName, asmDisplayName == null ? null : GetAssembly(asmDisplayName));
        }
        public TypeDefinition GetType(string fullName, AssemblyDefinition asm = null)
        {
            IEnumerable<AsmInfo> i = asm == null ? AllDefinedAssemblies.Select(ad => InfoOf(ad)).Where(ai => ai.assembly != null) : new[] { InfoOf(asm) };

            foreach (var ai in i)
            {
                var fod = ai.types.FirstOrDefault(td => td.FullName == fullName);
                if (fod != null)
                    return fod;
            }

            return null;
        }

        public bool HasAssemblyDef(Assembly a)
        {
            var an = a.GetName().FullName;

            if (an == reflectionOnlyAsm.GetName().FullName)
                return true;

            return stdLibAsms.Any(ai => ai.assembly.Name.FullName == an);
        }
        public bool HasTypeDefinition(Type t)
        {
            if (!HasAssemblyDef(t.Assembly))
                return false;

            return primaryAssembly.types.Any(td => td.FullName == t.FullName) || stdLibAsms.Any(ai => ai.types.Any(td => td.FullName == t.FullName));
        }

        public AssemblyDefinition DefinitionOf(Assembly a)
        {
            if (!HasAssemblyDef(a))
                return null;

            var an = a.GetName().FullName;

            if (an == reflectionOnlyAsm.GetName().FullName)
                return primaryAssembly.assembly;

            return stdLibAsms.FirstOrDefault(ai => ai.assembly.Name.FullName == an).assembly;
        }
        public TypeDefinition DefinitionOf(Type t)
        {
            if (!HasTypeDefinition(t))
                return null;

            return primaryAssembly.types.FirstOrDefault(td => td.FullName == t.FullName)
                ?? stdLibAsms.Select(ai => ai.types.FirstOrDefault(td => td.FullName == t.FullName)).FirstOrDefault();
        }
        public TypeReference ReferenceOf(Type t)
        {
            var fod = loadedRefTypes.FirstOrDefault(td => td.FullName == t.FullName);
            if (fod != null)
                return fod;

            if (HasTypeDefinition(t))
                return DefinitionOf(t);

            if (!HasAssemblyDef(t.Assembly))
                return null;

            // meh
            return null;
        }

        // TODO: member -> cecil stuff methods

        AssemblyNameReference TranslateReference(AssemblyName name)
        {
            var anr = new AssemblyNameReference(name.Name, name.Version);

            anr.Attributes = (AssemblyAttributes)name.Flags;
            anr.Culture = name.CultureInfo.Name;
            anr.HashAlgorithm = (AssemblyHashAlgorithm)name.HashAlgorithm;
            anr.PublicKey = name.GetPublicKey();
            anr.PublicKeyToken = name.GetPublicKeyToken();

            return anr;
        }
    }
}

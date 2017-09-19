using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class TilePatcher
    {
        const int
            LARGEST_WLD_X = 8400,
            LARGEST_WLD_Y = 2400;

        static DNContext context;
        static MemberResolver memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Tile;

        static void ChangeFieldType ()
        {
            typeDef_Tile.GetField("wall").FieldType = typeSys.UInt16;
        }
        static void ChangeLocalTypes()
        {
            var wall = typeDef_Tile.GetField("wall");

            foreach (var td in context.PrimaryAssembly.ManifestModule.Types)
                foreach (var md in td.Methods)
                {
                    if (!md.HasBody)
                        continue;

                    var body = md.Body;

                    if (!body.HasVariables) // no local vars
                        continue;

                    // if there are no byte variables, we don't have to change antyhing
                    // either, speeding up things a lot (it's probably provable that
                    // body.Variables.Length < body.Instructions.Length)
                    for (int i = 0; i < body.Variables.Count; i++)
                        if (context.SigComparer.Equals(body.Variables[i].Type, typeSys.Byte))
                            goto DO_ENUMERATE;

                    continue;
                DO_ENUMERATE:

                    md.EnumerateWithStackAnalysis((ind, i, s) =>
                    {
                        if (s.Count == 0)
                            return ind;

                        var p = s.Peek();
                        var t = p.Type;
                        var c = p.Instr;

                        // ldfld Tile::wall
                        // stloc*
                        if (c.OpCode.Code == Code.Ldfld)
                        {
                            if (!context.SigComparer.Equals((IField)c.Operand, wall))
                                return ind;

                            if (i.OpCode.Code.Simplify() != Code.Stloc)
                                return ind;

                            var loc = i.GetLocal(body.Variables);

                            if (context.SigComparer.Equals(loc.Type, typeSys.Byte))
                                loc.Type = typeSys.UInt16;
                        }

                        return ind;
                    });
                }
        }

        internal static void Patch(Action<string> log)
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Tile = memRes.GetType("Terraria.Tile");

            ChangeFieldType ();
            ChangeLocalTypes();
        }
    }
}

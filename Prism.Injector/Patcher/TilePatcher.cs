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

        readonly static Code[] Stlocs = { Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3, Code.Stloc_S };

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

                    if (!body.InitLocals) // no local vars
                        continue;

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

                            if (Array.IndexOf(Stlocs, i.OpCode.Code) == -1)
                                return ind;

                            var li = 0;

                            switch (i.OpCode.Code)
                            {
                                case Code.Stloc:
                                case Code.Stloc_S:
                                    li = i.Operand is int ? (int)i.Operand : ((Local)i.Operand).Index;
                                    break;
                                // 0 not needed (default)
                                case Code.Stloc_1:
                                    li = 1;
                                    break;
                                case Code.Stloc_2:
                                    li = 2;
                                    break;
                                case Code.Stloc_3:
                                    li = 3;
                                    break;
                            }

                            if (context.SigComparer.Equals(body.Variables[li].Type, typeSys.Byte))
                                body.Variables[li].Type = typeSys.UInt16;
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

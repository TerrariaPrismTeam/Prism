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

                    md.EnumerateWithStackAnalysis((i, s) =>
                    {
                        if (s.Count == 0)
                            return;

                        var p = s.Peek();
                        var t = p.Item1;
                        var c = p.Item2;

                        // ldfld Tile::wall
                        // stloc*
                        if (c.OpCode.Code == Code.Ldfld)
                        {
                            if (c.Operand is FieldDef)
                            {
                                if (!context.SigComparer.Equals((FieldDef )c.Operand, wall))
                                    return;
                            }
                            else if (c.Operand is MemberRef)
                            {
                                if (!context.SigComparer.Equals((MemberRef)c.Operand, wall))
                                    return;
                            }
                            else
                            {
                                //! PLACE BREAKPOINT HERE
                                int iii = 0;
                                iii = ++iii - 1;
                            }

                            if (Array.IndexOf(Stlocs, i.OpCode.Code) == -1)
                                return;

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
                    });
                }
        }

        #region unused
        /*static bool IsGet2DArrCall(TypeReference arrayType, Instruction i)
        {
            var inner = arrayType.GetElementType();

            if (i.OpCode.Code != Code.Call)
                return false;

            var mtd = i.Operand as MethodReference;

            // for debugging
            //if (mtd == null)
            //    return false;
            //if (mtd.Name != "Get")
            //    return false;
            //if (mtd.DeclaringType.FullName != arrayType.FullName)
            //    return false;
            //if (!mtd.HasThis)
            //    return false;
            //if (mtd.ReturnType != inner)
            //    return false;
            //if (mtd.Parameters.Count != 2)
            //    return false;
            //if (!mtd.Parameters.All(pd => pd.ParameterType == typeSys.Int32))
            //    return false;

            //return true;

            return mtd != null && mtd.Name == "Get" && mtd.DeclaringType.FullName == arrayType.FullName && mtd.ReturnType == inner && !mtd.HasGenericParameters && mtd.Parameters.Count == 2 && mtd.Parameters.All(pd => pd.ParameterType == typeSys.Int32) && mtd.HasThis;
        }*/
        /*static void AddExtendedWallTypeField()
        {
            var wallTypeEx = new FieldDefinition("P_wallTypeEx", FieldAttributes.Assembly | FieldAttributes.Static, memRes.ReferenceOf(typeof(ushort[])));

            var main_t = memRes.GetType("Terraria.Main");

            #region add field
            {
                typeDef_Tile.Fields.Add(wallTypeEx);

                // initialize in static ctor
                var cb = typeDef_Tile.GetOrCreateStaticCtor().Body;
                var cproc = cb.GetILProcessor();

                var finalRet = cb.Instructions.Last();

                cproc.InsertBefore(finalRet, Instruction.Create(OpCodes.Ldsfld, main_t.GetField("maxTilesX")));
                cproc.InsertBefore(finalRet, Instruction.Create(OpCodes.Ldsfld, main_t.GetField("maxTilesY")));
                cproc.InsertBefore(finalRet, Instruction.Create(OpCodes.Dup));
                cproc.InsertBefore(finalRet, Instruction.Create(OpCodes.Newarr, wallTypeEx.FieldType));
                cproc.InsertBefore(finalRet, Instruction.Create(OpCodes.Stsfld, wallTypeEx));
            }
            #endregion

            #region add twodimentional getter
            {
                var getWallType = new MethodDefinition("GetWallType", MethodAttributes.Public | MethodAttributes.Static, typeSys.UInt16);

                getWallType.Parameters.Add(new ParameterDefinition("x", 0, typeSys.Int32));
                getWallType.Parameters.Add(new ParameterDefinition("y", 0, typeSys.Int32));

                var gwproc = getWallType.Body.GetILProcessor();

                gwproc.Emit(OpCodes.Ldsfld, wallTypeEx);

                // wallTypeEx[y * Main.maxTilesY + x]
                gwproc.Emit(OpCodes.Ldarg_1);
                //gwproc.Emit(OpCodes.Ldsfld, main_t.GetField("maxTilesY"));
                gwproc.Emit(OpCodes.Ldc_I4, LARGEST_WLD_Y);
                gwproc.Emit(OpCodes.Mul);
                gwproc.Emit(OpCodes.Ldarg_0);
                gwproc.Emit(OpCodes.Add);
                gwproc.Emit(OpCodes.Ldelem_Ref);
                gwproc.Emit(OpCodes.Ret);

                typeDef_Tile.Methods.Add(getWallType);

                // add an overload with a tile argument that ignores the tile, so the stack doesn't get messed up when using injected GetWallType(I) calls.
                {
                    var getWallTypeI = new MethodDefinition("GetWallTypeI", MethodAttributes.Assembly | MethodAttributes.Static, typeSys.UInt16);

                    getWallTypeI.Parameters.Add(new ParameterDefinition("tile", 0, typeSys.Object));
                    getWallTypeI.Parameters.Add(new ParameterDefinition("x", 0, typeSys.Int32));
                    getWallTypeI.Parameters.Add(new ParameterDefinition("y", 0, typeSys.Int32));

                    var gwiproc = getWallTypeI.Body.GetILProcessor();

                    gwiproc.Emit(OpCodes.Ldsfld, wallTypeEx);

                    // wallTypeEx[y * Main.maxTilesY + x]
                    gwiproc.Emit(OpCodes.Ldarg_2);
                    //gwiproc.Emit(OpCodes.Ldsfld, main_t.GetField("maxTilesY"));
                    gwiproc.Emit(OpCodes.Ldc_I4, LARGEST_WLD_Y);
                    gwiproc.Emit(OpCodes.Mul);
                    gwiproc.Emit(OpCodes.Ldarg_1);
                    gwiproc.Emit(OpCodes.Add);
                    gwiproc.Emit(OpCodes.Ldelem_Ref);
                    gwiproc.Emit(OpCodes.Ret);

                    typeDef_Tile.Methods.Add(getWallTypeI);
                }
            }
            #endregion

            #region add twodimentional setter
            {
                var setWallType = new MethodDefinition("SetWallType", MethodAttributes.Public | MethodAttributes.Static, typeSys.Void);

                setWallType.Parameters.Add(new ParameterDefinition("x"   , 0, typeSys. Int32));
                setWallType.Parameters.Add(new ParameterDefinition("y"   , 0, typeSys. Int32));
                setWallType.Parameters.Add(new ParameterDefinition("value", 0, typeSys.UInt16));

                var swproc = setWallType.Body.GetILProcessor();

                swproc.Emit(OpCodes.Ldsfld, wallTypeEx);

                // wallTypeEx[y * Main.maxTilesY + x]
                swproc.Emit(OpCodes.Ldarg_1);
                //swproc.Emit(OpCodes.Ldsfld, main_t.GetField("maxTilesY"));
                swproc.Emit(OpCodes.Ldc_I4, LARGEST_WLD_Y);
                swproc.Emit(OpCodes.Mul);
                swproc.Emit(OpCodes.Ldarg_0);
                swproc.Emit(OpCodes.Add);
                swproc.Emit(OpCodes.Ldarg_2);
                swproc.Emit(OpCodes.Stelem_Ref);
                swproc.Emit(OpCodes.Ret);

                typeDef_Tile.Methods.Add(setWallType);

                // add an overload with a tile argument that ignores the tile, so the stack doesn't get messed up when using injected SetWallType(I) calls.
                {
                    var setWallTypeI = new MethodDefinition("SetWallTypeI", MethodAttributes.Assembly | MethodAttributes.Static, typeSys.Void);

                    setWallTypeI.Parameters.Add(new ParameterDefinition("tile", 0, typeDef_Tile));
                    setWallTypeI.Parameters.Add(new ParameterDefinition("x", 0, typeSys.Int32));
                    setWallTypeI.Parameters.Add(new ParameterDefinition("y", 0, typeSys.Int32));
                    setWallTypeI.Parameters.Add(new ParameterDefinition("value", 0, typeSys.UInt16));

                    var swb = setWallTypeI.Body;

                    var swiproc = swb.GetILProcessor();

                    swiproc.Emit(OpCodes.Ldsfld, wallTypeEx);

                    // wallTypeEx[y * Main.maxTilesY + x]
                    swiproc.Emit(OpCodes.Ldarg_2);
                    //swiproc.Emit(OpCodes.Ldsfld, main_t.GetField("maxTilesY"));
                    swiproc.Emit(OpCodes.Ldc_I4, LARGEST_WLD_Y);
                    swiproc.Emit(OpCodes.Mul);
                    swiproc.Emit(OpCodes.Ldarg_1);
                    swiproc.Emit(OpCodes.Add);
                    swiproc.Emit(OpCodes.Ldarg_3);
                    swiproc.Emit(OpCodes.Stelem_Ref);
                    swiproc.Emit(OpCodes.Ret);

                    typeDef_Tile.Methods.Add(setWallTypeI);
                }
            }
            #endregion
        }*/
        /*static void ReplaceGetWallTypeCalls()
        {
            var gw = typeDef_Tile.GetMethod("GetWallTypeI"); // using the internal version

            var main_t = memRes.GetType("Terraria.Main");
            var main_tile = main_t.GetField("tile");
            var tile_wall = typeDef_Tile.GetField("wall");

            foreach (var td in context.PrimaryAssembly.MainModule.Types)
                foreach (var md in td.Methods)
                {
                    if (!md.HasBody)
                        continue;

                    var body = md.Body;
                    var ins = body.Instructions;
                    var proc = body.GetILProcessor();

                    for (int i = 0; i < ins.Count; i++)
                    {
                        var n = ins[i];

                        if (IsGet2DArrCall(main_tile.FieldType, n))
                        {
                            n = n.Next;

                            if (n == null)
                                continue;

                            if (n.OpCode.Code == Code.Ldfld && n.Operand == tile_wall)
                            {
                                var p = ins[i].Previous; // shouldn't be null, tile array + indices are loaded on the IL stack

                                proc.Remove(p.Next);
                                proc.Remove(n);

                                proc.InsertAfter(p, Instruction.Create(OpCodes.Call, gw));

                                // rewire branch targets
                                foreach (var i_ in ins)
                                    if (i_ != n && i_ != ins[i] && i_.Operand == ins[i])
                                        i_.Operand = p.Next;

                                i--;
                            }
                        }
                    }
                }
        }*/
        /*static void ReplaceSetWallTypeCalls()
        {
            var sw = typeDef_Tile.GetMethod("SetWallTypeI"); // using the internal version

            var main_t = memRes.GetType("Terraria.Main");
            var main_tile = main_t.GetField("tile");
            var tile_wall = typeDef_Tile.GetField("wall");

            foreach (var td in context.PrimaryAssembly.MainModule.Types)
                foreach (var md in td.Methods)
                {
                    if (!md.HasBody)
                        continue;

                    var body = md.Body;
                    var ins = body.Instructions;
                    var proc = body.GetILProcessor();

                    for (int i = 0; i < ins.Count; i++)
                    {
                        var n = ins[i];

                        if (n.OpCode.Code == Code.Stfld && n.Operand == tile_wall)
                        {
                            var p = ins[i].Previous; // shouldn't be null, tile array + indices are loaded on the IL stack

                            proc.Remove(n);

                            proc.InsertAfter(p, Instruction.Create(OpCodes.Call, sw));

                            // rewire branch targets
                            foreach (var i_ in ins)
                                if (i_ != n && i_.Operand == n)
                                    i_.Operand = p.Next;
                        }
                    }
                }
        }
        static void HideWallField()
        {
            var wall = typeDef_Tile.GetField("wall");

            wall.Name = "P_wall";

            wall.Attributes = FieldAttributes.Assembly;
        }*/
        #endregion

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Tile = memRes.GetType("Terraria.Tile");

            ChangeFieldType ();
            ChangeLocalTypes();

          //AddExtendedWallTypeField();
          //ReplaceGetWallTypeCalls ();
          //ReplaceSetWallTypeCalls ();
          //HideWallField();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class PlayerPatcher
    {
        static DNContext      context;
        static MemberResolver memRes ;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Player;

        static void WrapMethods()
        {
            typeDef_Player.GetMethod("GetFileData"    , MethodFlags.Public | MethodFlags.Static, new[]{ typeSys.String, typeSys.Boolean }).Wrap(context);
            typeDef_Player.GetMethod("ItemCheck"      , MethodFlags.Public | MethodFlags.Instance                                        ).Wrap(context);
            typeDef_Player.GetMethod("KillMe"         , MethodFlags.Public | MethodFlags.Instance                                        ).Wrap(context);
            typeDef_Player.GetMethod("Update"         , MethodFlags.Public | MethodFlags.Instance, new[]{ typeSys.Int32                 }).Wrap(context);
            typeDef_Player.GetMethod("UpdateEquips"   , MethodFlags.Public | MethodFlags.Instance, new[]{ typeSys.Int32                 }).Wrap(context);
            typeDef_Player.GetMethod("UpdateArmorSets", MethodFlags.Public | MethodFlags.Instance, new[]{ typeSys.Int32                 }).Wrap(context);
            typeDef_Player.GetMethod("WingMovement"   , MethodFlags.Public | MethodFlags.Instance                                        ).Wrap(context);
            typeDef_Player.GetMethod("UpdateBuffs"    , MethodFlags.Public | MethodFlags.Instance, new[]{ typeSys.Int32                 }).Wrap(context);
            typeDef_Player.GetMethod("AddBuff"        , MethodFlags.Public | MethodFlags.Instance                                        ).Wrap(context);

            var typeDef_uiCharSelect = memRes.GetType("Terraria.GameContent.UI.States.UICharacterSelect");

            typeDef_uiCharSelect.GetMethod("NewCharacterClick").Wrap(context);
        }

        static void AddFieldForBHandler()
        {
            typeDef_Player.Fields.Add(new FieldDefUser("P_BHandler"    , new FieldSig(typeSys.Object), FieldAttributes.Public));
            typeDef_Player.Fields.Add(new FieldDefUser("P_BuffBHandler", new FieldSig(memRes.ReferenceOf(typeof(object[])).ToTypeSig()), FieldAttributes.Public));
        }
        static void AddPlaceThingHook()
        {
            /*

            IL_2D37: ldsfld    int32 Terraria.Player::tileTargetX
            IL_2D3C: ldsfld    int32 Terraria.Player::tileTargetY
            IL_2D41: ldarg.0
            IL_2D42: ldfld     class Terraria.Item[] Terraria.Player::inventory
            IL_2D47: ldarg.0
            IL_2D48: ldfld     int32 Terraria.Player::selectedItem
            IL_2D4D: ldelem.ref
            IL_2D4E: ldfld     int32 Terraria.Item::createTile
            IL_2D53: ldc.i4.0
            IL_2D54: ldloc.s   78
            IL_2D56: ldarg.0
            IL_2D57: ldfld     int32 Terraria.Entity::whoAmI
            IL_2D5C: ldloc.s   72
            IL_2D5E: call      bool Terraria.WorldGen::PlaceTile(int32, int32, int32, bool, bool, int32, int32)
            IL_2D63: stloc.s   79
            */

            OpCode[] toFind =
            {
                OpCodes.Ldsfld,
                OpCodes.Ldsfld,
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Ldelem_Ref,
                OpCodes.Ldfld,
                OpCodes.Ldc_I4_0,
                OpCodes.Ldloc_S,
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Ldloc_S,
                OpCodes.Call,
                OpCodes.Stloc_S
            };

            /*var placeThing = typeDef_Player.GetMethod("PlaceThing");

            MethodDef invokePlaceThing;
            var onPlaceThingDel = context.CreateDelegate("Terraria.PrismInjections", "Player_OnPlaceThingDel", typeSys.Void, out invokePlaceThing, typeSys.Boolean);

            var onPlaceThing = new FieldDefUser("P_OnPlaceThing", new FieldSig(onPlaceThingDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_Player.Fields.Add(onPlaceThing);

            var ptb = placeThing.Body;
            using (var ptbproc = ptb.GetILProcessor())
            {
                Instruction[] toInj =
                {
                    Instruction.Create(OpCodes.Ldsfld  , onPlaceThing    ),
                    Instruction.Create(OpCodes.Ldloc   , ptb.Variables[79]),
                    Instruction.Create(OpCodes.Callvirt, invokePlaceThing )
                };

                var first = ptb.FindInstrSeqStart(toFind);
                for (int i = 0; i < toFind.Length - 1; i++)
                    first = first.Next(ptbproc);

                foreach (var i in toInj.Reverse())
                    ptbproc.InsertAfter(first, i);
            }*/
        }
        static void InsertSaveLoadHooks()
        {
            TypeDef typeDef_PlayerFileData = memRes.GetType("Terraria.IO.PlayerFileData");

            #region SavePlayer
            {
                var savePlayer = typeDef_Player.GetMethod("SavePlayer");

                MethodDef invokeSavePlayer;
                var onSavePlayerDel = context.CreateDelegate("Terraria.PrismInjections", "Player_OnSavePlayerDel", typeSys.Void, out invokeSavePlayer, typeDef_PlayerFileData.ToTypeSig());

                var onSavePlayer = new FieldDefUser("P_OnSavePlayer", new FieldSig(onSavePlayerDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(onSavePlayer);

                var spb = savePlayer.Body;
                using (var spproc = spb.GetILProcessor())
                {
                    var last = spb.Instructions.Last();

                    spproc.Remove(last);

                    Instruction newF;
                    spproc.Append(newF = Instruction.Create(OpCodes.Ldsfld, onSavePlayer));
                    spproc.Emit(OpCodes.Ldarg_0);
                    spproc.Emit(OpCodes.Callvirt, invokeSavePlayer);
                    spproc.Emit(OpCodes.Ret);

                    for (int i = 0; i < spb.Instructions.Count; i++)
                        if (spb.Instructions[i].Operand == last)
                            spb.Instructions[i].Operand = newF;
                    for (int i = 0; i < spb.ExceptionHandlers.Count; i++)
                        if (spb.ExceptionHandlers[i].HandlerEnd == last)
                            spb.ExceptionHandlers[i].HandlerEnd = newF;
                }
            }
            #endregion

            #region LoadPlayer
            {
                // Insert load hook near end of LoadPlayer
                var loadPlayer = typeDef_Player.GetMethod("LoadPlayer", MethodFlags.Public | MethodFlags.Static, typeSys.String.ToTypeDefOrRef(), typeSys.Boolean.ToTypeDefOrRef());

                MethodDef invokeLoadPlayer;
                var onLoadPlayerDel = context.CreateDelegate("Terraria.PrismInjections", "Player_OnLoadPlayerDel", typeSys.Void, out invokeLoadPlayer, typeDef_Player.ToTypeSig(), typeSys.String);

                var onLoadPlayer = new FieldDefUser("P_OnLoadPlayer", new FieldSig(onLoadPlayerDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(onLoadPlayer);

                var lpb = loadPlayer.Body;
                using (var lpproc = lpb.GetILProcessor())
                {
                    // player.skinVariant = (int)MathHelper.Clamp((float)player.skinVariant, 0f, 7f);
                    OpCode[] toFind =
                    {
                        OpCodes.Ldloc_1, // player (for the stfld instruction at the end)
                        OpCodes.Ldloc_1,
                        OpCodes.Ldfld,   // player.skinVariant
                        OpCodes.Conv_R4, // (float)^
                        OpCodes.Ldc_R4,
                        OpCodes.Ldc_R4,
                        OpCodes.Call,    // MathHelper.Clamp(^, 0f, 7f)
                        OpCodes.Conv_I4, // (int)^
                        OpCodes.Stfld    // ^^.skinVariant = ^
                    };

                    /*Instruction[] toInject =
                    {
                        Instruction.Create(OpCodes.Ldsfld, onLoadPlayer),
                        Instruction.Create(OpCodes.Ldloc_1),
                        Instruction.Create(OpCodes.Ldarg_0),
                        Instruction.Create(OpCodes.Callvirt, invokeLoadPlayer)
                    };

                    var first = lpb.FindInstrSeqStart(toFind);

                    foreach (var i in toInject)
                        lpproc.InsertBefore(first, i);

                    // rewire the if before it to end at the injected instructions instead of the code we looked for
                    foreach (var i in lpb.Instructions)
                        if (i.Operand == first)
                            i.Operand = toInject[0];

                    // not rewiring the if will lead to invalid IL, because the target instruction won't exist (because we're removing it here)
                    lpproc.RemoveInstructions(first, toFind.Length); // remove the limitation while we're at it*/
                }
            }
            #endregion
        }
        /// <summary>
        /// Removes the max item type limitation in vanilla player loading code.
        /// If an item's id is >= Main.maxItems (i.e. a mod item), it is defaulted to 0.
        /// This method removes that limitation.
        /// </summary>
        static void RemoveItemTypeLimitation()
        {
            /*
             *     ldloc.s <item netid>
             *     ldc.i4 <max item id>
             *     blt[.s] NORMAL
             *
             *     // set netid to 0, skip prefix
             *
             * NORMAL:
             *     // ...
             */

            OpCode[] seqToRemove =
            {
                OpCodes.Ldloc_S,
                OpCodes.Ldc_I4,
                OpCodes.Blt_S
            };

            var loadPlayerBody = typeDef_Player.GetMethod("LoadPlayer", MethodFlags.Public | MethodFlags.Static, typeSys.String.ToTypeDefOrRef(), typeSys.Boolean.ToTypeDefOrRef()).Body;
            /*using (var processor = loadPlayerBody.GetILProcessor())
            {

                for (int count = 0, firstInd = 0; ;)
                {
                    var firstInstr = loadPlayerBody.FindInstrSeqStart(seqToRemove, firstInd);
                    firstInd = loadPlayerBody.Instructions.IndexOf(firstInstr) + 1;

                    if (firstInstr != null)
                    {
                        var ldc_i4 = firstInstr.Next(processor);

                        if (!(ldc_i4.Operand is int) || (int)ldc_i4.Operand != 3770)
                            continue;

                        var blt_s = ldc_i4.Next(processor);
                        var target = (Instruction)blt_s.Operand;

                        count++;

                        var toRem = firstInstr;

                        while (toRem != target)
                        {
                            var n = toRem.Next(processor);
                            processor.Remove(toRem);
                            toRem = n;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            Console.WriteLine("Warning: PlayerPatcher.RemoveItemTypeLimitation() could not find the target instruction sequence; Terraria.Player.LoadPlayer() may have been fixed, and this hack can be removed.");
                        }
                        else if (count != 6)
                        {
                            Console.WriteLine("Warning: PlayerPatcher.RemoveItemTypeLimitation() removed " + count.ToString() + " instances of the target instruction sequence instead of 6; Terraria.Player.LoadPlayer() logic may have changed, and this hack may be superflous/harmful!");
                        }

                        break;
                    }
                }
            }*/
        }
        static void RemoveStatCaps()
        {
            var lpb = typeDef_Player.GetMethod("LoadPlayer", MethodFlags.Public | MethodFlags.Static, typeSys.String.ToTypeDefOrRef(), typeSys.Boolean.ToTypeDefOrRef()).Body;
            using (var lpproc = lpb.GetILProcessor())
            {

                /*
                 *
                 *     ldloc.1 // player
                 *     ldfld <stat>
                 *     ldc.i4 <amt> // 200, 400 or 500
                 *     ble.s NORMAL
                 *
                 *     // reset
                 *     ldloc.1 // player
                 *     ldc.i4 <amt>
                 *     stfld <stat>
                 *
                 * NORMAL:
                 *     // ...
                 */

                OpCode[] toRem =
                {
                    OpCodes.Ldloc_1,
                    OpCodes.Ldfld,
                    OpCodes.Ldc_I4,
                    OpCodes.Ble_S,

                    OpCodes.Ldloc_1,
                    OpCodes.Ldc_I4,
                    OpCodes.Stfld
                };

                /*for (int count = 0; ;)
                {
                    var firstI = lpb.FindInstrSeqStart(toRem);

                    if (firstI != null)
                    {
                        count++;

                        lpproc.RemoveInstructions(firstI, toRem.Length);
                    }
                    else
                    {
                        if (count == 0)
                        {
                            Console.WriteLine("PlayerPatcher.RemoveStatCaps() could not find the target instruction sequence; Terraria.Player.LoadPlayer() may have been fixed, and this hack can be removed.");
                        }
                        else if (count != 3)
                        {
                            Console.WriteLine("PlayerPatcher.RemoveStatCaps() removed " + count.ToString() + " instances of the target instruction sequence instead of 3; Terraria.Player.LoadPlayer() logic may have changed, and this hack may be superflous/harmful!");
                        }

                        break;
                    }
                }*/
            }
        }
        static void ReplaceUseSoundCalls()
        {
            var typeDef_Item = memRes.GetType("Terraria.Item");

            #region ItemCheck
            {
                var itemCheck = typeDef_Player.GetMethod("RealItemCheck"); // this method is wrapped

                MethodDef invokeUseSound;
                var onItemCheckUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_ItemCheck_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var itemCheck_PlayUseSound0 = new FieldDefUser("P_ItemCheck_PlayUseSound0", new FieldSig(onItemCheckUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(itemCheck_PlayUseSound0);

                OpCode[] toRem =
                {
                    OpCodes.Ldloc_2,
                    OpCodes.Ldfld, // Item.sound
                    OpCodes.Ldc_I4_0,
                    OpCodes.Ble_S, // <after the call>

                    OpCodes.Ldc_I4_2, // UseItem ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.Y
                    OpCodes.Conv_I4,
                    OpCodes.Ldloc_2,
                    OpCodes.Ldfld, // Item.useSound
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                /*using (var icproc = itemCheck.Body.GetILProcessor())
                {
                    var first = itemCheck.Body.FindInstrSeqStart(toRem);
                    first = icproc.RemoveInstructions(first, toRem.Length);

                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, itemCheck_PlayUseSound0));
                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Ldloc_2));
                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeUseSound));

                    // ---

                    var itemCheck_PlayUseSound1 = new FieldDefUser("P_ItemCheck_PlayUseSound1", new FieldSig(onItemCheckUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                    typeDef_Player.Fields.Add(itemCheck_PlayUseSound1);

                    toRem = new[]
                    {
                        OpCodes.Ldloc_2,
                        OpCodes.Ldfld, // Item.sound
                        OpCodes.Ldc_I4_0,
                        OpCodes.Ble_S, // <after the call>

                        OpCodes.Ldc_I4_2, // UseItem ID
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.Y
                        OpCodes.Conv_I4,
                        OpCodes.Ldloc_2,
                        OpCodes.Ldfld, // Item.useSound
                        OpCodes.Call // Main.PlaySound(int, int, int, int)
                    };

                    first = itemCheck.Body.FindInstrSeqStart(toRem);
                    first = icproc.RemoveInstructions(first, toRem.Length);

                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, itemCheck_PlayUseSound1));
                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Ldloc_2));
                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    icproc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeUseSound));
                }*/
            }
            #endregion

            #region QuickBuff
            {
                var quickBuff = typeDef_Player.GetMethod("QuickBuff");

                MethodDef invokeUseSound;
                var onQuickBuffUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_QuickBuff_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var quickBuff_PlayUseSound = new FieldDefUser("P_QuickBuff_PlayUseSound", new FieldSig(onQuickBuffUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(quickBuff_PlayUseSound);

                var qbb = quickBuff.Body;
                /*using (var qbproc = qbb.GetILProcessor())
                {
                    // change local 0 to an item (instead of item.useSound int)
                    qbb.Variables[0].Type = typeDef_Item.ToTypeSig();

                    // things are too specific -> offsets have to be used

                    // remove .useSound
                    var inst = qbb.Instructions.First(i => i.Offset == (TerrariaPatcher.Platform == Platform.Windows ? 0x0247 : 0x0248));
                    qbproc.Remove(inst);

                    // change ldc.i4.0 to ldnull
                    inst = qbb.Instructions.First(i => i.Offset == 0x0009);
                    inst.OpCode = OpCodes.Ldnull;
                    inst = qbb.Instructions.First(i => i.Offset == (TerrariaPatcher.Platform == Platform.Windows ? 0x02D2 : 0x02D3));
                    inst.OpCode = OpCodes.Ldnull;

                    // change ble(.s) to beq(.s)
                    inst = qbb.Instructions.First(i => i.Offset == (TerrariaPatcher.Platform == Platform.Windows ? 0x02D3 : 0x02D4));
                    inst.OpCode = OpCodes.Beq_S;

                    OpCode[] toRem =
                    {
                        OpCodes.Ldc_I4_2, // UseItem ID
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.Y
                        OpCodes.Conv_I4,
                        OpCodes.Ldloc_0,
                        OpCodes.Call // Main.PlaySound(int, int, int, int)
                    };

                    var first = qbb.FindInstrSeqStart(toRem);

                    qbproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, quickBuff_PlayUseSound));
                    qbproc.InsertBefore(first, Instruction.Create(OpCodes.Ldloc_0)); // load item instance ons stack
                    qbproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    qbproc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeUseSound));

                    first = qbproc.RemoveInstructions(first, toRem.Length);
                }*/
            }
            #endregion

            #region QuickGrapple
            {
                var quickGrapple = typeDef_Player.GetMethod("QuickGrapple");

                MethodDef invokeUseSound;
                var onQuickGrappleUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_QuickGrapple_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var quickGrapple_PlayUseSound = new FieldDefUser("P_QuickGrapple_PlayUseSound", new FieldSig(onQuickGrappleUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(quickGrapple_PlayUseSound);

                OpCode[] toRem =
                {
                    OpCodes.Ldc_I4_2, // UseItem ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.Y
                    OpCodes.Conv_I4,
                    TerrariaPatcher.Platform == Platform.Windows ? OpCodes.Ldloc_3 : OpCodes.Ldloc_0,
                    OpCodes.Ldfld, // Item.useSound
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                /*using (var qgproc = quickGrapple.Body.GetILProcessor())
                {
                    var first = quickGrapple.Body.FindInstrSeqStart(toRem);
                    first = qgproc.RemoveInstructions(first, toRem.Length);

                    qgproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, quickGrapple_PlayUseSound));
                    qgproc.InsertBefore(first, Instruction.Create(TerrariaPatcher.Platform == Platform.Windows ? OpCodes.Ldloc_3 : OpCodes.Ldloc_0)); // load item instance ons stack
                    qgproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    qgproc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeUseSound));
                }*/
            }
            #endregion

            #region QuickHeal
            {
                var quickHeal = typeDef_Player.GetMethod("QuickHeal");

                MethodDef invokeUseSound;
                var onQuickHealUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_QuickHeal_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var quickHeal_PlayUseSound = new FieldDefUser("P_QuickHeal_PlayUseSound", new FieldSig(onQuickHealUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(quickHeal_PlayUseSound);

                OpCode[] toRem =
                {
                    OpCodes.Ldc_I4_2, // UseItem ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.Y
                    OpCodes.Conv_I4,
                    OpCodes.Ldloc_0,
                    OpCodes.Ldfld, // Item.useSound
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                /*using (var qhproc = quickHeal.Body.GetILProcessor())
                {
                    var first = quickHeal.Body.FindInstrSeqStart(toRem);
                    var first_ = qhproc.RemoveInstructions(first, toRem.Length);

                    Instruction newF;
                    qhproc.InsertBefore(first_, newF = Instruction.Create(OpCodes.Ldsfld, quickHeal_PlayUseSound));
                    qhproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldloc_0)); // load item instance ons stack
                    qhproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldarg_0));
                    qhproc.InsertBefore(first_, Instruction.Create(OpCodes.Callvirt, invokeUseSound));

                    for (int i = 0; i < quickHeal.Body.Instructions.Count; i++)
                        if (quickHeal.Body.Instructions[i].Operand == first)
                            quickHeal.Body.Instructions[i].Operand = newF;
                }*/
            }
            #endregion

            #region QuickMana
            {
                var quickMana = typeDef_Player.GetMethod("QuickMana");

                MethodDef invokeUseSound;
                var onQuickManaUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_QuickMana_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var quickMana_PlayUseSound = new FieldDefUser("P_QuickMana_PlayUseSound", new FieldSig(onQuickManaUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(quickMana_PlayUseSound);

                OpCode[] toRem =
                {
                    OpCodes.Ldc_I4_2, // UseItem ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.Y
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // Player.inventory
                    OpCodes.Ldloc_0,
                    OpCodes.Ldelem_Ref, // <top of stack>[i]
                    OpCodes.Ldfld, // Item.useSound
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                /*using (var qmproc = quickMana.Body.GetILProcessor())
                {
                    var first = quickMana.Body.FindInstrSeqStart(toRem);
                    var first_ = qmproc.RemoveInstructions(first, toRem.Length);

                    Instruction newF;
                    qmproc.InsertBefore(first_, newF = Instruction.Create(OpCodes.Ldsfld, quickMana_PlayUseSound));
                    qmproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldarg_0));
                    qmproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldfld, typeDef_Player.GetField("inventory")));
                    qmproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldloc_0));
                    qmproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldelem_Ref));
                    qmproc.InsertBefore(first_, Instruction.Create(OpCodes.Ldarg_0));
                    qmproc.InsertBefore(first_, Instruction.Create(OpCodes.Call, invokeUseSound));

                    for (int i = 0; i < quickMana.Body.Instructions.Count; i++)
                        if (quickMana.Body.Instructions[i].Operand == first)
                            quickMana.Body.Instructions[i].Operand = newF;
                }*/
            }
            #endregion

            #region QuickMount
            {
                var quickMount = typeDef_Player.GetMethod("QuickMount");

                MethodDef invokeUseSound;
                var onQuickMountUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_QuickMount_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var quickMount_PlayUseSound = new FieldDefUser("P_QuickMount_PlayUseSound", new FieldSig(onQuickMountUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(quickMount_PlayUseSound);

                OpCode[] toRem = TerrariaPatcher.Platform == Platform.Windows
                    ? new[]
                    {
                        OpCodes.Ldloc_0,
                        OpCodes.Ldfld, // Item.sound
                        OpCodes.Ldc_I4_0,
                        OpCodes.Ble_S, // <after the call>

                        OpCodes.Ldc_I4_2, // UseItem ID
                        OpCodes.Ldarg_0,
                        OpCodes.Call, // Entity.Center
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Call, // Entity.Center
                        OpCodes.Ldfld, // Vector2.Y
                        OpCodes.Conv_I4,
                        OpCodes.Ldloc_0,
                        OpCodes.Ldfld, // Item.useSound
                        OpCodes.Call // Main.PlaySound(int, int, int, int)
                    }
                    : new[]
                    {
                        OpCodes.Ldloc_0,
                        OpCodes.Ldfld, // Item.sound
                        OpCodes.Ldc_I4_0,
                        OpCodes.Ble_S, // <after the call>

                        OpCodes.Ldc_I4_2, // UseItem ID
                        OpCodes.Ldarg_0,
                        OpCodes.Call, // Entity.Center
                        OpCodes.Stloc_2,
                        OpCodes.Ldloca_S,
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Call, // Entity.Center
                        OpCodes.Stloc_2,
                        OpCodes.Ldloca_S,
                        OpCodes.Ldfld, // Vector2.Y
                        OpCodes.Conv_I4,
                        OpCodes.Ldloc_0,
                        OpCodes.Ldfld, // Item.useSound
                        OpCodes.Call // Main.PlaySound(int, int, int, int)
                    };

                /*using (var qmproc = quickMount.Body.GetILProcessor())
                {
                    var first = quickMount.Body.FindInstrSeqStart(toRem);
                    var index = quickMount.Body.Instructions.IndexOf(first);
                    var next = quickMount.Body.Instructions[index + toRem.Length];

                    first = qmproc.RemoveInstructions(first, toRem.Length);

                    qmproc.InsertBefore(next, Instruction.Create(OpCodes.Ldsfld, quickMount_PlayUseSound));
                    qmproc.InsertBefore(next, Instruction.Create(OpCodes.Ldloc_0));
                    qmproc.InsertBefore(next, Instruction.Create(OpCodes.Ldarg_0));
                    qmproc.InsertBefore(next, Instruction.Create(OpCodes.Call, invokeUseSound));
                }*/
            }
            #endregion

            #region UpdatePet
            {
                var updatePet = typeDef_Player.GetMethod("UpdatePet");

                MethodDef invokeUseSound;
                var onUpdatePetUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_UpdatePet_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var updatePet_PlayUseSound = new FieldDefUser("P_UpdatePet_PlayUseSound", new FieldSig(onUpdatePetUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(updatePet_PlayUseSound);

                OpCode[] toRem =
                {
                    OpCodes.Ldc_I4_2, // UseItem ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.Y
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // Player.miscEquips
                    OpCodes.Ldc_I4_0,
                    OpCodes.Ldelem_Ref, // <top of stack>[0]
                    OpCodes.Ldfld, // Item.useSound
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                /*using (var upproc = updatePet.Body.GetILProcessor())
                {
                    var first = updatePet.Body.FindInstrSeqStart(toRem);
                    first = upproc.RemoveInstructions(first, toRem.Length);

                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, updatePet_PlayUseSound));
                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Ldfld, typeDef_Player.GetField("miscEquips")));
                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Ldc_I4_0));
                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Ldelem_Ref));
                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    upproc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeUseSound));
                }*/
            }
            #endregion

            #region UpdatePetLight
            {
                var updatePetLight = typeDef_Player.GetMethod("UpdatePetLight");

                MethodDef invokeUseSound;
                var onUpdatePetLightUseSound = context.CreateDelegate("Terraria.PrismInjections", "Player_UpdatePetLight_PlayUseSoundDel", typeSys.Void, out invokeUseSound, typeDef_Item.ToTypeSig(), typeDef_Player.ToTypeSig());

                var updatePetLight_PlayUseSound = new FieldDefUser("P_UpdatePetLight_PlayUseSound", new FieldSig(onUpdatePetLightUseSound.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_Player.Fields.Add(updatePetLight_PlayUseSound);

                OpCode[] toRem =
                {
                    OpCodes.Ldc_I4_2, // UseItem ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.Y
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // Player.miscEquips
                    OpCodes.Ldc_I4_1,
                    OpCodes.Ldelem_Ref, // <top of stack>[1]
                    OpCodes.Ldfld, // Item.useSound
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                /*using (var uplproc = updatePetLight.Body.GetILProcessor())
                {
                    var first = updatePetLight.Body.FindInstrSeqStart(toRem);
                    first = uplproc.RemoveInstructions(first, toRem.Length);

                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, updatePetLight_PlayUseSound));
                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Ldfld, typeDef_Player.GetField("miscEquips")));
                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Ldc_I4_1));
                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Ldelem_Ref));
                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
                    uplproc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeUseSound));
                }*/
            }
            #endregion
        }
        static void FixOnEnterWorldBackingFieldName()
        {
            var hooks_t = memRes.GetType("Terraria.Player/Hooks");

            var onEnterWorld = hooks_t.GetField("OnEnterWorld");

            onEnterWorld.Name = "_onEnterWorld_backingField";
        }
        static void InjectMidUpdate()
        {
            var update = typeDef_Player.GetMethod("RealUpdate" /* method is wrapped */, MethodFlags.Public | MethodFlags.Instance, typeSys.Int32.ToTypeDefOrRef());
            var grabItems = typeDef_Player.GetMethod("GrabItems", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32.ToTypeDefOrRef());

            MethodDef invokeMidUpdate;
            var onMidUpdateDel = context.CreateDelegate("Terraria.PrismInjections", "Player_MidUpdateDel", typeSys.Void, out invokeMidUpdate, typeDef_Player.ToTypeSig(), typeSys.Int32);

            var onMidUpdate = new FieldDefUser("P_OnMidUpdate", new FieldSig(onMidUpdateDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_Player.Fields.Add(onMidUpdate);

            var ub = update.Body;
            /*using (var uproc = ub.GetILProcessor())
            {
                OpCode[] callGrabItems =
                {
                    OpCodes.Ldarg_0, // this
                    OpCodes.Ldarg_1, // id
                    OpCodes.Call, // Player.GrabItems

                    OpCodes.Ldsfld, // Main.mapFullScreen
                    OpCodes.Brtrue
                };

                Instruction instrs;

                int startInd = 0;
                while (true)
                {
                    instrs = ub.FindInstrSeqStart(callGrabItems, startInd);

                    if (instrs == null)
                        break;

                    startInd = ub.Instructions.IndexOf(instrs) + 3;

                    var mtdToCall = instrs.Next(uproc).Next(uproc).Operand as MethodDef;

                    if (mtdToCall == null)
                        continue;

                    // close enough
                    if (context.SigComparer.Equals(mtdToCall.DeclaringType, grabItems.DeclaringType) && mtdToCall.FullName == grabItems.FullName)
                        break;
                }

                instrs = instrs.Next(uproc).Next(uproc).Next(uproc);

                uproc.InsertBefore(instrs, Instruction.Create(OpCodes.Ldsfld, onMidUpdate));
                uproc.EmitWrapperCall(invokeMidUpdate, instrs);
            }*/
        }
        static void InitBuffBHandlerArray()
        {
            var ctor = typeDef_Player.GetConstructor();
            var buffBHandler = typeDef_Player.GetField("P_BuffBHandler");

            using (var cproc = ctor.Body.GetILProcessor())
            {
                var l = ctor.Body.Instructions.Last().Previous(cproc).Previous(cproc);

                cproc.InsertBefore(l, Instruction.Create(OpCodes.Ldarg_0));
                cproc.InsertBefore(l, Instruction.Create(OpCodes.Ldc_I4, 22));
                cproc.InsertBefore(l, Instruction.Create(OpCodes.Newarr, typeSys.Object));
                cproc.InsertBefore(l, Instruction.Create(OpCodes.Stfld, buffBHandler));
            }
        }
        static void InjectPreShootHook()
        {
            // public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 255, float ai0 = 0f, float ai1 = 0f)

            // get stuff
            var proj_t = memRes.GetType("Terraria.Projectile");
            var newProj = proj_t.GetMethod("NewProjectile", MethodFlags.Static | MethodFlags.Public, Empty<TypeSig>.Array);

            var itemCheck = typeDef_Player.GetMethod("RealItemCheck"); // already wrapped
            var icb = itemCheck.Body;
            var icins = icb.Instructions;

            // create hook delegate
            MethodDef invokeOnPreShoot;
            var onPreShootDel   = context.CreateDelegate("Terraria.PrismInjections", "Player_OnPreShoot", newProj.ReturnType, out invokeOnPreShoot, newProj.Parameters.Select(p => p.Type).ToArray());
            var onPreShootField = new FieldDefUser("P_OnPreShoot", new FieldSig(onPreShootDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_Player.Fields.Add(onPreShootField);

            // create static method that will invoke the handler (so we don't have to mess with weird stack stuff)
            var invInvOPS = new MethodDefUser("P_InvokeOnPreShoot", MethodSig.CreateStatic(newProj.ReturnType, newProj.Parameters.Select(p => p.Type).ToArray()),
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig);
            for (int i = 0; i < newProj.Parameters.Count; i++)
            {
                invInvOPS.Parameters[i].CreateParamDef();
                invInvOPS.Parameters[i].ParamDef.Name = newProj.Parameters[i].Name;
            }
            invInvOPS.Body = new CilBody();
            invInvOPS.Body.MaxStack = 12;
            using (var iiopsproc = invInvOPS.Body.GetILProcessor())
            {
                /*
                 *   ldsfld onPreShootField
                 *   brfalse.s VANILLA
                 *
                 *   ldsfld onPreShootField
                 *   ldargs...
                 *   callvirt int32 Invoke
                 *   ret
                 *
                 * VANILLA:
                 *   ldargs...
                 *   call NewProjectile
                 *   ret
                 */

                var VANILLA = Instruction.Create(OpCodes.Ldarg_0);

                iiopsproc.Emit(OpCodes.Ldsfld, onPreShootField);
                iiopsproc.Emit(OpCodes.Brfalse_S, VANILLA);

                iiopsproc.Emit(OpCodes.Ldsfld, onPreShootField);
                for (ushort i = 0; i < newProj.Parameters.Count; i++)
                    iiopsproc.Append(newProj.Parameters.GetLdargOf(i, false));
                iiopsproc.Emit(OpCodes.Callvirt, invokeOnPreShoot);
                iiopsproc.Emit(OpCodes.Ret);

                iiopsproc.Append(VANILLA); // ldarg.0
                for (ushort i = 1; i < newProj.Parameters.Count; i++)
                    iiopsproc.Append(newProj.Parameters.GetLdargOf(i, false));
                iiopsproc.Emit(OpCodes.Call, newProj);
                iiopsproc.Emit(OpCodes.Ret);
            }
            typeDef_Player.Methods.Add(invInvOPS);

            // replace calls
            for (int i = 0; i < icins.Count; i++)
            {
                var inst = icins[i];

                if (inst.OpCode == OpCodes.Call && context.SigComparer.Equals((IMethod)inst.Operand, newProj))
                    inst.Operand = invInvOPS;
            }
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Player = memRes.GetType("Terraria.Player");

            WrapMethods();

            AddFieldForBHandler();
            AddPlaceThingHook();
            InsertSaveLoadHooks();
            RemoveItemTypeLimitation();
            RemoveStatCaps();
            ReplaceUseSoundCalls();
            FixOnEnterWorldBackingFieldName();
            InjectMidUpdate();
            InitBuffBHandlerArray();
            InjectPreShootHook();
        }
    }
}

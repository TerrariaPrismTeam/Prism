using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class MainPatcher
    {
        static DNContext context;
        static MemberResolver memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Main;

        static void WrapMethods()
        {
            typeDef_Main.GetMethod("UpdateAudio").Wrap(context);
            //typeDef_Main.GetMethod("PlaySound"     , MethodFlags.Static   | MethodFlags.Public, new[] { typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
            //        typeSys.Single, typeSys.Single }).Wrap(context, "Terraria.PrismInjections", "Main_PlaySoundDel", "P_OnPlaySound");
            typeDef_Main.GetMethod("DrawNPC"       , MethodFlags.Instance | MethodFlags.Public, new[] { typeSys.Int32, typeSys.Boolean                             }).Wrap(context);
            typeDef_Main.GetMethod("DrawProj"      , MethodFlags.Instance | MethodFlags.Public, new[] { typeSys.Int32                                              }).Wrap(context);
            typeDef_Main.GetMethod("DrawPlayer"    , MethodFlags.Instance | MethodFlags.Public).Wrap(context);
            typeDef_Main.GetMethod("DrawBackground", MethodFlags.Instance | MethodFlags.Public).Wrap(context);
        }

        static void RemoveVanillaNpcDrawLimitation()
        {
            OpCode[] seqToRemove =
            {
                // original code:

                // ldsfld class Terraria.NPC[] Terraria.Main::npc
                // ldloc.2
                // ldelem.ref
                // ldfld int32 Terraria.NPC::'type'
                // ldc.i4 540
                // bge <end-of-if-body>

                // in 1.3.0.7 & 1.3.0.8, this starts at address 0x006f

                OpCodes.Ldsfld,
                OpCodes.Ldloc_2,
                OpCodes.Ldelem_Ref,
                OpCodes.Ldfld,
                OpCodes.Ldc_I4,
                OpCodes.Bge
            };

            var drawNpcs = typeDef_Main.GetMethod("DrawNPCs", MethodFlags.Instance | MethodFlags.Public, new[] { typeSys.Boolean });

            /*var firstInstr = drawNpcs.Body.FindInstrSeqStart(seqToRemove);

            using (var p = drawNpcs.Body.GetILProcessor())
            {
                p.RemoveInstructions(firstInstr, seqToRemove.Length);
            }*/
        }
        static void AddIsChatAllowedHook()
        {
            OpCode[] searchSeq =
            {
                //IL_2a83: ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                //IL_2a88: ldc.i4.s 13
                //IL_2a8a: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                //IL_2a8f: brfalse IL_2b20

                OpCodes.Ldsfld,     //IL_2a94: ldsfld int32 Terraria.Main::netMode
                OpCodes.Ldc_I4_1,   //IL_2a99: ldc.i4.1
                OpCodes.Bne_Un,     //IL_2a9a: bne.un IL_2b20

                OpCodes.Ldsflda,    //IL_2a9f: Main.ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                OpCodes.Ldc_I4,     //IL_2aa4: ldc.i4 164
                OpCodes.Call,       //IL_2aa9: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                OpCodes.Brtrue_S,   //IL_2aae: brtrue.s IL_2b20

                OpCodes.Ldsflda,    //IL_2ab0: ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                OpCodes.Ldc_I4,     //IL_2ab5: ldc.i4 165
                OpCodes.Call,       //IL_2aba: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                OpCodes.Brtrue_S,   //IL_2abf: brtrue.s IL_2b20

                OpCodes.Ldsfld,     //IL_2ac1: ldsfld bool Terraria.Main::hasFocus
                OpCodes.Brfalse_S   //IL_2ac6: brfalse.s IL_2b20
            };

            var mainUpdate = typeDef_Main.GetMethod("Update").Body;

            //public virtual bool IsChatAllowedHook() { return Main.netMode == 1; }

            var chatCheckHook = new MethodDefUser("P_IsChatAllowed", MethodSig.CreateStatic(typeSys.Boolean),
                MethodAttributes.Public | MethodAttributes.Static);
            chatCheckHook.Body = new CilBody();

            var proc = chatCheckHook.Body.GetILProcessor();

            proc.Append(Instruction.Create(OpCodes.Ldsfld, typeDef_Main.GetField("netMode")));
            proc.Append(Instruction.Create(OpCodes.Ldc_I4_1));
            // The bne.un goes here
            proc.Append(Instruction.Create(OpCodes.Ldc_I4_1));
            proc.Append(Instruction.Create(OpCodes.Ret));
            proc.Append(Instruction.Create(OpCodes.Ldc_I4_0));
            proc.Append(Instruction.Create(OpCodes.Ret));
            Instruction[] chatCheckHookInstr = chatCheckHook.Body.Instructions.ToArray();
            var nextInstrReturnFalse = chatCheckHookInstr[4]; //Offset of the ldc.i4.0
            var instrAfterWhichToInsert = chatCheckHookInstr[1]; //dat variable name tho
            var instrToInsert = Instruction.Create(OpCodes.Bne_Un, nextInstrReturnFalse);
            proc.InsertAfter(instrAfterWhichToInsert, instrToInsert);

            chatCheckHook.Body.SimplifyBranches();
            chatCheckHook.Body.OptimizeBranches();

            typeDef_Main.Methods.Add(chatCheckHook);

            // ---

            /*proc = mainUpdate.GetILProcessor();
            var mainInstrs = mainUpdate.FindInstrSeq(proc, searchSeq);

            if (mainInstrs[0] == null)
            {
                Console.Error.WriteLine("MainPatcher.AddIsChatAllowedHook() could not find opcodes for checking Main.netMode to open chat. Update the opcode search array pls thx.");
                return;
            }

            var skipToOffset = ((Instruction)(mainInstrs[2]/ *bne.un* /.Operand));

            proc.ReplaceInstructions(mainInstrs.Take(3), new Instruction[]
            {
                Instruction.Create(OpCodes.Call, chatCheckHook),
                Instruction.Create(OpCodes.Brfalse, skipToOffset)
            });*/
        }
        static void AddLocalChatHook(Action<string> log)
        {
            #region VERY LONG OPCODE SEARCH
            OpCode[] searchSeq =
            {
                OpCodes.Ldsfld      ,//IL_293b: ldsfld bool Terraria.Main::chatRelease
                OpCodes.Brfalse     ,//IL_2940: brfalse IL_2a83

                OpCodes.Ldsfld      ,//IL_2945: ldsfld string Terraria.Main::chatText
                OpCodes.Ldstr       ,//IL_294a: ldstr ""
                OpCodes.Call        ,//IL_294f: call bool [mscorlib]System.String::op_Inequality(string, string)
                OpCodes.Brfalse_S   ,//IL_2954: brfalse.s IL_297b

                OpCodes.Ldc_I4_S    ,//IL_2956: ldc.i4.s 25
                OpCodes.Ldc_I4_M1   ,//IL_2958: ldc.i4.m1
                OpCodes.Ldc_I4_M1   ,//IL_2959: ldc.i4.m1
                OpCodes.Ldsfld      ,//IL_295a: ldsfld string Terraria.Main::chatText
                OpCodes.Ldsfld      ,//IL_295f: ldsfld int32 Terraria.Main::myPlayer
                OpCodes.Ldc_R4      ,//IL_2964: ldc.r4 0.0
                OpCodes.Ldc_R4      ,//IL_2969: ldc.r4 0.0
                OpCodes.Ldc_R4      ,//IL_296e: ldc.r4 0.0
                OpCodes.Ldc_I4_0    ,//IL_2973: ldc.i4.0
                OpCodes.Ldc_I4_0    ,//IL_2974: ldc.i4.0
                OpCodes.Ldc_I4_0    ,//IL_2975: ldc.i4.0
                OpCodes.Call        ,//IL_2976: call void Terraria.NetMessage::SendData(int32, int32, int32, string, int32, float32, float32,    float32,        int32,   int32, int32)

                OpCodes.Ldsfld      ,//IL_297b: ldsfld int32 Terraria.Main::netMode
                OpCodes.Brtrue      ,//IL_2980: brtrue IL_2a41

                OpCodes.Ldsfld      ,//IL_2985: ldsfld string Terraria.Main::chatText
                OpCodes.Ldstr       ,//IL_298a: ldstr ""
                OpCodes.Call        ,//IL_298f: call bool [mscorlib]System.String::op_Inequality(string, string)
                OpCodes.Brfalse     ,//IL_2994: brfalse IL_2a41

                OpCodes.Call        ,//IL_2999: call valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color [Microsoft.Xna.Framework]            OpCodes.Ldelem_R   ef,Microsoft.Xna.Framework.Color::get_White/()
                OpCodes.Stloc_S     ,//IL_299e: stloc.s 10
                OpCodes.Ldsfld      ,//IL_29a0: ldsfld class Terraria.Player[] Terraria.Main::player
                OpCodes.Ldsfld      ,//IL_29a5: ldsfld int32 Terraria.Main::myPlayer
                OpCodes.Ldelem_Ref  ,//IL_29aa: ldelem.ref
                OpCodes.Ldfld       ,//IL_29ab: ldfld uint8 Terraria.Player::difficulty
                OpCodes.Ldc_I4_2    ,//IL_29b0: ldc.i4.2
                OpCodes.Bne_Un_S    ,//IL_29b1: bne.un.s IL_29bc

                OpCodes.Ldsfld      ,//IL_29b3: ldsfld valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color Terraria.Main::hcColor
                OpCodes.Stloc_S     ,//IL_29b8: stloc.s 10
                OpCodes.Br_S        ,//IL_29ba: br.s IL_29d6

                OpCodes.Ldsfld      ,//IL_29bc: ldsfld class Terraria.Player[] Terraria.Main::player
                OpCodes.Ldsfld      ,//IL_29c1: ldsfld int32 Terraria.Main::myPlayer
                OpCodes.Ldelem_Ref  ,//IL_29c6: ldelem.ref
                OpCodes.Ldfld       ,//IL_29c7: ldfld uint8 Terraria.Player::difficulty
                OpCodes.Ldc_I4_1    ,//IL_29cc: ldc.i4.1
                OpCodes.Bne_Un_S    ,//IL_29cd: bne.un.s IL_29d6

                OpCodes.Ldsfld      ,//IL_29cf: ldsfld valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color Terraria.Main::mcColor
                OpCodes.Stloc_S     ,//IL_29d4: stloc.s 10

                OpCodes.Ldsfld      ,//IL_29d6: ldsfld string Terraria.Main::chatText
              //OpCodes.Stloc_S     ,//IL_29db: stloc.s 11
                OpCodes.Pop         ,
                OpCodes.Ldsfld      ,//IL_29dd: ldsfld class Terraria.Player[] Terraria.Main::player
                OpCodes.Ldsfld      ,//IL_29e2: ldsfld int32 Terraria.Main::myPlayer
                OpCodes.Ldelem_Ref  ,//IL_29e7: ldelem.ref
                OpCodes.Ldfld       ,//IL_29e8: ldfld string Terraria.Entity::name
                OpCodes.Call        ,//IL_29ed: call string Terraria.GameContent.UI.Chat.NameTagHandler::GenerateTag(string)
                OpCodes.Ldstr       ,//IL_29f2: ldstr " "
                OpCodes.Ldsfld      ,//IL_29f7: ldsfld string Terraria.Main::chatText
                OpCodes.Call        ,//IL_29fc: call string [mscorlib]System.String::Concat(string, string, string)
              //OpCodes.Stloc_S     ,//IL_2a01: stloc.s 11
                OpCodes.Ldsfld      ,//IL_2a03: ldsfld class Terraria.Player[] Terraria.Main::player
                OpCodes.Ldsfld      ,//IL_2a08: ldsfld int32 Terraria.Main::myPlayer
                OpCodes.Ldelem_Ref  ,//IL_2a0d: ldelem.ref
                OpCodes.Ldflda      ,//IL_2a0e: ldflda valuetype Terraria.Player/OverheadMessage Terraria.Player::chatOverhead
                OpCodes.Ldsfld      ,//IL_2a13: ldsfld string Terraria.Main::chatText
                OpCodes.Ldsfld      ,//IL_2a18: ldsfld int32 Terraria.Main::chatLength
                OpCodes.Ldc_I4_2    ,//IL_2a1d: ldc.i4.2
                OpCodes.Div         ,//IL_2a1e: div
                OpCodes.Call        ,//IL_2a1f: call instance void Terraria.Player/OverheadMessage::NewMessage(string, int32)
                OpCodes.Ldc_I4_0,
                OpCodes.Ldloc_S,
                OpCodes.Call,
                OpCodes.Call,

                /*OpCodes.Ldloc_S     ,//IL_2a24: ldloc.s 11
                OpCodes.Ldloca_S    ,//IL_2a26: ldloca.s 10
                OpCodes.Call        ,//IL_2a28: call instance uint8 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color::get_R()
                OpCodes.Ldloca_S    ,//IL_2a2d: ldloca.s 10
                OpCodes.Call        ,//IL_2a2f: call instance uint8 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color::get_G()
                OpCodes.Ldloca_S    ,//IL_2a34: ldloca.s 10
                OpCodes.Call        ,//IL_2a36: call instance uint8 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color::get_B()
                OpCodes.Ldc_I4_0    ,//IL_2a3b: ldc.i4.0
                OpCodes.Call        ,//IL_2a3c: call void Terraria.Main::NewText(string, uint8, uint8, uint8, bool)
                */
                /*
                    if (Main.inputTextEnter && Main.chatRelease)
                    {
                        if (Main.chatText != String.Empty)
                        {
                //////////////INSTRUCTIONS BEGIN HERE//////////////////////////////////////////////////////////////////////////
                //                                                                                                           //
                //          NetMessage.SendData(25, -1, -1, Main.chatText, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);              //
                //      }                                                                                                    //
                //      if (Main.netMode == 0 && Main.chatText != String.Empty)                                              //
                //      {                                                                                                    //
                //          Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;                       //
                //          if (Main.player[Main.myPlayer].difficulty == 2)                                                  //
                //          {                                                                                                //
                //              white = Main.hcColor;                                                                        //
                //          }                                                                                                //
                //          else if (Main.player[Main.myPlayer].difficulty == 1)                                             //
                //          {                                                                                                //
                //              white = Main.mcColor;                                                                        //
                //          }                                                                                                //
                //          string newText = Main.chatText;                                                                  //
                //          newText = NameTagHandler.GenerateTag(Main.player[Main.myPlayer].name) + " " + Main.chatText;     //
                //          Main.player[Main.myPlayer].chatOverhead.NewMessage(Main.chatText, Main.chatLength / 2);          //
                //          Main.NewText(newText, white.R, white.G, white.B, false);                                         //
                //                                                                                                           //
                //////////////INSTRUCTIONS END HERE////////////////////////////////////////////////////////////////////////////
                        }
                ////////// Instructions skip over above block and go straight to here if hook returns false ///////////////////
                        Main.chatText = String.Empty;
                        Main.chatMode = false;
                        Main.chatRelease = false;
                        Main.player[Main.myPlayer].releaseHook = false;
                        Main.player[Main.myPlayer].releaseThrow = false;
                        Main.PlaySound(11, -1, -1, 1);
                    }
                */
            };
            #endregion

            //public virtual bool PlayerChatLocalHook() { return true; }
            var localChatHook = new MethodDefUser("P_LocalChat", MethodSig.CreateStatic(typeSys.Boolean),
                MethodAttributes.Public | MethodAttributes.Static);
            localChatHook.Body = new CilBody();

            using (var proc = localChatHook.Body.GetILProcessor())
            {
                // Return true;
                proc.Emit(OpCodes.Ldc_I4_1);
                proc.Emit(OpCodes.Ret);

                localChatHook.Body.SimplifyBranches();
                localChatHook.Body.OptimizeBranches();
            }

            typeDef_Main.Methods.Add(localChatHook);

            var mainUpdate = typeDef_Main.GetMethod("Update");
            using (var proc = mainUpdate.Body.GetILProcessor())
            {
                var instrSeq = mainUpdate.Body.FindInstrSeq(proc, searchSeq);

                if (instrSeq[0] == null)
                    log("MainPatcher.AddLocalChatHook() could not find all the local chat opcodes. The search array needs to be updated.");
                else
                {
                    var newInstrBrfalse = Instruction.Create(OpCodes.Brfalse_S, (Instruction)(instrSeq[1].Operand));
                    proc.InsertBefore(instrSeq[1], newInstrBrfalse);

                    var newInstrCall = Instruction.Create(OpCodes.Call, localChatHook);
                    proc.InsertBefore(instrSeq[1], newInstrCall);
                }

                mainUpdate.Body.SimplifyBranches();
                mainUpdate.Body.OptimizeBranches();
            }
        }
        static void FixOnEngineLoadField()
        {
            // wtf?
            typeDef_Main.GetField("OnEngineLoad").Name = "_onEngineLoad_backingField";
        }
        static void RemoveArmourDrawLimitations()
        {
            #region DrawPlayerHead
            {
                var drawPlayerHead = typeDef_Main.GetMethod("DrawPlayerHead", MethodFlags.Public | MethodFlags.Instance);

                /*while (true)
                {
                    OpCode[] toRem =
                    {
                        OpCodes.Ldarg_1,
                        OpCodes.Ldfld,
                        OpCodes.Ldc_I4,
                        OpCodes.Bge
                    };

                    var i = drawPlayerHead.Body.FindInstrSeqStart(toRem);

                    if (i == null)
                        break;

                    using (var p = drawPlayerHead.Body.GetILProcessor())
                    {
                        p.RemoveInstructions(i, toRem.Length);
                    }
                }*/
            }
            #endregion

            #region DrawPlayer
            {
                var drawPlayer = typeDef_Main.GetMethod("RealDrawPlayer", MethodFlags.Public | MethodFlags.Instance);

                /*while (true)
                {
                    OpCode[] toRem =
                    {
                        OpCodes.Ldarg_1,
                        OpCodes.Ldfld,
                        OpCodes.Ldc_I4,
                        OpCodes.Bge
                    };

                    var i = drawPlayer.Body.FindInstrSeqStart(toRem);

                    if (i == null)
                        break;

                    using (var p = drawPlayer.Body.GetILProcessor())
                    {
                        p.RemoveInstructions(i, toRem.Length);
                    }
                }*/
            }
            #endregion
        }
        static void AddPreDrawHook()
        {
            OpCode[] toFind =
            {
                /*
                    spriteBatch.Draw(...);

                    IL_29AF: ldsfld    class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch
                    IL_29B4: ldc.i4.0
                    IL_29B5: ldsfld    class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.BlendState [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.BlendState::AlphaBlend
                    IL_29BA: ldsfld    class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SamplerState [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SamplerState::LinearClamp
                    IL_29BF: ldsfld    class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.DepthStencilState [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.DepthStencilState::None
                    IL_29C4: ldarg.0
                    IL_29C5: ldfld     class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.RasterizerState Terraria.Main::Rasterizer
                    IL_29CA: ldnull
                    IL_29CB: ldarg.0
                    IL_29CC: ldfld     valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Matrix Terraria.Main::Transform
                    IL_29D1: callvirt  instance void [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SpriteBatch::Begin(valuetype [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SpriteSortMode, class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.BlendState, class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SamplerState, class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.DepthStencilState, class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.RasterizerState, class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.Effect, valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Matrix)
                */

                OpCodes.Ldsfld,
                OpCodes.Ldc_I4_0,
                OpCodes.Ldsfld,
                OpCodes.Ldsfld,
                OpCodes.Ldsfld,
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Ldnull,
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Callvirt
            };

            var draw = typeDef_Main.GetMethod("Draw");

            var spriteBatch = typeDef_Main.GetField("spriteBatch");

            MethodDef invokePreDraw;
            var onPreDrawDel = context.CreateDelegate("Terraria.PrismInjections", "Main_OnPreDrawDel", typeSys.Void, out invokePreDraw, spriteBatch.FieldType);

            var onPreDraw = new FieldDefUser("P_OnPreDraw", new FieldSig(onPreDrawDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_Main.Fields.Add(onPreDraw);

            var drb = draw.Body;
            /*using (var drbproc = drb.GetILProcessor())
            {
                Instruction[] toInj =
                {
                    Instruction.Create(OpCodes.Ldsfld  , onPreDraw    ),
                    Instruction.Create(OpCodes.Ldsfld  , spriteBatch  ),
                    Instruction.Create(OpCodes.Callvirt, invokePreDraw)
                };

                var first = drb.FindInstrSeqStart(toFind);
                for (int i = 0; i < toFind.Length; i++)
                    first = first.Next(drbproc);

                foreach (var i in toInj.Reverse())
                    drbproc.InsertAfter(first, i);

                // rewire the if before it to end at the injected instructions instead of the code we looked for
                //foreach (var i in drb.Instructions)
                //    if (i.Operand == first)
                //        i.Operand = toInj[0];

                // not rewiring the if will lead to invalid IL, because the target instruction won't exist (because we're removing it here)
            }*/
        }
        static void AddOnUpdateKeyboardHook(Action<string> log)
        {
            OpCode[] search =
            {
                OpCodes.Call     , //IL_27a6: call valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Microsoft.Xna.Framework.Input.Keyboard::GetState()
                OpCodes.Stsfld   , //IL_27ab: stsfld valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                OpCodes.Ldsfld   , //IL_27b0: ldsfld bool Terraria.Main::editSign
                OpCodes.Brfalse_S, //IL_27b5: brfalse.s IL_27bd

                OpCodes.Ldc_I4_0 , //IL_27b7: ldc.i4.0
                OpCodes.Stsfld   , //IL_27b8: stsfld bool Terraria.Main::chatMode

                OpCodes.Ldsfld   , //IL_27bd: ldsfld bool Terraria.Main::chatMode
                OpCodes.Brtrue_S , //IL_27c2: brtrue.s IL_27cf

                OpCodes.Ldc_I4_0 , //IL_27c4: ldc.i4.0
                OpCodes.Stsfld   , //IL_27c5: stsfld int32 Terraria.Main::startChatLine
                OpCodes.Br       , //IL_27ca: br IL_2a83

                OpCodes.Ldsfld   , //IL_27cf: ldsfld int32 Terraria.Main::screenHeight
                OpCodes.Ldc_I4_3 , //IL_27d4: ldc.i4.3
                OpCodes.Div      , //IL_27d5: div
                OpCodes.Conv_R4  , //IL_27d6: conv.r4
                OpCodes.Ldsfld   , //IL_27d7: ldsfld class [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SpriteFont Terraria.Main::fontMouseText
                OpCodes.Ldstr    , //IL_27dc: ldstr "1"
                OpCodes.Callvirt , //IL_27e1: callvirt instance valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2 [Microsoft.Xna.Framework.Graphics]Microsoft.Xna.Framework.Graphics.SpriteFont::MeasureString(string)

                OpCodes.Ldfld    , //IL_27e6: ldfld float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2::Y
                OpCodes.Div      , //IL_27eb: div
                OpCodes.Conv_I4  , //IL_27ec: conv.i4
                OpCodes.Ldc_I4_1 , //IL_27ed: ldc.i4.1
                OpCodes.Sub      , //IL_27ee: sub
                OpCodes.Stsfld   , //IL_27ef: stsfld int32 Terraria.Main::showCount
                OpCodes.Ldsflda  , //IL_27f4: ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                OpCodes.Ldc_I4_S , //IL_27f9: ldc.i4.s 38
                OpCodes.Call     , //IL_27fb: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                OpCodes.Brfalse_S, //IL_2800: brfalse.s IL_2864
            };

            var mainUpdate = typeDef_Main.GetMethod("Update");
            var mainUpdateBody = mainUpdate.Body;
            var first = mainUpdateBody.FindInstrSeqStart(search);
            var mainUpdateProc = mainUpdateBody.GetILProcessor();

            if (!(first != null && (first = first.Next(mainUpdateProc)) != null && (first = first.Next(mainUpdateProc)) != null))
                log("Couldn't find instructions for MainPatcher::AddOnUpdateKeyboardHook()");

            //public virtual void P_OnUpdateInputHook() { }
            MethodDef invokeOnUpdateKeyboardHook;
            var onUpdateKeyboardDelType = context.CreateDelegate("Terraria.PrismInjections", "Main_Update_OnUpdateKeyboardDel", typeSys.Void, out invokeOnUpdateKeyboardHook, typeDef_Main.ToTypeSig(), mainUpdate.Parameters[1].Type /* HAH I WIN, XNA */);
            var onUpdateKeyboardDelField = new FieldDefUser("P_OnUpdateKeyboard", new FieldSig(onUpdateKeyboardDelType.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_Main.Fields.Add(onUpdateKeyboardDelField);

            /*mainUpdateProc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, onUpdateKeyboardDelField));
            mainUpdateProc.EmitWrapperCall(invokeOnUpdateKeyboardHook, first);*/
            //mainUpdateProc.InsertBefore(first, Instruction.Create(OpCodes.Call, invokeOnUpdateKeyboardHook));
        }
        static void AddPostScreenClearHook()
        {
            OpCode[] toFind =
            {
                OpCodes.Call,
                OpCodes.Call,
                OpCodes.Callvirt
            };

            var draw = typeDef_Main.GetMethod("Draw");

            var spriteBatch = typeDef_Main.GetField("spriteBatch");

            MethodDef invokePostScrCl;
            var onPostScrClDel = context.CreateDelegate("Terraria.PrismInjections", "Main_OnPostScrClDel", typeSys.Void, out invokePostScrCl);

            var onPostScrClDraw = new FieldDefUser("P_OnPostScrClDraw", new FieldSig(onPostScrClDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_Main.Fields.Add(onPostScrClDraw);

            /*var drb = draw.Body;
            using (var drbproc = drb.GetILProcessor())
            {
                Instruction[] toInj =
                {
                    Instruction.Create(OpCodes.Ldsfld  , onPostScrClDraw),
                    Instruction.Create(OpCodes.Callvirt, invokePostScrCl)
                };

                var first = drb.FindInstrSeqStart(toFind).Next(drbproc) / * call 2 * /.Next(drbproc) / * callvirt * /.Next(drbproc); // ldsfld (spriteBatch)

                foreach (var i in toInj)
                    drbproc.InsertBefore(first, i);
            }*/
        }

        static void RemoveResolutionChangedMessage()
        {
            var sdmm = typeDef_Main.GetMethod("SetDisplayMode");

            OpCode[] toFind =
            {
                OpCodes.Ldstr,
                OpCodes.Ldarg_0,
                OpCodes.Box,
                OpCodes.Ldarg_1,
                OpCodes.Box,
                OpCodes.Call,
                OpCodes.Call
            };

            var sdmb = sdmm.Body;
            using (var sdmproc = sdmb.GetILProcessor())
            {
                var first = sdmb.FindInstrSeqStart(toFind);
                sdmproc.Remove(first, toFind.Length);
            }
        }

        internal static void Patch(Action<string> log)
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys      = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Main = memRes.GetType("Terraria.Main");

            WrapMethods();
            RemoveVanillaNpcDrawLimitation();
            FixOnEngineLoadField();
            RemoveArmourDrawLimitations();
            AddPreDrawHook();
            AddOnUpdateKeyboardHook(log);
            AddPostScreenClearHook();
            RemoveResolutionChangedMessage();

            AddIsChatAllowedHook();
            typeDef_Main.GetMethod("P_IsChatAllowed", MethodFlags.Public | MethodFlags.Static).Wrap(context);
            AddLocalChatHook(log);
            typeDef_Main.GetMethod("P_LocalChat", MethodFlags.Public | MethodFlags.Static).Wrap(context);
        }
    }
}

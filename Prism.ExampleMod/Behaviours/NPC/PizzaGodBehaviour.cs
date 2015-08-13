using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;

namespace Prism.ExampleMod.Behaviours.NPC
{
    public enum PizzaGodState
    {
        SpinStart,
        SpinStop,
        WaitingToDash,
        Dash
    }

    //If anyone wants to make this work using Entity.ai[] then go ahead .-.
    public class PizzaGodBehaviour : NpcBehaviour
    {
        public static readonly float MaxRotSpeed = 9; //rot/sec
        public static readonly float RotAcc = 2.0f; //rot/sec/sec
        public static readonly float DashInitSpeed = 80; //tile/sec
        public static readonly float DashWaitDur = 2; //sec
        public static readonly float DashSlowdownAcc = 20; //tile/sec/sec
        public static readonly float MinionSpawnDelayMin = 0.5f;
        public static readonly float MinionSpawnDelayMax = 1.5f;

        public static float MinionSpawnDelay
        {
            get
            {
                return MinionSpawnDelayMin + (float)(Main.rand.NextDouble() * MinionSpawnDelayMax - MinionSpawnDelayMin);
            }
        }

        public Player Target
        {
            get
            {
                return Main.player[Entity.target];
            }
        }

        PizzaGodState State = PizzaGodState.SpinStart;
        float RotationSpeed = 0;
        float Dash = 0;
        bool RotatingCCW = false;
        float DashWaitTimer = 0;
        bool ReachedPlayerWithDash = false;
        Vector2 CurrentDashDir = Vector2.Zero;
        float prevRot = 0;
        float MinionSpawnTimer = 0;

        void Rotate(float speed)
        {
            Entity.rotation += speed;

            if ((!RotatingCCW && Entity.rotation > -(MathHelper.PiOver4) && prevRot < -(MathHelper.PiOver4)) || (RotatingCCW && Entity.rotation < -(MathHelper.PiOver4) && prevRot > -(MathHelper.PiOver4)))
            {
                Main.PlaySound(2, Entity.Center, 7);
                Main.soundInstanceItem[7].Volume = MathHelper.Clamp(Main.soundInstanceItem[7].Volume * 1.5f, 0, 1);

                Main.PlaySound(2, Entity.Center, 1);
                Main.soundInstanceItem[1].Volume = MathHelper.Clamp(Main.soundInstanceItem[1].Volume * 1.5f, 0, 1);

                Main.PlaySound(2, Entity.Center, 18);
                Main.soundInstanceItem[18].Volume = MathHelper.Clamp(Main.soundInstanceItem[18].Volume * 1.5f, 0, 1);
            }

            Entity.rotation = MathHelper.WrapAngle(Entity.rotation);
        }

        float GetQuickestRotDir(float targetAngle)
        {
            float aNpc = GetCircularAngle(Entity.rotation),
                  aTar = GetCircularAngle(targetAngle),
                  cw = (MathHelper.TwoPi - aNpc + aTar) % MathHelper.TwoPi,
                  ccw = (MathHelper.TwoPi + aNpc - aTar) % MathHelper.TwoPi;

            return (ccw < cw) ? -ccw : cw;
        }

        float GetCircularAngle(float regularAngle)
        {
            if (regularAngle >= 0f)
                return regularAngle;

            return MathHelper.TwoPi + regularAngle;
        }

        float GetDistDelta(float unitPerSec)
        {
            return TMain.ElapsedTime * unitPerSec;
        }

        float GetRandDir()
        {
            return (float)(Main.rand.NextDouble() * 1.25f - 0.25f);
        }

        Vector2 GetDir(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        void SpawnNewMinion()
        {
            var c = Entity.Center.ToPoint();
            int n = Terraria.NPC.NewNPC(c.X, c.Y, NpcDef.Defs["PizzaGodJr"].Type, 0, 0, 0, 0, 0, Entity.target);
            Main.npc[n].velocity = Main.npc[n].DirectionTo(Main.player[Main.npc[n].target].Center) * 20;

            MinionSpawnTimer = MinionSpawnDelay;
        }

        void CheckMinionSpawns()
        {
            if (MinionSpawnTimer > 0)
            {
                MinionSpawnTimer -= TMain.ElapsedTime;
            }
            else
            {
                SpawnNewMinion();
            }
        }

        public override void OnAI()
        {
            Entity.TargetClosest(false);
            if (Entity.target >= 0)
            {
                Entity.timeLeft = Terraria.NPC.activeTime;

                if (State == PizzaGodState.SpinStart)
                {
                    CheckMinionSpawns();

                    if (RotationSpeed < MaxRotSpeed)
                    {
                        RotationSpeed += GetDistDelta(RotAcc);
                    }
                    else
                    {
                        State = PizzaGodState.WaitingToDash;
                        DashWaitTimer = 0;
                    }
                }
                else if (State == PizzaGodState.SpinStop)
                {
                    if (RotationSpeed > 0)
                    {
                        RotationSpeed -= GetDistDelta(RotAcc);
                    }
                    else
                    {
                        RotationSpeed = -RotationSpeed;
                        State = PizzaGodState.SpinStart;
                        RotatingCCW = !RotatingCCW;
                        MinionSpawnTimer = MinionSpawnDelay;
                    }
                }
                else if (State == PizzaGodState.WaitingToDash)
                {
                    if (DashWaitTimer < DashWaitDur)
                    {
                        DashWaitTimer += TMain.ElapsedTime;
                    }
                    else
                    {
                        DashWaitTimer = 0;
                        State = PizzaGodState.Dash;
                        Dash = DashInitSpeed;
                        ReachedPlayerWithDash = false;
                        Main.PlaySound(15, Entity.Center, 0);
                    }
                }
                else if (State == PizzaGodState.Dash)
                {
                    if (Dash > 0)
                    {
                        if (!ReachedPlayerWithDash && Entity.getRect().Intersects(Entity.targetRect))
                            ReachedPlayerWithDash = true;

                        Dash -= GetDistDelta(DashSlowdownAcc);
                    }
                    else
                    {
                        Dash = 0;
                        State = PizzaGodState.SpinStop;
                    }
                }

                Rotate(MathHelper.TwoPi * GetDistDelta(RotationSpeed * (RotatingCCW ? -1 : 1)));
                Entity.SimpleFlyMovement(CurrentDashDir * GetDistDelta(Dash * 16), GetDistDelta(Dash * 16));

                if (Dash > 0 && !ReachedPlayerWithDash)
                    CurrentDashDir = Entity.DirectionTo(Main.player[Entity.target].Center);

                Entity.scale = 6 - (2 * (RotationSpeed / MaxRotSpeed));
                Entity.width = Entity.height = (int)(48 * Entity.scale);

                prevRot = Entity.rotation;

                Lighting.AddLight(Entity.Center, new Vector3(20, 20, 0) * 0.5f * (Entity.scale / 8));

                if ((Target.Center - Entity.Center).Length() >= (256 * 16))
                {
                    Entity.Teleport(Target.Center);
                }
            }
        }

        public override void OnInit()
        {
            Main.PlaySound(4, Entity.Center, 10);
        }
    }
}

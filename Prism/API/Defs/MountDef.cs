using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Util;
using Terraria;

namespace Prism.API.Defs
{
    public partial class MountDef : ObjectDef<MountBehaviour>
    {
        internal Point texSize;
        int[] playerYOff;

        public int MaxAbilityCharge
        {
            get;
            set;
        }
        public int AbilityCooldown
        {
            get;
            set;
        }
        public int AbilityDuration
        {
            get;
            set;
        }

        public int AdditionalHeight
        {
            get;
            set;
        }
        public int PlayerHeadOffset
        {
            get;
            set;
        }

        public int MaxFatigue
        {
            get;
            set;
        }
        public int MaxFlightTime
        {
            get;
            set;
        }
        public int JumpHeight
        {
            get;
            set;
        }
        public int SpawnDust
        {
            get;
            set;
        }

        public int BodyFrame
        {
            get;
            set;
        }
        public int TotalFrames
        {
            get;
            set;
        }

        public float FallDamageMult
        {
            get;
            set;
        }
        public float Acceleration
        {
            get;
            set;
        }
        public float DashSpeed
        {
            get;
            set;
        }
        public float JumpSpeed
        {
            get;
            set;
        }
        public float RunSpeed
        {
            get;
            set;
        }
        public float SwimSpeed
        {
            get;
            set;
        }

        public bool BlockExtraJumps
        {
            get;
            set;
        }
        public bool ConstantJump
        {
            get;
            set;
        }
        public bool EmitsLight
        {
            get;
            set;
        }
        public bool IdleFrameLoop
        {
            get;
            set;
        }
        public bool IsMinecart
        {
            get;
            set;
        }
        public bool IsDirectionalMinecart
        {
            get;
            set;
        }
        public bool SpawnDustNoGravity
        {
            get;
            set;
        }
        public bool UsesHover
        {
            get;
            set;
        }

        public int[] PlayerYOffset
        {
            get
            {
                if (playerYOff == null)
                {
                    playerYOff = new int[TotalFrames];

                    for (int i = 0; i < TotalFrames; i++)
                        playerYOff[i] = AdditionalHeight;
                }

                return playerYOff;
            }
            set
            {
                playerYOff = value;
            }
        }

        public Action<Vector2> MinecartDust
        {
            get;
            set;
        }

        public Point Offset
        {
            get;
            set;
        }
        public Color LightColour
        {
            get;
            set;
        }

        public MountFrameData Dashing
        {
            get;
            set;
        }
        public MountFrameData Flying
        {
            get;
            set;
        }
        public MountFrameData Idle
        {
            get;
            set;
        }
        public MountFrameData InAir
        {
            get;
            set;
        }
        public MountFrameData Running
        {
            get;
            set;
        }
        public MountFrameData Standing
        {
            get;
            set;
        }
        public MountFrameData Swim
        {
            get;
            set;
        }

        public MountFrameData AllFrameData
        {
            set
            {
                Dashing = Flying = Idle = InAir = Running = Standing = Swim = value;
            }
        }

        public BuffRef Buff
        {
            get;
            set;
        }
        public BuffRef ExpertBuff
        {
            get;
            set;
        }

        public MountTextureData Front
        {
            get;
            set;
        }
        public MountTextureData Back
        {
            get;
            set;
        }

        public MountDef(ObjectName displayName, Func<MountBehaviour> newBehaviour = null, MountTextureData front = default(MountTextureData), MountTextureData back = default(MountTextureData), BuffRef buff = null)
            : base(displayName, newBehaviour)
        {
            Front = front;
            Back = back;

            AllFrameData = MountFrameData.None;

            Buff = buff;
            ExpertBuff = buff;

            MinecartDust = Empty<Vector2>.Action;
        }

        public static implicit operator MountRef(MountDef  def)
        {
            return new MountRef(def.InternalName, def.Mod.InternalName);
        }
        public static explicit operator MountDef(MountRef @ref)
        {
            return @ref.Resolve();
        }
    }
}

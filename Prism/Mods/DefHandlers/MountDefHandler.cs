using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;
using Prism.API.Defs;
using Prism.Util;
using Terraria;

using MountSets = Terraria.ID.MountID.Sets;

namespace Prism.Mods.DefHandlers
{
    sealed class MountDefHandler
    {
        internal int NextTypeIndex;

#pragma warning disable 414
        static bool FillingVanilla = false;
#pragma warning restore 414

        bool hasReset = false;

        public Dictionary<int, MountDef> DefsByType = new Dictionary<int, MountDef>();
        public Dictionary<string, MountDef> VanillaDefsByName = new Dictionary<string, MountDef>();

        int? minVanillaId = null, maxVanillaId = null;
        FieldInfo[] idFields = null;
        int[] idValues = null;
        string[] idNames = null;

        internal FieldInfo[] IDFields
        {
            get
            {
                if (idFields == null)
                    idFields = typeof(MountID).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType.IsPrimitive && f.FieldType != typeof(bool) && f.Name != "Count").ToArray();

                return idFields;
            }
        }
        internal int[] IDValues
        {
            get
            {
                if (idValues == null)
                    idValues = IDFields.Select(f => (int)Convert.ChangeType(f.GetValue(null), typeof(int))).ToArray();

                return idValues;
            }
        }
        internal string[] IDNames
        {
            get
            {
                if (idNames == null)
                    idNames = IDFields.Select(f => f.Name).ToArray();

                return idNames;
            }
        }

        int MinVanillaID
        {
            get
            {
                if (minVanillaId == null)
                    minVanillaId = IDFields.Select(f => (int)Convert.ChangeType(f.GetValue(null), typeof(int))).Min();

                return minVanillaId.Value;
            }
        }
        int MaxVanillaID
        {
            get
            {
                if (maxVanillaId == null)
                    maxVanillaId = (int)Convert.ChangeType(typeof(MountID).GetField("Count", BindingFlags.Public | BindingFlags.Static).GetValue(null), typeof(int));

                return maxVanillaId.Value;
            }
        }

        internal MountDefHandler()
        {
            //Reset();
        }

        void ExtendVanillaArrays(int amt = 1)
        {
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Mount.mounts.Length + amt : MountID.Count;

            Array.Resize(ref Mount.mounts, newLen);
            Array.Resize(ref MountSets.Cart, newLen);
        }

        void CopyEntityToDef(int id, Mount.MountData mount, MountDef def)
        {
            def.Type = id;

            def.MaxAbilityCharge = mount.abilityChargeMax;
            def.AbilityCooldown  = mount.abilityCooldown ;
            def.AbilityDuration  = mount.abilityDuration ;

            def.AdditionalHeight = mount.heightBoost     ;
            def.PlayerHeadOffset = mount.playerHeadOffset;

            def.MaxFatigue    = mount.fatigueMax   ;
            def.MaxFlightTime = mount.flightTimeMax;
            def.JumpHeight    = mount.jumpHeight   ;
            def.SpawnDust     = mount.spawnDust    ;

            def.BodyFrame   = mount.bodyFrame  ;
            def.TotalFrames = mount.totalFrames;

            def.FallDamageMult = mount.fallDamage  ;
            def.Acceleration   = mount.acceleration;
            def.DashSpeed      = mount.dashSpeed   ;
            def.JumpSpeed      = mount.jumpSpeed   ;
            def.RunSpeed       = mount.runSpeed    ;
            def.SwimSpeed      = mount.swimSpeed   ;

            def.BlockExtraJumps       = mount.blockExtraJumps              ;
            def.ConstantJump          = mount.constantJump                 ;
            def.EmitsLight            = mount.emitsLight                   ;
            def.IdleFrameLoop         = mount.idleFrameLoop                ;
            def.IsMinecart            = mount.Minecart | MountSets.Cart[id];
            def.IsDirectionalMinecart = mount.MinecartDirectional          ;
            def.SpawnDustNoGravity    = mount.spawnDustNoGravity           ;
            def.UsesHover             = mount.usesHover                    ;

            def.PlayerYOffset = mount.playerYOffsets;

            def.MinecartDust = mount.MinecartDust;

            def.Offset = new Point(mount.xOffset, mount.yOffset);
            def.LightColour = new Color(mount.lightColor);

            def.Dashing  = new MountFrameData(mount.dashingFrameCount , mount.dashingFrameDelay , mount.dashingFrameStart );
            def.Flying   = new MountFrameData(mount.flyingFrameCount  , mount.flyingFrameDelay  , mount.flyingFrameStart  );
            def.Idle     = new MountFrameData(mount.idleFrameCount    , mount.idleFrameDelay    , mount.idleFrameStart    );
            def.InAir    = new MountFrameData(mount.inAirFrameCount   , mount.inAirFrameDelay   , mount.inAirFrameStart   );
            def.Running  = new MountFrameData(mount.runningFrameCount , mount.runningFrameDelay , mount.runningFrameStart );
            def.Standing = new MountFrameData(mount.standingFrameCount, mount.standingFrameDelay, mount.standingFrameStart);
            def.Swim     = new MountFrameData(mount.swimFrameCount    , mount.swimFrameDelay    , mount.swimFrameStart    );

            def.Buff       = new BuffRef(mount.buff     );
            def.ExpertBuff = new BuffRef(mount.extraBuff);

            def.Front = new MountTextureData(
                () => mount.frontTexture,
                () => mount.frontTextureExtra,
                () => mount.frontTextureGlow,
                () => mount.frontTextureExtraGlow
            );
            def.Back  = new MountTextureData(
                () => mount.backTexture,
                () => mount.backTextureExtra,
                () => mount.backTextureGlow,
                () => mount.backTextureExtraGlow
            );
        }
        void CopyDefToEntity(MountDef def, ref Mount.MountData mount)
        {
            if (mount == null)
                mount = new Mount.MountData();

            mount.abilityChargeMax = def.MaxAbilityCharge;
            mount.abilityCooldown  = def.AbilityCooldown ;
            mount.abilityDuration  = def.AbilityDuration ;

            mount.heightBoost      = def.AdditionalHeight;
            mount.playerHeadOffset = def.PlayerHeadOffset;

            mount.fatigueMax    = def.MaxFatigue   ;
            mount.flightTimeMax = def.MaxFlightTime;
            mount.jumpHeight    = def.JumpHeight   ;
            mount.spawnDust     = def.SpawnDust    ;

            mount.bodyFrame   = def.BodyFrame  ;
            mount.totalFrames = def.TotalFrames;

            mount.fallDamage   = def.FallDamageMult;
            mount.acceleration = def.Acceleration  ;
            mount.dashSpeed    = def.DashSpeed     ;
            mount.jumpSpeed    = def.JumpSpeed     ;
            mount.runSpeed     = def.RunSpeed      ;
            mount.swimSpeed    = def.SwimSpeed     ;

            mount.blockExtraJumps     = def.BlockExtraJumps      ;
            mount.constantJump        = def.ConstantJump         ;
            mount.emitsLight          = def.EmitsLight           ;
            mount.idleFrameLoop       = def.IdleFrameLoop        ;
            mount.Minecart            = def.IsMinecart           ;
            mount.MinecartDirectional = def.IsDirectionalMinecart;
            mount.spawnDustNoGravity  = def.SpawnDustNoGravity   ;
            mount.usesHover           = def.UsesHover            ;

            mount.playerYOffsets = def.PlayerYOffset;

            mount.MinecartDust = def.MinecartDust;

            mount.xOffset = def.Offset.X;
            mount.yOffset = def.Offset.Y;
            mount.lightColor = def.LightColour.ToVector3();

            mount.dashingFrameCount  = def.Dashing.Count ;
            mount.dashingFrameDelay  = def.Dashing.Delay ;
            mount.dashingFrameStart  = def.Dashing.Start ;
            mount.flyingFrameCount   = def.Flying.Count  ;
            mount.flyingFrameDelay   = def.Flying.Delay  ;
            mount.flyingFrameStart   = def.Flying.Start  ;
            mount.idleFrameCount     = def.Idle.Count    ;
            mount.idleFrameDelay     = def.Idle.Delay    ;
            mount.idleFrameStart     = def.Idle.Start    ;
            mount.inAirFrameCount    = def.InAir.Count   ;
            mount.inAirFrameDelay    = def.InAir.Delay   ;
            mount.inAirFrameStart    = def.InAir.Start   ;
            mount.runningFrameCount  = def.Running.Count ;
            mount.runningFrameDelay  = def.Running.Delay ;
            mount.runningFrameStart  = def.Running.Start ;
            mount.standingFrameCount = def.Standing.Count;
            mount.standingFrameDelay = def.Standing.Delay;
            mount.standingFrameStart = def.Standing.Start;
            mount.swimFrameCount     = def.Swim.Count    ;
            mount.swimFrameDelay     = def.Swim.Delay    ;
            mount.swimFrameStart     = def.Swim.Start    ;

            mount.buff      =                    def.Buff .Resolve().Type;
            mount.extraBuff = (def.ExpertBuff ?? def.Buff).Resolve().Type;

            mount.frontTexture          = def.Front.GetNormal   ();
            mount.frontTextureExtra     = def.Front.GetExtra    ();
            mount.frontTextureGlow      = def.Front.GetGlow     ();
            mount.frontTextureExtraGlow = def.Front.GetExtraGlow();

            mount.backTexture          = def.Back.GetNormal   ();
            mount.backTextureExtra     = def.Back.GetExtra    ();
            mount.backTextureGlow      = def.Back.GetGlow     ();
            mount.backTextureExtraGlow = def.Back.GetExtraGlow();

            mount.textureWidth  = def.texSize.X;
            mount.textureHeight = def.texSize.Y;
        }
        void CopySetProperties(MountDef def)
        {
            MountSets.Cart[def.Type] = def.IsMinecart;
        }

        internal void FillVanilla()
        {
            if (!hasReset)
            {
                Reset();
                hasReset = true;
            }

            FillingVanilla = true;

            Mount.Initialize();

            MountDef def;

            for (int id = MinVanillaID; id < MaxVanillaID; id++)
            {
                if (id == -1)
                    continue;

                var index = Array.IndexOf(IDValues, id);
                if (index == -1)
                    continue;

                def = new MountDef(new ObjectName(IDNames[index]), null);

                DefsByType.Add(id, def);
                VanillaDefsByName.Add(IDNames[index], def);

                def.Mod = PrismApi.VanillaInfo;

                CopyEntityToDef(id, Mount.mounts[id], def); // TEntityDef is a class -> dictionary entries are updated, too

                def.InternalName = IDNames[index];
            }

            FillingVanilla = false;
        }

        internal void Reset()
        {
            ExtendVanillaArrays(-1);

            Mount.Initialize();

            NextTypeIndex = MaxVanillaID;

            DefsByType.Clear();
        }

        List<LoaderError> CheckTextures(MountDef def)
        {
            var ret = new List<LoaderError>();

            if (def.Front.Normal == null && def.Back.Normal == null)
                ret.Add(new LoaderError(def.Mod, "Front.Normal and Back.Normal of MountDef " + def + " are both null."));

            return ret;
        }
        List<LoaderError> LoadTextures(MountDef def)
        {
            var ret = new List<LoaderError>();

            Texture2D main = null;
            if (def.Back.Normal != null)
            {
                var t = def.Back.Normal();

                if (t == null)
                    ret.Add(new LoaderError(def.Mod, "Return value of Front.Back of MountDef " + def + " is null."));

                main = t;
            }
            if (def.Front.Normal != null)
            {
                var t = def.Front.Normal();

                if (t == null && main == null) // don't return it if the other error has been returned, too
                    ret.Add(new LoaderError(def.Mod, "Return value of Front.Normal of MountDef " + def + " is null."));

                if (t != null)
                    main = t;
            }

            if (main != null)
                def.texSize = new Point(main.Width, main.Height);

            return ret;
        }

        internal IEnumerable<LoaderError> Load(Dictionary<string, MountDef> dict)
        {
            var err = new List<LoaderError>();

            ExtendVanillaArrays(dict.Count);

            foreach (var def in dict.Values)
            {
                if (def.Buff == null)
                {
                    err.Add(new LoaderError(def.Mod, "Buff of MountDef " + def + " is null."));
                    continue;
                }
                if (def.PlayerYOffset.Length != def.TotalFrames)
                {
                    err.Add(new LoaderError(def.Mod, "PlayerYOffset.Length of MountDef " + def + " is not equal to TotalFrames."));
                    continue;
                }

                if (!Main.dedServ)
                {
                    var cterrs = CheckTextures(def);

                    if (cterrs.Count > 0)
                    {
                        err.AddRange(cterrs);
                        continue;
                    }
                }

                def.Type = NextTypeIndex;

                if (!Main.dedServ)
                {
                    var lterrs = LoadTextures(def);

                    if (lterrs.Count > 0)
                    {
                        err.AddRange(lterrs);
                        continue;
                    }
                }

                CopyDefToEntity(def, ref Mount.mounts[def.Type]);
                CopySetProperties(def);
                DefsByType.Add(NextTypeIndex++, def);
            }

            return err;
        }
    }
}

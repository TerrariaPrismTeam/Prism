using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prism.Util;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class CraftGroup<TDef, TRef> : IList<TRef>
        where TDef : ObjectDef
        where TRef : EntityRef<TDef>
    {
        readonly static string  OR =  " or ";
        readonly static string COR = ", or ";
        readonly static string COM = ", "   ;

        ObjectName dispName, defDn;
        TRef dispT, defDt;
        TDef dispTcache = null;

        List<TRef> list;

        public TRef DisplayT
        {
            get
            {
                if (dispT == null)
                    dispT = defDt ?? list.FirstOrDefault();

                return dispT;
            }
        }
        public TDef CachedDisplayT
        {
            get
            {
                if (dispTcache == null)
                    dispTcache = DisplayT.Resolve();

                return dispTcache;
            }
        }
        public ObjectName DisplayName
        {
            get
            {
                if (String.IsNullOrEmpty(dispName.ToString()))
                    // a
                    // a or b
                    // a, b[...] or c
                    dispName = String.IsNullOrEmpty(defDn.ToString())
                        ? new ObjectName(list.Select(r =>
                                    CachedDisplayT.DisplayName.ToString()).Join(i =>
                            i == 0 ? String.Empty : i == list.Count - 1
                                ? (list.Count == 2 ? OR : COR) : COM))
                        : defDn;

                return dispName;
            }
        }

        public IList<TRef> List
        {
            get
            {
                return this;
            }
        }

        public CraftGroup(IEnumerable<TRef> initial)
        {
            list = new List<TRef>(initial);
        }
        public CraftGroup(IEnumerable<TRef> initial, ObjectName displayName, TRef displayT = null)
        {
            list = new List<TRef>(initial);

            defDn = displayName;
            defDt = displayT;
        }

        void Invalidate()
        {
            dispT    = null;
            dispName = ObjectName.Empty;
        }

        #region IList impl
        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public TRef this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;

                Invalidate();
            }
        }

        public void Add(TRef item)
        {
            list.Add(item);

            Invalidate();
        }
        public void Insert(int index, TRef item)
        {
            list.Insert(index, item);

            Invalidate();
        }

        public bool Remove(TRef item)
        {
            if (list.Remove(item))
            {
                Invalidate();

                return true;
            }

            return false;
        }
        public void RemoveAt(int index)
        {
            list.RemoveAt(index);

            Invalidate();
        }

        public void Clear()
        {
            list.Clear();

            Invalidate();
        }

        public int IndexOf(TRef item)
        {
            return list.IndexOf(item);
        }
        public bool Contains(TRef item)
        {
            return list.Contains(item);
        }

        public void CopyTo(TRef[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TRef> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
        #endregion

        public static implicit operator Either<TRef, CraftGroup<TDef, TRef>>(CraftGroup<TDef, TRef> g)
        {
            return Either<TRef, CraftGroup<TDef, TRef>>.NewLeft(g);
        }
    }

    public class ItemGroup : CraftGroup<ItemDef, ItemRef>
    {
        public ItemGroup(IEnumerable<ItemRef> initial)
            : base(initial)
        {

        }
        public ItemGroup(IEnumerable<ItemRef> initial, ObjectName displayName, ItemRef displayT = null)
            : base(initial, displayName, displayT)
        {

        }

        #region predefined groups
        // TODO: translate these
        public static ItemGroup Wood = new ItemGroup(new[]
        {
            new ItemRef(ItemID.Wood        ),
            new ItemRef(ItemID.RichMahogany),
            new ItemRef(ItemID.Ebonwood    ),
            new ItemRef(ItemID.Pearlwood   ),
            new ItemRef(ItemID.Shadewood   ),
            new ItemRef(ItemID.BorealWood  ),
            new ItemRef(ItemID.DynastyWood ),
            new ItemRef(ItemID.PalmWood    ),
            new ItemRef(ItemID.SpookyWood  )
        }, new ObjectName("Any wood"));
        public static ItemGroup Sand = new ItemGroup(new[]
        {
            new ItemRef(ItemID.SandBlock     ),
            new ItemRef(ItemID.EbonsandBlock ),
            new ItemRef(ItemID.PearlsandBlock),
            new ItemRef(ItemID.CrimsandBlock )
        }, new ObjectName("Any sand"));
        public static ItemGroup Stone = new ItemGroup(new[]
        {
            new ItemRef(ItemID.StoneBlock     ),
            new ItemRef(ItemID.EbonstoneBlock ),
            new ItemRef(ItemID.PearlstoneBlock),
            new ItemRef(ItemID.CrimstoneBlock )
        }, new ObjectName("Any stone"));
        public static ItemGroup Fragment = new ItemGroup(new[]
        {
            new ItemRef(ItemID.FragmentSolar   ),
            new ItemRef(ItemID.FragmentVortex  ),
            new ItemRef(ItemID.FragmentNebula  ),
            new ItemRef(ItemID.FragmentStardust)
        }, new ObjectName("Any fragment"));
        public static ItemGroup PressurePlate = new ItemGroup(new[]
        {
            new ItemRef(ItemID.GrayPressurePlate    ),
            new ItemRef(ItemID.BrownPressurePlate   ),
            new ItemRef(ItemID.LihzahrdPressurePlate),
            new ItemRef(ItemID.RedPressurePlate     ),
            new ItemRef(ItemID.GreenPressurePlate   ),
            new ItemRef(ItemID.YellowPressurePlate  )
        }, new ObjectName("Any pressure plate"));
        public static ItemGroup Tier1Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.CopperBar),
            new ItemRef(ItemID.TinBar   )
        });
        public static ItemGroup Tier2Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.IronBar),
            new ItemRef(ItemID.LeadBar)
        });
        public static ItemGroup Tier3Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.SilverBar  ),
            new ItemRef(ItemID.TungstenBar)
        });
        public static ItemGroup Tier4Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.GoldBar    ),
            new ItemRef(ItemID.PlatinumBar)
        });
        public static ItemGroup EvilBar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.DemoniteBar),
            new ItemRef(ItemID.CrimtaneBar)
        });
        public static ItemGroup HmTier1Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.CobaltBar   ),
            new ItemRef(ItemID.PalladiumBar)
        });
        public static ItemGroup HmTier2Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.MythrilBar   ),
            new ItemRef(ItemID.OrichalcumBar)
        });
        public static ItemGroup HmTier3Bar = new ItemGroup(new[]
        {
            new ItemRef(ItemID.AdamantiteBar),
            new ItemRef(ItemID.TitaniumBar  )
        });

        public static ItemGroup Birds = new ItemGroup(new[]
        {
            new ItemRef(ItemID.Bird    ),
            new ItemRef(ItemID.BlueJay ),
            new ItemRef(ItemID.Cardinal)
        }, new ObjectName("Bird"));
        public static ItemGroup Scorpions = new ItemGroup(new[]
        {
            new ItemRef(ItemID.BlackScorpion),
            new ItemRef(ItemID.Scorpion     )
        }, new ObjectName("Scorpion"));
        public static ItemGroup Squirrels = new ItemGroup(new[]
        {
            new ItemRef(ItemID.Squirrel   ),
            new ItemRef(ItemID.SquirrelRed)
        }, new ObjectName("Squirrel"));
        public static ItemGroup Bugs = new ItemGroup(new[]
        {
            new ItemRef(ItemID.Grubby),
            new ItemRef(ItemID.Sluggy),
            new ItemRef(ItemID.Buggy ),
        }, new ObjectName("Bug"));
        public static ItemGroup Ducks = new ItemGroup(new[]
        {
            new ItemRef(ItemID.MallardDuck),
            new ItemRef(ItemID.Duck       )
        }, new ObjectName("Duck"));
        public static ItemGroup Butterflies = new ItemGroup(new[]
        {
            new ItemRef(ItemID.MonarchButterfly         ),
            new ItemRef(ItemID.PurpleEmperorButterfly   ),
            new ItemRef(ItemID.RedAdmiralButterfly      ),
            new ItemRef(ItemID.UlyssesButterfly         ),
            new ItemRef(ItemID.SulphurButterfly         ),
            new ItemRef(ItemID.TreeNymphButterfly       ),
            new ItemRef(ItemID.ZebraSwallowtailButterfly),
            new ItemRef(ItemID.JuliaButterfly           )
        });
        public static ItemGroup Fireflies = new ItemGroup(new[]
        {
            new ItemRef(ItemID.Firefly     ),
            new ItemRef(ItemID.LightningBug)
        }, new ObjectName("Firefly"));
        public static ItemGroup Snails = new ItemGroup(new[]
        {
            new ItemRef(ItemID.Snail       ),
            new ItemRef(ItemID.GlowingSnail)
        }, new ObjectName("Snail"));

        public static ItemGroup GoldCritters = new ItemGroup(new[]
        {
            new ItemRef(ItemID.GoldBird       ),
            new ItemRef(ItemID.GoldBunny      ),
            new ItemRef(ItemID.GoldButterfly  ),
            new ItemRef(ItemID.GoldenCarp     ),
            new ItemRef(ItemID.GoldFrog       ),
            new ItemRef(ItemID.GoldGrasshopper),
            new ItemRef(ItemID.GoldMouse      ),
            new ItemRef(ItemID.GoldWorm       ),
            new ItemRef(ItemID.SquirrelGold   )
        }, new ObjectName("Gold critter"));
        #endregion
    }
    public class TileGroup : CraftGroup<TileDef, TileRef>
    {
        public TileGroup(IEnumerable<TileRef> initial)
            : base(initial)
        {

        }
        public TileGroup(IEnumerable<TileRef> initial, ObjectName displayName, TileRef displayT = null)
            : base(initial, displayName, displayT)
        {

        }

        //TODO: predefined groups (wood, stone, grass, sand, ...)
    }
}

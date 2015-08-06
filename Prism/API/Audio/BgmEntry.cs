using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.API.Audio
{
    public enum BgmPriority
    {
        /// <summary>
        /// Ambient background stuff. Always playing when not in the game menu (and ShouldPlay returns true).
        /// </summary>
        Ambient = -1,
        /// <summary>
        /// Environmental, low-priority music.
        /// </summary>
        /// <example>Underground Hallow, Underground Desert, Underground, Blood Moon, Rain, Night, Hallow, Desert, Day</example>
        Environment,
        /// <summary>
        /// Biome-bound, medium-priority music. Overrides environmental music.
        /// </summary>
        /// <example>Underworld, Eclipse, Space, Lihzahrd Temple, Glowing Mushrooms, (Underground) Corruption, (Underground) Crimson, Dungeon, Meteor (Eerie), Jungle, (Underground) Snow</example>
        Biome,
        /// <summary>
        /// Boss- or invasion-bound, high-priority music. Overrides biome-bound and environmental music.
        /// </summary>
        /// <example>Moon Lord, Martian Madness, Lunar Pillar, Plantera, Boss 2 (Wall of Flesh, The Twins), Boss 1 (default boss music: Eye of Cthulhu, Eater of Worlds, Skeletron, Skeletron Prime), Boss 3 (The Destroyer, Brain of Cthulhu, Frost Legion), Golem, Queen Bee, Pirates, Goblin Army</example>
        Boss,
        /// <summary>
        /// Event-bound, even higher-priority music. Overrides boss-, biome-bound and environmental music.
        /// </summary>
        /// <example>Frost Moon, Pumpkin Moon</example>
        Event,
        /// <summary>
        /// Music that plays when a music box is active. Overrides all other ingame priority classes.
        /// </summary>
        MusicBox,
        /// <summary>
        /// Music that plays on the title screen. Has the highest priority. Only BGMs marked as 'Title' will be played on the title screen, and never ingame.
        /// </summary>
        Title
    }

    public class BgmEntry
    {
        internal float fade;

        public IBgm Music
        {
            get;
            private set;
        }
        public BgmPriority Priority
        {
            get;
            private set;
        }
        public Func<bool> ShouldPlay
        {
            get;
            private set;
        }

        public BgmEntry(IBgm music, BgmPriority priority, Func<bool> play)
        {
            Music = music;
            Priority = priority;
            ShouldPlay = play;
        }
    }
}

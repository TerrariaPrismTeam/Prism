using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Prism.Util;

namespace Prism.API
{
    // NOTE: this is written in such a way that the amount of heap
    // allocations is minimised. Please beware when editing.

    /// <summary>
    /// The name of a Prism object, providing translations.
    /// </summary>
    public struct ObjectName
    {
        // change this if you don't like it
        internal readonly static GameCulture
            InvariantCulture = GameCulture.English;

        readonly static string BACKUP_TOSTR = "<unknown object>";

        public static readonly ObjectName Empty = new ObjectName(BACKUP_TOSTR);

        /// <summary>
        /// Gets the non-translated string of the object name.
        /// <para />
        /// Note: it is usually a better idea to cast the ObjectName to
        /// a string instead of using this directly.
        /// </summary>
        public string NonTranslatedString;
        /// <summary>
        /// Gets the translation strings of the object name.
        /// <para />
        /// Note: it is usually a better idea to cast the ObjectName to
        /// a string or to use the indexer instead of using this directly.
        /// </summary>
        public IDictionary<GameCulture, string> Translations;

        /// <summary>
        /// Gets the translated string, given a GameCulture.
        /// </summary>
        public string this[GameCulture gc]
        {
            get
            {
                string t = null;
                if (Translations != null && Translations.TryGetValue(gc, out t))
                    return t;

                return NonTranslatedString ?? BACKUP_TOSTR;
            }
        }

        public string CultureInvariantString
        {
            get
            {
                return this[InvariantCulture];
            }
        }

        public ObjectName(IDictionary<GameCulture, string> translations)
        {
            Translations = translations;
            NonTranslatedString = null;

            if (!translations.TryGetValue(InvariantCulture, out NonTranslatedString))
                using (IEnumerator<string> e = translations.Values.GetEnumerator())
                {
                    NonTranslatedString = e.Current;
                }
        }
        public ObjectName(IEnumerable<KeyValuePair<GameCulture, string>> translations)
        {
            Translations = translations.ToDictionary();
            NonTranslatedString = null;

            if (!Translations.TryGetValue(InvariantCulture, out NonTranslatedString))
                using (IEnumerator<KeyValuePair<GameCulture, string>> e = translations.GetEnumerator())
                {
                    NonTranslatedString = e.Current.Value;
                }
        }
        // for the lazy people out there~
        public ObjectName(string nonTranslated)
        {
            Translations = null;
            NonTranslatedString = nonTranslated;
        }
        public ObjectName(LocalizedText lt)
            : this(lt.Value)
        {

        }

        public override string ToString()
        {
            return this[Language.ActiveCulture];
        }
        public LocalizedText ToLocalization()
        {
            return new LocalizedText(String.Empty, this[Language.ActiveCulture]);
        }

        public static explicit operator string(ObjectName objn)
        {
            return objn.ToString();
        }
        public static explicit operator ObjectName(string nonTranslated)
        {
            return new ObjectName(nonTranslated);
        }

        public static explicit operator LocalizedText(ObjectName objn)
        {
            return objn.ToLocalization();
        }
    }
}


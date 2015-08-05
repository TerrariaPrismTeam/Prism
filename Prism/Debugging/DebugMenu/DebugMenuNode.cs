
#if DEV_BUILD

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Prism.Debugging
{
    /// <summary>
    /// MY CODEMAID WONT WORK IM SORRY
    /// </summary>
    public class DebugMenuNode : IDictionary<string, DebugMenuNode>
    {
        static Dictionary<Type, List<FieldInfo>> ReflectedFields = new Dictionary<Type, List<FieldInfo>>();

        int _recursiveChildCount = 0;
        int _recursiveChildVisibleCount = 0;

        int visibleIndex = 0;
        int height = 1;
        int indent = 0;

        bool isSelected = false;
        bool isExpanded = false;

        string key = "[NO_KEY]";

        Color colour = Color.White;
        Dictionary<string, DebugMenuNode> Children = new Dictionary<string, DebugMenuNode>();

        public Color Colour = Color.White;

        public DebugMenuNode()
        {

        }
        public DebugMenuNode(string key)
        {
            Key = key;
        }

        public int RecursiveChildVisibleCount
        {
            get
            {
                return _recursiveChildVisibleCount;
            }
            private set
            {
                if (Parent != null)
                {
                    Parent.RecursiveChildVisibleCount += (value - _recursiveChildVisibleCount);
                }
                _recursiveChildVisibleCount = value;
            }
        }
        public int RecursiveChildCount
        {
            get
            {
                return _recursiveChildCount;
            }
            private set
            {
                if (Parent != null)
                {
                    Parent.RecursiveChildCount += (value - _recursiveChildCount);
                }
                _recursiveChildCount = value;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                int change = (value - height);
                height = value;
                FindChildVisibleIndeces();
                if (Parent != null)
                    Parent.Height += change;
            }
        }
        public int VisibleIndex
        {
            get
            {
                return visibleIndex;
            }
            private set
            {
                visibleIndex = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            private set
            {
                isExpanded = value;
            }
        }
        public bool IsVisible
        {
            get
            {
                return (Parent != null) ? Parent.isExpanded : true;
            }
        }

        public int Indent
        {
            get
            {
                return indent;
            }
            private set
            {
                indent = value;
            }
        }

        public string Key
        {
            get
            {
                return key;
            }
            private set
            {
                key = value;
            }
        }

        public DebugMenuNode Parent
        {
            get;
            private set;
        }

        public object DebugValue
        {
            get;
            set;
        }

        void ModChildCountsInc(int amt = 1)
        {
            if (amt < 1)
                amt = 1;
            RecursiveChildCount += amt;
            if (IsExpanded)
                RecursiveChildVisibleCount += amt;
        }
        void ModChildCountsDec(int amt = 1)
        {
            if (amt < 1)
                amt = 1;
            RecursiveChildCount -= amt;
            if (IsExpanded)
                RecursiveChildVisibleCount -= amt;
        }

        void FindChildVisibleIndeces()
        {
            int i = Height;
            foreach (var entry in this)
            {
                entry.Value.VisibleIndex = VisibleIndex + i;
                i += entry.Value.Height;
            }
        }

        void AdoptChild(string key)
        {
            this[key].Parent = this;
            this[key].Key = key;
            this[key].Colour = Colour;
            this[key].Indent = this.Indent + 1;

            ModChildCountsInc();
        }
        void DisownChild(int amt = 1)
        {
            if (amt < 1)
                amt = 1;
            ModChildCountsDec(amt);
        }

        public void Expand(bool recursive = false)
        {
            if (!isExpanded)
            {
                RecursiveChildVisibleCount += Count;
                int i = 1;
                foreach (var entry in this)
                {
                    if (recursive)
                        entry.Value.Expand(recursive);

                    entry.Value.VisibleIndex = VisibleIndex + i;
                    i++;
                }
                isExpanded = true;
            }
        }
        public void Collapse(bool recursive = false)
        {
            if (isExpanded)
            {
                RecursiveChildVisibleCount -= Count;
                if (recursive)
                {
                    foreach (var entry in this)
                    {
                        entry.Value.Collapse(recursive);
                    }
                }
                isExpanded = false;
            }
        }

        public void EnsureChild(string key)
        {
            if (!ContainsKey(key))
            {
                Add(key, new DebugMenuNode());
                AdoptChild(key);
            }
        }

        public void EnsureReflectionFields(Type t)
        {
            if (!ReflectedFields.ContainsKey(t))
            {
                var fields = t.GetFields();
                var list = new List<FieldInfo>();

                foreach (FieldInfo f in fields)
                {
                    if (!f.IsInitOnly && !f.IsLiteral)
                    {
                        list.Add(f);
                    }
                }

                ReflectedFields.Add(t, list);
            }
        }

        public object Reflect(object obj, Action<FieldInfo, object> postChange)
        {
            Type t = obj.GetType();
            EnsureReflectionFields(t);

            foreach (FieldInfo f in ReflectedFields[t])
            {
                var origVal = f.GetValue(obj);
                if (origVal == null)
                {
                    this[f.Name].DebugValue = null;
                    continue;
                }
                var val = this[f.Name].UpdateValue(origVal, null, x => postChange(f, x));
                if (!origVal.GetType().IsArray && val.GetType() != origVal.GetType() && val is IConvertible && origVal is IConvertible)
                    val = Convert.ChangeType(val, origVal.GetType());
                try
                {
                    f.SetValue(obj, val);
                }
                catch
                {
                    //asdf
                }
            }

            return obj;
        }

        public static object ICantBelieveIWroteThisMethod(dynamic start, params dynamic[] nums)
        {
            dynamic product = 1;

            for (int i = 0; i < nums.Length; i++)
            {
                product *= nums[i];
            }

            return start + product;
        }

        public object UpdateValue(object dbgVal, object baseAdjustAmt = null, Action<object> postChange = null, bool adjustable = true)
        {
            DebugValue = dbgVal;

            if (dbgVal == null)
                return dbgVal;

            if (dbgVal.GetType().IsArray)
            {
                object[] arr = null;
                //asdfasdfasdfasdfasdfasdfasdf
                if      (dbgVal is   bool[]) arr = Array.ConvertAll((  bool[])dbgVal, x => (object)x);
                else if (dbgVal is   byte[]) arr = Array.ConvertAll((  byte[])dbgVal, x => (object)x);
                else if (dbgVal is  sbyte[]) arr = Array.ConvertAll(( sbyte[])dbgVal, x => (object)x);
                else if (dbgVal is ushort[]) arr = Array.ConvertAll((ushort[])dbgVal, x => (object)x);
                else if (dbgVal is  short[]) arr = Array.ConvertAll(( short[])dbgVal, x => (object)x);
                else if (dbgVal is   uint[]) arr = Array.ConvertAll((  uint[])dbgVal, x => (object)x);
                else if (dbgVal is    int[]) arr = Array.ConvertAll((   int[])dbgVal, x => (object)x);
                else if (dbgVal is  ulong[]) arr = Array.ConvertAll(( ulong[])dbgVal, x => (object)x);
                else if (dbgVal is   long[]) arr = Array.ConvertAll((  long[])dbgVal, x => (object)x);
                else if (dbgVal is  float[]) arr = Array.ConvertAll(( float[])dbgVal, x => (object)x);
                else if (dbgVal is double[]) arr = Array.ConvertAll((double[])dbgVal, x => (object)x);
                else if (dbgVal is object[])
                    arr = (object[])dbgVal;

                if (arr != null)
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = this["[" + i + "]"].UpdateValue(arr[i]);

                try
                {
                    if      (dbgVal is   bool[]) dbgVal = Array.ConvertAll(arr, x => (  bool)x);
                    else if (dbgVal is   byte[]) dbgVal = Array.ConvertAll(arr, x => (  byte)x);
                    else if (dbgVal is  sbyte[]) dbgVal = Array.ConvertAll(arr, x => ( sbyte)x);
                    else if (dbgVal is ushort[]) dbgVal = Array.ConvertAll(arr, x => (ushort)x);
                    else if (dbgVal is  short[]) dbgVal = Array.ConvertAll(arr, x => ( short)x);
                    else if (dbgVal is   uint[]) dbgVal = Array.ConvertAll(arr, x => (  uint)x);
                    else if (dbgVal is    int[]) dbgVal = Array.ConvertAll(arr, x => (   int)x);
                    else if (dbgVal is  ulong[]) dbgVal = Array.ConvertAll(arr, x => ( ulong)x);
                    else if (dbgVal is   long[]) dbgVal = Array.ConvertAll(arr, x => (  long)x);
                    else if (dbgVal is  float[]) dbgVal = Array.ConvertAll(arr, x => ( float)x);
                    else if (dbgVal is double[]) dbgVal = Array.ConvertAll(arr, x => (double)x);
                    else if (dbgVal is object[])
                        dbgVal = arr;
                }
                catch
                {
                    //asdfasdfasdfasdfasdf
                }

                DebugValue = dbgVal;
                return dbgVal;
            }
            else if (dbgVal is string)
            {
                DebugValue = dbgVal;
                return dbgVal;
            }
            else if (dbgVal is IEnumerable)
            {
                var enumerable = (IEnumerable)dbgVal;
                List<object> newList = new List<object>();

                int i = 0;

                foreach (var e in enumerable)
                {
                    newList.Add(this["[" + i++ + "]"].UpdateValue(e));
                }

                DebugValue = dbgVal = (IEnumerable<object>)newList;
                return dbgVal;
            }

            int adjustDir = ((DebugMenu.Nav & Ctrl.Left) == Ctrl.Left && IsSelected) ? -1 : ((DebugMenu.Nav & Ctrl.Right) == Ctrl.Right && IsSelected) ? 1 : 0;

            Type t = dbgVal.GetType();

            if (baseAdjustAmt != null && t != baseAdjustAmt.GetType())
                throw new ArgumentException("baseAdjustAmt must either be null or a value of the same type as dbgVal", "baseAdjustAmt");

            if (t == typeof(bool) && adjustDir != 0)
            {
                dbgVal = !((bool)dbgVal);
                DebugValue = dbgVal;
                return dbgVal;
            }

            if (baseAdjustAmt == null)
                baseAdjustAmt = 1d;
            double modAdj = (DebugMenu.DbgModMult * Convert.ToDouble(baseAdjustAmt) * adjustDir);

            if      (t == typeof(  byte)) dbgVal = (  byte)DebugNodeHelper.ModifyValue<  byte>((  byte)dbgVal, modAdj);
            else if (t == typeof( sbyte)) dbgVal = ( sbyte)DebugNodeHelper.ModifyValue< sbyte>(( sbyte)dbgVal, modAdj);
            else if (t == typeof(ushort)) dbgVal = (ushort)DebugNodeHelper.ModifyValue<ushort>((ushort)dbgVal, modAdj);
            else if (t == typeof( short)) dbgVal = ( short)DebugNodeHelper.ModifyValue< short>(( short)dbgVal, modAdj);
            else if (t == typeof(  uint)) dbgVal = (  uint)DebugNodeHelper.ModifyValue<  uint>((  uint)dbgVal, modAdj);
            else if (t == typeof(   int)) dbgVal = (   int)DebugNodeHelper.ModifyValue<   int>((   int)dbgVal, modAdj);
            else if (t == typeof( ulong)) dbgVal = ( ulong)DebugNodeHelper.ModifyValue< ulong>(( ulong)dbgVal, modAdj);
            else if (t == typeof(  long)) dbgVal = (  long)DebugNodeHelper.ModifyValue<  long>((  long)dbgVal, modAdj);
            else if (t == typeof( float)) dbgVal = ( float)DebugNodeHelper.ModifyValue< float>(( float)dbgVal, modAdj);
            else if (t == typeof(double)) dbgVal = (double)DebugNodeHelper.ModifyValue<double>((double)dbgVal, modAdj);
            else
            {
                dbgVal = Reflect(dbgVal, (x, y) =>
                {
                    //asdf
                });
            }

            DebugValue = dbgVal;
            return dbgVal;
        }

        internal int Draw(SpriteBatch sb, SpriteFont font, Vector2 startPos, int currentIndex = 0)
        {
            VisibleIndex = currentIndex;

            if (DebugMenu.DebugSelection == VisibleIndex)
                DebugMenu.SelectedNode = this;

            IsSelected = DebugMenu.SelectedNode == this;

            if (Parent != null && !Parent.isExpanded)
                return currentIndex;

            sb.DrawString(font, DrawText, startPos + new Vector2(0, (VisibleIndex * (font.LineSpacing / 2f)) - (DebugMenu.DebugSelection * font.LineSpacing / 2f)).Floor(), (IsSelected && DebugMenu.IsOpen ? DebugMenu.MainDiscoColor : Colour) * (DebugMenu.IsOpen || IsSelected ? 1.0f : 0.5f), 0, new Vector2(0, font.LineSpacing / 2).Floor(), 0.5f, SpriteEffects.None, 0);

            if (IsExpanded)
                foreach (DebugMenuNode d in Values)
                    currentIndex = d.Draw(sb, font, startPos, ++currentIndex);

            return currentIndex;
        }

        #region IDictionary stuff
        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, DebugMenuNode>)Children).ContainsKey(key);
        }

        public void Add(string key, DebugMenuNode value)
        {
            ((IDictionary<string, DebugMenuNode>)Children).Add(key, value);

            AdoptChild(key);
        }

        public bool Remove(string key)
        {
            if (((IDictionary<string, DebugMenuNode>)Children).Remove(key))
            {
                DisownChild();
                return true;
            }
            return false;
        }

        public bool TryGetValue(string key, out DebugMenuNode value)
        {
            return ((IDictionary<string, DebugMenuNode>)Children).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, DebugMenuNode> item)
        {
            AdoptChild(item.Key);

            ((IDictionary<string, DebugMenuNode>)Children).Add(item);
        }

        public void Clear()
        {
            DisownChild(Count);

            ((IDictionary<string, DebugMenuNode>)Children).Clear();
        }

        public bool Contains(KeyValuePair<string, DebugMenuNode> item)
        {
            return ((IDictionary<string, DebugMenuNode>)Children).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, DebugMenuNode>[] array, int arrayIndex)
        {
            ((IDictionary<string, DebugMenuNode>)Children).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, DebugMenuNode> item)
        {
            if (((IDictionary<string, DebugMenuNode>)Children).Remove(item))
            {
                DisownChild();
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<string, DebugMenuNode>> GetEnumerator()
        {
            return ((IDictionary<string, DebugMenuNode>)Children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, DebugMenuNode>)Children).GetEnumerator();
        }

        public string DrawText
        {
            get
            {
                return (new string(' ', 4 * Indent)
                    + (Count > 0 ? (IsExpanded ? "[-]" : "[+]") : "")
                    + Key
                    + (DebugValue != null ? "<" + DebugValue.GetType().Name + "> = " + DebugValue.ToString() : "")
                    + (Count > 0 ? " (" + Count + ")" : ""));
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, DebugMenuNode>)Children).Keys;
            }
        }

        public ICollection<DebugMenuNode> Values
        {
            get
            {
                return ((IDictionary<string, DebugMenuNode>)Children).Values;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<string, DebugMenuNode>)Children).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, DebugMenuNode>)Children).IsReadOnly;
            }
        }

        public DebugMenuNode this[string key]
        {
            get
            {
                EnsureChild(key);
                return ((IDictionary<string, DebugMenuNode>)Children)[key];
            }

            set
            {
                ((IDictionary<string, DebugMenuNode>)Children)[key] = value;
            }
        }
        #endregion
    }
}

#endif

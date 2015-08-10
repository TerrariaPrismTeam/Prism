using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.DebuggingMod
{
    [Flags]
    public enum Ctrl : byte
    {
        None  = 0,
        Up    = 1,
        Down  = 2,
        Left  = 4,
        Right = 8,
        Enter = 16,
        Back  = 32,
        x10   = 64,
        x100  = 128
    }

    public class DebugNodeHelper
    {
        public static object ModifyValue<T>(double val, double mod)
        {
            var t = typeof(T);

                 if (t == typeof(  byte)) val = (  byte)Math.Min(Math.Max(val + mod,   byte.MinValue),   byte.MaxValue);
            else if (t == typeof( sbyte)) val = ( sbyte)Math.Min(Math.Max(val + mod,  sbyte.MinValue),  sbyte.MaxValue);
            else if (t == typeof(ushort)) val = (ushort)Math.Min(Math.Max(val + mod, ushort.MinValue), ushort.MaxValue);
            else if (t == typeof( short)) val = ( short)Math.Min(Math.Max(val + mod,  short.MinValue),  short.MaxValue);
            else if (t == typeof(  uint)) val = (  uint)Math.Min(Math.Max(val + mod,   uint.MinValue),   uint.MaxValue);
            else if (t == typeof(   int)) val = (   int)Math.Min(Math.Max(val + mod,    int.MinValue),    int.MaxValue);
            else if (t == typeof( ulong)) val = ( ulong)Math.Min(Math.Max(val + mod,  ulong.MinValue),  ulong.MaxValue);
            else if (t == typeof(  long)) val = (  long)Math.Min(Math.Max(val + mod,   long.MinValue),   long.MaxValue);
            else if (t == typeof( float)) val = ( float)Math.Min(Math.Max(val + mod,  float.MinValue),  float.MaxValue);
            else if (t == typeof(double)) return        Math.Min(Math.Max(val + mod, double.MinValue), double.MaxValue); //aint nobody got time fo dat

            return Convert.ChangeType(val, t);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using Terraria;
using Prism.Util;
using System.Reflection;

namespace Prism.Vanilla
{
    public class EntityConstData
    {       
        public readonly Type Type; 
        public readonly Type IDType;
        public readonly int MinID;
        public readonly int MaxID;
        public readonly SafeDictionaryIndexer<short, string> Name;        

        public EntityConstData(Type entityType, Type idType, int minID, int maxID)
        {
            Type = entityType;
            IDType = idType;
            MinID = minID;
            MaxID = maxID;
            Name = new SafeDictionaryIndexer<short, string>
                (() => 
                {
                    return (from field in idType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                            let value = (short)field.GetValue(null) let name = field.Name
                            where (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(short) 
                                && value >= MinID && value < MaxID)
                            select new KeyValuePair<short, string>(value, name))
                            .ToDictionary(x => x.Key, x => x.Value);
                }, 
                 true, (short id) => { return "?" + this.Type.ToString() + "_" + id + "?"; });            
        }
    }
}

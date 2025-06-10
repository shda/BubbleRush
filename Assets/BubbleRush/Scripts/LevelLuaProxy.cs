using System;
using System.Linq;
using Lua;
using UnityEngine;
using UnityEngine.Serialization;

namespace BubbleRush.Scripts
{
    [Serializable]
    [LuaObject]
    public partial class LevelLuaProxy
    {
        [SerializeField] private FieldBubbles fieldBubbles;

        [LuaMember]
        private void CreateMap(LuaTable stringsMap)
        {
            fieldBubbles.ClearField();
            foreach (var str in stringsMap)
            {
                var values = str.Value.ToString();
                fieldBubbles.CreateField(values.Select(c => int.Parse(c.ToString())));
            }
        }
    }
}
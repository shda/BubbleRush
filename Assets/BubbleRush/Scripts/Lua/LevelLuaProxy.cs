using System;
using System.Linq;
using Lua;
using UnityEngine;

namespace BubbleRush.Scripts.Lua
{
    [Serializable]
    [LuaObject]
    public partial class LevelLuaProxy
    {
        [SerializeField] private Field field;

        [LuaMember]
        private void CreateMap(LuaTable stringsMap)
        {
            /*
            field.ClearField();
            foreach (var str in stringsMap)
            {
                var values = str.Value.ToString();
                field.CreateField(values.Select(c => int.Parse(c.ToString())));
            }
            */
        }
    }
}
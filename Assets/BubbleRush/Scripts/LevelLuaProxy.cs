using System;
using System.Linq;
using Lua;
using UnityEngine;

namespace BubbleRush.Scripts
{
    [Serializable]
    [LuaObject]
    public partial class LevelLuaProxy
    {
        [SerializeField] private BubblesField bubblesField;

        [LuaMember]
        private void CreateMap(LuaTable stringsMap)
        {
            bubblesField.ClearField();
            foreach (var str in stringsMap)
            {
                var values = str.Value.ToString();
                bubblesField.CreateField(values.Select(c => int.Parse(c.ToString())));
            }
        }
    }
}
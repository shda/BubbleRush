using System;
using System.Linq;
using Lua;
using UnityEngine;
using UnityEngine.Serialization;

namespace BubbleRush.Scripts.Lua
{
    [Serializable]
    [LuaObject]
    public partial class LevelLuaProxy
    {
        [FormerlySerializedAs("field")] [SerializeField] private GameField gameField;

        [LuaMember]
        private void CreateMap(LuaTable stringsMap)
        {
            return;
            gameField.ClearField();
            foreach (var str in stringsMap)
            {
                var values = str.Value.ToString();
                gameField.CreateField(values.Select(c => int.Parse(c.ToString())));
            }
        }
    }
}
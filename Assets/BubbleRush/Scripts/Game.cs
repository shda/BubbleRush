using System;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private LuaScriptLoader luaScriptLoader;

        private void Awake()
        {
            luaScriptLoader.StartLevel();
        }
    }
}
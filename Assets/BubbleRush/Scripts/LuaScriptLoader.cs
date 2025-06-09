using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lua;
using Lua.Unity;
using TriInspector;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class LuaScriptLoader : MonoBehaviour
    {
        [SerializeField] private LuaAsset luaAsset;
        [SerializeField] private LevelLuaProxy  levelLuaProxy;

        private LuaState _luaState;

        private CancellationTokenSource _cancellation;

        [Button]
        public void StartLevel()
        {
            _cancellation?.Cancel();
            _cancellation = new CancellationTokenSource();
            
            LoadLevelFromLuaLogic(luaAsset.Text , _cancellation.Token);
        }

        private async void LoadLevelFromLuaLogic(string luaCode, CancellationToken token)
        {
            await UniTask.Yield();

            try
            {
                _luaState = LuaState.Create();

                _luaState.Environment["level"] = levelLuaProxy;
                await _luaState.DoStringAsync(luaCode, cancellationToken: token);
            }
            catch (LuaCanceledException luaCanceled)
            {
                Debug.Log(luaCanceled.Message);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private void OnDestroy()
        {
            _cancellation?.Cancel();
        }

        public void StopLevel()
        {
            _cancellation?.Cancel();
        }
    }
}
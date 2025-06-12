using System;
using BubbleRush.Scripts.Lua;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace BubbleRush.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private LuaScriptLoader luaScriptLoader;
        [SerializeField] private Cannon cannon;
        [SerializeField] private BubbleSpawner bubbleSpawner;
        [SerializeField] private Field field;
        [SerializeField] private TriggerLineCrossedObjects triggerLineCrossedObjects;
        [SerializeField] private InputController inputController;
        [SerializeField] private int addLineAfterShots = 5;

        private int _currentShot = 0;

        private void Awake()
        {
            inputController.OnShot = () =>
            {
                _currentShot++;
                cannon.Shot();
            };

            inputController.OnUpdateDiraection = vec =>
            {
                cannon.PointToTarget = Camera.main.ScreenToWorldPoint(vec);
            };
        }

        private void Start()
        {
            cannon.OnCollisionEnter2D = CollisionEnter2D;
            luaScriptLoader.StartLevel();
            _currentShot = 0;
            
            bubbleSpawner.SetNextBubble(1);
            SetAnimationNewBubbledAsync(2).Forget();
        }

        private void CollisionEnter2D(Collision2D collision, Bubble projectile)
        {
            CheckFieldIsCompleteAsync(collision , projectile).Forget();
        }

        private async UniTask CheckFieldIsCompleteAsync(Collision2D collision, Bubble projectile)
        {
            var wall = collision.collider.GetComponent<Wall>();
            if (wall)
            {
                if (wall.IsBubbleEnd)
                {
                    //projectile.SetStatic();
                    projectile.gameObject.SetActive(false);
                    projectile.SetStatic();
                    projectile.CollisionEnter2D = null;
                    
                    field.ReturnBubbleToPool(projectile);
                    
                    await SetAnimationNewBubbledAsync(projectile.Color);
                    return;
                }
            }
            
            var bubble = collision.collider.GetComponent<Bubble>();
            if (bubble)
            {
                projectile.SetStatic();
                projectile.CollisionEnter2D = null;
                
                await UniTask.Yield();
                field.FindAndInsertBubbleToFreePoint(projectile);
                await field.CheckAndRemoveFullLinesAsync(projectile);
                await AddNewLineIfNeed();
                await SetAnimationNewBubbledAsync(projectile.Color);
                await CheckToGameOverAsync();
            }
        }

        private async UniTask AddNewLineIfNeed()
        {
            _currentShot++;
            if(_currentShot >= addLineAfterShots)
            {
                _currentShot = 0;
                await field.AddNewLineAsync();
            }
            
            await UniTask.Yield();
        }

        private async UniTask CheckToGameOverAsync()
        {
            var crossCount = triggerLineCrossedObjects.CheckOverlappingObjects2D();
            if(crossCount.Length > 1)
            {
                // Game Over logic here
                Debug.Log("Game Over! You crossed the end line.");
                // Optionally, you can reset the game or show a game over screen.
            }
        }

        private async UniTask SetAnimationNewBubbledAsync(int lastColor)
        {
            await UniTask.Yield();
            await cannon.AnimatedSetNextBubbleAsync(bubbleSpawner.CurrentBubble);
           // reflectingRay.SetBody(nextBubble.CurrentBubble);
            
            var nextColorIndex = field.GetRandomColorIndex();
            while (nextColorIndex == lastColor)
            {
                nextColorIndex = field.GetRandomColorIndex();
            }
            bubbleSpawner.SetNextBubble(nextColorIndex);
        }
    }
}
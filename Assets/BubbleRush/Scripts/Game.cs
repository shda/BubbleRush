using System;
using BubbleRush.Scripts.Bonuses;
using BubbleRush.Scripts.GameUi;
using BubbleRush.Scripts.Lua;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        [SerializeField] private BonusController bonusController;
        [SerializeField] private GameStats gameStats;
        [SerializeField] private int addLineAfterShots = 5;

        private int _currentShot;

        private void Awake()
        {
            gameStats.Init();
            gameStats.LoadValues();
            
            inputController.OnShot = () =>
            {
                _currentShot++;
                bonusController.StartShot();
                cannon.Shot();
            };

            inputController.OnUpdateDirection = vec =>
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
            AnimationCreateNewBubble(2).Forget();
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
                    projectile.gameObject.SetActive(false);
                    projectile.SetStatic();
                    projectile.CollisionEnter2D = null;
                    
                    field.ReturnBubbleToPool(projectile);
                    
                    await AnimationCreateNewBubble(projectile.Color);
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
                await UniTask.Yield();
                
                await CompleteMovingBubble(projectile);
            }
        }

        private async UniTask CompleteMovingBubble(Bubble projectile)
        {
            if (!await bonusController.CompleteShotBubbleOnTarget(projectile))
            {
                var allBubbles = 0;
                var countRemoveEqualsColors = await field.RemoveEqualsColorsBubblesAsync(projectile);
                ScoreChangeTools.RemoveEqualsColors(gameStats , countRemoveEqualsColors);
                
                var countRemoveFreeBubbles  = await field.RemoveFreeBubblesAsync();
                ScoreChangeTools.RemoveEqualsColors(gameStats , countRemoveFreeBubbles);
                
                allBubbles += countRemoveEqualsColors + countRemoveFreeBubbles;
                
                if (await AddNewLineIfNeed())
                {
                    countRemoveFreeBubbles = await field.RemoveFreeBubblesAsync();
                    allBubbles += countRemoveFreeBubbles;
                    ScoreChangeTools.RemoveEqualsColors(gameStats , countRemoveFreeBubbles);
                }

                AddBonusIfNeed(allBubbles);
                
                gameStats.SaveValues();
                await AnimationCreateNewBubble(projectile.Color);
                await CheckToGameOverAsync();
            }
        }

        private void AddBonusIfNeed(int countDestroyedBubbles)
        {
            if (countDestroyedBubbles >= 5)
            {
                if (Random.Range(0, 100) > 50)
                {
                    gameStats.BonusExplosion.Value = true;
                }
                else
                {
                    gameStats.BonusRay.Value = true;
                }
            }
        }
        
        private async UniTask<bool> AddNewLineIfNeed()
        {
            _currentShot++;
            if(_currentShot >= addLineAfterShots)
            {
                _currentShot = 0;
                await field.AddNewLineAsync();
                return true;
            }

            return false;
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

        private async UniTask AnimationCreateNewBubble(int lastColor)
        {
            await UniTask.Yield();
            await cannon.AnimatedSetNextBubbleAsync(bubbleSpawner.CurrentBubble);
            
            var nextColorIndex = field.GetRandomColorIndex();
            while (nextColorIndex == lastColor)
            {
                nextColorIndex = field.GetRandomColorIndex();
            }
            bubbleSpawner.SetNextBubble(nextColorIndex);
        }
    }
}
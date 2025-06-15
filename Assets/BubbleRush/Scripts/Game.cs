using BubbleRush.Scripts.Bonuses;
using BubbleRush.Scripts.GameUi;
using BubbleRush.Scripts.Lua;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BubbleRush.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private LuaScriptLoader luaScriptLoader;
        [SerializeField] private Cannon cannon;
        [SerializeField] private BubbleSpawner bubbleSpawner;
        [SerializeField] private GameField gameField;
        [SerializeField] private TriggerLineCrossedObjects triggerLineCrossedObjects;
        [SerializeField] private InputController inputController;
        [SerializeField] private BonusController bonusController;
        [SerializeField] private GameStats gameStats;
        [SerializeField] private GameOverMenu gameOverMenu;
        [SerializeField] private int addLineAfterShots = 5;

        private int _currentShot;

        private void Awake()
        {
            gameOverMenu.gameObject.SetActive(false);
            
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
            
            cannon.OnBubbleCollisionEnter2D = CollisionEnter2D;
        }

        private void Start()
        {
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
                    
                    gameField.ReturnBubbleToPool(projectile);
                    
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
                gameField.FindAndInsertBubbleToFreePoint(projectile);
                await UniTask.Yield();
                
                await CompleteMovingBubble(projectile);
            }
        }

        private async UniTask CompleteMovingBubble(Bubble projectile)
        {
            if (!await bonusController.CompleteShotBubbleOnTarget(projectile))
            {
                var allBubbles = 0;
                var countRemoveEqualsColors = await gameField.RemoveEqualsColorsBubblesAsync(projectile);
                ScoreChangeTools.RemoveEqualsColors(gameStats , countRemoveEqualsColors);
                
                var countRemoveFreeBubbles  = await gameField.RemoveFreeBubblesAsync();
                ScoreChangeTools.RemoveFreeBubbles(gameStats , countRemoveFreeBubbles);
                
                allBubbles += countRemoveEqualsColors + countRemoveFreeBubbles;
                
                if (await AddNewLineIfNeed())
                {
                    countRemoveFreeBubbles = await gameField.RemoveFreeBubblesAsync();
                    allBubbles += countRemoveFreeBubbles;
                    ScoreChangeTools.RemoveFreeBubbles(gameStats , countRemoveFreeBubbles);
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
                await gameField.AddNewLineAsync();
                return true;
            }

            return false;
        }

        private async UniTask CheckToGameOverAsync()
        {
            var crossCount = triggerLineCrossedObjects.CheckOverlappingObjects2D();
            if(crossCount.Length > 1 || gameField.CheckIsGameOver())
            {
                gameOverMenu.gameObject.SetActive(true);
            }
        }

        private async UniTask AnimationCreateNewBubble(int lastColor)
        {
            await UniTask.Yield();
            await cannon.AnimatedSetNextBubbleAsync(bubbleSpawner.CurrentBubble);
            
            var nextColorIndex = gameField.GetRandomColorIndex();
            while (nextColorIndex == lastColor)
            {
                nextColorIndex = gameField.GetRandomColorIndex();
            }
            bubbleSpawner.SetNextBubble(nextColorIndex);
        }
    }
}
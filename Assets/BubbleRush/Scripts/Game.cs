using BubbleRush.Scripts.Bonuses;
using BubbleRush.Scripts.GameUi;
using BubbleRush.Scripts.Services;
using BubbleRush.Scripts.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BubbleRush.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Cannon cannon;
        [SerializeField] private BubbleSpawner bubbleSpawner;
        [SerializeField] private GameField gameField;
        [SerializeField] private TriggerLineCrossedObjects triggerLineCrossedObjects;
        [SerializeField] private BonusController bonusController;
        [SerializeField] private GameOverMenu gameOverMenu;
        
        private int _currentShot;

        private GameStats _gameStats;
        private IInputService _inputService;
        private Camera _camera;
        private GameSettings _gameSettings;

        [Inject]
        private void Initialize(
            GameSettings gameSettings,
            GameStats gameStats , 
            IInputService inputService ,
            Camera camera)
        {
            _gameSettings = gameSettings;
            _camera = camera;
            _inputService = inputService;
            _gameStats = gameStats;
        }
        
        private void Awake()
        {
            gameOverMenu.gameObject.SetActive(false);
            
            _gameStats.Init();
            _gameStats.LoadValues();
            
            _inputService.OnShotEvent = () =>
            {
                _currentShot++;
                bonusController.StartShot();
                cannon.Shot();
            };

            _inputService.OnPointToScreen = vec =>
            {
                cannon.PointToTarget = _camera.ScreenToWorldPoint(vec);
            };
            
            cannon.OnBubbleCollisionEnter2D = CollisionEnter2D;
        }

        private void Start()
        {
            _currentShot = 0;
            
            bubbleSpawner.SetNextBubble(1);
            AnimationCreateNewBubble(2).Forget();
        }

        private void CollisionEnter2D(Collision2D collision, Bubble projectile)
        {
            OnBubbleCollision(collision , projectile).Forget();
        }

        private async UniTask OnBubbleCollision(Collision2D collision, Bubble projectile)
        {
            var wall = collision.collider.GetComponent<WallCollider>();
            if (wall)
            {
                if (wall.IsBubbleEnd)
                {
                    projectile.gameObject.SetActive(false);
                    projectile.SetStatic();
                    projectile.CollisionEnter2D = null;
                    
                    gameField.ReturnBubbleToPool(projectile);
                    await AddNewLineIfNeedAsync();
                    await AnimationCreateNewBubble(projectile.Color);
                    return;
                }
            }
            
            var bubble = collision.collider.GetComponent<Bubble>();
            if (bubble)
            {
                projectile.SetStatic();
                projectile.CollisionEnter2D = null;
                gameField.FindAndInsertBubbleToFreePoint(projectile);
                await CompleteAddBubbleToField(projectile);
            }
        }

        private async UniTask CompleteAddBubbleToField(Bubble projectile)
        {
            if (!await bonusController.CompleteShotBubbleOnTarget(projectile))
            {
                var allBubbles = 0;
                var countRemoveEqualsColors = await gameField.RemoveEqualsColorsBubblesAsync(projectile);
                ScoreChangeTools.RemoveEqualsColors(_gameStats , countRemoveEqualsColors);
                
                var countRemoveFreeBubbles  = await gameField.RemoveFreeBubblesAsync();
                ScoreChangeTools.RemoveFreeBubbles(_gameStats , countRemoveFreeBubbles);
                
                allBubbles += countRemoveEqualsColors + countRemoveFreeBubbles;
                
                if (await AddNewLineIfNeedAsync())
                {
                    countRemoveFreeBubbles = await gameField.RemoveFreeBubblesAsync();
                    allBubbles += countRemoveFreeBubbles;
                    ScoreChangeTools.RemoveFreeBubbles(_gameStats , countRemoveFreeBubbles);
                }

                AddBonusIfNeed(allBubbles);
                
                _gameStats.SaveValues();
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
                    _gameStats.BonusExplosion.Value = true;
                }
                else
                {
                    _gameStats.BonusRay.Value = true;
                }
            }
        }
        
        private async UniTask<bool> AddNewLineIfNeedAsync()
        {
            if(_currentShot >= _gameSettings.CountShotsAddLine)
            {
                _currentShot = 0;
                await gameField.AddNewLineAsync();
                return true;
            }

            return false;
        }

        private async UniTask CheckToGameOverAsync()
        {
            var crossCount = triggerLineCrossedObjects.GetOverlappingObjects2D();
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
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
      
        
        private void Start()
        {
            cannon.OnCollisionEnter2D = CollisionEnter2D;
            luaScriptLoader.StartLevel();
            
            bubbleSpawner.SetNextBubble(1);
            SetAnimationNewBubble(2).Forget();
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
                    
                    await SetAnimationNewBubble(projectile.Color);
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
                await SetAnimationNewBubble(projectile.Color);
                
                return;
            }
        }

        private async UniTask SetAnimationNewBubble(int lastColor)
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
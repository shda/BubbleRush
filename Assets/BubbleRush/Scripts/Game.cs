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
        [FormerlySerializedAs("field")] [SerializeField] private FieldBubbles fieldBubbles;
        [SerializeField] private int maxColorIndex = 6;
        
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
                    
                    fieldBubbles.ReturnToPool(projectile);
                    
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
                fieldBubbles.PlaceBubbleInField(projectile);
                await fieldBubbles.CheckAndRemoveFullLinesAsync(projectile);
                await SetAnimationNewBubble(projectile.Color);
                
                return;
            }
        }

        private async UniTask SetAnimationNewBubble(int lastColor)
        {
            await UniTask.Yield();
            await cannon.AnimatedSetNextBubbleAsync(bubbleSpawner.CurrentBubble);
           // reflectingRay.SetBody(nextBubble.CurrentBubble);
            
            var nextColorIndex = Random.Range(0, maxColorIndex);
            while (nextColorIndex == lastColor)
            {
                nextColorIndex = Random.Range(0, maxColorIndex);
            }
            bubbleSpawner.SetNextBubble(nextColorIndex);
        }
    }
}
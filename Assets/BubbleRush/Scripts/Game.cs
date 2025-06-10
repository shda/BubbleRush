using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private LuaScriptLoader luaScriptLoader;
        [SerializeField] private Cannon cannon;
        [SerializeField] private BubblesField field;
        private void Start()
        {
            cannon.OnCollisionEnter2D = CollisionEnter2D;
            luaScriptLoader.StartLevel();
        }

        private void CollisionEnter2D(Collision2D collision, Bubble projectile)
        {

            CheckFieldIsCompleteAsync(collision , projectile).Forget();
            //  Debug.Log(bubble.gameObject.name + " collided with " + projectile.gameObject.name);
        }

        private async UniTask CheckFieldIsCompleteAsync(Collision2D collision, Bubble projectile)
        {
            var bubble = collision.collider.GetComponent<Bubble>();
            if (bubble)
            {
                projectile.SetStatic();
                projectile.CollisionEnter2D = null;

                await field.CheckAndRemoveFullLinesAsync(projectile);
                
                field.AddBubbleToField(projectile);
                cannon.CreateNextBubble();
            }
        }
    }
}
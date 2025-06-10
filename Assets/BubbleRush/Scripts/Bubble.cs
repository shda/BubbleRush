using System;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites;

        public int Color { get; private set; }
        public Action<Collision2D, Bubble> CollisionEnter2D;

        private Vector2 _linearVelocity;
        public void SetColorIndex(int bubbleColor)
        {
            Color = bubbleColor;
            spriteRenderer.sprite = sprites[bubbleColor];
        }

        public void SetStatic()
        {
            body.bodyType = RigidbodyType2D.Static;
        }
        
        public void SetDynamic()
        {
            body.bodyType = RigidbodyType2D.Dynamic;
        }

        public void SetLinearVelocity(Vector2 linearVelocity)
        {
           // _linearVelocity = linearVelocity;
           
           body.linearVelocity = linearVelocity;
        }

        private void FixedUpdate()
        {
            if(body.bodyType == RigidbodyType2D.Static)
                return;
            
            //body.linearVelocity = _linearVelocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CollisionEnter2D?.Invoke(other , this);
        }
    }
}

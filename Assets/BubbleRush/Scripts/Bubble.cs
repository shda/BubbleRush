using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites;
        public void SetColorIndex(int bubbleColor)
        {
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
    }
}

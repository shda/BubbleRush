using System;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites;

        private Vector2Int _positionInGrid;
        public Vector2Int PositionInGrid
        {
            get => _positionInGrid;
            set
            {
                _positionInGrid = value;
                UpdateWorldPositon();
            }
        }

        public int Color { get; private set; }
        public Action<Collision2D, Bubble> CollisionEnter2D;
        private Vector2 _linearVelocity;
        private Vector2 _offsetToBubbles;

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
            body.linearVelocity = linearVelocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CollisionEnter2D?.Invoke(other, this);
        }

        public void SetOffset(Vector2 offsetToBubbles)
        {
            _offsetToBubbles = offsetToBubbles;
        }

        public void UpdateWorldPositon()
        {
            transform.localPosition =
                new Vector3(PositionInGrid.x * _offsetToBubbles.x, PositionInGrid.y * -_offsetToBubbles.y);
            if (PositionInGrid.y % 2 != 0)
            {
                transform.localPosition += new Vector3(_offsetToBubbles.x * 0.5f, 0, 0);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.Label(
                transform.position - new Vector3(0.25f, 0, 0), $"{PositionInGrid.x}, {PositionInGrid.y}",
                new GUIStyle
                {
                    normal = new GUIStyleState()
                    {
                        textColor = UnityEngine.Color.black
                    },
                    fontSize = 15
                }
            );
        }
#endif
    }
}
using System;
using UnityEngine;

namespace BubbleRush.Scripts.Debug
{
    public class TestPoint : MonoBehaviour
    {
        public float sizePoint = 1.0f;
        public Color color = Color.gray;
        
        private Vector3 _position;
        
        public Vector2 offsetToBubbles = new(1,1);
        private void Update()
        {
           // var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
          //  transform.position = mousePosition;
            var pos = transform.localPosition;

            float x = (float)(Math.Round(pos.x / offsetToBubbles.x) * offsetToBubbles.x);
            float y = (float)(Math.Round(pos.y / offsetToBubbles.y) * offsetToBubbles.y);
            if ( Math.Round(pos.y / offsetToBubbles.y) % 2 != 0)
            {
                x += offsetToBubbles.x * 0.5f;
            }

            _position = transform.TransformPoint(new Vector3(x, y, pos.z));
        }
        
  
        void OnDrawGizmos()
        {
            if (enabled)
            {
                var oldColor = Gizmos.color;
                Gizmos.color = color;
                Gizmos.DrawSphere(transform.position, sizePoint);
                Gizmos.color = oldColor;
            }
        }
    }
}
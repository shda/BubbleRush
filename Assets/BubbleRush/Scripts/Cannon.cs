using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BubbleRush.Scripts
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private BubblesPool poolBubbles;
        [SerializeField] private Vector2 minMaxAngle;
        [SerializeField] private Transform rootRotate;
        [SerializeField] private Transform rootBubble;
        [SerializeField] private float speedBubbleMove = 1.0f;
        
        private Bubble _currentBubble;
        
        public Action<Collision2D , Bubble >  OnCollisionEnter2D;

        private void Awake()
        {
            CreateNextBubble();
        }

        public void CreateNextBubble()
        {
            _currentBubble = PrepareBubble();
        }

        private Bubble PrepareBubble()
        {
            var bubble = poolBubbles.Get();
            bubble.transform.SetParent(rootBubble);
            bubble.transform.localPosition = Vector3.zero;
            bubble.transform.localRotation = Quaternion.identity;
            bubble.CollisionEnter2D = CollisionEnter2D;
            
            bubble.SetStatic();
            //bubble.SetColorIndex(0);
            bubble.SetColorIndex(Random.Range(0, 4));

            return bubble;
        }

        private void CollisionEnter2D(Collision2D collision, Bubble bubble)
        {
            OnCollisionEnter2D?.Invoke(collision , bubble);
        }

        private void Shot()
        {
            if(!_currentBubble)
                return;
            
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - rootRotate.position;
            _currentBubble.transform.SetParent(null);
            _currentBubble.SetDynamic();
            _currentBubble.SetLinearVelocity(direction.normalized * speedBubbleMove);

            _currentBubble = null;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shot();
            }
            
            RotateToPoint();
        }

        private void RotateToPoint()
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            
            Vector2 direction = mousePosition - rootRotate.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, minMaxAngle.x, minMaxAngle.y);
            
            if (minMaxAngle.x < angle && angle < minMaxAngle.y)
            {
                rootRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}
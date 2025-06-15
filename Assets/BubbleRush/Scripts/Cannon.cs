using System;
using BubbleRush.Scripts.Bonuses;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private Transform rootRotate;
        [SerializeField] private Transform rootBubble;
       // [SerializeField] private BubblesPool poolBubbles;
        [SerializeField] private BubbleRay bubbleRay;
        
        [SerializeField] private Vector2 minMaxAngle;
        [SerializeField] private float speedBubbleMove = 1.0f;
        
        private Bubble _currentBubble;
        
        public Vector3 PointToTarget { get; set; }
        
        public Action<Collision2D , Bubble >  OnBubbleCollisionEnter2D;
        
        public async UniTask AnimatedSetNextBubbleAsync(Bubble bubble)
        {
            await UniTask.Yield();
            bubble.transform.SetParent(rootBubble);
            await bubble.transform.DOLocalMove(Vector3.zero, 0.5f);
            _currentBubble = bubble;
            bubble.CollisionEnter2D = CollisionEnter2D;
        }
        
        private void CollisionEnter2D(Collision2D collision, Bubble bubble)
        {
            OnBubbleCollisionEnter2D?.Invoke(collision , bubble);
        }

        public void Shot()
        {
            if(!_currentBubble)
                return;
            
            _currentBubble.transform.SetParent(null);
            _currentBubble.SetDynamic();
            _currentBubble.SetLinearVelocity(rootRotate.right * speedBubbleMove);
            _currentBubble = null;
        }
        
        private void Update()
        {
            RotateToPoint();
        }

        private void RotateToPoint()
        {
            var mousePosition = PointToTarget;// Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            
            Vector2 direction = mousePosition - rootRotate.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, minMaxAngle.x, minMaxAngle.y);
            
            if (minMaxAngle.x < angle && angle < minMaxAngle.y)
            {
                rootRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
                bubbleRay.SetVelocity(direction.normalized * speedBubbleMove);
            }
        }
    }
}
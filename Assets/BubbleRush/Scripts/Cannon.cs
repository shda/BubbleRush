using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private BubblesPool poolBubbles;
        [SerializeField] private Vector2 minMaxAngle;
        [SerializeField] private Transform rootRotate;
        [SerializeField] private Transform rootBubble;
        [SerializeField] private BubbleRay bubbleRay;
        [SerializeField] private float speedBubbleMove = 1.0f;
        
        private Bubble _currentBubble;
        
        public Action<Collision2D , Bubble >  OnCollisionEnter2D;

        private void Awake()
        {
            bubbleRay.StopSimulate();
        }

        public async UniTask AnimatedSetNextBubbleAsync(Bubble bubble)
        {
            await UniTask.Yield();
            bubble.transform.SetParent(rootBubble);
            await bubble.transform.DOLocalMove(Vector3.zero, 0.5f);
            _currentBubble = bubble;
            bubble.CollisionEnter2D = CollisionEnter2D;
            
            bubbleRay.StartSimulate();
        }
        
        private void CollisionEnter2D(Collision2D collision, Bubble bubble)
        {
            OnCollisionEnter2D?.Invoke(collision , bubble);
        }

        private void Shot()
        {
            if(!_currentBubble)
                return;
            
            bubbleRay.StopSimulate();
            
            _currentBubble.transform.SetParent(null);
            _currentBubble.SetDynamic();
            _currentBubble.SetLinearVelocity(rootRotate.right * speedBubbleMove);
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
                
                bubbleRay.UpdateVelocity(direction.normalized * speedBubbleMove);
            }
        }
    }
}
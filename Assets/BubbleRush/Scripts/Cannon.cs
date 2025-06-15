using System;
using BubbleRush.Scripts.Bonuses;
using BubbleRush.Scripts.Settings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace BubbleRush.Scripts
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private Transform rootRotate;
        [SerializeField] private Transform rootBubble;
        [SerializeField] private BubbleRay bubbleRay;
        
        private Bubble _currentBubble;
        private GameSettings _settings;
        
        public Vector3 PointToTarget { get; set; }
        public Action<Collision2D , Bubble >  OnBubbleCollisionEnter2D;
        
        [Inject]
        public void Initialize(GameSettings settings)
        {
            Debug.Log("Initialize");
            _settings = settings;
        }
        
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
            _currentBubble.SetLinearVelocity(rootRotate.right * _settings.SpeedBubbleMove);
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
            angle = Mathf.Clamp(angle, _settings.MinMaxAngleCannon.x, _settings.MinMaxAngleCannon.y);
            
            if (_settings.MinMaxAngleCannon.x < angle && angle < _settings.MinMaxAngleCannon.y)
            {
                rootRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
                bubbleRay.SetVelocity(direction.normalized * _settings.SpeedBubbleMove);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace BubbleRush.Scripts
{
    public class Bubble : MonoBehaviour , IPoolable<IMemoryPool>
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
        
        public HashSet<Bubble> Neighbors { get; set; } = new ();
        
        private Vector2 _linearVelocity;
        private Vector2 _offsetToBubbles;
        private IMemoryPool _memoryPool;
        
        public Vector2 GetOffsetToBubbles()
        {
            return _offsetToBubbles;
        }

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

        private void UpdateWorldPositon()
        {
            transform.localPosition = ConvertLocalPointToWorld(PositionInGrid);
        }

        public Vector3 ConvertLocalPointToWorld(Vector2Int position)
        {
            var point = new Vector3(position.x * _offsetToBubbles.x, position.y * -_offsetToBubbles.y);
            if (position.y % 2 != 0)
            {
                point += new Vector3(_offsetToBubbles.x * 0.5f, 0, 0);
            }

            return point;
        }
        
        public void RemoveNeighborsRefs()
        {
            foreach (var neighbor in Neighbors)
            {
                neighbor.Neighbors.Remove(this);
            }
            Neighbors.Clear();
        }

        public void RefreshNeighbors()
        {
            Neighbors.Clear();
            
            var colliders = System.Buffers.ArrayPool<Collider2D>.Shared.Rent(10);
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, _offsetToBubbles.x, colliders);
            for (int i = 0; i < size; i++)
            {
                var coll = colliders[i];
                if (coll != null && 
                    coll.gameObject.activeInHierarchy &&
                    coll.gameObject != gameObject && 
                    coll.TryGetComponent<Bubble>(out var bubble))
                {
                    Neighbors.Add(bubble);
                    bubble.Neighbors.Add(this);
                } 
            }

            System.Buffers.ArrayPool<Collider2D>.Shared.Return(colliders);
        }
        
        public async UniTask AnimationHideBubbleAsync()
        {
            await transform.DOScale(0, 0.1f);
        }
        
        void IPoolable<IMemoryPool>.OnDespawned()
        {
            
        }

        void IPoolable<IMemoryPool>.OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
        }

        public void ReturnToPool()
        {
            _memoryPool.Despawn(this);
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

            foreach (var neighbor in Neighbors)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
#endif
        
    }
}
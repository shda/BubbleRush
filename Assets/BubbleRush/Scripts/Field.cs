using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TriInspector;
using UnityEngine;
using ZLinq;
using Random = UnityEngine.Random;

namespace BubbleRush.Scripts
{
    public class Field : MonoBehaviour
    {
        [SerializeField] private Vector2 offsetToBubbles = new(1, 1);
        [SerializeField] private BubblesPool poolBubbles;
        [SerializeField] private AnimationRemoveBubbles animationRemoveBubbles;
        [SerializeField] private float offset = 0.5f;
        [SerializeField] private int maxColorIndex = 5;
        
        private FieldGenerator fieldGenerator = new();
        private readonly HashSet<Bubble> _bubbles = new();
        private int _lineCurrent;
        
        public int GetRandomColorIndex()
        {
            return Random.Range(0, maxColorIndex);
        }
        
        private async void Start()
        {
          //  return;
            await UniTask.DelayFrame(5);
            
            var field = fieldGenerator.GenerateField(6,6, 5);
            var maxX = field.GetLength(0);
            var maxY = field.GetLength(1);
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    var bubble = poolBubbles.Get();
                    SetBubbleInfo(bubble , x, y, field[x , y]);
                    _bubbles.Add(bubble);
                }
            }

            await UniTask.DelayFrame(5);
            UpdateAllNeighbors();
        }
        
        [Button]
        public void UpdateAllNeighbors()
        {
            foreach (var bubble in _bubbles)
            {
                bubble.UpdateNeighbors();
            }
        }
        
        [Button]
        public async UniTask AddNewLineAsync()
        {
            var newLine = fieldGenerator.CreateNewLine(maxColorIndex);
            foreach (var bubble in _bubbles)
            {
                var newPos = bubble.PositionInGrid + new Vector2Int(0, 1);
                var pointMove = bubble.GetMovingPoint(newPos);
                bubble.transform.DOLocalMove(pointMove, 0.5f).onComplete = () =>
                {
                    bubble.PositionInGrid = newPos;
                };
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            for (var i = 0; i < newLine.Length; i++)
            {
                var bubbleColor = newLine[i];
                var newBubble = poolBubbles.Get();
                _bubbles.Add(newBubble);
                
                newBubble.transform.localScale = Vector3.zero;
                newBubble.transform.DOScale(Vector3.one, 0.2f);
                
                SetBubbleInfo(newBubble , i , 0 , bubbleColor);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
        
        
        public void CreateField(IEnumerable<int> bubblesColors)
        {
            float x = 0;
            int coordX = 0;
            foreach (var bubbleColor in bubblesColors)
            {
                if (bubbleColor > 0)
                {
                    var bubble = poolBubbles.Get();
                    bubble.SetOffset(offsetToBubbles);
                    SetBubbleInfo(bubble , coordX, _lineCurrent, bubbleColor - 1);
                    _bubbles.Add(bubble);
                }

                x += offsetToBubbles.x;
                coordX++;
            }

            _lineCurrent++;
        }

        private void SetBubbleInfo(Bubble bubble, int x, int y, int colorIndex)
        {
            bubble.SetOffset(offsetToBubbles);
            bubble.SetColorIndex(colorIndex);
            bubble.SetStatic();
            bubble.transform.SetParent(transform);
            bubble.PositionInGrid = new Vector2Int(x, y);
        }
        
        public void ClearField()
        {
            _lineCurrent = 0;
            foreach (var bubble in _bubbles)
            {
                poolBubbles.Release(bubble);
            }

            _bubbles.Clear();
        }

        public void FindAndInsertBubbleToFreePoint(Bubble bubble)
        {
            bubble.transform.SetParent(transform);

            var gridPosition = FindNearestPoint(bubble.transform.position);
            bubble.SetOffset(offsetToBubbles);
            bubble.PositionInGrid = gridPosition;
            _bubbles.Add(bubble);
        }

        private Vector2Int FindNearestPoint(Vector3 worldPoint)
        {
            Vector3[] candidatePositions =
            {
                worldPoint,
                worldPoint + new Vector3(0.1f, 0),
                worldPoint + new Vector3(-0.1f, 0),
                worldPoint + new Vector3(0, 0.1f),
                worldPoint + new Vector3(0, 0.2f)
            };

            var gridPosition = Vector2Int.zero;
            foreach (var candidate in candidatePositions)
            {
                gridPosition = ClosestPositionInGrid(candidate);
                if (_bubbles.AsValueEnumerable().All(b => b.PositionInGrid != gridPosition))
                {
                    break;
                }
            }

            return gridPosition;
        }

        private Vector2Int ClosestPositionInGrid(Vector2 worldPoint)
        {
            var pos = transform.InverseTransformPoint(worldPoint);
            if (Math.Round(pos.y / offsetToBubbles.y) % 2 != 0)
            {
                pos -= new Vector3(offsetToBubbles.x * offset, 0, 0);
            }

            return new Vector2Int(
                (int)Math.Round(pos.x / offsetToBubbles.x),
                (int)Math.Round(-pos.y / offsetToBubbles.y));
        }

        private Vector2 ClosestPointCalculation(Vector2 worldPoint)
        {
            var pos = transform.InverseTransformVector(worldPoint);

            if (Math.Round(pos.y / offsetToBubbles.y) % 2 != 0)
            {
                pos -= new Vector3(offsetToBubbles.x * 0.25f, 0, 0);
            }
            else
            {
                pos += new Vector3(offsetToBubbles.x * 0.25f, 0, 0);
            }

            float x = (float)Math.Round(pos.x / offsetToBubbles.x) * offsetToBubbles.x;
            float y = (float)Math.Round(pos.y / offsetToBubbles.y) * offsetToBubbles.y;
            if (Math.Round(pos.y / offsetToBubbles.y) % 2 != 0)
            {
                x += offsetToBubbles.x * offset;
            }

            return new Vector2(x, y);
        }

        public async UniTask<int> RemoveEqualsColorsBubblesAsync(Bubble projectile)
        {
            projectile.UpdateNeighbors();
            return await animationRemoveBubbles.RemoveEqualsColorsBubblesAsync(_bubbles, projectile, offsetToBubbles);
        }

        public async UniTask<int> RemoveFreeBubblesAsync()
        {
            UpdateAllNeighbors();
            return await animationRemoveBubbles.RemoveFreeBubblesAsync(_bubbles);
        }
        
        public async UniTask AnimationRemoveBubbles(Bubble[] bubbles)
        {
            foreach (var bubble in bubbles)
            {
                await animationRemoveBubbles.AnimationRemoveBubble(_bubbles, bubble);
            }
        }
        
        public void ReturnBubbleToPool(Bubble projectile)
        {
            poolBubbles.Release(projectile);
            _bubbles.Remove(projectile);
        }
        
        public bool CheckIsGameOver()
        {
            return _bubbles.Count <= 0;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var point = ClosestPositionInGrid(mousePosition);

            var point2 = ClosestPointCalculation(mousePosition);

            UnityEditor.Handles.Label(mousePosition, $"{point.x}-{point.y}", new GUIStyle
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white
                },
                fontSize = 30
            });

            Gizmos.DrawSphere(point2, 0.3f);
        }
#endif
    }
}
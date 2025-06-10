using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using ZLinq;

namespace BubbleRush.Scripts
{
    public class FieldBubbles : MonoBehaviour
    {
        [SerializeField] private Vector2 offsetToBubbles = new(1, 1);
        [SerializeField] private BubblesPool poolBubbles;
        [SerializeField] private FieldLogic fieldLogic;
        [SerializeField] private float offset = 0.5f;
        
        private readonly HashSet<Bubble> _bubbles = new();
        private int _lineCurrent;

        private async void Start()
        {
            await UniTask.DelayFrame(5);
            fieldLogic.FindFreeBubbles(_bubbles);
        }
        
        public void CreateField(IEnumerable<int> bubblesColors)
        {
            float x = 0;
            foreach (var bubbleColor in bubblesColors)
            {
                if (bubbleColor > 0)
                {
                    var bubble = poolBubbles.Get();
                    bubble.SetColorIndex(bubbleColor - 1);
                    bubble.SetStatic();

                    bubble.transform.SetParent(transform);
                    bubble.transform.localPosition = new Vector3(x, _lineCurrent * -offsetToBubbles.y);
                    bubble.SetPositionInGrid(new Vector2Int((int)Math.Round(x / offsetToBubbles.x),
                        _lineCurrent));

                    if (_lineCurrent % 2 != 0)
                    {
                        bubble.transform.localPosition += new Vector3(offsetToBubbles.x * offset, 0, 0);
                    }

                    _bubbles.Add(bubble);
                }

                x += offsetToBubbles.x;
            }

            _lineCurrent++;
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

        public void PlaceBubbleInField(Bubble bubble)
        {
            bubble.transform.SetParent(transform);

            var originalPosition = bubble.transform.position;
            Vector3[] candidatePositions =
            {
                originalPosition,
                originalPosition + new Vector3(0.1f, 0),
                originalPosition + new Vector3(-0.1f, 0),
                originalPosition + new Vector3(0, 0.1f),
                originalPosition + new Vector3(0, 0.2f)
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

            bubble.transform.localPosition = new Vector3(gridPosition.x * offsetToBubbles.x, gridPosition.y * -offsetToBubbles.y);
            if (gridPosition.y % 2 != 0)
            {
                bubble.transform.localPosition += new Vector3(offsetToBubbles.x * offset, 0, 0);
            }

            bubble.SetPositionInGrid(gridPosition);
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

        public async UniTask CheckAndRemoveFullLinesAsync(Bubble projectile)
        {
            _bubbles.Add(projectile);
            
            var color = projectile.Color;
            HashSet<Bubble> queue = new() { projectile };
            HashSet<Bubble> allBubble = new();

            while (queue.Count > 0)
            {
                var first = queue.First();
                queue.Remove(first);

                var findObjects = Physics2D.OverlapCircleAll(first.transform.position, offsetToBubbles.x);
                foreach (var findObject in findObjects)
                {
                    var bubble = findObject.GetComponent<Bubble>();
                    if (!bubble)
                        continue;
                    if (allBubble.Contains(bubble))
                        continue;

                    if (bubble.Color == color)
                    {
                        queue.Add(bubble);
                        allBubble.Add(bubble);
                    }
                }
            }

            if (allBubble.Count >= 3)
            {
                foreach (var bubble in allBubble)
                {
                    await bubble.transform.DOScale(0, 0.03f);
                    bubble.transform.localScale = Vector3.one;
                    poolBubbles.Release(bubble);
                    _bubbles.Remove(bubble);
                }
            }

            await UniTask.DelayFrame(5);

            var bubbles = fieldLogic.FindFreeBubbles(_bubbles);
            HashSet<Bubble> bubblesHash = new();
            foreach (var bubbleList in bubbles)
            {
                foreach (var bubble in bubbleList)
                {
                    bubblesHash.Add(bubble);
                }
            }

            foreach (var bubble in bubblesHash)
            {
                await bubble.transform.DOScale(0, 0.03f);
                bubble.transform.localScale = Vector3.one;
                poolBubbles.Release(bubble);
                _bubbles.Remove(bubble);
            }
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
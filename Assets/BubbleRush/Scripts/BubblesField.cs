using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBlast.Scripts.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace BubbleRush.Scripts
{
    public class BubblesField : MonoBehaviour
    {
        [SerializeField] private Vector2 offsetToBubbles = new(1,1);
        [SerializeField] private float offsetLine = 0.5f;
        [SerializeField] private BubblesPool poolBubbles;
        
        private readonly HashSet<Bubble> _bubbles = new();
        private int _lineCurrent = 0;
        
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
                    bubble.transform.localPosition = new Vector3(x , _lineCurrent * -offsetToBubbles.y);

                    if (_lineCurrent % 2 != 0)
                    {
                        bubble.transform.localPosition += new Vector3(offsetToBubbles.x * 0.5f , 0 , 0);
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

        public void AddBubbleToField(Bubble bubble)
        {
            bubble.transform.SetParent(transform);
            
            var pos = bubble.transform.localPosition;

            var x = (float)(Math.Round(pos.x / offsetToBubbles.x) * offsetToBubbles.x);
            var y = (float)(Math.Round(pos.y / offsetToBubbles.y) * offsetToBubbles.y);
            if ( Math.Round(pos.y / offsetToBubbles.y) % 2 != 0)
            {
                x += offsetToBubbles.x * 0.5f;
            }

            bubble.transform.localPosition = new Vector3(x, y, pos.z);
        }

        public async UniTask CheckAndRemoveFullLinesAsync(Bubble projectile)
        {
            _bubbles.Add(projectile);
            
            var currentBubble = projectile;
            var color = projectile.Color;
            
            HashSet<Bubble> queue = new(){ projectile };
            HashSet<Bubble> allBubble = new() ;

            while (queue.Count > 0)
            {
                var first = queue.First();
                queue.Remove(first);
                
                var findObjects = Physics2D.OverlapCircleAll(first.transform.position, offsetToBubbles.x);
                foreach (var findObject in findObjects)
                {
                    var bubble = findObject.GetComponent<Bubble>();
                    if(!bubble)
                        continue;
                    if(allBubble.Contains(bubble))
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
                    await bubble.transform.DOScale(0, 0.1f);
                    bubble.transform.localScale = Vector3.one;
                    poolBubbles.Release(bubble);
                }
            }
        }
    }
}
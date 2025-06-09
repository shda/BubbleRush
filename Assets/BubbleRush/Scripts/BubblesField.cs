using System.Collections.Generic;
using BlockBlast.Scripts.Common;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class BubblesField : MonoBehaviour
    {
        [SerializeField] private Vector2 offsetToBubbles = new(1,1);
        [SerializeField] private float offsetLine = 0.5f;
        [SerializeField] private PoolGameObjectsComponent<Bubble> poolBubbles;
        
        private readonly List<Bubble> _bubbles = new();
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
    }
}
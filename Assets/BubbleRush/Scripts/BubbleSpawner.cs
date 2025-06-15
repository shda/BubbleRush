using UnityEngine;
using Zenject;

namespace BubbleRush.Scripts
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private Transform rootBubble;
        
        public Bubble CurrentBubble { get; private set;}
        
        private BubblePool _bubblesPool;
        
        [Inject]
        public void Initialize(BubblePool bubblesPool)
        {
            _bubblesPool = bubblesPool;
        }
        
        public void SetNextBubble(int colorIndex)
        {
            var bubble = _bubblesPool.Create();
            bubble.SetColorIndex(colorIndex);
            bubble.PositionInGrid = Vector2Int.zero;
            bubble.transform.position = rootBubble.position;
            bubble.SetStatic();

            CurrentBubble = bubble;
        }
    }
}
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private BubblesPool  bubblesPool;
        [SerializeField] private Transform rootBubble;
        
        public Bubble CurrentBubble { get; private set;}
        public void SetNextBubble(int colorIndex)
        {
            var bubble = bubblesPool.Get();
            bubble.SetColorIndex(colorIndex);
            bubble.PositionInGrid = Vector2Int.zero;
            bubble.transform.position = rootBubble.position;
            bubble.SetStatic();

            CurrentBubble = bubble;
        }
    }
}
using System.Collections.Generic;

namespace BubbleRush.Scripts.Services
{
    public class FindOneColorBubblesConnect
    {
        private readonly Queue<Bubble> _queue = new();
        private readonly HashSet<Bubble> _visited = new();
        private readonly HashSet<Bubble> _findBubbles = new();
        
        public HashSet<Bubble> FindOneColorBubblesToOne(Bubble bubble)
        {
            _visited.Clear();
            _findBubbles.Clear();
            _queue.Enqueue(bubble);
            
            while (_queue.Count > 0)
            {
                var current = _queue.Dequeue();
                foreach (var neighbor in current.Neighbors)
                {
                    if(neighbor.Color != bubble.Color)
                        continue;
                    
                    _findBubbles.Add(current);
                    if (_visited.Add(neighbor))
                    {
                        _queue.Enqueue(neighbor);
                    }
                }
            }

            return _findBubbles;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace BubbleRush.Scripts
{
    public class FindUnconnectedBubblesToRoot
    {
        private Dictionary<Bubble,Bubble> parentMap = new();
        private HashSet<Bubble> visited = new();
        private Queue<Bubble> queue = new();

        public IEnumerable<Bubble> FindUnconnectedBubbles(HashSet<Bubble> allBubbles)
        {
            return allBubbles.Where(bubble => !CheckBubbleToConnectRoot(bubble));
        }

        private bool CheckBubbleToConnectRoot(Bubble bubble)
        {
            if (bubble.PositionInGrid.y == 0)
                return true;
            
            queue.Clear();
            visited.Clear();
            parentMap.Clear();
            
            queue.Enqueue(bubble);
            visited.Add(bubble);
            parentMap[bubble] = null;
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.PositionInGrid.y == 0) 
                    return true;
                
                foreach (var neighbor in current.Neighbors)
                {
                    if (visited.Add(neighbor))
                    {
                        parentMap[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return false;
        }
    }
}
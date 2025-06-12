using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class AnimationRemoveBubbles : MonoBehaviour
    {
        [SerializeField] private BubblesPool poolBubbles;
        
        private readonly FindUnconnectedBubblesToRoot findUnconnectedAreas = new();
        private readonly FindOneColorBubblesConnect findOneColorBubblesConnect = new();
        public async UniTask CheckAndRemoveFullLinesAsync(HashSet<Bubble> bubbles , Bubble projectile , Vector2 offsetToBubbles)
        {
            bubbles.Add(projectile);
            
            var color = projectile.Color;
            /*
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
            */

            var allBubble = findOneColorBubblesConnect.FindOneColorBubblesToOne(projectile);

            if (allBubble.Count >= 3)
            {
                foreach (var bubble in allBubble)
                {
                    await bubble.transform.DOScale(0, 0.03f);
                    bubble.transform.localScale = Vector3.one;
                    poolBubbles.Release(bubble);
                    bubbles.Remove(bubble);
                }
            }
        }
        
        public async UniTask RemoveFreeBubblesAsync(HashSet<Bubble> allBubbles)
        {
            var bubbles = findUnconnectedAreas.FindUnconnectedBubbles(allBubbles);
            HashSet<Bubble> bubblesHash = new();
            foreach (var bubble in bubbles)
            {
                bubblesHash.Add(bubble);
            }

            foreach (var bubble in bubblesHash)
            {
                await bubble.transform.DOScale(0, 0.03f);
                bubble.transform.localScale = Vector3.one;
                try
                {
                    allBubbles.Remove(bubble);
                    poolBubbles.Release(bubble);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                bubble.RemoveNeighbors();
            }
        }
    }
}
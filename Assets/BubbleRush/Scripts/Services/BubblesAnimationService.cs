using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts.Services
{
    public class BubblesAnimationService
    {
        private readonly FindUnconnectedBubblesToRoot _findUnconnectedAreas = new();
        private readonly FindOneColorBubblesConnect _findOneColorBubblesConnect = new();
        
        public async UniTask<int> RemoveEqualsColorsBubblesAsync(HashSet<Bubble> bubbles , Bubble projectile)
        {
            bubbles.Add(projectile);
            var allBubble = _findOneColorBubblesConnect.FindOneColorBubblesToOne(projectile);
            int countRemoved = 0;
            
            if (allBubble.Count >= 3)
            {
                countRemoved = allBubble.Count;
                foreach (var bubble in allBubble)
                {
                    await AnimationRemoveBubble(bubbles ,bubble);
                }
            }

            return countRemoved;
        }

        public async UniTask AnimationRemoveBubble(HashSet<Bubble> bubbles , Bubble bubble)
        {
            await bubble.AnimationHideBubbleAsync();
            bubble.transform.localScale = Vector3.one;
            try
            {
                bubble.ReturnToPool();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            bubbles.Remove(bubble);
            bubble.RemoveNeighborsRefs();
        }
        
        public async UniTask<int> RemoveFreeBubblesAsync(HashSet<Bubble> allBubbles)
        {
            var bubbles = _findUnconnectedAreas.FindUnconnectedBubbles(allBubbles);
            var  countRemoved = bubbles.Count();
            
            HashSet<Bubble> bubblesHash = new();
            foreach (var bubble in bubbles)
            {
                bubblesHash.Add(bubble);
            }

            foreach (var bubble in bubblesHash)
            {
                await AnimationRemoveBubble(allBubbles ,bubble);
            }

            return countRemoved;
        }
    }
}
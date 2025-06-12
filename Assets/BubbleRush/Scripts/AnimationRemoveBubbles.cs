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
        
        private readonly FindUnconnectedAreas findUnconnectedAreas = new();
        public async UniTask CheckAndRemoveFullLinesAsync(HashSet<Bubble> _bubbles , Bubble projectile , Vector2 offsetToBubbles)
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

            var bubbles = findUnconnectedAreas.FindFreeBubbles(_bubbles);
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
                try
                {
                    _bubbles.Remove(bubble);
                    poolBubbles.Release(bubble);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
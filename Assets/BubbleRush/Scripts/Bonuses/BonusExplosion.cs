using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusExplosion : BonusBase
    {
        [SerializeField] private Field field;

        public override async UniTask<bool> CompleteShotBubbleOnTarget(Bubble bubble)
        {
            var bubbles = FindBubbles(bubble);
            await field.AnimationRemoveBubbles(bubbles);
            await UniTask.DelayFrame(5);
            OnCompleteBonus?.Invoke(this);
            return false;
        }

        private Bubble[] FindBubbles(Bubble bubble)
        {
            HashSet<Bubble> radiusDestruction = new() { bubble };

            var colliders = ArrayPool<Collider2D>.Shared.Rent(20);
            var size = Physics2D.OverlapCircleNonAlloc(bubble.transform.position, bubble.GetOffsetToBubbles().x , colliders);
            for (int i = 0; i < size; i++)
            {
                var coll = colliders[i];
                if (coll != null && 
                    coll.gameObject.activeInHierarchy &&
                    coll.gameObject != gameObject && 
                    coll.TryGetComponent<Bubble>(out var findBuggles))
                {
                    radiusDestruction.Add(findBuggles);
                } 
            }
            
            ArrayPool<Collider2D>.Shared.Return(colliders);

            return radiusDestruction.ToArray();
        }
    }
}
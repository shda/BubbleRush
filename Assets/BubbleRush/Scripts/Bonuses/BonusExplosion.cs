using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace BubbleRush.Scripts.Bonuses
{
    public class BonusExplosion : BonusBase
    {
        [FormerlySerializedAs("field")] [SerializeField]
        private GameField gameField;
        // [SerializeField] private GameStats stats;

        private GameStats _gameStats;

        [Inject]
        private void Initialize(GameStats gameStats)
        {
            _gameStats = gameStats;
        }

        public override async UniTask<bool> CompleteShotBubbleOnTarget(Bubble bubble)
        {
            var bubbles = FindBubbles(bubble);
            await gameField.AnimationRemoveBubbles(bubbles);

            ScoreChangeTools.RemoveExplosionBonus(_gameStats, bubbles.Length);

            await UniTask.DelayFrame(5);
            OnCompleteBonus?.Invoke(this);
            return false;
        }

        private Bubble[] FindBubbles(Bubble bubble)
        {
            HashSet<Bubble> radiusDestruction = new() { bubble };

            var colliders = System.Buffers.ArrayPool<Collider2D>.Shared.Rent(20);
            var size = Physics2D.OverlapCircleNonAlloc(bubble.transform.position, bubble.GetOffsetToBubbles().x,
                colliders);
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

            System.Buffers.ArrayPool<Collider2D>.Shared.Return(colliders);

            return radiusDestruction.ToArray();
        }
    }
}
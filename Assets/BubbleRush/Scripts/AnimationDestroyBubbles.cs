using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class AnimationDestroyBubbles : MonoBehaviour
    {
        [SerializeField] private float timeToAnimation;
        
        public async UniTask PalayAnition(IEnumerable<Bubble> bubbles , Action<Bubble> OnComplete , CancellationToken token)
        {
            
        }
    }
}
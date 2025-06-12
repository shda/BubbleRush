using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class TriggerLineCrossedObjects : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _boxCollider2D;
        
        public Collider2D[] CheckOverlappingObjects2D()
        {
            var center = (Vector2)transform.position + _boxCollider2D.offset;
            var size = _boxCollider2D.size;
            var angle = transform.eulerAngles.z;

            var hitColliders = Physics2D.OverlapBoxAll(
                center,
                size,
                angle
            );

            return hitColliders;
        }

#if UNITY_EDITOR
        public GameObject[] overlappingObjects;
        private void Update()
        {
            overlappingObjects = CheckOverlappingObjects2D().Select(x => x.gameObject).ToArray();
        }
#endif
    }
}
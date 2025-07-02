using UnityEngine;

namespace BubbleRush.Scripts
{
    public class WallCollider : MonoBehaviour
    {
        [SerializeField] private bool isBubbleEnd = false;
        public bool IsBubbleEnd => isBubbleEnd;
    }
}
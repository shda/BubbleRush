using UnityEngine;

namespace BubbleRush.Scripts
{
    public class WallColloder : MonoBehaviour
    {
        [SerializeField] private bool isBubbleEnd = false;
        public bool IsBubbleEnd => isBubbleEnd;
    }
}
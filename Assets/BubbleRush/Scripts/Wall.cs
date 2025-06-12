using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private bool isBubbleEnd = false;
        public bool IsBubbleEnd => isBubbleEnd;
    }
}
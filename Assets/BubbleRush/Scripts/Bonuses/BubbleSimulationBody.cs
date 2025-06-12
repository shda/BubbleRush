using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BubbleSimulationBody : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rigidbody;
        public Rigidbody2D Rigidbody => rigidbody;
        public bool IsCollision { get; set; }
        public void OnCollisionEnter2D(Collision2D other)
        {
            if(IsCollision)
                return;
            
            if (other.collider.GetComponent<Bubble>())
            {
                IsCollision = true;
            }

            var wall = other.collider.GetComponent<Wall>();
            if (wall && wall.IsBubbleEnd)
            {
                IsCollision = true;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Serialization;

namespace BubbleRush.Scripts.Settings
{
    [CreateAssetMenu(menuName = "Create Settings/GameSettings", fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [field:Header("Game field settings")]
        [field:SerializeField] public Vector2 OffsetToBubbles { get; set; }= new(1.2f, 1.2f);
        [field:SerializeField] public int MaxColorIndex { get; set; } = 5;
        
        [field:Header("Cannon settings")] 
        [field:SerializeField] public Vector2 MinMaxAngleCannon { get; set; } = new(40,140);
        [field:SerializeField] public float SpeedBubbleMove { get; set; } = 1.0f;
        
        [field:Header("Game settings")] 
        [field:SerializeField] public int CountShotsAddLine { get; set; } = 5;
    }
}
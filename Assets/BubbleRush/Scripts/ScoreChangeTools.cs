namespace BubbleRush.Scripts
{
    public class ScoreChangeTools
    {
        public static void RemoveEqualsColors(GameStats stats , int countBubbles)
        {
            stats.Score.Value += countBubbles * 10;
        }
        
        public static void RemoveFreeBubbles(GameStats stats , int countBubbles)
        {
            stats.Score.Value += countBubbles * 5; 
        }
        
        public static void RemoveExplosionBonus(GameStats stats , int countBubbles)
        {
            stats.Score.Value += countBubbles * 5;
        }
    }
}
namespace BubbleRush.Scripts
{
    public interface IGameFieldMaker
    {
        int[,] GenerateField(int maxX, int maxY, int maxColorIndex);
        int[] CreateNewLine(int maxColorIndex);
    }
}
using System.Collections.Generic;
using System.Linq;
using BlockBlast.Scripts.Common.Extensions;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class FieldGenerator
    {
        public int[,] GenerateField(int maxX, int maxY, int maxColorIndex)
        {
            var field = new int[maxX, maxY];

            var offsetX = maxX / 2;
            var offsetY = maxY / 2;

            CreateRandomBlock(field, 0, 0, maxColorIndex);
            CreateRandomBlock(field, 0, offsetY, maxColorIndex);
            CreateRandomBlock(field, offsetX, 0, maxColorIndex);
            CreateRandomBlock(field, offsetX, offsetY, maxColorIndex);

            return field;
        }

        private void CreateRandomBlock(int[,] field, int startX, int startY, int maxColorIndex)
        {
            var usedColors = new HashSet<int>();
            var color1 = GetUniqueColor(usedColors , maxColorIndex);
            var color2 = GetUniqueColor(usedColors , maxColorIndex);
            var color3 = GetUniqueColor(usedColors , maxColorIndex);

            var usingColors = new List<int>();
            usingColors.AddRange(Enumerable.Repeat(color1, 4));
            usingColors.AddRange(Enumerable.Repeat(color2, 3));
            usingColors.AddRange(Enumerable.Repeat(color3, 2));
            usingColors.Shuffle();

            const int size = 3;
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    field[startX + x, startY + y] = usingColors[x * size + y];
                }
            }
        }
        
        private  int GetUniqueColor(HashSet<int> usedColors , int maxColorIndex)
        {
            int color;
            do
            {
                color = Random.Range(1, maxColorIndex);
            } while (usedColors.Contains(color));

            usedColors.Add(color);
            return color;
        }

        public int[] CreateNewLine(int maxColorIndex)
        {
            var usedColors = new HashSet<int>();
            var color1 = GetUniqueColor(usedColors , maxColorIndex);
            var color2 = GetUniqueColor(usedColors , maxColorIndex);
            var color3 = GetUniqueColor(usedColors , maxColorIndex);
            
            var usingColors = new List<int>();
            usingColors.AddRange(Enumerable.Repeat(color1, 3));
            usingColors.AddRange(Enumerable.Repeat(color2, 2));
            usingColors.AddRange(Enumerable.Repeat(color3, 1));
            usingColors.Shuffle();
            
            return usingColors.ToArray();
        }
    }
}
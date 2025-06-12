using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class FindUnconnectedAreas
    {
        private Bubble[,] grid;
        private int maxX;
        private int maxY;
        private bool[,] visited;

        
        public List<List<Bubble>> FindFreeBubbles(HashSet<Bubble> bubbles)
        {
            maxX = bubbles.Max(b => b.PositionInGrid.x) + 1;
            maxY = bubbles.Max(b => b.PositionInGrid.y) + 1;

            grid = new Bubble[maxX, maxY];
            foreach (var b in bubbles)
            {
                grid[b.PositionInGrid.x, b.PositionInGrid.y] = b;
            }

            visited = new bool[maxX, maxY];

            List<List<Bubble>> isolatedAreas = new();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (grid[x, y] && !visited[x, y])
                    {
                        List<Bubble> area = new();
                        ExploreArea(x, y, area);

                        // Если область состоит из одной клетки или не соединена с другими
                        if (area.All(areaPoint => areaPoint.PositionInGrid.y != 0))
                        {
                            isolatedAreas.Add(area);
                        }
                    }
                }
            }

            return isolatedAreas;
        }

        private void ExploreArea(int x, int y, List<Bubble> area)
        {
            if (x < 0 || x >= maxX || y < 0 || y >= maxY || grid[x, y] == null || visited[x, y])
                return;

            visited[x, y] = true;
            area.Add(grid[x , y]);

            // Проверяем 4 соседние клетки (верх, низ, лево, право)
            ExploreArea(x - 1, y, area);
            ExploreArea(x + 1, y, area);
            ExploreArea(x, y - 1, area);
            ExploreArea(x, y + 1, area);

            // Для диагональных связей добавьте эти строки:
            ExploreArea(x - 1, y - 1, area);
           // ExploreArea(x - 1, y + 1, area);
            ExploreArea(x + 1, y - 1, area);
            ExploreArea(x + 1, y + 1, area);
        }
    }
}
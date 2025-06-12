using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class FieldLogic : MonoBehaviour
    {
        private Bubble[,] grid;
        private int rows;
        private int cols;
        private bool[,] visited;

        
        public List<List<Bubble>> FindFreeBubbles(HashSet<Bubble> bubbles)
        {
            rows = bubbles.Max(b => b.PositionInGrid.x) + 1;
            cols = bubbles.Max(b => b.PositionInGrid.y) + 1;

            grid = new Bubble[rows, cols];
            foreach (var b in bubbles)
            {
                grid[b.PositionInGrid.x, b.PositionInGrid.y] = b;
            }

            visited = new bool[rows, cols];

            List<List<Bubble>> isolatedAreas = new();
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
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
            if (x < 0 || x >= rows || y < 0 || y >= cols || grid[x, y] == null || visited[x, y])
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
        
        private bool IsIsolated(List<Tuple<int, int>> area)
        {
            // Проверяем, есть ли связь с другими областями
            // В этой реализации все найденные области уже изолированы,
            // так как алгоритм помечает все связанные клетки как посещенные
            return true;
        }
    }
}
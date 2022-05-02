using System;
using System.Runtime.CompilerServices;

namespace CS5410
{
    public class Maze
    {
        public Cell[,] cellMap;
        public int height;
        public int width;
        public Cell start;
        public Cell finish;
        public int score;
        public double time;

        public Maze(int height, int width)
        {
            time = 0;
            this.height = height;
            this.width = width;
            cellMap = new Cell[width, height];
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    cellMap[i, j] = new Cell(i, j);
                }
            }
            start = cellMap[0, 0];
        }
    }
}
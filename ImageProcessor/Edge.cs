using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    class Edge
    {
        public int currentY;
        public int currentX;

        public int neighbourY;
        public int neighbourX;
        public double w;

        public int A => numberInOneDimArray(currentY, currentX);
        public int B => numberInOneDimArray(neighbourY, neighbourX);

        static int numberInOneDimArray(int y, int x) => y * x + x;

        public Edge(int x, int y)
        {
            currentX = x;
            currentY = y;
        }

        public void SetNeighbour(int x, int y)
        {
            neighbourX = x;
            neighbourY = y;
        }

        public void SetWeight(double w)
        {
            this.w = w;
        }

        public override string ToString()
        {
            return String.Format("Current Coords({0}, {1}) | NeighbourCoords({2}, {3}) | Weight:{4}", currentX, currentY, neighbourX, neighbourY, w);
        }
    }
}

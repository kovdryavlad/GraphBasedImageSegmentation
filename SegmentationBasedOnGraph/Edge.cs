using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph
{
    class Edge
    {
        public int currentY;
        public int currentX;

        public int neighbourY;
        public int neighbourX;
        public double w;

        int imageWidth;

        public int A => numberInOneDimArray(currentY, currentX);
        public int B => numberInOneDimArray(neighbourY, neighbourX);

        int numberInOneDimArray(int y, int x) => y * imageWidth + x;

        public NeightbourType neightbourType;

        public Edge(int x, int y, int imageWidth)
        {
            currentX = x;
            currentY = y;

            this.imageWidth = imageWidth;
        }

        public void SetNeighbour(int x, int y, NeightbourType neightbourType)
        {
            this.neightbourType = neightbourType;
            
            neighbourX = x;
            neighbourY = y;
        }

        public void SetWeight(double w)
        {
            this.w = w;
        }

        public override string ToString()
        {
            return String.Format("Current Coords({0}, {1}) | NeighbourCoords({2}, {3}) | Weight:{4} | NeightBour Type: {5}", currentX, currentY, neighbourX, neighbourY, w, neightbourType);
        }
    }
}

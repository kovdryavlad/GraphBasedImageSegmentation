using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{

    public class Segment
    {
        public Bitmap DoSegmentation(Bitmap bmp, double k = 200)
        {
            double[,,] arrayImage = BitmapConverter.BitmapToDoubleRgb(bmp);

            return DoSegmentation(arrayImage);
        }

        public Bitmap DoSegmentation(double[,,] arrayImage, double k = 200)
        {
            //препроцессинг иображения
            //arrayImage = DoubleArrayImageOperations.GetGrayScale(arrayImage);

            //построение графа
            Edge[] edges = buildGraphByImage(arrayImage)
                                .OrderBy(el => el.w)
                                .ToArray();
            //debugging
            double minWeight = edges.Min(el => el.w);
            double maxWeight = edges.Max(el => el.w);
            Edge[] EdgesMoreThanMin = edges.Where(el => el.w > minWeight + 0.1).ToArray();
            Edge[] EdgesZeroWidth = edges.Where(el => el.w < 0.01).ToArray();

            //сегментированный лес непересекающихся деревьев
            DisjointSet segmentedSet = SegmentOnDisjointSet(k, arrayImage, edges);

            int height = arrayImage.GetLength(1);
            int width = arrayImage.GetLength(2);

            return BitmapConverter.SegmentedSetToBitmap(segmentedSet, height, width);
        }

        private DisjointSet SegmentOnDisjointSet(double k, double[,,] grayScaleArrayImage, Edge[] edges)
        {
            int vertices = grayScaleArrayImage.GetLength(1) * grayScaleArrayImage.GetLength(2);

            DisjointSet disjointSet = new DisjointSet(vertices);

            //начальные значения устанавливаются в k, поскольку по формулам должно быть k/claster_size
            //claster size начальное равно 1
            double[] threshold = Enumerable.Range(0, vertices)
                                           .Select(el => k)
                                           .ToArray();


            // for each edge, in non-decreasing weight order...
            for (int i = 0; i < edges.Length; i++)
            {
                Edge edge = edges[i];

                // components conected by this edge
                int a = disjointSet.Find(edge.A);
                int b = disjointSet.Find(edge.B);
                if (a != b)
                {
                    if ((edge.w <= threshold[a]) && (edge.w <= threshold[b]))
                    {
                        disjointSet.Join(a, b);

                        a = disjointSet.Find(a);
                        threshold[a] = edge.w + k / disjointSet.Size(a);
                    }
                }
            }

            return disjointSet;
        }

        private Edge[] buildGraphByImage(double[,,] arrayImage)
        {
            List<Edge> result = new List<Edge>();

            int width = arrayImage.GetLength(2);
            int height = arrayImage.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x + 1 < width)
                    {
                        Edge gv = new Edge(x, y);
                        gv.SetNeighbour(x + 1, y);
                        gv.SetWeight(diff(arrayImage, x, y, x + 1, y));
                        result.Add(gv);
                    }

                    if (y + 1 < height)
                    {
                        Edge gv = new Edge(x, y);
                        gv.SetNeighbour(x, y+1);
                        gv.SetWeight(diff(arrayImage, x, y, x, y + 1));
                        result.Add(gv);
                    }

                    if ((x+1 < width) && (y +1< height))
                    {
                        Edge gv = new Edge(x, y);
                        gv.SetNeighbour(x+1, y + 1);
                        gv.SetWeight(diff(arrayImage, x, y, x + 1, y + 1));
                        result.Add(gv);
                    }

                    if ((x +1 < width) && (y > 0))
                    {
                        Edge gv = new Edge(x, y);
                        gv.SetNeighbour(x + 1, y - 1);
                        gv.SetWeight(diff(arrayImage, x, y, x + 1, y - 1));
                        result.Add(gv);
                    }
                }
            }

            return result.ToArray();
        }

        private double diff(double[,,] arrayImage, int x1, int y1, int x2, int y2)
        {
            /*
            //for monochrome image
            double I1 = arrayImage[0, y1, x1];
            double I2 = arrayImage[0, y2, x2];

            return Math.Abs(I1 - I2);
            */

            
            double rDiff = Math.Pow(arrayImage[0, y1, x1] - arrayImage[0, y2, x2],2);
            double gDiff = Math.Pow(arrayImage[1, y1, x1] - arrayImage[1, y2, x2],2);
            double bDiff = Math.Pow(arrayImage[2, y1, x1] - arrayImage[2, y2, x2],2);

            return Math.Sqrt(rDiff + gDiff + bDiff);
            
        }
    }

   
}

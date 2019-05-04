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
        public Bitmap DoSegmentation(Bitmap bmp, double sigma, double k, int minSize)
        {
            double[,,] arrayImage = BitmapConverter.BitmapToDoubleRgb(bmp);

            return DoSegmentation(arrayImage, sigma, k, minSize);
        }

        public Bitmap DoSegmentation(double[,,] arrayImage, double sigma, double k, int minSize)
        {
            //препроцессинг иображения
            //arrayImage = DoubleArrayImageOperations.GetGrayScale(arrayImage);

            //DebugImageInfo(arrayImage);

            GaussianBlur gaussianBlur = new GaussianBlur();
            double[][] filter = gaussianBlur.getKernel(sigma);

            arrayImage = DoubleArrayImageOperations.ConvolutionFilter(arrayImage, filter);

            //построение графа
            Edge[] edges = buildGraphByImage(arrayImage)
                                .OrderBy(el => el.w)
                                .ToArray();


            //debugging
            double minWeight = edges.Min(el => el.w);
            double maxWeight = edges.Max(el => el.w);
            Edge[] EdgesMoreThanMin = edges.Where(el => el.w > minWeight + 0.1).ToArray();
            Edge[] EdgesZeroWidth = edges.Where(el => el.w < 0.01).ToArray();

            Edge[] edgesHor = edges.Where(el => el.neightbourType == NeightbourType.Horizontal).ToArray();
            Edge[] edgesVer = edges.Where(el => el.neightbourType == NeightbourType.Vertical).ToArray();
            Edge[] edgesTopDiag = edges.Where(el => el.neightbourType == NeightbourType.TopDiagonal).ToArray();
            Edge[] edgesBottom = edges.Where(el => el.neightbourType == NeightbourType.BottomDiagonal).ToArray();
            
            //сегментированный лес непересекающихся деревьев
            DisjointSet segmentedSet = SegmentOnDisjointSet(k, arrayImage, edges);

            //присоеденить маленькие коппоненты к большим
            PostProcessSmallComponents(edges, segmentedSet, minSize);

            //присоеденить те, что меньше min_size к соседу по ребру

            int height = arrayImage.GetLength(1);
            int width = arrayImage.GetLength(2);

            return BitmapConverter.SegmentedSetToBitmap(segmentedSet, height, width);
        }

        private void PostProcessSmallComponents(Edge[] edges, DisjointSet segmentedSet, int minSize)
        {
            for (int i = 0; i < edges.Length; i++)
            {
                int a = segmentedSet.Find(edges[i].A);
                int b = segmentedSet.Find(edges[i].B);
                if ((a != b) && ((segmentedSet.Size(a) < minSize) || (segmentedSet.Size(b) < minSize)))
                    segmentedSet.Join(a, b);
            }
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
                        Edge gv = new Edge(x, y, width);
                        gv.SetNeighbour(x + 1, y, NeightbourType.Horizontal);
                        gv.SetWeight(diff(arrayImage, x, y, x + 1, y));
                        result.Add(gv);
                    }


                    if (y + 1 < height)
                    {
                        Edge gv = new Edge(x, y, width);
                        gv.SetNeighbour(x, y + 1, NeightbourType.Vertical);
                        gv.SetWeight(diff(arrayImage, x, y, x, y + 1));
                        result.Add(gv);
                    }


                    if ((x + 1 < width) && (y + 1 < height))
                    {
                        Edge gv = new Edge(x, y, width);
                        gv.SetNeighbour(x + 1, y + 1, NeightbourType.BottomDiagonal);
                        gv.SetWeight(diff(arrayImage, x, y, x + 1, y + 1));
                        result.Add(gv);
                    }


                    if ((x + 1 < width) && (y > 0))
                    {
                        Edge gv = new Edge(x, y, width);
                        gv.SetNeighbour(x + 1, y - 1, NeightbourType.TopDiagonal);
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


            double rDiff = Math.Pow(arrayImage[0, y1, x1] - arrayImage[0, y2, x2], 2);
            double gDiff = Math.Pow(arrayImage[1, y1, x1] - arrayImage[1, y2, x2], 2);
            double bDiff = Math.Pow(arrayImage[2, y1, x1] - arrayImage[2, y2, x2], 2);

            return Math.Sqrt(rDiff + gDiff + bDiff);

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
        
        private void DebugImageInfo(double[,,] arrayImage)
        {
            int width = arrayImage.GetLength(2);
            int height = arrayImage.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double r = arrayImage[0, y, x];
                    double g = arrayImage[1, y, x];
                    double b = arrayImage[2, y, x];

                    string s = String.Format("pixel({0}, {1}) | r:{2:0.0} g:{3:0.0} b:{4:0.0}", x, y, r, g, b);
                    System.Diagnostics.Debug.WriteLine(s);
                }
            }
        }
    }
}

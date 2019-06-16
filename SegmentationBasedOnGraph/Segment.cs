using ImageProcessor;
using SegmentationBasedOnGraph.ColorShemes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph
{

    public class Segment
    {
        public Bitmap DoSegmentation(Bitmap bmp, double sigma, double k, int minSize, IColorSheme colorSheme)
        {
            double[,,] arrayImage = BitmapConverter.BitmapToDoubleRgb(bmp);

            return DoSegmentation(arrayImage, sigma, k, minSize, colorSheme);
        }

        IColorSheme m_colorSheme;

        public Bitmap DoSegmentation(double[,,] arrayImage, double sigma, double k, int minSize, IColorSheme colorSheme)
        {
            //debug
            System.Diagnostics.Debug.WriteLine("Reading done: " + DateTime.Now);

            m_colorSheme = colorSheme;

            //препроцессинг иображения
            arrayImage = colorSheme.Convert(arrayImage);
            
            //debug
            System.Diagnostics.Debug.WriteLine("color sheme changed: " + DateTime.Now);
            //DebugImageInfo(arrayImage);

            //smoothing
            GaussianBlur gaussianBlur = new GaussianBlur();
            double[][] filter = gaussianBlur.getKernel(sigma);
            arrayImage = DoubleArrayImageOperations.ConvolutionFilter(arrayImage, filter);

            //debug
            System.Diagnostics.Debug.WriteLine("Smooting done: " + DateTime.Now);
            //тест размещения преобразования цвета
            //arrayImage = colorSheme.Convert(arrayImage);

            //построение графа
            Edge[] edges = buildGraphByImage(arrayImage)
                                .OrderBy(el => el.w)
                                .ToArray();

            //debug
            System.Diagnostics.Debug.WriteLine("graph builded: " + DateTime.Now);

            //debugging

            System.Diagnostics.Debug.WriteLine("edges total: "+ edges.Length);

            //double minWeight = edges.Min(el => el.w);
            //double maxWeight = edges.Max(el => el.w);
            //Edge[] EdgesMoreThanMin = edges.Where(el => el.w > minWeight + 0.1).ToArray();
            //Edge[] EdgesZeroWidth = edges.Where(el => el.w < 0.01).ToArray();
            //
            //Edge[] edgesHor = edges.Where(el => el.neightbourType == NeightbourType.Horizontal).ToArray();
            //Edge[] edgesVer = edges.Where(el => el.neightbourType == NeightbourType.Vertical).ToArray();
            //Edge[] edgesTopDiag = edges.Where(el => el.neightbourType == NeightbourType.TopDiagonal).ToArray();
            //Edge[] edgesBottom = edges.Where(el => el.neightbourType == NeightbourType.BottomDiagonal).ToArray();

            //сегментированный лес непересекающихся деревьев
            DisjointSet segmentedSet = SegmentOnDisjointSet(k, arrayImage, edges);
            //debug
            System.Diagnostics.Debug.WriteLine("Segmented: " + DateTime.Now);

            //присоеденить маленькие коппоненты к большим
            PostProcessSmallComponents(edges, segmentedSet, minSize);

            //debug
            System.Diagnostics.Debug.WriteLine("Small Component Merged: " + DateTime.Now);

            //присоеденить те, что меньше min_size к соседу по ребру

            int height = arrayImage.GetLength(1);
            int width = arrayImage.GetLength(2);

            return SegmentedSetToBitmapConverter.Convert(segmentedSet, height, width);
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
            double[] colorA = new[] { arrayImage[0, y1, x1], arrayImage[1, y1, x1], arrayImage[2, y1, x1] };
            double[] colorB = new[] { arrayImage[0, y2, x2], arrayImage[1, y2, x2], arrayImage[2, y2, x2] };

            return m_colorSheme.Difference(colorA, colorB);
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
                if(i%100000==0)
                    System.Diagnostics.Debug.WriteLine("itaration: " + i);

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

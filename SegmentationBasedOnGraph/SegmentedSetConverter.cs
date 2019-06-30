using ImageProcessor;
using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph
{
    class SegmentedSetConverter
    {
        public static Bitmap ConvertToBitmap(DisjointSet segmentedSet, int height, int width, out int segmentsCount)
        {
            double[,,] im = new double[3, height, width];

            Dictionary<int, ColorCustom> colors = new Dictionary<int, ColorCustom>();
            int totalSize = 0;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    ColorCustom ccc;

                    int comp = segmentedSet.Find(h * width + w);

                    if (colors.TryGetValue(comp, out ccc) == false)
                    {
                        ccc = ColorCustom.GetRandomColor();
                        colors.Add(comp, ccc);

                        int compSize = segmentedSet.Size(comp);
                        totalSize += compSize;
                        //System.Diagnostics.Debug.WriteLine("Component: " + comp + " | size: " + compSize);
                    }


                    im[0, h, w] = BitmapConverter.Limit(ccc.r);
                    im[1, h, w] = BitmapConverter.Limit(ccc.g);
                    im[2, h, w] = BitmapConverter.Limit(ccc.b);
                }
            }

            //System.Diagnostics.Debug.WriteLine("Total Size: " + totalSize);
            //System.Diagnostics.Debug.WriteLine("Height*Width: " + height * width);

            segmentsCount = colors.Count;
            return BitmapConverter.DoubleRgbToBitmap(im);
        }

        public static AssesmentsSegment[] ConvertToAssessmentSegments(DisjointSet segmentedSet, int height, int width, double[,,] arrayImage,IColorSheme colorSheme)
        {
            Dictionary<int, List<double[]>> segments = new Dictionary<int, List<double[]>>();
            
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int comp = segmentedSet.Find(h * width + w);
                    List<double[]> currentSegmentPointsList;

                    if (!segments.TryGetValue(comp, out currentSegmentPointsList))
                    {
                        currentSegmentPointsList = new List<double[]>();
                        segments.Add(comp, currentSegmentPointsList);
                    }

                    currentSegmentPointsList.Add(new double[] {arrayImage[0,h,w], arrayImage[1, h, w], arrayImage[2, h, w]});
                }
            }
            
            return segments.Values.ToList()
                                  .Select(segmentPoints=> new AssesmentsSegment(segmentPoints))
                                  .ToArray();
        }

        public static RealCoordsSegmentResult ConvertToRealCoordsSegments(DisjointSet segmentedSet, int height, int width)
        {
            Dictionary<int, List<Point>> segmentsDictionary = new Dictionary<int, List<Point>>();

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int comp = segmentedSet.Find(h * width + w);
                    List<Point> currentSegmentPointsList;

                    if (!segmentsDictionary.TryGetValue(comp, out currentSegmentPointsList))
                    {
                        currentSegmentPointsList = new List<Point>();
                        segmentsDictionary.Add(comp, currentSegmentPointsList);
                    }

                    currentSegmentPointsList.Add(new Point(w, h));
                }
            }

            RealCoordsSegment[] segments = segmentsDictionary.Values.ToList()
                                           .Select(segmentPoints => new RealCoordsSegment(segmentPoints.ToArray()))
                                           .ToArray();

            return new RealCoordsSegmentResult()
            {
                imageheight = height,
                imageWidth = width,
                realCoordsSegments = segments
            };
        }

        public static  Bitmap RealCoordsSegmentResultToBitmap(RealCoordsSegmentResult r)
        {
            double[,,] im = new double[3, r.imageheight, r.imageWidth];

            var segments = r.realCoordsSegments;

            for (int s = 0; s < segments.Length; s++)
            {
                ColorCustom ccc = ColorCustom.GetRandomColor();
                Point[] points = segments[s].coords;
                
                for (int i = 0; i < points.Length; i++)
                {
                    int y = points[i].Y;
                    int x = points[i].X;

                    im[0, y, x] = ccc.r;
                    im[1, y, x] = ccc.g;
                    im[2, y, x] = ccc.b;
                }
            }

            return BitmapConverter.DoubleRgbToBitmap(im);
        }
    }
}

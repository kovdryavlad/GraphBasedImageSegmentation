using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class Color
    {
        public double r;
        public double g;
        public double b;

        public static  Color GetRandomColor()
        {
            Random random = new Random();
            Color color = new Color();

            color.r = random.Next(256);
            color.g = random.Next(256);
            color.b = random.Next(256);

            return color;
        }

        public Color() { }

        public Color(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    public class DoubleArrayImageOperations
    {
        private static double[,,] processImage(double[,,] arrayImage, Action<Color> pixelAction)
        {
            double[,,] res = (double[,,])arrayImage.Clone();

            int width = arrayImage.GetLength(2),
                height = arrayImage.GetLength(1);

            Color c = new Color();

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {                    
                    c.r = res[0, h, w];
                    c.g = res[1, h, w];
                    c.b = res[2, h, w];
                    

                    pixelAction(c);

                    res[0, h, w] = c.r;
                    res[1, h, w] = c.g;
                    res[2, h, w] = c.b;
                }
            }

            return res;
        }

        public static double[,,] ChangeBrightness(double[,,] arrayImage, double n)
        {
            return processImage(arrayImage, (c) =>
            {
                c.r += n;
                c.g += n;
                c.b += n;
            });
        }

        public static double[,,] Negative(double[,,] arrayImage)
        {
            return processImage(arrayImage, (c) =>
            {
                c.r = 255 - c.r;
                c.g = 255 - c.g;
                c.b = 255 - c.b;
            });
        }

        public static double[,,] GetWhiteBlack(double[,,] arrayImage, double brightness)
        {
            return processImage(arrayImage, (c) =>
            {
                double sum = c.r + c.g + c.b;
                if (sum/3d > brightness)
                {
                    c.r = 255;
                    c.g = 255;
                    c.b = 255;
                }
                else {
                    c.r = 0;
                    c.g = 0;
                    c.b = 0;
                }
            });
        }

        public static double[,,] GetGrayScale(double[,,] arrayImage)
        {
            return processImage(arrayImage, (c) =>
            {
                double gray = c.r * 0.2126 + c.g * 0.7152 + c.b * 0.0722;
                c.r = gray;
                c.g = gray;
                c.b = gray;
            });
        }

        public static double[,,] Sepia(double[,,] arrayImage)
        {
            return processImage(arrayImage, (c) =>
            {
                double r = c.r, g = c.g, b = c.b;
                c.r = r * 0.393 + g * 0.769 + b * 0.189;
                c.g = r * 0.349 + g * 0.686 + b * 0.168;
                c.b = r * 0.272 + g * 0.534 + b * 0.131;
            });
        }

        public static double[,,] contrast(double[,,] arrayImage, double coef)
        {
            double average = 0;
            processImage(arrayImage, (c) =>
            {
                average += c.r * 0.299 + c.g * 0.587 + c.b * 0.114;
            });
            average /= arrayImage.GetLength(1) * arrayImage.GetLength(2);

            return processImage(arrayImage, (c) =>
            {
                c.r = average + coef* (c.r - average);
                c.g = average + coef* (c.g - average);
                c.b = average + coef* (c.b - average);
            });
        }
    }


}

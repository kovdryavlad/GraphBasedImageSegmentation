using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMatrix;

namespace ImageProcessor
{
    public class GaussianBlur
    {
        double m_sigma;

        double G(double x, double y) => Math.Exp(-(x * x + y * y) / (2 * m_sigma * m_sigma)) / Math.Sqrt(2 * Math.PI * m_sigma * m_sigma);

        public double[][] getKernel(double sigma)
        {
            m_sigma = sigma;

            int radius = kernelRadius(m_sigma);
            int size = 2 * radius + 1;

            double sum = 0;
            Matrix kernel = Matrix.Create.New(size);

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    sum += kernel[i, j] = G(i - radius, j - radius);
           
            kernel = 1d/sum * kernel;
            
            return kernel.data;
        }

        int kernelRadius(double sigma) => (int)Math.Ceiling(sigma * 3);
    }
}

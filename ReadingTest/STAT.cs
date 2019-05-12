using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingTest
{
    public partial class STAT
    {   
        public double[] d;              //массив для изменения
        
        //для использования в двумерных данных
        public void SetData(double[] input)
        {
            d = input;
            CalcMainParams();
        }

        public void CalcMainParams()
        {
            GetKvantil();
            CalcExpectation();
            CalcDisp();
            FillIntervalsForExpectation();
            FillIntervalsForSigmas();
        }
        
        //когда графики построены

        //точечные оценки

        public double Expectation;  //Мат ожидание
        public double Dispersia;    //дисперсия
        public double Sigma;        //среднее квадр. отклонение
        
        public void CalcExpectation() // математическое ожидание
        {
            double _result = 0;
            for (int i = 0; i < d.Length; i++)
                _result += d[i];
            Expectation = _result / d.Length;
        }

        public void CalcDisp() //Дисперсия
        {
            double S = 0;
            for (int i = 0; i < d.Length; i++)
                S += Math.Pow((d[i] - Expectation), 2);

            S /= d.Length - 1;

            Dispersia = S;
            Sigma = Math.Sqrt(S);
        }       
        //закончились точечные оценки

        //интервальные оценки 
        //Сигмы для интервальных оценок
        public double Sigma_Expectation;       //для мат ожидания
        public double Sigma_Dispersia;         //для дисперсии

       
        public void CalcSigma_Expectation()
        {
            Sigma_Expectation = Sigma / Math.Sqrt(d.Length);
        }

        public void CalcSigma_Dispersia()
        {
            Sigma_Dispersia = Sigma / Math.Sqrt(2 * d.Length);
        }

        //Оценки
        //формат названия переменных - Параментр_нижняя тета,Параментр_верхняя тета
        public double Expectation_niz;
        public double Expectation_verh;

        public double Sigma_niz;
        public double Sigma_verh;
        
        public double gamma = 0.95;

        public double _kvantil;

        //интервальные оценки среднего //для StatNd
        public void FillIntervalsForExpectation()
        {
            CalcSigma_Expectation();

            Expectation_niz = Expectation - _kvantil * Sigma_Expectation;
            Expectation_verh = Expectation + _kvantil * Sigma_Expectation;
        }

        //интервальные оценки среднего //для StatNd
        public void FillIntervalsForSigmas()
        {
            CalcSigma_Dispersia();

            Sigma_niz = Sigma - _kvantil * Sigma_Dispersia;
            Sigma_verh = Sigma + _kvantil * Sigma_Dispersia;
        }

       private double GetKvantil()
        {
            double alpha = 1 - gamma;
            
            return Kvantili.Normal(alpha / 2);
        }
    }

}

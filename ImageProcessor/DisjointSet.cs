using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class DisjointSet
    {
        class SetElement    //tree
        {
            public int m_rank;
            public int m_parent;
            public int m_size;

            public SetElement(int parent)
            {
                m_rank = 0;
                m_size = 1;

                m_parent = parent;
            }
        }

        SetElement[] m_elements;
        int m_number;       //количество компонентов. != m_elements.Length

        public DisjointSet(int numberOfElements)
        {
            m_number = numberOfElements;

            /*
            m_elements = new SetElement[m_number];

            for (int i = 0; i < m_number; i++)
                m_elements[i] = new SetElement(i);
            */

            //в начале алгоритма, каждый эелемент назначается родителем самому себе
            m_elements = Enumerable.Range(0, m_number)
                                   .Select(i => new SetElement(i))
                                   .ToArray();

        }

        //найти представителя класса, для элемента x
        public int Find(int x)
        {
            int represEl = x;   //representative element

            //пока в родителе не встеретим себя (особенность структуры)
            while (represEl != m_elements[represEl].m_parent)
                represEl = m_elements[represEl].m_parent;

            //сжатие путей
            m_elements[x].m_parent = represEl;

            return represEl;
        }

        //Мой вариант - потом для проверки
        public int FindHardCompressionPath(int x)
        {
            int represEl = x;   //representative element

            List<int> els_forPathCompression = new List<int>();

            //пока в родителе не встеретим себя (особенность структуры)
            while (represEl != m_elements[represEl].m_parent)
            {
                //запоминаем какие элемент проходим
                els_forPathCompression.Add(represEl);
                represEl = m_elements[represEl].m_parent;

            }

            //тяжелое сжатие путей;
            els_forPathCompression.Select(index => m_elements[index].m_parent = represEl);

            return represEl;
        }

        public void Join(int x, int y)
        {
            if (m_elements[x].m_rank > m_elements[y].m_rank)
            {
                m_elements[y].m_parent = x;
                m_elements[x].m_size += m_elements[y].m_size;
            }
            else
            {
                m_elements[x].m_parent = y;
                m_elements[y].m_size += m_elements[x].m_size;

                if (m_elements[x].m_rank == m_elements[y].m_rank)
                    m_elements[y].m_rank++;
            }

            m_number--;
        }

        public int Size(int componentNumber) => m_elements[componentNumber].m_size;

        public int Components => m_number;
    }


}

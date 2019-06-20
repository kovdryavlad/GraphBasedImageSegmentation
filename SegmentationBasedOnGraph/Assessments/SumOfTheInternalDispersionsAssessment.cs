using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.Assessments
{
    class SumOfTheInternalDispersionsAssessment : AssessmentBase
    {
        public SumOfTheInternalDispersionsAssessment()
        {
            this.name = "Сума внутрішньокластерних дисперсій"; 
        }

        internal override double GeAssessment(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            double distance = 0;
            int k = segments.Length;

            for (int j = 0; j < k; j++)
                for (int l = 0; l < segments[j].Length; l++)
                    distance += Math.Pow(colorSheme.AssessmentsDifference(segments[j].points[l], segments[j].center), 2);
                    //distance += colorSheme.AssessmentsDifference(segments[j].points[l], segments[j].center);

            return distance;
        }
    }
}

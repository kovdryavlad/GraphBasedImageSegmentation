using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;

namespace SegmentationBasedOnGraph.Assessments
{
    class SumOfPairwiseInternalDistancesAssessment : AssessmentBase
    {
        public SumOfPairwiseInternalDistancesAssessment()
        {
            this.name = "Сума попарних внутрішньокластерних відстаней";
        }
        internal override double GeAssessment(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            double distance = 0;
            int k = segments.Length;

            for (int j = 0; j < k; j++)
                for (int l = 0; l < segments[j].Length - 1; l++)
                    for (int h = l + 1; h < segments[j].Length; h++)
                        distance += colorSheme.AssessmentsDifference(segments[j].points[l], segments[j].points[h]);

            return distance;
        }
    }
}

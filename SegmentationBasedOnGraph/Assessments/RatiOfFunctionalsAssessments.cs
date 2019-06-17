using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;

namespace SegmentationBasedOnGraph.Assessments
{
    class RatiOfFunctionalsAssessments : AssessmentBase
    {
        internal override double GeAssessment(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            int k = segments.Length;

            SumOfPairwiseInternalDistancesAssessment tempassessment = new SumOfPairwiseInternalDistancesAssessment();
            
            double a = tempassessment.GeAssessment(segments, colorSheme);

            double denominatorToACoef = 0;
            double denominatorToBCoef = 1;

            for (int i = 0; i < k; i++)
            {
                var Nj = segments[i].Length;
                denominatorToACoef += (Nj * (Nj - 1) / 2d);

                denominatorToBCoef *= Nj;
            }

            a /= 1d / denominatorToACoef;

            double b = 0;

            for (int j = 0; j < k - 1; j++)
                for (int l = 0; l < segments[j].Length; l++)
                    for (int m = j + 1; m < k; m++)
                        for (int h = 0; h < segments[m].Length; h++)
                            b += colorSheme.Difference(segments[j].points[l], segments[m].points[h]);

            b /= 1d / denominatorToBCoef;

            return a / b;
        }
    }
}

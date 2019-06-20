using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;
using SimpleMatrix;

namespace SegmentationBasedOnGraph.Assessments
{
    class ScoreFunctionAssessment:AssessmentBase
    {
        public ScoreFunctionAssessment()
        {
            this.name = "Score function";
        }

        internal override double GeAssessment(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            double bdc = GetBDC(segments, colorSheme);
            double wcd = GetWDC(segments, colorSheme);

            return 1 - 1d / Math.Exp(Math.Exp(bdc-wcd));
        }

        private double GetBDC(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            int N = segments.Sum(s => s.points.Count);

            Vector v = new Vector(new double[3]);
            for (int i = 0; i < segments.Length; i++)
                v += new Vector(segments[i].center);

            v /= segments.Length;

            double[] centerOfCenters = v.GetCloneOfData();

            double bcd = 0;

            for (int i = 0; i < segments.Length; i++)
                bcd += colorSheme.AssessmentsDifference(segments[i].center, centerOfCenters)*segments[i].Length;

            return bcd / (N * segments.Length);
        }

        private double GetWDC(AssesmentsSegment[] segments, IColorSheme colorSheme)
        {
            double wcd = 0;

            for (int i = 0; i < segments.Length; i++)
                for (int j = 0; j < segments[i].Length; j++)
                    wcd += colorSheme.AssessmentsDifference(segments[i].points[j], segments[i].center)/ segments[i].Length;

            return wcd / segments.Length;
        }

        
    }
}

using SegmentationBasedOnGraph.ColorShemes;
using SegmentationBasedOnGraph.ResultSegments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.Assessments
{
    internal abstract class AssessmentBase
    {
        internal string name; //если вдруг будет нужно написать нормальное название
        internal abstract double GeAssessment(AssesmentsSegment[] segments, IColorSheme colorSheme);
    }
}

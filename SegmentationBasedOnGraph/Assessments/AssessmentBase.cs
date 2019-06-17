using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.Assessments
{
    internal abstract class AssessmentBase
    {
        internal string name;
        internal abstract double GeAssessment();
    }
}

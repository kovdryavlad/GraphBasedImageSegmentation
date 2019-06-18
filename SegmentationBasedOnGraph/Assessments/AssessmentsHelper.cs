using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationBasedOnGraph.Assessments
{
    class AssessmentsHelper
    {
        public static AssessmentBase[] GetAllAssessments()
        {
            return new AssessmentBase[]
            {
                new SumOfTheInternalDispersionsAssessment(),
                //new SumOfPairwiseInternalDistancesAssessment(),
                //new TotalIntroClusterDispersionAssessment(),
                //new RatiOfFunctionalsAssessments() // вычислительно очень сложный
            };
        }
    }
}

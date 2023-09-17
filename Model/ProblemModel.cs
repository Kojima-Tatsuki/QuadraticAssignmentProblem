using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAP.Model
{
    internal class ProblemModel
    {
        private DistanceModel Factory { get; init; }
        private FlowModel Flow { get; init; }

        public ProblemModel(DistanceModel factoy, FlowModel flow)
        {
            Factory = factoy;
            Flow = flow;
        }

        public int GetScore()
        {
            throw new NotImplementedException();
        }
    }
}

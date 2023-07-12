using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAP.Model
{
    internal class ProbremModel
    {
        private List<DistanceModel> Factories { get; init; }
        private List<FlowModel> Flows { get; init; }

        public ProbremModel(IReadOnlyList<DistanceModel> factories, IReadOnlyList<FlowModel> flows)
        {
            Factories = factories.ToList();
            Flows = flows.ToList();
        }

        public int GetScore()
        {
            throw new NotImplementedException();
        }
    }
}

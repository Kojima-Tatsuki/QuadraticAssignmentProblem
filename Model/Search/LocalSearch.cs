using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAP.Model.Search
{
    internal class LocalSearch : ISearch
    {
        public ProblemModel Problem { get; init; }

        public LocalSearch(ProblemModel problem)
        {
            Problem = problem;
        }

        public SearchResult Search(IReadOnlyList<int> initAns)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using Alastri.SpryGraph;

namespace UnitTestProject1
{
    public class TestEdge : ICostedEdge<TestVertex>, QuickGraph.IEdge<TestVertex>
    {


        private readonly double _cost;

        public TestEdge(TestVertex source, TestVertex target)
        {
            Target = target;
            Source = source;
            _cost = Source.Heuristic(Target);
        }

        public TestVertex Source { get; set; }

        public TestVertex Target { get; set; }

        public double GetCost()
        {
            return _cost;
        }

        public override string ToString()
        {
            return String.Format("{0} -> {1}", Source, Target);
        }
    }
}
using System;
using System.Collections.Generic;
using Alastri.SpryGraph;

namespace UnitTestProject1
{
    public class TestVertex : IHeuristicVertex<TestVertex>
    {
        private double x;        

        public TestVertex(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public List<TestVertex> Out { get { return _out; } }

        readonly List<TestVertex> _out = new List<TestVertex>();
        private List<TestEdge> _outEdges;
        private double y;
        private double z;

        public List<TestEdge> OutEdges
        {
            get
            {
                if (_outEdges == null)
                {
                    _outEdges = new List<TestEdge>();
                    foreach (var item in Out)
                    {
                        _outEdges.Add(new TestEdge(this, item));
                    }
                }
                return _outEdges;
            }
        }

        public double Heuristic(TestVertex destination)
        {
            double xd = destination.x - this.x;
            double yd = destination.y - this.y;
            double zd = destination.z - this.z;
            return Math.Sqrt(xd*xd + yd*yd + zd*zd);
        }

        public override string ToString()
        {
            return String.Format("{0:0.0},{1:0.0},{2:0.0}", x, y, z);
        }
    }
}
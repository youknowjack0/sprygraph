using System;
using System.Collections.Generic;
using Alastri.SpryGraph;

namespace UnitTestProject1
{
    public class TestVertex : IHeuristicVertex<TestVertex>
    {
        private double _x;        

        public TestVertex(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public List<TestVertex> Out { get { return _out; } }

        readonly List<TestVertex> _out = new List<TestVertex>();
        private List<TestEdge> _outEdges;
        private readonly double _y;
        private readonly double _z;

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
            double xd = destination._x - _x;
            double yd = destination._y - _y;
            double zd = destination._z - _z;
            return Math.Sqrt(xd*xd + yd*yd + zd*zd);
        }

        public override string ToString()
        {
            return String.Format("{0:0.0},{1:0.0},{2:0.0}", _x, _y, _z);
        }
    }
}
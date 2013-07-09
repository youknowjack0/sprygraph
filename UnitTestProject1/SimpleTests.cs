using System;
using System.Collections.Generic;
using System.Linq;
using Alastri.SpryGraph;
using NUnit.Framework;
using QuickGraph;


namespace UnitTestProject1
{
    /// <summary>
    /// Test against known-good implementation (QuickGraph)
    /// </summary>
    [TestFixture]
    public class SimpleTests
    {
        [Test]
        public void TestMethod1()
        {
            IImplicitCostedHeuristicGraph<TestVertex, TestEdge> graph = new TestGraph(null,null);

            var gr = new GraphReader<TestVertex, TestEdge>(graph);

            var v1 = new TestVertex(1);
            var v2 = new TestVertex(2);
            var v3 = new TestVertex(3);
            var v5 = new TestVertex(5);
            var v4 = new TestVertex(4);

            v1.Out.Add(v2);
            v2.Out.Add(v3);
            v3.Out.Add(v4);
            v3.Out.Add(v5);
            v5.Out.Add(v4);

            PathFinder<TestVertex, TestEdge> pathfinder = gr.GetPathFinder(v1);
            

            for (int i = 0; i < 2; i++)
            {
                TestEdge[] path;
                Assert.True(pathfinder.TryGetPath(v4, out path));

                Assert.True(path[0].Target == v2);
                Assert.True(path[1].Target == v3);
                Assert.True(path[2].Target == v4 || path[1].Target == v5);
                Assert.True(path.Length == 3 || path.Length == 4);

                PathFinder<TestVertex, TestEdge> pathfinder2 = gr.GetPathFinder(v2);

                Assert.True(pathfinder2.TryGetPath(v4, out path));

                Assert.True(path[0].Target==v3);
                Assert.True(path[1].Target == v4 || path[1].Target == v5);
                Assert.True(path.Length == 2 || path.Length == 3);
            }

            
        }    

        [Test]
        public void SourceIsDest()
        {
            IImplicitCostedHeuristicGraph<TestVertex, TestEdge> graph = new TestGraph(null,null);

            var gr = new GraphReader<TestVertex, TestEdge>(graph);

            var v1 = new TestVertex(1);
            var v2 = new TestVertex(2);
            var v3 = new TestVertex(3);
            var v5 = new TestVertex(5);
            var v4 = new TestVertex(4);

            v1.Out.Add(v2);
            v2.Out.Add(v3);
            v3.Out.Add(v4);
            v3.Out.Add(v5);
            v5.Out.Add(v4);

            
            

            for (int i = 0; i < 2; i++)
            {
                TestEdge[] path;
                PathFinder<TestVertex, TestEdge> pathfinder = gr.GetPathFinder(v1);
                Assert.True(pathfinder.TryGetPath(v1, out path));
                Assert.True(path.Length == 0);

                pathfinder = gr.GetPathFinder(v2);
                Assert.True(pathfinder.TryGetPath(v2, out path));
                Assert.True(path.Length == 0);

                pathfinder = gr.GetPathFinder(v3);
                pathfinder.TryGetPath(v4, out path);

                Assert.True(pathfinder.TryGetPath(v3, out path));
                Assert.True(path.Length == 0);
            }

            
        }
    }

    public class TestGraph : IHybridGraph
    {
        private IList<TestVertex> _vertices;
        private readonly IList<TestEdge> _edges;

        public TestGraph(IList<TestVertex> vertexList, IList<TestEdge> edges )
        {
            _vertices = vertexList;
            _edges = edges;
        }

        public IReadOnlyList<TestEdge> GetOutEdges(TestVertex vertex)
        {
            return vertex.OutEdges;
        }

        public bool IsDirected { get { return true; } }
        public bool AllowParallelEdges { get { return true; } }
        public bool ContainsVertex(TestVertex vertex)
        {
            return _vertices.Contains(vertex);
        }

        public bool IsOutEdgesEmpty(TestVertex v)
        {
            return v.OutEdges.Count > 0;
        }

        public int OutDegree(TestVertex v)
        {
            return v.OutEdges.Count;
        }

        public IEnumerable<TestEdge> OutEdges(TestVertex v)
        {
            return v.OutEdges;
        }

        public bool TryGetOutEdges(TestVertex v, out IEnumerable<TestEdge> edges)
        {
            edges = v.OutEdges;
            return true;
        }

        public TestEdge OutEdge(TestVertex v, int index)
        {
            return v.OutEdges[index];
        }

        public bool ContainsEdge(TestVertex source, TestVertex target)
        {
            return source.OutEdges.Any(x => x.Target == target);
        }

        public bool TryGetEdges(TestVertex source, TestVertex target, out IEnumerable<TestEdge> edges)
        {
            edges = source.OutEdges.Where(x => x.Target == target);

            return true;
        }

        public bool TryGetEdge(TestVertex source, TestVertex target, out TestEdge edge)
        {
            edge = source.OutEdges.FirstOrDefault(x => x.Target == target);
            if (edge == null) return false;
            return true;
        }

        public bool IsVerticesEmpty
        {
            get { return _vertices.Count == 0; }
        }

        public int VertexCount {
            get { return _vertices.Count; }
        }
        public IEnumerable<TestVertex> Vertices { get { return _vertices; } }
        public bool ContainsEdge(TestEdge edge)
        {
            return _edges.Contains(edge);
        }

        public bool IsEdgesEmpty { get { return _edges.Count == 0; } }

        public int EdgeCount { get { return _edges.Count; } }


        public IEnumerable<TestEdge> Edges { get { return _edges; } }

        public IList<TestVertex> VerticesList { get { return _vertices; } }
    }

    public class TestEdge : ICostedEdge<TestVertex>, QuickGraph.IEdge<TestVertex>
    {
        private readonly double _cost;

        private static readonly Random R = new Random();

        public TestEdge(TestVertex source, TestVertex target)
        {
            Target = target;
            Source = source;
            _cost = R.NextDouble();
        }

        public TestEdge(TestVertex source,TestVertex target,double cost) : this(target, source)
        {
            _cost = cost;
        }

        public TestVertex Source { get; set; }
        public TestVertex Target { get; set; }
        public double GetCost()
        {
            return _cost;
        }
    }

    public class TestVertex : IHeuristicVertex<TestVertex>
    {
        private int x;

        public TestVertex(int x)
        {
            this.x = x;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public List<TestVertex> Out { get { return _out; } }

        readonly List<TestVertex> _out = new List<TestVertex>();
        private List<TestEdge> _outEdges;

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
            throw new NotImplementedException();
        }
    }
}

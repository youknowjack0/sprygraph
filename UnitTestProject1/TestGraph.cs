using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
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
}
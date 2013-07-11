using System.Runtime.CompilerServices;

namespace Alastri.SpryGraph
{
    internal sealed class VertexInternal<TVertex, TEdge> where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {
        private TVertex _vertex;
        private EdgeInternal<TVertex,TEdge>[] _outEdges;
        private int _id;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VertexInternal(int id, TVertex vertex)
        {
            _id = id;
            _vertex = vertex;
        }


        public int Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _id; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EdgeInternal<TVertex,TEdge>[] GetOutEdges(GraphReader<TVertex,TEdge> graph )
        {
            if (_outEdges == null)
            {
                var outEdges = graph.Source.GetOutEdges(_vertex);
                _outEdges = new EdgeInternal<TVertex, TEdge>[outEdges.Count];
                for (int i=0;i<outEdges.Count;i++)
                {
                    var edge = outEdges[i];
                    _outEdges[i] = new EdgeInternal<TVertex, TEdge>(edge.GetCost(), graph.GetVertexInternal(edge.Target), edge);
                }
            }

            return _outEdges;
        }

    }
}
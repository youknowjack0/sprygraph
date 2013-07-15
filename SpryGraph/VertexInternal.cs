using System.Runtime.CompilerServices;

namespace Alastri.SpryGraph
{
    public sealed class VertexInternal<TVertex, TEdge> where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {
        private readonly TVertex _vertex;
        private EdgeInternal<TVertex,TEdge>[] _outEdges;
        private readonly int _id;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal VertexInternal(int id, TVertex vertex)
        {
            _id = id;
            _vertex = vertex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal double Heuristic(VertexInternal<TVertex,TEdge> destination)
        {
            return _vertex.Heuristic(destination._vertex);
        }


        internal int Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _id; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EdgeInternal<TVertex, TEdge>[] GetOutEdges(GraphReader<TVertex, TEdge> graph)
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
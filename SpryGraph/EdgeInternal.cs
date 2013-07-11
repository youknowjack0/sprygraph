using System.Runtime.CompilerServices;

namespace Alastri.SpryGraph
{    
    internal sealed class EdgeInternal<TVertex, TEdge> where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {
        private readonly double _cost;        
        private readonly VertexInternal<TVertex, TEdge> _target;
        private readonly TEdge _underlyingEdge;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EdgeInternal(double cost, VertexInternal<TVertex, TEdge> target, TEdge underlyingEdge)
        {
            _cost = cost;
            _target = target;
            _underlyingEdge = underlyingEdge;
        }

        public double Cost
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _cost; }
        }

        public VertexInternal<TVertex, TEdge> Target
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _target; }
        }

        public TEdge UnderlyingEdge
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _underlyingEdge; }
        }
    }
}
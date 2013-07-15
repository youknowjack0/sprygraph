using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    public interface IBidirectionalGraph<TVertex, TEdge> : IImplicitGraph<TVertex, TEdge> 
        where TEdge : IEdge<TVertex>
    {
        IReadOnlyList<TEdge> GetInEdges(TVertex vertex);
    }
}
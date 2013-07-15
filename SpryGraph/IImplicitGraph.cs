using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    /// <summary>
    /// A implicit immutable directed graph    
    /// </summary>
    public interface IImplicitGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        IReadOnlyList<TEdge> GetOutEdges(TVertex vertex);
    }
}

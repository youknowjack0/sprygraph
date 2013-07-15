using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    public interface IEdgeListGraph<TVertex, TEdge>
    {
        ICollection<TEdge> Edges { get; }
    }
}
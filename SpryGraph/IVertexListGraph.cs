using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    public interface IVertexListGraph<TVertex, TEdge>
    {
        ICollection<TVertex> Vertices { get; }
    }
}
using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    /// <summary>
    /// A implicit immutable directed graph    
    /// </summary>
    /// <typeparam name="TVertex"></typeparam><typeparam name="TEdge"></typeparam>
    public interface IImplicitCostedGraph<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
    {
        IReadOnlyList<TEdge> GetOutEdges(TVertex vertex);
    }

    public interface ICostedEdge<TVertex> : IEdge<TVertex>
    {
        double GetCost();
    }

    public interface IEdge<TVertex>
    {
        TVertex Target { get; }
    }
}

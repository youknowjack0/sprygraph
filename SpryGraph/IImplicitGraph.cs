using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    /// <summary>
    /// A implicit immutable directed graph    
    /// </summary>
    /// <typeparam name="TVertex"></typeparam><typeparam name="TEdge"></typeparam>
    public interface IImplicitGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        IReadOnlyList<TEdge> GetOutEdges(TVertex vertex);
    }

    public interface IImplicitCostedHeuristicGraph<TVertex, TEdge> : IImplicitCostedGraph<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
        
    }


    public interface IImplicitCostedGraph<TVertex, TEdge> : IImplicitGraph<TVertex,TEdge> 
        where TEdge : ICostedEdge<TVertex>         
    {        
        
    }

    public interface IHeuristicVertex<TVertex>
    {
        double Heuristic(TVertex destination);
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

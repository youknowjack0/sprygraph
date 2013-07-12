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

    public interface IMinimalPathFindingGraph<TVertex, TEdge> : IImplicitCostedGraph<TVertex, TEdge>,
                                                                ICostedHeuristicGraph<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {

    }

    /// <summary>
    /// A fully featured graph
    /// </summary>
    public interface IGraph<TVertex, TEdge> : IMinimalPathFindingGraph<TVertex, TEdge>, IVertexListGraph<TVertex, TEdge>,
                                              IEdgeListGraph<TVertex, TEdge>, IBidirectionalGraph<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
    }



    public interface IHeuristicGraph<TVertex, TEdge> : IImplicitGraph<TVertex,TEdge> 
        where TVertex : IHeuristicVertex<TVertex> 
        where TEdge : IEdge<TVertex>
    {        
    }

    public interface ICostedGraph<TVertex, TEdge> : IImplicitGraph<TVertex,TEdge> 
        where TVertex : IHeuristicVertex<TVertex> 
        where TEdge : IEdge<TVertex>
    {
        
    }

    public interface ICostedHeuristicGraph<TVertex, TEdge> : IHeuristicGraph<TVertex, TEdge>, ICostedGraph<TVertex,TEdge> 
        where TVertex : IHeuristicVertex<TVertex> 
        where TEdge : IEdge<TVertex>
    {
        
    }

    public interface IBidirectionalGraph<TVertex, TEdge> : IImplicitGraph<TVertex, TEdge> 
        where TEdge : IEdge<TVertex>
    {
        IReadOnlyList<TEdge> GetInEdges(TVertex vertex);
    }

    public interface IVertexListGraph<TVertex, TEdge>
    {
        ICollection<TVertex> Vertices { get; }
    }

    public interface IEdgeListGraph<TVertex, TEdge>
    {
        ICollection<TEdge> Edges { get; }
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
        TVertex Source { get; }
        TVertex Target { get; }
    }
}

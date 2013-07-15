namespace Alastri.SpryGraph
{
    public interface IHeuristicGraph<TVertex, TEdge> : IImplicitGraph<TVertex,TEdge> 
        where TVertex : IHeuristicVertex<TVertex> 
        where TEdge : IEdge<TVertex>
    {        
    }
}
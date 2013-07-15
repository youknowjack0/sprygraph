namespace Alastri.SpryGraph
{
    public interface ICostedGraph<TVertex, TEdge> : IImplicitGraph<TVertex,TEdge> 
        where TVertex : IHeuristicVertex<TVertex> 
        where TEdge : IEdge<TVertex>
    {
        
    }
}
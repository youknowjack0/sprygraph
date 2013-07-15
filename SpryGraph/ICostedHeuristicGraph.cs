namespace Alastri.SpryGraph
{
    public interface ICostedHeuristicGraph<TVertex, TEdge> : IHeuristicGraph<TVertex, TEdge>, ICostedGraph<TVertex,TEdge> 
        where TVertex : IHeuristicVertex<TVertex> 
        where TEdge : IEdge<TVertex>
    {
        
    }
}
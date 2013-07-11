namespace Alastri.SpryGraph
{
    public interface IPathFinder<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>        
    {
        TVertex Source { get; }
        bool TryGetPath(TVertex destination, out TEdge[] path );
    }
}
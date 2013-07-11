namespace Alastri.SpryGraph
{
    public interface IPathFinder<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
        TVertex Source { get; }
        bool TryGetPath(TVertex destination, out TEdge[] path );
    }
}
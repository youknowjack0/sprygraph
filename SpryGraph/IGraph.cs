namespace Alastri.SpryGraph
{
    /// <summary>
    /// A fully featured graph
    /// </summary>
    public interface IGraph<TVertex, TEdge> : IMinimalPathFindingGraph<TVertex, TEdge>, IVertexListGraph<TVertex, TEdge>,
                                              IEdgeListGraph<TVertex, TEdge>, IBidirectionalGraph<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
    }
}
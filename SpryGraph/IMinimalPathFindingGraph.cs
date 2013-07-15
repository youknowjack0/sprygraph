namespace Alastri.SpryGraph
{
    public interface IMinimalPathFindingGraph<TVertex, TEdge> : IImplicitCostedGraph<TVertex, TEdge>,
                                                                ICostedHeuristicGraph<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {

    }
}
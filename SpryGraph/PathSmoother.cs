namespace Alastri.SpryGraph
{
    public class PathSmoother<TVertex, TEdge> : IPathFinder<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
        private readonly IPathFinder<TVertex, TEdge> _pathFinder;

        public PathSmoother(IPathFinder<TVertex, TEdge> pathFinder, TVertex source)
        {
            _pathFinder = pathFinder;
            Source = source;
        }

        public TVertex Source { get; private set; }

        public bool TryGetPath(TVertex destination, out TEdge[] path)
        {
            return _pathFinder.TryGetPath(destination, out path);
        }


    }
}

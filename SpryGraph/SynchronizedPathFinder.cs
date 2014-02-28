using System;

namespace Alastri.SpryGraph
{
    public sealed class SynchronizedPathFinder<TVertex, TEdge> : IPathFinder<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
        private readonly IPathFinder<TVertex, TEdge> _underlyingPathFinder;
        private readonly object _syncRoot;

        public SynchronizedPathFinder(IPathFinder<TVertex, TEdge> underlyingPathFinder, object syncRoot)
        {
            if (underlyingPathFinder == null) throw new ArgumentNullException("underlyingPathFinder");
            _underlyingPathFinder = underlyingPathFinder;
            _syncRoot = syncRoot;
        }

        public TVertex Source { get { return _underlyingPathFinder.Source; }}

        public bool TryGetPath(TVertex destination, out TEdge[] path)
        {
            lock (_syncRoot)
            {
                return _underlyingPathFinder.TryGetPath(destination, out path);
            }
        }
    }
}
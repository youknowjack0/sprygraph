using System;
using System.Collections.Generic;

namespace Alastri.SpryGraph
{
    public class PathSmoother<TVertex, TEdge> : IPathFinder<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
        private readonly IPathFinder<TVertex, TEdge> _pathFinder;
        private readonly Func<TVertex, TVertex, bool> _intersectionCheck;
        private readonly Func<TVertex, TVertex, TEdge> _edgeContructor;

        public PathSmoother(IPathFinder<TVertex, TEdge> pathFinder, TVertex source, Func<TVertex, TVertex, bool> intersectionCheck, Func<TVertex,TVertex,TEdge> edgeContructor)
        {
            _pathFinder = pathFinder;
            _intersectionCheck = intersectionCheck;
            _edgeContructor = edgeContructor;
            Source = source;
        }

        public TVertex Source { get; private set; }

        public bool TryGetPath(TVertex destination, out TEdge[] path)
        {
            TEdge[] fullpath;
            if (!_pathFinder.TryGetPath(destination, out fullpath))
            {
                path = null;
                return false;
            }

            if (fullpath.Length < 2)
            {
                path = fullpath;
                return true;
            }
            
            var v0 = fullpath[0].Source;
            var vlast = fullpath[fullpath.Length - 1].Target;

            //special case; check the full path first
            if (!_intersectionCheck(v0, vlast))
            {
                path = new TEdge[2];
                path[0] = fullpath[0];
                path[1] = fullpath[fullpath.Length - 1];
                return true;
            }

            path = Simplify(fullpath);
            return true;
        }

        private TEdge[] Simplify(TEdge[] fp)
        {
            var res = new List<TEdge>();

            var last = fp[0].Source;
            for (int i = 1; i < fp.Length; i++)
            {
                if (_intersectionCheck(last, fp[i].Target))
                {
                    res.Add(_edgeContructor(last, fp[i].Source));
                    last = fp[i].Source;
                }
            }

            res.Add(_edgeContructor(last, fp[fp.Length - 1].Target));

            return res.ToArray();
        }
    }
}

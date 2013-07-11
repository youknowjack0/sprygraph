using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Alastri.SpryGraph
{
    /// <summary>
    /// read a graph
    /// 
    /// caches the graph in a fast data structure for path finding (or other) operations
    /// </summary>    
    public sealed class GraphReader<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex> 
        where TVertex : IHeuristicVertex<TVertex>
    {
        private readonly IImplicitCostedHeuristicGraph<TVertex, TEdge> _source;

        private readonly Dictionary<TVertex, VertexInternal<TVertex, TEdge>> _vertexMap = new Dictionary<TVertex, VertexInternal<TVertex, TEdge>>();
        private readonly List<VertexInternal<TVertex, TEdge>> _vertices = new List<VertexInternal<TVertex, TEdge>>(); 

        public GraphReader(IImplicitCostedHeuristicGraph<TVertex, TEdge> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            _source = source;
        }

        public IImplicitCostedHeuristicGraph<TVertex, TEdge> Source
        {
            get { return _source; }
        }


        public int VertexCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _vertices.Count; }            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal VertexInternal<TVertex, TEdge> GetVertexInternal(TVertex v)
        {
            VertexInternal<TVertex, TEdge> vi;
            if (!_vertexMap.TryGetValue(v, out vi))
            {
                vi = new VertexInternal<TVertex, TEdge>(_vertices.Count, v);
                _vertexMap.Add(v,vi);
                _vertices.Add(vi);
            }
            return vi;
        } 

        public DijkstraPathFinder<TVertex, TEdge> GetPathFinder(TVertex from)
        {
            return new DijkstraPathFinder<TVertex, TEdge>(this, from, GetVertexInternal(from) );
        }        
    }
}

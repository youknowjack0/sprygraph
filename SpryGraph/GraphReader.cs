using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Alastri.DataStructures;

namespace Alastri.SpryGraph
{
    public sealed class GraphReader<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
    {
        private readonly IImplicitCostedGraph<TVertex, TEdge> _source;

        private readonly Dictionary<TVertex, VertexInternal<TVertex, TEdge>> _vertexMap = new Dictionary<TVertex, VertexInternal<TVertex, TEdge>>();
        private readonly List<VertexInternal<TVertex, TEdge>> _vertices = new List<VertexInternal<TVertex, TEdge>>(); 

        public GraphReader(IImplicitCostedGraph<TVertex, TEdge> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            _source = source;
        }

        public IImplicitCostedGraph<TVertex, TEdge> Source
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

        public PathFinder<TVertex, TEdge> GetPathFinder(TVertex from)
        {
            return new PathFinder<TVertex, TEdge>(this, from, GetVertexInternal(from) );
        }        
    }

    public sealed class PathFinder<TVertex, TEdge> where TEdge : ICostedEdge<TVertex>

    {
        private GraphReader<TVertex, TEdge> _graph;
        private readonly TVertex _source;
        private readonly VertexInternal<TVertex,TEdge> _sourceI;
        private MinHeap<VertexInternal<TVertex, TEdge>> _unvisited;
        private readonly List<Tuple<TEdge,int>> _precedent = new List<Tuple<TEdge, int>>();
        private readonly List<double> _costs = new List<double>();
        private readonly List<int> _heapIndex = new List<int>(); //-2 indicating unvisited and uninitialized, -1 indicating visited

        internal PathFinder(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex,TEdge> sourceI)
        {
            _graph = graph;
            _source = source;
            _sourceI = sourceI;

            ExpandInternals();
            _unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(graph.VertexCount);
            _heapIndex[_sourceI.Id] = _unvisited.Add(0, sourceI);

            _costs[_sourceI.Id] = 0;
            _precedent[_sourceI.Id] = null;            
        }

        private void ExpandInternals()
        {
            while (_costs.Count < _graph.VertexCount)
            {
                _costs.Add(double.MaxValue);
                _precedent.Add(null);
                _heapIndex.Add(-2);
            }
        }

        public TVertex Source { get { return _source; } }

        public bool TryGetPath(TVertex destination, out TEdge[] path )
        {

            var destVertex = _graph.GetVertexInternal(destination);

            if (_costs.Count < _graph.VertexCount)
            {
                ExpandInternals();
            }
                        

            //todo: check if dest == src
            
            while (_unvisited.Count > 0)
            {
                var kvp = _unvisited.RemoveMinimum();
                var v = kvp.Value;
                var cost = kvp.Key;

                if (cost >= _costs[destVertex.Id]) //terminate
                {
                    _unvisited.Add(kvp);                                     
                    break;
                }

                _heapIndex[v.Id] = -1;

                foreach (var edge in v.GetOutEdges(_graph))
                {
                    double totalCost = cost + edge.Cost;
                    int targetId = edge.Target.Id;

                    if (_costs.Count <= targetId)
                    {
                        Debug.Assert(targetId == _costs.Count);
                        _costs.Add(totalCost);
                        _precedent.Add(new Tuple<TEdge, int>(edge.UnderlyingEdge,v.Id));
                        _heapIndex.Add(_unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target)));

                    }
                    else if (totalCost < _costs[targetId])
                    {
                        _costs[targetId] = totalCost;
                        _precedent[targetId] = new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id);
                        int heapIndex = _heapIndex[targetId];
                        if (heapIndex == -2)
                            _unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target));
                        else if(heapIndex >= 0)                            
                            _unvisited.DecreaseKey(heapIndex, totalCost);
                        //else already visited
                    }                                       
                }
            }

            if (_costs[destVertex.Id] == double.MaxValue)
            {
                path = null;
                return false;
            }

            List<TEdge> ps = new List<TEdge>();

            for (Tuple<TEdge, int> precedent = _precedent[destVertex.Id]; 
                precedent != null;
                precedent = _precedent[precedent.Item2])
            {
                
                ps.Add(precedent.Item1);

            }

            TEdge[] returnEdges = new TEdge[ps.Count];
            int pct1 = ps.Count - 1;
            for (int i = 0; i < ps.Count; i++)
            {
                returnEdges[i] = ps[pct1 - i];
            }

            path = returnEdges;
            return true;
        }
    }

    internal class KeyComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    {
        private IComparer<TKey> _comparer; 
        public KeyComparer(IComparer<TKey> comparer = null )
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
        }


        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return _comparer.Compare(x.Key, y.Key);
        }
    }

    internal sealed class VertexInternal<TVertex, TEdge> where TEdge : ICostedEdge<TVertex>
    {
        private TVertex _vertex;
        private EdgeInternal<TVertex,TEdge>[] _outEdges;
        private int _id;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VertexInternal(int id, TVertex vertex)
        {
            _id = id;
            _vertex = vertex;
        }

        public int Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _id; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EdgeInternal<TVertex,TEdge>[] GetOutEdges(GraphReader<TVertex,TEdge> graph )
        {
            if (_outEdges == null)
            {
                var outEdges = graph.Source.GetOutEdges(_vertex);
                _outEdges = new EdgeInternal<TVertex, TEdge>[outEdges.Count];
                for (int i=0;i<outEdges.Count;i++)
                {
                    var edge = outEdges[i];
                    _outEdges[i] = new EdgeInternal<TVertex, TEdge>(edge.GetCost(), graph.GetVertexInternal(edge.Target), edge);
                }
            }

            return _outEdges;
        }

    }

    internal sealed class EdgeInternal<TVertex, TEdge> where TEdge : ICostedEdge<TVertex>
    {
        private readonly double _cost;        
        private readonly VertexInternal<TVertex, TEdge> _target;
        private readonly TEdge _underlyingEdge;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EdgeInternal(double cost, VertexInternal<TVertex, TEdge> target, TEdge underlyingEdge)
        {
            _cost = cost;
            _target = target;
            _underlyingEdge = underlyingEdge;
        }

        public double Cost
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _cost; }
        }

        public VertexInternal<TVertex, TEdge> Target
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _target; }
        }

        public TEdge UnderlyingEdge
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _underlyingEdge; }
        }
    }
}

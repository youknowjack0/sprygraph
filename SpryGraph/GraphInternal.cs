using System;
using System.Collections.Generic;
using System.Linq;

namespace Alastri.SpryGraph
{
    internal sealed class GraphInternal<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
    {
        private readonly IImplicitCostedGraph<TVertex, TEdge> _source;

        private readonly Dictionary<TVertex, VertexInternal<TVertex, TEdge>> _vertexMap = new Dictionary<TVertex, VertexInternal<TVertex, TEdge>>();
        private readonly List<VertexInternal<TVertex, TEdge>> _vertices = new List<VertexInternal<TVertex, TEdge>>(); 

        public GraphInternal(IImplicitCostedGraph<TVertex, TEdge> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            _source = source;
        }

        public IImplicitCostedGraph<TVertex, TEdge> Source
        {
            get { return _source; }
        }

        public VertexInternal<TVertex, TEdge> GetVertexInternal(TVertex v)
        {
            VertexInternal<TVertex, TEdge> vi;
            if (!_vertexMap.TryGetValue(v, out vi))
            {
                vi = new VertexInternal<TVertex, TEdge>(_vertices.Count);
                _vertexMap.Add(v,vi);
                _vertices.Add(vi);
            }
            return vi;
        } 

        public DijkstraPathFinder<TVertex, TEdge> Dij(TVertex from)
        {
            return new DijkstraPathFinder<TVertex, TEdge>(this, from, GetVertexInternal(from) );
        }        
    }

    public sealed class DijkstraPathFinder<TVertex, TEdge> where TEdge : ICostedEdge<TVertex>

    {
        private GraphInternal<TVertex, TEdge> _graph;
        private readonly TVertex _source;
        private readonly VertexInternal<TVertex,TEdge> _sourceI;
        private C5.IntervalHeap<KeyValuePair<double,VertexInternal<TVertex,TEdge>>> _unvisited = new C5.IntervalHeap<KeyValuePair<double, VertexInternal<TVertex, TEdge>>>(new KeyComparer<double, VertexInternal<TVertex, TEdge>>());
        private readonly List<int> _precedent = new List<int>();
        private readonly List<double> _costs = new List<double>();

        internal DijkstraPathFinder(GraphInternal<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex,TEdge> sourceI)
        {
            _graph = graph;
            _source = source;
            _sourceI = sourceI;
            _unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(0, _sourceI));
        }

        public TVertex Source { get { return _source; } }

        public IEnumerable<TEdge> FindPath(TVertex destination)
        {

            var destVertex = _graph.GetVertexInternal(destination);

            if (_costs.Count <= destVertex.Id)
            {
                _costs.Add(double.MaxValue);
                _precedent.Add(-1);
            }

            double bestDestinationCost = _costs[destVertex.Id];    
            
            while (_unvisited.Count > 0)
            {
                var kvp = _unvisited.DeleteMin();
                var v = kvp.Value;
                var cost = kvp.Key;

                if (_costs[v.Id] <= cost)
                    continue; //this heap record is invalid

                foreach (var edge in v.GetOutEdges(_graph))
                {
                    double totalCost = cost + edge.Cost;
                    int targetId = edge.Target.Id;
                    if (totalCost < _costs[edge.Target.Id])
                    {
                        _costs[targetId] = totalCost;
                        _precedent[targetId] = v.Id;
                        _unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target));
                    }
                }
            }

            
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



        public VertexInternal(int id)
        {
            _id = id;
        }

        public int Id { get { return _id; } }

        public EdgeInternal<TVertex,TEdge>[] GetOutEdges(GraphInternal<TVertex,TEdge> graph )
        {
            if (_outEdges == null)
            {
                var outEdges = graph.Source.GetOutEdges(_vertex);
                _outEdges = new EdgeInternal<TVertex, TEdge>[outEdges.Count];
                for (int i=0;i<outEdges.Count;i++)
                {
                    var edge = outEdges[i];
                    _outEdges[i] = new EdgeInternal<TVertex, TEdge>(edge.GetCost(), graph.GetVertexInternal(edge.Target));
                }
            }

            return _outEdges;
        }

    }

    internal sealed class EdgeInternal<TVertex, TEdge> where TEdge : ICostedEdge<TVertex>
    {
        private readonly double _cost;        
        private readonly VertexInternal<TVertex, TEdge> _target;

        public EdgeInternal(double cost, VertexInternal<TVertex, TEdge> target )
        {
            _cost = cost;
            _target = target;
        }

        public double Cost { get { return _cost; } }

        public VertexInternal<TVertex, TEdge> Target
        {
            get { return _target; }
        }
    }
}

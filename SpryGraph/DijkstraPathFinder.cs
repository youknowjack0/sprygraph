using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Alastri.DataStructures;

namespace Alastri.SpryGraph
{
    public sealed class DijkstraPathFinder<TVertex, TEdge> : IPathFinder<TVertex, TEdge> where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>

    {
        private struct PrecedentEntry
        {
            public TEdge Edge;
            public int VertexId;

            public PrecedentEntry(TEdge edge, int vertexId)
            {
                VertexId = vertexId;
                Edge = edge;
            }

            public PrecedentEntry(int vertexId)
            {
                VertexId = vertexId;
                Edge = default(TEdge);
            }

        }

        private GraphReader<TVertex, TEdge> _graph;
        private readonly TVertex _source;
        private readonly VertexInternal<TVertex,TEdge> _sourceI;
        private MinHeap<VertexInternal<TVertex, TEdge>> _unvisited;
        private  PrecedentEntry[] _precedent;
        private  double[] _costs ;
        private  int[] _heapIndex ; //-2 indicating unvisited and uninitialized, -1 indicating visited

        private int _vCount;

        internal DijkstraPathFinder(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex,TEdge> sourceI)
        {
            _graph = graph;
            _source = source;
            _sourceI = sourceI;

            InitializeInternals();
            _unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(graph.VertexCount);
            _heapIndex[_sourceI.Id] = _unvisited.Add(0, sourceI);

            _costs[_sourceI.Id] = 0;
            _precedent[_sourceI.Id] = new PrecedentEntry(-1);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeInternals()
        {
            
            if (_costs == null)
            {
                int size = _graph.VertexCount < 32 ? 32 : _graph.VertexCount;
                _costs = new double[size];
                _heapIndex = new int[size];
                _precedent = new PrecedentEntry[size];
            } 
            
            ExpandInternals();

            for (; _vCount < _graph.VertexCount;_vCount++ )
            {
                _costs[_vCount] = (double.MaxValue);
                _precedent[_vCount] = new PrecedentEntry(-1);
                _heapIndex[_vCount] = -2;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandInternals()
        {
            if (_graph.VertexCount >= _costs.Length)
            {
                Array.Resize(ref _costs, _costs.Length*2);
                Array.Resize(ref _heapIndex, _heapIndex.Length * 2);
                Array.Resize(ref _precedent, _precedent.Length * 2);
            }
        }

        public TVertex Source { get { return _source; } }

        public bool TryGetPath(TVertex destination, out TEdge[] path )
        {

            var destVertex = _graph.GetVertexInternal(destination);

            if (_vCount < _graph.VertexCount)
            {
                InitializeInternals();
            }                       
            
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

                    if (_vCount <= edge.Target.Id)                    
                        AddNewlyFoundVertex(totalCost, edge, v);
                    else if (totalCost < _costs[edge.Target.Id])
                        UpdateVertexCost(totalCost, edge, v);                                     
                }
            }

            if (_costs[destVertex.Id] == double.MaxValue)
            {
                path = null;
                return false;
            }

            var returnEdges = WalkPrecedenceList(destVertex);

            path = returnEdges;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TEdge[] WalkPrecedenceList(VertexInternal<TVertex, TEdge> destVertex)
        {
            List<TEdge> ps = new List<TEdge>();

            for (PrecedentEntry precedent = _precedent[destVertex.Id];
                 precedent.VertexId != -1;
                 precedent = _precedent[precedent.VertexId])
            {
                ps.Add(precedent.Edge);
            }

            TEdge[] returnEdges = new TEdge[ps.Count];
            int pct1 = ps.Count - 1;
            for (int i = 0; i < ps.Count; i++)
            {
                returnEdges[i] = ps[pct1 - i];
            }
            return returnEdges;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateVertexCost(double totalCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            _costs[edge.Target.Id] = totalCost;
            _precedent[edge.Target.Id] = new PrecedentEntry(edge.UnderlyingEdge, v.Id);
            int heapIndex = _heapIndex[edge.Target.Id];
            if (heapIndex == -2) //not in heap, not visited
                _unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target));
            else if (heapIndex >= 0) //in heap, visited
                _unvisited.DecreaseKey(heapIndex, totalCost);
            //else already visited
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddNewlyFoundVertex(double totalCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            ExpandInternals();
            _costs[_vCount] = (totalCost);
            _precedent[_vCount] = new PrecedentEntry(edge.UnderlyingEdge, v.Id);
            _heapIndex[_vCount] = (_unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target)));
            _vCount++;            
        }
    }
}
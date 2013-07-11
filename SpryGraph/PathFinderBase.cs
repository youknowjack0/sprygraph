using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Alastri.DataStructures;

namespace Alastri.SpryGraph
{
    public abstract class PathFinderBase<TVertex, TEdge> : IPathFinder<TVertex, TEdge>
        where TEdge : ICostedEdge<TVertex>
        where TVertex : IHeuristicVertex<TVertex>
    {
        public TVertex Source { get { return _source; } }
        public abstract bool TryGetPath(TVertex destination, out TEdge[] path);

        protected GraphReader<TVertex, TEdge> _graph;
        protected readonly TVertex _source;
        protected readonly VertexInternal<TVertex,TEdge> _sourceI;
        protected MinHeap<VertexInternal<TVertex, TEdge>> _unvisited;
        protected  Tuple<TEdge, int>[] _precedent;
        protected  double[] _costs ;
        protected  int[] _heapIndex ; //-2 indicating unvisited and uninitialized, -1 indicating visited
        
        protected int _vCount;

        internal PathFinderBase(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex, TEdge> sourceI)
        {
            _graph = graph;
            _source = source;
            _sourceI = sourceI;

            InitializeInternals();

            

            _costs[_sourceI.Id] = 0;
            _precedent[_sourceI.Id] = null;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InitializeInternals()
        {
            
            if (_costs == null)
            {
                int size = _graph.VertexCount < 32 ? 32 : _graph.VertexCount;
                _costs = new double[size];
                _heapIndex = new int[size];
                _precedent = new Tuple<TEdge, int>[size];
            } 
            
            ExpandInternals();

            for (; _vCount < _graph.VertexCount;_vCount++ )
            {
                _costs[_vCount] = (double.MaxValue);
                _precedent[_vCount] = null;
                _heapIndex[_vCount] = -2;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ExpandInternals()
        {
            if (_graph.VertexCount >= _costs.Length)
            {
                Array.Resize(ref _costs, _costs.Length*2);
                Array.Resize(ref _heapIndex, _heapIndex.Length * 2);
                Array.Resize(ref _precedent, _precedent.Length * 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdateVertexCost(double totalCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            _costs[edge.Target.Id] = totalCost;
            _precedent[edge.Target.Id] = new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id);
            int heapIndex = _heapIndex[edge.Target.Id];
            if (heapIndex == -2)
                _unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target));
            else if (heapIndex >= 0)
                _unvisited.DecreaseKey(heapIndex, totalCost);
            //else already visited
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AddNewlyFoundVertex(double totalCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            ExpandInternals();
            _costs[_vCount] = (totalCost);
            _precedent[_vCount] = (new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id));
            _heapIndex[_vCount] = (_unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target)));
            _vCount++;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected TEdge[] WalkPrecedenceList(VertexInternal<TVertex, TEdge> destVertex)
        {
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
            return returnEdges;
        }
    }
}
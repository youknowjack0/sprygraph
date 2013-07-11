using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Alastri.DataStructures;

namespace Alastri.SpryGraph
{
    public sealed class AStarPathFinder<TVertex, TEdge> : PathFinderBase<TVertex,TEdge>
         where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {

        protected VertexInternal<TVertex, TEdge> _lastVertex;

        internal AStarPathFinder(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex, TEdge> sourceI)
            : base(graph, source, sourceI)
        {

        }

        public override bool TryGetPath(TVertex destination, out TEdge[] path )
        {

            var destVertex = _graph.GetVertexInternal(destination);

            if (_vCount < _graph.VertexCount)
            {
                InitializeInternals();
            }

            if (_lastVertex == null) //init unvisited
            {
                _unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(_graph.VertexCount);
                _heapIndex[_sourceI.Id] = _unvisited.Add(_sourceI.Heuristic(destVertex), _sourceI);
                _lastVertex = destVertex;
            }
            else if (destVertex != _lastVertex) //update unvisited for new dest
            {             
                UpdateUnvisitedSet(destVertex);
                _lastVertex = destVertex;
            }
            
            
            while (_unvisited.Count > 0)
            {
                var kvp = _unvisited.RemoveMinimum();
                var v = kvp.Value;                

                if (v == destVertex) //terminate
                {
                    _unvisited.Add(kvp);                                     
                    break;
                }

                _heapIndex[v.Id] = -1;

                var cost = _costs[v.Id]; //it MAY be more efficient to embed the path cost in the heap item

                foreach (var edge in v.GetOutEdges(_graph))
                {
                    double totalCost = cost + edge.Cost;
                    double heuristicPlus = totalCost + edge.Target.Heuristic(destVertex);

                    if (_vCount <= edge.Target.Id)                    
                        AddNewlyFoundVertex(totalCost, heuristicPlus, edge, v);
                    else if (totalCost < _costs[edge.Target.Id])
                        UpdateVertexCost(totalCost, heuristicPlus, edge, v);                                     
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
        private void UpdateUnvisitedSet(VertexInternal<TVertex,TEdge> destination)
        {
            var oldheap = _unvisited;
            _unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(_graph.VertexCount);
            foreach(VertexInternal<TVertex, TEdge> v in oldheap)
            {
                _heapIndex[v.Id] = _unvisited.Add(_costs[v.Id] + v.Heuristic(destination), v);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new void UpdateVertexCost(double totalCost, double heuristicCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            _costs[edge.Target.Id] = totalCost;
            _precedent[edge.Target.Id] = new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id);
            int heapIndex = _heapIndex[edge.Target.Id];
            if (heapIndex == -2)
                _unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(heuristicCost, edge.Target));
            else if (heapIndex >= 0)               
                _unvisited.DecreaseKey(heapIndex, heuristicCost);
            //else already visited
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new void AddNewlyFoundVertex(double totalCost, double heuristicCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            ExpandInternals();
            _costs[_vCount] = (totalCost);
            _precedent[_vCount] = (new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id));
            _heapIndex[_vCount] = (_unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(heuristicCost, edge.Target)));
            _vCount++;
        }

    }

}
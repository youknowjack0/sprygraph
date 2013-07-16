using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Alastri.DataStructures;

namespace Alastri.SpryGraph
{
    public sealed class AStarPathFinder<TVertex, TEdge> : PathFinderBase<TVertex,TEdge>
         where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {
        private VertexInternal<TVertex, TEdge> _lastVertex;

        internal AStarPathFinder(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex, TEdge> sourceI)
            : base(graph, source, sourceI)
        {

        }

        public override bool TryGetPath(TVertex destination, out TEdge[] path )
        {

            var destVertex = Graph.GetVertexInternal(destination);

            if (VCount < Graph.VertexCount)
            {
                InitializeInternals();
            }

            if (_lastVertex == null) //init unvisited
            {
                Unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(Graph.VertexCount);
                HeapIndex[SourceI.Id] = Unvisited.Add(SourceI.Heuristic(destVertex), SourceI);
                _lastVertex = destVertex;
            }
            else if (destVertex != _lastVertex) //update unvisited for new dest
            {             
                UpdateUnvisitedSet(destVertex);
                _lastVertex = destVertex;
            }
            
            
            while (Unvisited.Count > 0)
            {
                var kvp = Unvisited.RemoveMinimum();
                var v = kvp.Value;

                var cost = Costs[v.Id]; //it MAY be more efficient to embed the path cost in the heap item

                if (cost >= Costs[destVertex.Id]) //terminate
                {
                    Unvisited.Add(kvp);
                    break;
                }

                HeapIndex[v.Id] = -1;

                

                foreach (var edge in v.GetOutEdges(Graph))
                {
                    double totalCost = cost + edge.Cost;
                    double heuristicPlus = totalCost + edge.Target.Heuristic(destVertex);

                    if (VCount <= edge.Target.Id)                    
                        AddNewlyFoundVertex(totalCost, heuristicPlus, edge, v);
                    else if (totalCost < Costs[edge.Target.Id])
                        UpdateVertexCost(totalCost, heuristicPlus, edge, v);                                     
                }
            }

            if (Costs[destVertex.Id] == double.MaxValue)
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
            var oldheap = Unvisited;
            Unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(Graph.VertexCount);
            foreach(VertexInternal<TVertex, TEdge> v in oldheap)
            {
                HeapIndex[v.Id] = Unvisited.Add(Costs[v.Id] + v.Heuristic(destination), v);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
         void UpdateVertexCost(double totalCost, double heuristicCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            Costs[edge.Target.Id] = totalCost;
            Precedent[edge.Target.Id] = new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id);
            int heapIndex = HeapIndex[edge.Target.Id];
            if (heapIndex == -2)
                Unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(heuristicCost, edge.Target));
            else if (heapIndex >= 0)               
                Unvisited.DecreaseKey(heapIndex, heuristicCost);
            //else already visited
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
         void AddNewlyFoundVertex(double totalCost, double heuristicCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            ExpandInternals();
            Costs[VCount] = (totalCost);
            Precedent[VCount] = (new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id));
            HeapIndex[VCount] = (Unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(heuristicCost, edge.Target)));
            VCount++;
        }

    }

}
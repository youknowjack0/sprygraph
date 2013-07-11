using System.Diagnostics;
using Alastri.DataStructures;

namespace Alastri.SpryGraph
{


    public sealed class DijkstraPathFinder<TVertex, TEdge> : PathFinderBase<TVertex,TEdge>
         where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {
        internal DijkstraPathFinder(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex, TEdge> sourceI) : base(graph, source, sourceI)
        {
            _unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(graph.VertexCount);
            _heapIndex[_sourceI.Id] = _unvisited.Add(0, sourceI);
        }

        public override sealed bool TryGetPath(TVertex destination, out TEdge[] path )
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

                if (v == destVertex) //terminate
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



    }
}
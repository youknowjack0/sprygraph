using Alastri.DataStructures;

namespace Alastri.SpryGraph
{


    public sealed class DijkstraPathFinder<TVertex, TEdge> : PathFinderBase<TVertex,TEdge>
         where TEdge : ICostedEdge<TVertex> where TVertex : IHeuristicVertex<TVertex>
    {
        internal DijkstraPathFinder(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex, TEdge> sourceI) : base(graph, source, sourceI)
        {
            Unvisited = new MinHeap<VertexInternal<TVertex, TEdge>>(graph.VertexCount);
            HeapIndex[SourceI.Id] = Unvisited.Add(0, sourceI);
        }

        public override bool TryGetPath(TVertex destination, out TEdge[] path )
        {

            var destVertex = Graph.GetVertexInternal(destination);            

            if (VCount < Graph.VertexCount)
            {
                InitializeInternals();
            }                       
            
            while (Unvisited.Count > 0)
            {
                var kvp = Unvisited.RemoveMinimum();
                var v = kvp.Value;
                var cost = kvp.Key;

                if (v == destVertex) //terminate
                {
                    Unvisited.Add(kvp);                                     
                    break;
                }

                HeapIndex[v.Id] = -1;

                foreach (var edge in v.GetOutEdges(Graph))
                {
                    double totalCost = cost + edge.Cost;

                    if (VCount <= edge.Target.Id)                    
                        AddNewlyFoundVertex(totalCost, edge, v);
                    else if (totalCost < Costs[edge.Target.Id])
                        UpdateVertexCost(totalCost, edge, v);                                     
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



    }
}
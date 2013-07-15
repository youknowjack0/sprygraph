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
        public TVertex Source { get { return SourceVertex; } }
        public abstract bool TryGetPath(TVertex destination, out TEdge[] path);

        protected GraphReader<TVertex, TEdge> Graph;
        protected readonly TVertex SourceVertex;
        protected readonly VertexInternal<TVertex,TEdge> SourceI;
        protected MinHeap<VertexInternal<TVertex, TEdge>> Unvisited;
        protected  Tuple<TEdge, int>[] Precedent;
        protected  double[] Costs ;
        protected  int[] HeapIndex ; //-2 indicating unvisited and uninitialized, -1 indicating visited
        
        protected int VCount;

        internal PathFinderBase(GraphReader<TVertex, TEdge> graph, TVertex source, VertexInternal<TVertex, TEdge> sourceI)
        {
            Graph = graph;
            SourceVertex = source;
            SourceI = sourceI;

            InitializeInternals();

            

            Costs[SourceI.Id] = 0;
            Precedent[SourceI.Id] = null;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InitializeInternals()
        {
            
            if (Costs == null)
            {
                int size = Graph.VertexCount < 32 ? 32 : Graph.VertexCount;
                Costs = new double[size];
                HeapIndex = new int[size];
                Precedent = new Tuple<TEdge, int>[size];
            } 
            
            ExpandInternals();

            for (; VCount < Graph.VertexCount;VCount++ )
            {
                Costs[VCount] = (double.MaxValue);
                Precedent[VCount] = null;
                HeapIndex[VCount] = -2;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ExpandInternals()
        {
            if (Graph.VertexCount >= Costs.Length)
            {
                Array.Resize(ref Costs, Costs.Length*2);
                Array.Resize(ref HeapIndex, HeapIndex.Length * 2);
                Array.Resize(ref Precedent, Precedent.Length * 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdateVertexCost(double totalCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            Costs[edge.Target.Id] = totalCost;
            Precedent[edge.Target.Id] = new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id);
            int heapIndex = HeapIndex[edge.Target.Id];
            if (heapIndex == -2)
                Unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target));
            else if (heapIndex >= 0)
                Unvisited.DecreaseKey(heapIndex, totalCost);
            //else already visited
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AddNewlyFoundVertex(double totalCost, EdgeInternal<TVertex, TEdge> edge, VertexInternal<TVertex, TEdge> v)
        {
            ExpandInternals();
            Costs[VCount] = (totalCost);
            Precedent[VCount] = (new Tuple<TEdge, int>(edge.UnderlyingEdge, v.Id));
            HeapIndex[VCount] = (Unvisited.Add(new KeyValuePair<double, VertexInternal<TVertex, TEdge>>(totalCost, edge.Target)));
            VCount++;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected TEdge[] WalkPrecedenceList(VertexInternal<TVertex, TEdge> destVertex)
        {
            List<TEdge> ps = new List<TEdge>();

            for (Tuple<TEdge, int> precedent = Precedent[destVertex.Id];
                 precedent != null;
                 precedent = Precedent[precedent.Item2])
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
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Alastri.SpryGraph;
using NUnit.Framework;
using QuickGraph;
using QuickGraph.Algorithms;


namespace UnitTestProject1
{
    /// <summary>
    /// Test against known-good implementation (QuickGraph)
    /// </summary>
    [TestFixture]
    public class QuickGraphComparisons
    {
        private double _sglen;
        private double _qglen;

        [Test]
        public void Dijkstra()
        {
            var rg = GenerateRandomGraph(500, 3);


            GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);

            Random r = new Random();
            foreach (var v in rg.VerticesList)
            {
                DijkstraPathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetDijkstraPathFinder(v);
                TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsDijkstra(x => x.GetCost(), v);
                foreach (var vt in rg.VerticesList)
                {
                    IEnumerable<TestEdge> qgresult;
                    bool qggot = qgsolver(vt, out qgresult);
                    TestEdge[] sgresult;
                    bool sggot = sgsolver.TryGetPath(vt, out sgresult);

                    if (v == vt) //quickgraph???
                    {
                        //Assert.True(qggot == false && sggot == true);
                        Assert.True(sggot && sgresult.Length == 0);
                        continue;
                    }

                    Assert.True(qggot == sggot);
                    if (qggot)
                    {
                        int i = 0;
                        foreach (var item in qgresult)
                        {
                            Assert.True(item == sgresult[i]);
                            i++;
                        }
                        Assert.True(sgresult.Length == i);
                    }
                }
            }
        }

        [Test]
        public void AStar()
        {
            var rg = GenerateRandomGraph(500, 4);


            GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);

            Random r = new Random();
            foreach (var v in rg.VerticesList)
            {
                AStarPathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetAStarPathFinder(v);
                TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsAStar( x => x.GetCost(), x => x.Heuristic(x), v);
                foreach (var vt in rg.VerticesList)
                {
                    IEnumerable<TestEdge> qgresult;
                    bool qggot = qgsolver(vt, out qgresult);
                    TestEdge[] sgresult;
                    bool sggot = sgsolver.TryGetPath(vt, out sgresult);

                    if (v == vt) //quickgraph???
                    {
                        //Assert.True(qggot == false && sggot == true);
                        Assert.True(sggot && sgresult.Length == 0);
                        continue;
                    }

                    TestEdge[] qgresultarray = null;
                    if (qggot && sggot)
                    {
                         qgresultarray = qgresult as TestEdge[] ?? qgresult.ToArray();
                        _qglen = qgresultarray.Aggregate(0.0, (sum, edge) => sum + edge.GetCost());
                        _sglen = sgresult.Aggregate(0.0, (sum, edge) => sum + edge.GetCost());

                    }
                    Assert.True(qggot == sggot);
                    if (qggot)
                    {
                        int i = 0;
                        foreach (var item in qgresultarray)
                        {
                            Assert.True(item == sgresult[i]);
                            i++;
                        }
                        Assert.True(sgresult.Length == i);
                    }
                }
            }
        }

        [Test]
        public void AStarSpatial()
        {
            var rg = GenerateRandomGraph2(15);


            GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);

            Random r = new Random();
            foreach (var v in rg.VerticesList)
            {
                AStarPathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetAStarPathFinder(v);
                TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsAStar(x => x.GetCost(), x => x.Heuristic(x), v);
                foreach (var vt in rg.VerticesList)
                {
                    IEnumerable<TestEdge> qgresult;
                    bool qggot = qgsolver(vt, out qgresult);
                    TestEdge[] sgresult;
                    bool sggot = sgsolver.TryGetPath(vt, out sgresult);

                    if (v == vt) //quickgraph???
                    {
                        //Assert.True(qggot == false && sggot == true);
                        Assert.True(sggot && sgresult.Length == 0);
                        continue;
                    }

                    TestEdge[] qgresultarray = null;
                    if (qggot && sggot)
                    {
                        qgresultarray = qgresult as TestEdge[] ?? qgresult.ToArray();
                        _qglen = qgresultarray.Aggregate(0.0, (sum, edge) => sum + edge.GetCost());
                        _sglen = sgresult.Aggregate(0.0, (sum, edge) => sum + edge.GetCost());

                    }
                    Assert.True(qggot == sggot);
                    if (qggot)
                    {
                        int i = 0;
                        foreach (var item in qgresultarray)
                        {
                            Assert.True(item == sgresult[i]);
                            i++;
                        }
                        Assert.True(sgresult.Length == i);
                    }
                }
            }
        }

        public  static IHybridGraph GenerateRandomGraph(int vertices, int degree)
        {

            var vl = new List<TestVertex>();
            var el = new List<TestEdge>();

            Random r = new Random();

            for (int i = 0; i < vertices; i++)
            {
                var vertex = new TestVertex(r.NextDouble(), r.NextDouble(), r.NextDouble());
                vl.Add(vertex);
            }

            

            foreach (var item in vl)
            {
                var @out = item.Out;
                for (int i = 0; i < degree; i++)
                {
                    int n = r.Next(0, vertices);

                    @out.Add(vl[n]);
                    
                }
                var x = item.OutEdges; //called for effect
                foreach (var e in x)
                {
                    el.Add(e);
                }
            }

            return new TestGraph(vl, el);
        }
    
        /// <summary>
        /// generate a more regular spatial graph, 2d
        /// </summary>        
        public  static IHybridGraph GenerateRandomGraph2(int sideCount)
        {
            directions = new List<int[]>();
            directions.Add(new int[]{0,1});
            directions.Add(new int[] { 0, -1 });
            directions.Add(new int[] { 1, 0 });
            directions.Add(new int[] { -1, 0 });
            directions.Add(new int[] { 1, 1 });
            directions.Add(new int[] { -1, 1 });
            directions.Add(new int[] { 1, -1 });
            directions.Add(new int[] { -1, -1 });
            var vl = new List<TestVertex>();
            var el = new List<TestEdge>();

            TestVertex[,] matrix = new TestVertex[sideCount,sideCount];

            Random r = new Random();


            for (int i = 0; i < sideCount; i++)
            {                
                for (int j = 0; j < sideCount; j++)
                {
                    var vertex = new TestVertex(i + (r.NextDouble() - 0.5) / 2, j + (r.NextDouble() - 0.5) / 2, 0); //jitter to make multiple valid paths unlikely (easier for testing)
                    matrix[i,j] = vertex;
                    vl.Add(vertex);
                }
            }



            for (int i = 1; i < sideCount - 1;i++)
            {
                for (int j = 1; j < sideCount - 1; j++)
                {
                    var v = matrix[i,j];
                    var @out = v.Out;
                    ConnectRandomly(v, matrix, i, j, r, @out);
                    var x = v.OutEdges; //called for effect
                    foreach (var e in x)
                    {
                        el.Add(e);
                    }
                }
                
            }

            return new TestGraph(vl, el);
        }

        private static List<int[]> directions; 

        private static void ConnectRandomly(TestVertex testVertex, TestVertex[,] matrix, int i, int j, Random r, List<TestVertex> outEdges)
        {
            foreach (var item in directions)
            {
                if (r.NextDouble() > 0.5)
                {
                    var v = matrix[i + item[0], j + item[1]];
                    outEdges.Add(v);
                }
            }
        }
    }

    public interface IHybridGraph : IVertexAndEdgeListGraph<TestVertex, TestEdge>, IImplicitCostedHeuristicGraph<TestVertex, TestEdge>
    {
        IList<TestVertex> VerticesList { get; }
    }
}

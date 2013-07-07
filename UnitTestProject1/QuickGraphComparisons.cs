using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        [Test]
        public void TestMethod1()
        {
            var rg = GenerateRandomGraph(500, 3);

            
            GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);
            
            Random r = new Random();
            foreach (var v in rg.VerticesList)
            {
                PathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetPathFinder(v);
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

        public  static IHybridGraph GenerateRandomGraph(int vertices, int degree)
        {

            var vl = new List<TestVertex>();
            var el = new List<TestEdge>();

            for (int i = 0; i < vertices; i++)
            {
                var vertex = new TestVertex(i);
                vl.Add(vertex);
            }

            Random r = new Random();

            foreach (var item in vl)
            {
                var @out = item.Out;
                for (int i = 0; i < degree; i++)
                {
                    int n = r.Next(0, vertices);

                    @out.Add(vl[n]);
                }
                var x = item.OutEdges; //called for effect
            }

            return new TestGraph(vl, el);
        }
    }

    public interface IHybridGraph : IVertexAndEdgeListGraph<TestVertex, TestEdge>, IImplicitCostedGraph<TestVertex, TestEdge>
    {
        IList<TestVertex> VerticesList { get; }
    }
}

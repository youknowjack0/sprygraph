using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alastri.SpryGraph;
using QuickGraph;
using QuickGraph.Algorithms;
using UnitTestProject1;

namespace UnitTest.Performance
{
    class Program
    {
        static void Main(string[] args)
        {            
            

            BasicDijkstra();
            
            int coldcalls = 10;

            Coldcalls(coldcalls);

            Spatial(coldcalls);
        }

        private static void BasicDijkstra()
        {

            var sw = new Stopwatch();
            var rg = QuickGraphComparisons.GenerateRandomGraph(2000, 2);
            Random r = new Random();
            sw.Restart();
            {
                GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);
                foreach (var source in rg.VerticesList)
                {
                    DijkstraPathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetDijkstraPathFinder(source);

                    foreach (var v in rg.VerticesList)
                    {
                        TestEdge[] sgresult;
                        bool sggot = sgsolver.TryGetPath(v, out sgresult);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("Sprygraph took " + sw.ElapsedMilliseconds);
            sw.Restart();
            {
                foreach (var source in rg.VerticesList)
                {
                    TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsDijkstra(x => x.GetCost(),
                                                                                                   source);

                    foreach (var v in rg.VerticesList)
                    {
                        IEnumerable<TestEdge> qgresult;
                        bool qggot = qgsolver(v, out qgresult);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("Quickgraph took " + sw.ElapsedMilliseconds);
        }

        private static int Coldcalls(int coldcalls)
        {
            var sw = new Stopwatch();
            var rg = QuickGraphComparisons.GenerateRandomGraph(200000, 2);
            Random r = new Random();

            List<TestVertex> randomSources = new List<TestVertex>(coldcalls);
            List<TestVertex> randomDestinations = new List<TestVertex>(coldcalls);
            for (int i = 0; i < coldcalls; i++)
            {
                randomSources.Add(rg.VerticesList[r.Next(0, rg.VertexCount - 1)]);
                randomDestinations.Add(rg.VerticesList[r.Next(0, rg.VertexCount - 1)]);
            }

            sw.Restart();
            {
                GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);
                for (int index = 0; index < randomSources.Count; index++)
                {
                    var source = randomSources[index];
                    var destination = randomDestinations[index];
                    DijkstraPathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetDijkstraPathFinder(source);
                    TestEdge[] sgresult;
                    bool sggot = sgsolver.TryGetPath(destination, out sgresult);
                }
            }
            sw.Stop();
            Console.WriteLine("Sprygraph cold-query Dijkstra time: " + (double) sw.ElapsedMilliseconds/(coldcalls));

            sw.Restart();
            {
                for (int index = 0; index < randomSources.Count; index++)
                {
                    var source = randomSources[index];
                    var destination = randomDestinations[index];
                    TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsDijkstra(x => x.GetCost(),
                                                                                                   source);
                    IEnumerable<TestEdge> qgresult;
                    bool qggot = qgsolver(destination, out qgresult);
                }
            }
            sw.Stop();
            Console.WriteLine("Quickgraph cold-query Dijkstra time: " + (double) sw.ElapsedMilliseconds/(coldcalls));
            return coldcalls;
        }

        private static void Spatial(int coldcalls)
        {
            var sw = new Stopwatch();
            var rg = QuickGraphComparisons.GenerateRandomGraph2(1000);
            Random r = new Random();            

            List<TestVertex> randomSources = new List<TestVertex>(coldcalls);
            List<TestVertex> randomDestinations = new List<TestVertex>(coldcalls);
            for (int i = 0; i < coldcalls; i++)
            {
                randomSources.Add(rg.VerticesList[r.Next(0, rg.VertexCount - 1)]);
                randomDestinations.Add(rg.VerticesList[r.Next(0, rg.VertexCount - 1)]);
            }

            sw.Restart();
            {
                var sgreader = new GraphReader<TestVertex, TestEdge>(rg);
                for (int index = 0; index < randomSources.Count; index++)
                {
                    var source = randomSources[index];
                    var destination = randomDestinations[index];
                    AStarPathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetAStarPathFinder(source);
                    TestEdge[] sgresult;
                    bool sggot = sgsolver.TryGetPath(destination, out sgresult);
                }
            }
            sw.Stop();
            Console.WriteLine("Sprygraph cold-query A* time: " + (double) sw.ElapsedMilliseconds/(coldcalls));

            sw.Restart();
            {
                for (int index = 0; index < randomSources.Count; index++)
                {
                    var source = randomSources[index];
                    var destination = randomDestinations[index];
                    TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsAStar(x => x.GetCost(),
                                                                                                 x => x.Heuristic(destination),
                                                                                                 source);
                    IEnumerable<TestEdge> qgresult;
                    bool qggot = qgsolver(destination, out qgresult);
                }
            }
            sw.Stop();
            Console.WriteLine("Quickgraph cold-query A* time: " + (double) sw.ElapsedMilliseconds/(coldcalls));
        }
    }
}

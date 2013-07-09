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

            var rg = QuickGraphComparisons.GenerateRandomGraph(3000000, 2);
            Random r = new Random();
            
            var sw = new Stopwatch();
            /*
            sw.Restart();
            {
                GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);
                foreach (var source in rg.VerticesList)
                {
                    
                    PathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetPathFinder(source);

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
            */
            

            int coldcalls = 1; //squared

            List<TestVertex> randomTargets = new List<TestVertex>(coldcalls);
            List<TestVertex> randomSources = new List<TestVertex>(coldcalls);
            for (int i = 0; i < coldcalls; i++)
            {
                randomTargets.Add(rg.VerticesList[r.Next(0, rg.VertexCount - 1)]);
                randomSources.Add(rg.VerticesList[r.Next(0, rg.VertexCount - 1)]);
            }

            sw.Restart();
            {
                GraphReader<TestVertex, TestEdge> sgreader = new GraphReader<TestVertex, TestEdge>(rg);
                foreach (var source in randomSources)
                {
                    foreach (var target in randomTargets)
                    {
                        PathFinder<TestVertex, TestEdge> sgsolver = sgreader.GetPathFinder(source);
                        TestEdge[] sgresult;
                        bool sggot = sgsolver.TryGetPath(target, out sgresult);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("Sprygraph cold-query time: " + (double)sw.ElapsedMilliseconds/(coldcalls*coldcalls));

            sw.Restart();
            {                
                foreach (var source in randomSources)
                {
                    foreach (var target in randomTargets)
                    {
                        TryFunc<TestVertex, IEnumerable<TestEdge>> qgsolver = rg.ShortestPathsDijkstra(x => x.GetCost(),
                                                                                                   source);
                        IEnumerable<TestEdge> qgresult;
                        bool qggot = qgsolver(target, out qgresult);

                    }
                }
            }
            sw.Stop();
            Console.WriteLine("QuickGraph cold-query time: " + (double)sw.ElapsedMilliseconds / (coldcalls * coldcalls));

        }
    }
}

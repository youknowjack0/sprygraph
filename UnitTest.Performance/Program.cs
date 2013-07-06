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

            var rg = QuickGraphComparisons.GenerateRandomGraph(4000, 3);
            Random r = new Random();
            //test quickgraph
            var sw = new Stopwatch();
            sw.Restart();
            {
                //test sprygraph
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
            
        }
    }
}

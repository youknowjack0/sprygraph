using Alastri.SpryGraph;
using NUnit.Framework;


namespace UnitTestProject1
{
    /// <summary>
    /// Test against known-good implementation (QuickGraph)
    /// </summary>
    [TestFixture]
    public class SimpleTests
    {
        [Test]
        public void TestMethod1()
        {
            IMinimalPathFindingGraph<TestVertex, TestEdge> graph = new TestGraph(null,null);

            var gr = new GraphReader<TestVertex, TestEdge>(graph);

            var v1 = new TestVertex(0,0,0);
            var v2 = new TestVertex(1,1,1);
            var v3 = new TestVertex(2,2,2);
            var v5 = new TestVertex(2,2,3);
            var v4 = new TestVertex(3,3,3);

            v1.Out.Add(v2);
            v2.Out.Add(v3);
            v3.Out.Add(v4);
            v3.Out.Add(v5);
            v5.Out.Add(v4);

            DijkstraPathFinder<TestVertex, TestEdge> pathfinder = gr.GetDijkstraPathFinder(v1);
            

            for (int i = 0; i < 2; i++)
            {
                TestEdge[] path;
                Assert.True(pathfinder.TryGetPath(v4, out path));

                Assert.True(path[0].Target == v2);
                Assert.True(path[1].Target == v3);
                Assert.True(path[2].Target == v4 || path[2].Target == v5);
                Assert.True(path.Length == 3 || path.Length == 4);

                DijkstraPathFinder<TestVertex, TestEdge> pathfinder2 = gr.GetDijkstraPathFinder(v2);

                Assert.True(pathfinder2.TryGetPath(v4, out path));

                Assert.True(path[0].Target==v3);
                Assert.True(path[1].Target == v4 || path[1].Target == v5);
                Assert.True(path.Length == 2 || path.Length == 3);
            }

            
        }

        [Test]
        public void TestMethod2AStar()
        {
            IMinimalPathFindingGraph<TestVertex, TestEdge> graph = new TestGraph(null, null);

            var gr = new GraphReader<TestVertex, TestEdge>(graph);

            var v1 = new TestVertex(0, 0, 0);
            var v2 = new TestVertex(1, 1, 1);
            var v3 = new TestVertex(2, 2, 2);
            var v5 = new TestVertex(2, 2, 3);
            var v4 = new TestVertex(3, 3, 3);

            v1.Out.Add(v2);
            v2.Out.Add(v3);
            v3.Out.Add(v4);
            v3.Out.Add(v5);
            v5.Out.Add(v4);

            AStarPathFinder<TestVertex, TestEdge> pathfinder = gr.GetAStarPathFinder(v1);


            for (int i = 0; i < 2; i++)
            {
                TestEdge[] path;
                Assert.True(pathfinder.TryGetPath(v4, out path));

                Assert.True(path[0].Target == v2);
                Assert.True(path[1].Target == v3);
                Assert.True(path[2].Target == v4 || path[2].Target == v5);
                Assert.True(path.Length == 3 || path.Length == 4);

                AStarPathFinder<TestVertex, TestEdge> pathfinder2 = gr.GetAStarPathFinder(v2);

                Assert.True(pathfinder2.TryGetPath(v4, out path));

                Assert.True(path[0].Target == v3);
                Assert.True(path[1].Target == v4 || path[1].Target == v5);
                Assert.True(path.Length == 2 || path.Length == 3);
            }


        }    

        [Test]
        public void SourceIsDest()
        {
            IMinimalPathFindingGraph<TestVertex, TestEdge> graph = new TestGraph(null, null);

            var gr = new GraphReader<TestVertex, TestEdge>(graph);

            var v1 = new TestVertex(0, 0, 0);
            var v2 = new TestVertex(1, 1, 1);
            var v3 = new TestVertex(2, 2, 2);
            var v5 = new TestVertex(2, 2, 3);
            var v4 = new TestVertex(3, 3, 3);

            v1.Out.Add(v2);
            v2.Out.Add(v3);
            v3.Out.Add(v4);
            v3.Out.Add(v5);
            v5.Out.Add(v4);

            
            

            for (int i = 0; i < 2; i++)
            {
                TestEdge[] path;
                DijkstraPathFinder<TestVertex, TestEdge> pathfinder = gr.GetDijkstraPathFinder(v1);
                Assert.True(pathfinder.TryGetPath(v1, out path));
                Assert.True(path.Length == 0);

                pathfinder = gr.GetDijkstraPathFinder(v2);
                Assert.True(pathfinder.TryGetPath(v2, out path));
                Assert.True(path.Length == 0);

                pathfinder = gr.GetDijkstraPathFinder(v3);
                pathfinder.TryGetPath(v4, out path);

                Assert.True(pathfinder.TryGetPath(v3, out path));
                Assert.True(path.Length == 0);
            }

            
        }
    }
}

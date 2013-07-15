using System;
using System.Collections.Generic;
using Alastri.DataStructures;
using C5;
using NUnit.Framework;

namespace UnitTestProject1
{
    [TestFixture]
    public class HeapTests
    {
        [Test]
        public void Random()
        {
            int x = 300;
            List<double> ints = new List<double>();
            Random r = new Random();
            for (int i = 0; i < x; i++)
            {
                ints.Add(r.NextDouble());
            }

            IntervalHeap<double> goodHeap = new IntervalHeap<double>(); 
            MinHeap<bool> myHeap = new MinHeap<bool>();
            
            foreach (var item in ints)
            {
                myHeap.Add(item, true);
                goodHeap.Add(item);
                Assert.True(myHeap.Minimum().Key == goodHeap.FindMin());
            }

            for (int i = 0; i < x; i++)
            {
                double a = myHeap.RemoveMinimum().Key;
                double b = goodHeap.DeleteMin();
                Assert.True(a == b);
            }

            Assert.True(myHeap.Count == goodHeap.Count);
        }
    }
}

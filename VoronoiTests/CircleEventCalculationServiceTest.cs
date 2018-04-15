﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoronoiEngine.Elements;
using VoronoiEngine.Geomerty;

namespace VoronoiTests
{
    [TestFixture]
    public class CircleEventCalculationServiceTest
    {
        [Test]
        public void TestDetermineCircleEvent()
        {
            var node1 = new Leaf(new Point { X = 130, Y = 160 });
            var node2 = new Leaf(new Point { X = 110, Y = 150 });
            var node3 = new Leaf(new Point { X = 170, Y = 140 });
            var arcs = new INode[] { node1, node2, node3 };

            var expectedVertex = new Point { X = 136, Y = 121 };
            var expectedCricleEvent = new Point { X = 136, Y = 83 };

            var service = new CircleEventCalculationService();
            var result = service.DetermineCircleEvent(arcs);

            Assert.AreEqual(expectedCricleEvent, result.Point);
            Assert.AreEqual(expectedVertex, result.Vertex);
        }

        //Found circle event for arcs: Leaf: Point: X: 11, Y: 19, CircleEvent: Point: X: -2147483648, Y: -766958430, Leaf: Point: X: 2, Y: 5, CircleEvent: , Leaf: Point: X: 11, Y: 19, CircleEvent:  at Point: Point: X: -2147483648, Y: -766958430 and Vertex: Point: X: -2147483648, Y: 1380525218
        [Test]
        public void TestDetermineCircleNonUnique()
        {
            var node1 = new Leaf(new Point { X = 11, Y = 19 });
            var node2 = new Leaf(new Point { X = 2, Y = 5 });
            var node3 = new Leaf(new Point { X = 11, Y = 19 });
            var arcs = new INode[] { node1, node2, node3 };

            //var expectedVertex = new Point { X = -2147483648, Y = 1380525218 };
            //var expectedCricleEvent = new Point { X = -2147483648, Y = -766958430 };

            var service = new CircleEventCalculationService();
            var result = service.DetermineCircleEvent(arcs);

            Assert.IsNull(result);
        }
    }
}

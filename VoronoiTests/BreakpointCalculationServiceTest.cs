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
    public class BreakpointCalculationServiceTest
    {
        [Test]
        public void TestCalculateBreakpoint()
        {
            var breakpointCalculationService = new BreakpointCalculationService();
            var result = breakpointCalculationService.CalculateBreakpoint(new Point(4, 6), new Point(6, 4), 4);

            Assert.IsNotNull(result);
            Assert.AreEqual(new Point(6, 4), result);
        }

        [Test]
        public void TestCalculateBreakpointLeftRight()
        {
            var breakpointCalculationService = new BreakpointCalculationService();
            var result = breakpointCalculationService.CalculateBreakpoint(new Point(170, 140), new Point(130, 160), 120);

            Assert.IsNotNull(result);
        }

        [Test]
        public void TestCalculateBreakpointRightLeft()
        {
            var breakpointCalculationService = new BreakpointCalculationService();
            var result = breakpointCalculationService.CalculateBreakpoint(new Point(130, 160), new Point(170, 140), 120);

            Assert.IsNotNull(result);
        }
        
        [Test]
        public void TestCalculateBreakpointStep3FirstNode()
        {
            var breakpointCalculationService = new BreakpointCalculationService();
            var result = breakpointCalculationService.CalculateBreakpoint(new Point(130, 160), new Point(110, 150), 140);

            Assert.IsNotNull(result);
        }
        
        [Test]
        public void TestCalculateBreakpointStep3SecondNode()
        {
            var breakpointCalculationService = new BreakpointCalculationService();
            var result = breakpointCalculationService.CalculateBreakpoint(new Point(110, 150), new Point(130, 160), 140);

            Assert.IsNotNull(result);
        }
        
        [Test]
        public void TestCalculateBreakpointStep4()
        {
            var breakpointCalculationService = new BreakpointCalculationService();
            var result = breakpointCalculationService.CalculateBreakpoint(new Point(110, 150), new Point(170, 140), 120);

            Assert.IsNotNull(result);
        }
    }
}

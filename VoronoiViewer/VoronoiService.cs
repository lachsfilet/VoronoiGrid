﻿using System.Collections.Generic;
using VoronoiEngine;
using VoronoiEngine.Elements;
using VoronoiEngine.Structures;
using VoronoiEngine.Utilities;

namespace VoronoiViewer
{
    public class VoronoiService
    {
        private ISiteGenerator _siteGenerator;
        private VoronoiFactory _voronoiFactory;

        public VoronoiService()
        {
            _siteGenerator = new SiteGenerator();
            _voronoiFactory = new VoronoiFactory();
        }

        public ICollection<Site> GenerateSites(int width, int height, int count)
        {
            var sites = _siteGenerator.GenerateSites(width, height, count);
            return sites;
        }

        public VoronoiMap CreateDiagram(ICollection<Site> sites)
        {
            var diagram = _voronoiFactory.CreateVoronoiMap(sites);
            return diagram;
        }
    }
}
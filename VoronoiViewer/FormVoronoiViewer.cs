﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VoronoiEngine.Elements;
using VoronoiEngine.Utilities;

namespace VoronoiViewer
{
    public partial class FormVoronoiViewer : Form
    {
        private VoronoiService _voronoiService;
        private Session _session;
        private Bitmap _canvas;

        private int _factorX;
        private int _factorY;

        public FormVoronoiViewer()
        {
            InitializeComponent();

            _voronoiService = new VoronoiService();
            _session = new Session();
            _canvas = new Bitmap(
                 tabPageDiagram.ClientRectangle.Width,
                 tabPageDiagram.ClientRectangle.Height,
                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            InitCanvas();
        }

        private void InitCanvas()
        {
            using (var graphics = Graphics.FromImage(_canvas))
            {
                graphics.Clear(Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void buttonGenerateSites_Click(object sender, EventArgs e)
        {
            int width;
            if (!int.TryParse(textBoxWidth.Text, out width))
                return;

            int height;
            if (!int.TryParse(textBoxHeight.Text, out height))
                return;

            int count;
            if (!int.TryParse(textBoxSites.Text, out count))
                return;

            _session.Sites = _voronoiService.GenerateSites(width, height, count);

            using (var graphics = Graphics.FromImage(_canvas))
            {
                graphics.Clear(Color.FromArgb(255, 255, 255, 255));

                var brush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));

                _factorX = _canvas.Width / width;
                _factorY = _canvas.Height / height;
                foreach (var site in _session.Sites)
                {
                    graphics.FillRectangle(brush, site.Point.XInt * _factorX, _canvas.Height - (site.Point.YInt * _factorY), _factorX, _factorY);
                }
            }
            tabPageDiagram.Invalidate();
        }

        private void buttonVoronoiDiagram_Click(object sender, EventArgs e)
        {
            if (_session.Sites == null)
                return;

            try
            {
                var map = _voronoiService.CreateDiagram(_canvas.Height / _factorY, _canvas.Width / _factorX, _session.Sites);

                textBoxLog.Text = string.Join(Environment.NewLine, map.Select(g => $"{g.GetType().Name}:\tX: {g.Point.X}, Y: {g.Point.Y}").ToArray());

                using (var graphics = Graphics.FromImage(_canvas))
                {
                    var halfEdgeBrush = new SolidBrush(Color.FromArgb(255, 0, 255, 0));
                    var vertexBrush = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
                    var pen = new Pen(halfEdgeBrush);

                    foreach (var geo in map)
                    {
                        if (geo is Vertex)
                            graphics.FillRectangle(vertexBrush, geo.Point.XInt * _factorX, _canvas.Height - (geo.Point.YInt * _factorY), _factorX, _factorY);

                        var halfEdge = geo as HalfEdge;

                        if (halfEdge == null)
                            continue;

                        var startX = (halfEdge.Start != null ? halfEdge.Start.Point.XInt : halfEdge.Point.XInt) * _factorX;
                        var startY = _canvas.Height - (halfEdge.Start != null ? halfEdge.Start.Point.YInt : halfEdge.Point.YInt) * _factorY;
                        var endX = (halfEdge.End != null ? halfEdge.End.Point.XInt : halfEdge.Point.XInt) * _factorX;
                        var endY = _canvas.Height - (halfEdge.End != null ? halfEdge.End.Point.YInt : halfEdge.Point.YInt) * _factorY;

                        //if (startX > 0 || startX < _canvas.Width || startY > 0 || startY < _canvas.Height || endX > 0 || endX < _canvas.Width || endY > 0 || endY < _canvas.Height)
                        if (startX > 0 && startY > 0)
                            graphics.DrawLine(pen, new PointF(startX, startY), new PointF(endX, endY));

                        //graphics.FillRectangle(vertexBrush, geo.Point.X * _factorX, _canvas.Height - (geo.Point.Y * _factorY), _factorX, _factorY);

                        //if (geo is HalfEdge && geo.Point.X > 0 && geo.Point.X < _canvas.Width && geo.Point.X > 0 && geo.Point.X < _canvas.Height)
                        //    graphics.FillRectangle(halfEdgeBrush, geo.Point.X * _factorX, _canvas.Height - (geo.Point.Y * _factorY), _factorX, _factorY);
                    }
                }
            }
            catch (Exception exception)
            {
                textBoxLog.Text += $"{Environment.NewLine}{exception.Message}{Environment.NewLine}{Environment.NewLine}{exception.Source}{Environment.NewLine}{Environment.NewLine}{exception.TargetSite}{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}";
                Logger.Instance.ToFile();
            }
            tabPageDiagram.Invalidate();
        }

        private void tabPageDiagram_Paint(object sender, PaintEventArgs e)
        {
            using (var graphics = e.Graphics)
            {
                graphics.DrawImage(_canvas, 0, 0, _canvas.Width, _canvas.Height);
            }
        }
    }
}
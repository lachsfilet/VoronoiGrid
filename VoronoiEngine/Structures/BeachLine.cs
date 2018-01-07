﻿using System;
using System.Collections.Generic;
using System.Linq;
using VoronoiEngine.Elements;
using VoronoiEngine.Events;

namespace VoronoiEngine.Structures
{
    public class BeachLine
    {
        public INode Root { get; private set; }

        public CircleEvent FindCircleEventAbove(Point site)
        {
            if (Root == null)
                return null;

            var arc = Root.Find(site) as Leaf;
            if (arc.CircleEvent == null)
                return null;

            return arc.CircleEvent;
        }

        public void InsertSite(Point point)
        {
            var leaf = new Leaf(point);

            // Add new site to empty tree
            if (Root == null)
            {
                Root = leaf;
                return;
            }

            // Add site to existing tree
            var rootNode = Root as Node;
            if (rootNode != null)
            {
                rootNode.Insert(point, ReplaceLeaf);
                return;
            }

            var rootLeaf = Root as Leaf;
            var node = new Node(null);
            Root = node;
            ReplaceLeaf(node, leaf, rootLeaf);
        }

        public ICollection<CircleEvent> GenerateCircleEvent(Point site)
        {
            var root = Root as Node;
            if (root == null)
                return null;

            var circleEvents = new List<CircleEvent>();

            // Find triple of consecutive arcs where the arc for the new site is
            // the left arc...
            var leftArcs = new List<INode>();
            root.GetDescendants(site, TraverseDirection.Clockwise, leftArcs, 3);

            if (leftArcs.Count == 3)
            {
                var leftEvent = DetermineCircleEvent(leftArcs);
                if (leftEvent != null)
                {
                    var leaf = leftArcs.Cast<Leaf>().ElementAt(1);
                    leaf.CircleEvent = leftEvent;
                    leftEvent.Arc = leaf;
                    circleEvents.Add(leftEvent);
                }
            }

            // ... and where it is the right arc.
            var rightArcs = new List<INode>();
            root.GetDescendants(site, TraverseDirection.CounterClockwise, rightArcs, 3);

            if (rightArcs.Count < 3)
                return circleEvents;

            var rightEvent = DetermineCircleEvent(rightArcs);
            if (rightEvent != null)
            {
                var leaf = leftArcs.Cast<Leaf>().ElementAt(1);
                leaf.CircleEvent = rightEvent;
                rightEvent.Arc = leaf;
                circleEvents.Add(rightEvent);
            }
            return circleEvents;
        }

        public void RemoveLeaf(Leaf leaf)
        {
            var parent = leaf.Parent as Node;
            var parentParent = parent.Parent as Node;
            if (parent.Left == leaf)
            {
                parent.Left = null;
                var right = parent.Right;                
                if(parentParent.Left == parent)
                    parentParent.Left = right;
                else
                    parentParent.Right = right;
                return;
            }

            parent.Right = null;
            var left = parent.Left;
            if (parentParent.Left == parent)
                parentParent.Left = left;
            else
                parentParent.Right = left;
        }

        private static void ReplaceLeaf(Node subRoot, Leaf newLeaf, Leaf arc)
        {
            // Build subtree
            var node = new Node(subRoot);

            subRoot.Left = node;
            subRoot.Right = arc;
            subRoot.Breakpoint = new Tuple { Left = newLeaf.Site, Right = arc.Site };
            subRoot.Edge = subRoot.Edge ?? new HalfEdge();
            subRoot.Edge.Add(subRoot.CalculateBreakpoint(newLeaf.Site.Y));

            arc.Parent = subRoot;

            node.Left = arc.Clone();
            node.Right = newLeaf;
            node.Breakpoint = new Tuple { Left = arc.Site, Right = newLeaf.Site };
            node.Edge = new HalfEdge();
            node.Edge.Add(node.CalculateBreakpoint(newLeaf.Site.Y));
            node.Left.Parent = node;
            newLeaf.Parent = node;
        }

        private static CircleEvent DetermineCircleEvent(ICollection<INode> arcs)
        {
            var sites = arcs.Cast<Leaf>().Select(l => l.Site).OrderBy(s => s.X).ToList();
            var circumcenter = CheckConversion(sites[0], sites[1], sites[2]);
            if (circumcenter == null)
                return null;

            var circleEventPoint = CalculateCircle(circumcenter, sites[0]);
            return new CircleEvent
            {
                Point = circleEventPoint,
                Vertex = circumcenter,
            };
        }

        private static Point CheckConversion(Point a, Point b, Point c)
        {
            Point ba = b - a;
            Point ca = c - a;
            var baLength = Math.Pow(ba.X, 2) + Math.Pow(ba.Y, 2);
            var caLength = Math.Pow(ca.X, 2) + Math.Pow(ca.Y, 2);
            var denominator = 2 * (ba.X * ca.Y - ba.Y * ca.X);
            //if (denominator <= 0)
            if (denominator >= 0)
                // Equals 0 for colinear points.  Less than zero if points are ccw and arc is diverging.
                return null;  // Don't use this circle event!

            var circumcenter = new Point()
            {
                X = (int)Math.Round(a.X + (ca.Y * baLength - ba.Y * caLength) / denominator),
                Y = (int)Math.Round(a.Y + (ba.X * caLength - ca.X * baLength) / denominator)
            };
            return circumcenter;
        }

        private static Point CalculateCircle(Point circumcenter, Point a)
        {
            var radius = (int)(((circumcenter.X - a.X) * 2 + (circumcenter.Y - a.Y) * 2) * 0.5m);
            return new Point { X = circumcenter.X, Y = circumcenter.Y - radius };
        }
    }
}
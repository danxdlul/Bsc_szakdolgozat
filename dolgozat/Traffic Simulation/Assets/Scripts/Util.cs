using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Util
    {
        public static bool maxBranchesReached(Graph graph,Node node, int currentNodeIndex)
        {
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                if (graph.Nodes[i].Position.x == node.Position.x && graph.Nodes[i].Position.z == node.Position.z && i < currentNodeIndex)
                {
                    if (graph.Nodes[i].branches < 3)
                    {
                        graph.Nodes[i].branches++;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        public static float distance(Node node1, Node node2)
        {
            return Mathf.Sqrt(Mathf.Pow((node1.Position.x - node2.Position.x), 2) + Mathf.Pow((node1.Position.z - node2.Position.z), 2));
        }

        public static bool hasIntersected(Edge edge1, Edge edge2)
        {
            double det, gamma, lambda;
            det = (edge1.To.Position.x - edge1.From.Position.x) * (edge2.To.Position.z - edge2.From.Position.z) - (edge2.To.Position.x - edge2.From.Position.x) * (edge1.To.Position.z - edge1.From.Position.z);
            if (det == 0)
            {
                return false;
            }
            else
            {
                lambda = ((edge2.To.Position.z - edge2.From.Position.z) * (edge2.To.Position.x - edge1.From.Position.x) + (edge2.From.Position.x - edge2.To.Position.x) * (edge2.To.Position.z - edge1.From.Position.z)) / det;
                gamma = ((edge1.From.Position.z - edge1.To.Position.z) * (edge2.To.Position.x - edge1.From.Position.x) + (edge1.To.Position.x - edge1.From.Position.x) * (edge2.To.Position.z - edge1.From.Position.z)) / det;
                return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
            }
        }

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        public static double FindDistanceToSegment(Edge edge, Node node)
        {
            Vector3 closest;
            float dx = edge.To.Position.x - edge.From.Position.x;
            float dy = edge.To.Position.z - edge.From.Position.z;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = edge.From.Position;
                dx = node.Position.x - edge.From.Position.x;
                dy = node.Position.z - edge.From.Position.z;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((node.Position.x - edge.From.Position.x) * dx + (node.Position.z - edge.From.Position.z) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector3(edge.From.Position.x, 0.5f, edge.From.Position.z);
                dx = node.Position.x - edge.From.Position.x;
                dy = node.Position.z - edge.From.Position.z;
            }
            else if (t > 1)
            {
                closest = new Vector3(edge.To.Position.x, 0.5f, edge.To.Position.z);
                dx = node.Position.x - edge.To.Position.x;
                dy = node.Position.z - edge.To.Position.z;
            }
            else
            {
                closest = new Vector3(edge.From.Position.x + t * dx, 0.5f, edge.From.Position.z + t * dy);
                dx = node.Position.x - closest.x;
                dy = node.Position.z - closest.z;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static int GetBranchCount(int level)
        {
            if (Random.Range(1, 101) < 100 - level * 20)
            {
                return 3;
            }
            else
            {
                return Random.Range(1, 3);
            }
        }
    }
}

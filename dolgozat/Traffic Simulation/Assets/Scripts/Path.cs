using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Path
    {
        public List<Vector3> WayPoints = new List<Vector3>();

        public Path(List<Node> nodes, List<Edge> edges)
        {
            WayPoints.Add(nodes[0].Position);
            for(int i = 1; i < nodes.Count; i++)
            {
                switch (edges[i - 1].Direction)
                {
                    case "south":
                        if(nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerBL + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBR) / 2);
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerTR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTL) / 2);
                        }
                        break;
                    case "north":
                        if (nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerTR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTL) / 2);
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerBL + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBR) / 2);
                        }
                        break;
                    case "west":
                        if (nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerBL) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBL) / 2);
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerTR + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTR) / 2);
                        }
                        break;
                    case "east":
                        if (nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerTR + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTR) / 2);
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerBL) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBL) / 2);
                        }
                        break;
                }
            }
        }
    }
}

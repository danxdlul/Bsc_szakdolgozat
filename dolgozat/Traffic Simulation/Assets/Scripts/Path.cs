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
        public Vector3[] LeftLaneWPs;
        public Vector3[] RightLaneWPs;
        public List<Node> Nodes;
        public List<Edge> Edges;

        public Path(List<Node> nodes, List<Edge> edges)
        {
            Nodes = nodes;
            Edges = edges;
            LeftLaneWPs = new Vector3[Nodes.Count];
            RightLaneWPs = new Vector3[Nodes.Count];
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
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerBR + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerTR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTL) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerTL + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        
                        break;
                    case "north":
                        if (nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerTR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTL) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerTL + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerBL + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBR) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerBR + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        break;
                    case "west":
                        if (nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerBL) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBL) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerBL + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerTR + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTR) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerTR + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        break;
                    case "east":
                        if (nodes[i] == edges[i - 1].From)
                        {
                            Vector3 point = (nodes[i].WorldCornerTR + nodes[i].WorldCornerBR) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerTR) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerTR + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        else
                        {
                            Vector3 point = (nodes[i].WorldCornerTL + nodes[i].WorldCornerBL) / 2;
                            WayPoints.Add((point + nodes[i].WorldCornerBL) / 2);
                            if (edges[i - 1].Lanes == 2)
                            {
                                LeftLaneWPs[i] = (point + WayPoints[WayPoints.Count - 1]) / 2;
                                RightLaneWPs[i] = (nodes[i].WorldCornerBL + WayPoints[WayPoints.Count - 1]) / 2;
                            }
                        }
                        break;
                }
            }
        }
        public bool WillTurnRight(int idx)
        {
            string currentlyGoing = GetRealDirection(idx);
            string willGo = GetRealDirection(idx + 1);
            if((currentlyGoing == "north" && willGo == "east") || (currentlyGoing == "east" && willGo == "south") || (currentlyGoing == "south" && willGo == "west") || (currentlyGoing == "west" && willGo == "north"))
            {
                return true;
            }
            return false;
        }
        public bool WillGoStraight(int idx)
        {
            
            string currentlyGoing = GetRealDirection(idx);
            string willGo = GetRealDirection(idx + 1);
            if (currentlyGoing == willGo)
            {
                return true;
            }
            return false;
        }
        public string GetRealDirection(int idx)
        {
            if(Nodes[idx] == Edges[idx].To)
            {
                return Edges[idx].Direction;
            }
            else
            {
                switch (Edges[idx].Direction)
                {
                    case "north":
                        return "south";
                    case "south":
                        return "north";
                    case "west":
                        return "east";
                    case "east":
                        return "west";
                    default:
                        return null;
                }
            }
        }
        public void ListAllData()
        {
            for(int i = 0; i < WayPoints.Count; i++)
            {
                Debug.Log(WayPoints[i]);
                Debug.Log(Nodes[i].Position);
                Debug.Log(Edges[i].From.Position);
                Debug.Log(RightLaneWPs[i]);
            }
        }
    }
}

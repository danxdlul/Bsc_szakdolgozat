using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public Vector3[] RoadCorners = new Vector3[4];
        private int[] tri = new int[6];
        private Vector3[] normals = new Vector3[4];
        private Vector2[] uv = new Vector2[4];
        public int Lanes { get; set; }
        public bool Oneway { get; set; }
        public string Direction { get; set; }

        public Edge(Node from, Node to, int lanes, bool oneway,string direction)
        {
            this.From = from;
            this.To = to;
            this.Lanes = lanes;
            this.Oneway = oneway;
            this.Direction = direction;
        }
        public void GetWorldCoords()
        {
            switch (Direction)
            {
                case "south":
                    RoadCorners[0] = From.WorldCornerBR;
                    RoadCorners[1] = From.WorldCornerBL;
                    RoadCorners[2] = To.WorldCornerTR;
                    RoadCorners[3] = To.WorldCornerTL;
                    break;
                case "north":
                    RoadCorners[0] = From.WorldCornerTL;
                    RoadCorners[1] = From.WorldCornerTR;
                    RoadCorners[2] = To.WorldCornerBL;
                    RoadCorners[3] = To.WorldCornerBR;
                    break;
                case "west":
                    RoadCorners[0] = From.WorldCornerBL;
                    RoadCorners[1] = From.WorldCornerTL;
                    RoadCorners[2] = To.WorldCornerBR;
                    RoadCorners[3] = To.WorldCornerTR;
                    break;
                case "east":
                    RoadCorners[0] = From.WorldCornerTR;
                    RoadCorners[1] = From.WorldCornerBR;
                    RoadCorners[2] = To.WorldCornerTL;
                    RoadCorners[3] = To.WorldCornerBL;
                    break;
                default:
                    Debug.Log("No case match!");
                    break;
            }
            
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
        }
        public Mesh GetRoadMesh()
        {
            GetWorldCoords();
            Mesh mesh = new Mesh();
            mesh.vertices = RoadCorners;
            mesh.triangles = tri;
            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;
            mesh.normals = normals;
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;
            return mesh;
        }
    }
}

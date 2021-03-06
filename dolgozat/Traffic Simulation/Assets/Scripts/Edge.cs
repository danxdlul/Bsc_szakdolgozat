﻿using System;
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
        private int[] triLane = new int[6];
        private Vector3[] normals = new Vector3[4];
        private Vector2[] uv = new Vector2[4];
        private Vector3[] LaneDivider = new Vector3[4];
        private Vector3[] MultiLaneLeft = new Vector3[4];
        private Vector3[] MultiLaneRight = new Vector3[4];
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
                    LaneDivider[0] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 5;
                    LaneDivider[1] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 10;
                    LaneDivider[2] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 5;
                    LaneDivider[3] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 10;
                    if (Lanes == 2)
                    {
                        MultiLaneLeft[0] = ((RoadCorners[1] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneLeft[1] = ((RoadCorners[1] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneLeft[2] = ((RoadCorners[3] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneLeft[3] = ((RoadCorners[3] + Vector3.up * 10) + LaneDivider[3]) / 2;
                        MultiLaneRight[0] = ((RoadCorners[0] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneRight[1] = ((RoadCorners[0] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneRight[2] = ((RoadCorners[2] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneRight[3] = ((RoadCorners[2] + Vector3.up * 10) + LaneDivider[3]) / 2;
                    }
                    break;
                case "north":
                    RoadCorners[0] = From.WorldCornerTL;
                    RoadCorners[1] = From.WorldCornerTR;
                    RoadCorners[2] = To.WorldCornerBL;
                    RoadCorners[3] = To.WorldCornerBR;
                    LaneDivider[0] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 5;
                    LaneDivider[1] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 10;
                    LaneDivider[2] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 5;
                    LaneDivider[3] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 10;
                    if (Lanes == 2)
                    {
                        MultiLaneLeft[0] = ((RoadCorners[0] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneLeft[1] = ((RoadCorners[0] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneLeft[2] = ((RoadCorners[2] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneLeft[3] = ((RoadCorners[2] + Vector3.up * 10) + LaneDivider[3]) / 2;
                        MultiLaneRight[0] = ((RoadCorners[1] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneRight[1] = ((RoadCorners[1] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneRight[2] = ((RoadCorners[3] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneRight[3] = ((RoadCorners[3] + Vector3.up * 10) + LaneDivider[3]) / 2;
                    }
                    break;
                case "west":
                    RoadCorners[0] = From.WorldCornerBL;
                    RoadCorners[1] = From.WorldCornerTL;
                    RoadCorners[2] = To.WorldCornerBR;
                    RoadCorners[3] = To.WorldCornerTR;
                    LaneDivider[0] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 5;
                    LaneDivider[1] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 10;
                    LaneDivider[2] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 5;
                    LaneDivider[3] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 10;
                    if (Lanes == 2)
                    {
                        MultiLaneLeft[0] = ((RoadCorners[1] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneLeft[1] = ((RoadCorners[1] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneLeft[2] = ((RoadCorners[3] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneLeft[3] = ((RoadCorners[3] + Vector3.up * 10) + LaneDivider[3]) / 2;
                        MultiLaneRight[0] = ((RoadCorners[0] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneRight[1] = ((RoadCorners[0] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneRight[2] = ((RoadCorners[2] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneRight[3] = ((RoadCorners[2] + Vector3.up * 10) + LaneDivider[3]) / 2;
                    }
                    break;
                case "east":
                    RoadCorners[0] = From.WorldCornerTR;
                    RoadCorners[1] = From.WorldCornerBR;
                    RoadCorners[2] = To.WorldCornerTL;
                    RoadCorners[3] = To.WorldCornerBL;
                    LaneDivider[0] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 5;
                    LaneDivider[1] = ((RoadCorners[0] + RoadCorners[1]) / 2) + Vector3.up * 10;
                    LaneDivider[2] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 5;
                    LaneDivider[3] = ((RoadCorners[2] + RoadCorners[3]) / 2) + Vector3.up * 10;
                    if (Lanes == 2)
                    {
                        MultiLaneLeft[0] = ((RoadCorners[0] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneLeft[1] = ((RoadCorners[0] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneLeft[2] = ((RoadCorners[2] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneLeft[3] = ((RoadCorners[2] + Vector3.up * 10) + LaneDivider[3]) / 2;
                        MultiLaneRight[0] = ((RoadCorners[1] + Vector3.up * 5) + LaneDivider[0]) / 2;
                        MultiLaneRight[1] = ((RoadCorners[1] + Vector3.up * 10) + LaneDivider[1]) / 2;
                        MultiLaneRight[2] = ((RoadCorners[3] + Vector3.up * 5) + LaneDivider[2]) / 2;
                        MultiLaneRight[3] = ((RoadCorners[3] + Vector3.up * 10) + LaneDivider[3]) / 2;
                    }
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
            triLane[0] = 0;
            triLane[1] = 2;
            triLane[2] = 1;
            triLane[3] = 2;
            triLane[4] = 3;
            triLane[5] = 1;
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
        public Mesh GetLaneDividerMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = LaneDivider;
            mesh.triangles = triLane;
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
        public Mesh GetLeftLaneDivider()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = MultiLaneLeft;
            mesh.triangles = triLane;
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
        public Mesh GetRightLaneDivider()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = MultiLaneRight;
            mesh.triangles = triLane;
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
        public Mesh GetInverseRightLaneDivider()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = MultiLaneRight;
            Array.Reverse(triLane, 0, 6);
            mesh.triangles = triLane;
            normals[0] = Vector3.forward;
            normals[1] = Vector3.forward;
            normals[2] = Vector3.forward;
            normals[3] = Vector3.forward;
            mesh.normals = normals;
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;
            return mesh;
        }
        public Mesh GetInverseLeftLaneDivider()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = MultiLaneLeft;
            Array.Reverse(triLane, 0, 6);
            mesh.triangles = triLane;
            normals[0] = Vector3.forward;
            normals[1] = Vector3.forward;
            normals[2] = Vector3.forward;
            normals[3] = Vector3.forward;
            mesh.normals = normals;
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;
            return mesh;
        }
        public Mesh GetInverseLaneDividerMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = LaneDivider;
            Array.Reverse(triLane, 0, 6);
            mesh.triangles = triLane;
            normals[0] = Vector3.forward;
            normals[1] = Vector3.forward;
            normals[2] = Vector3.forward;
            normals[3] = Vector3.forward;
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

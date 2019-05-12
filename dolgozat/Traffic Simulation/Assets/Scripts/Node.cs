using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node
    {
        public Vector3 Position { get; set; }
        public Vector3 WorldCornerTR { get; set; }
        public Vector3 WorldCornerTL { get; set; }
        public Vector3 WorldCornerBR { get; set; }
        public Vector3 WorldCornerBL { get; set; }
        public List<Edge> EdgesFromNode { get; set; }
        public string ConnectedFrom { get; set; }
        public int branches { get; set; }
        public int level { get; set; }
        public int parentNodeIndex { get; set; }
        public bool didSouth, didNorth, didWest, didEast;

        public Node(float xpos,float zpos,int branches,int level,string connectedfrom,int parent)
        {
            this.EdgesFromNode = new List<Edge>();
            this.Position = new Vector3(xpos, 0.1f, zpos);
            this.branches = branches;
            this.level = level;
            this.ConnectedFrom = connectedfrom;
            this.parentNodeIndex = parent;
        }
        public void CalculateWorldCorners (Renderer renderer)
        {
            WorldCornerTR = renderer.bounds.max;
            WorldCornerTL = new Vector3(renderer.bounds.min.x, 0.1f, renderer.bounds.max.z);
            WorldCornerBL = renderer.bounds.min;
            WorldCornerBR = new Vector3(renderer.bounds.max.x, 0.1f, renderer.bounds.min.z);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Graph
    {
        public List<Node> Nodes;
        public List<Edge> Edges;
        public Graph()
        {
            this.Nodes = new List<Node>();
            this.Edges = new List<Edge>();
        }
        public List<Vector3> GenerateRandomPath(int length)
        {
            int currentsize = 0;
            List<Vector3> path = new List<Vector3>();
            Node nextnode = Nodes[Random.Range(0, Nodes.Count)];
            Node previousNode = nextnode;
            path.Add(nextnode.Position);
            if( nextnode.EdgesFromNode.Count > 1)
            {
                Edge nextedge = nextnode.EdgesFromNode[Random.Range(0, nextnode.EdgesFromNode.Count)];
                nextnode = nextnode == nextedge.From ? nextedge.To : nextedge.From;
                path.Add(nextnode.Position);
                currentsize++;
            }
            else
            {
                return path;
            }
            while(currentsize < length && nextnode.EdgesFromNode.Count > 1)
            {
                Edge nextedge = nextnode.EdgesFromNode[Random.Range(0, nextnode.EdgesFromNode.Count)];
                if(nextedge.From != previousNode && nextedge.To != previousNode)
                {
                    previousNode = nextnode;
                    nextnode = nextnode == nextedge.From ? nextedge.To : nextedge.From;
                    path.Add(nextnode.Position);
                    currentsize++;
                }
                
            }
            return path;
        }
    }
}

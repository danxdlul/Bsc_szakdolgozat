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
        public Path GenerateRandomPath(int length)
        {
            int currentsize = 0;
            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();
            Node nextnode = Nodes[Random.Range(0, Nodes.Count)];
            nodes.Add(nextnode);
            Node previousNode = nextnode;
            if( nextnode.EdgesFromNode.Count > 1)
            {
                Edge nextedge = nextnode.EdgesFromNode[Random.Range(0, nextnode.EdgesFromNode.Count)];
                edges.Add(nextedge);
                nextnode = nextnode == nextedge.From ? nextedge.To : nextedge.From;
                nodes.Add(nextnode);
                currentsize++;
            }
            else
            {
                return new Path(nodes,edges);
            }
            while(currentsize < length && nextnode.EdgesFromNode.Count > 1)
            {
                Edge nextedge = nextnode.EdgesFromNode[Random.Range(0, nextnode.EdgesFromNode.Count)];
                if(nextedge.From != previousNode && nextedge.To != previousNode)
                {
                    previousNode = nextnode;
                    nextnode = nextnode == nextedge.From ? nextedge.To : nextedge.From;
                    edges.Add(nextedge);
                    nodes.Add(nextnode);
                    currentsize++;
                }
                
            }
            return new Path(nodes,edges);
        }
    }
}

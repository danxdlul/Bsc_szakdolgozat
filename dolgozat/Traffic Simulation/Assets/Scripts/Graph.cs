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
        public List<Node> BusNodes;
        public List<Edge> BusEdges;
        public List<Vector3> BusStops;
        public Path BusPath;
        public Path ReverseBusPath;
        public Graph()
        {
            this.Nodes = new List<Node>();
            this.Edges = new List<Edge>();
            this.BusStops = new List<Vector3>();
            this.BusNodes = new List<Node>();
            this.BusEdges = new List<Edge>();
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
        public void GenerateBusStops()
        {
            List<Node> potentialNodes = Util.findMaxLevelNodes(Nodes);
            Node start = potentialNodes[Random.Range(0, potentialNodes.Count)];
            Node end = potentialNodes[0];
            for(int i = 0; i < potentialNodes.Count; i++)
            {
                if(Util.distance(potentialNodes[i],start) > Util.distance(start, end))
                {
                    end = potentialNodes[i];
                }
            }
            int j = start.level;
            Node currentnode = start;
            BusNodes.Add(start);
            while(j != 0)
            {
                int nextstop = Random.Range(1, 3);
                for(int i = 0; i < nextstop; i++)
                {
                    Node temp = currentnode;
                    currentnode = Nodes[currentnode.parentNodeIndex];
                    if(currentnode != temp)
                    {
                        BusNodes.Add(currentnode);
                        Debug.Log("added " + currentnode.Position);
                        foreach (Edge edge in Edges)
                        {
                            if ((edge.From == temp && edge.To == currentnode) || (edge.From == currentnode && edge.To == temp))
                            {
                                BusEdges.Add(edge);
                                Debug.Log("added edge " + edge.From.Position + " " + edge.To.Position);
                            }
                        }
                    }
                    
                    
                }
                BusStops.Add((BusEdges.Last().From.Position + BusEdges.Last().To.Position)/2);
                j = currentnode.level;
            }
            j = end.level;
            currentnode = end;
            List<Edge> edgesfromEnd = new List<Edge>();
            List<Node> nodesfromEnd = new List<Node>();
            nodesfromEnd.Add(end);
            Debug.Log("added " + end.Position);
            while (j != 0)
            {
                int nextstop = Random.Range(1, 3);
                for(int i = 0;i < nextstop; i++)
                {
                    Node temp = currentnode;
                    currentnode = Nodes[currentnode.parentNodeIndex];
                    if (currentnode != temp)
                    {
                        nodesfromEnd.Add(currentnode);
                        Debug.Log("added " + currentnode.Position);
                        foreach (Edge edge in Edges)
                        {
                            if ((edge.From == temp && edge.To == currentnode) || (edge.From == currentnode && edge.To == temp))
                            {
                                edgesfromEnd.Add(edge);
                                Debug.Log("added edge " + edge.From.Position + " " + edge.To.Position);
                            }
                        }
                    }
                    
                }
                BusStops.Add((edgesfromEnd.Last().From.Position + edgesfromEnd.Last().To.Position) / 2);
                j = currentnode.level;
            }
            nodesfromEnd.RemoveAt(nodesfromEnd.Count - 1);
            nodesfromEnd.Reverse();
            BusNodes.AddRange(nodesfromEnd);
            edgesfromEnd.Reverse();
            BusEdges.AddRange(edgesfromEnd);
            Debug.Log(BusNodes.Count);
            Debug.Log(BusEdges.Count);
            this.ReverseBusPath = new Path(BusNodes, BusEdges);
            BusEdges.Reverse();
            BusNodes.Reverse();
            this.BusPath = new Path(BusNodes, BusEdges);
        }
        public void CalculateActualBranchCount()
        {
            foreach (Node n in Nodes)
            {
                n.branches = 0;
                foreach(Edge e in Edges)
                {
                    if((e.To.Position == n.Position) || (e.From.Position == n.Position && !e.Oneway))
                    {
                        n.branches++;
                    }
                }
            }
        }
    }
}

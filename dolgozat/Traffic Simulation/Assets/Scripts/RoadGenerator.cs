using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public Graph graph = new Graph();
    public Material RoadMaterial;
    public Material MultiLanes;
    List<GameObject> RoadList = new List<GameObject>();
    List<GameObject> XRoadList = new List<GameObject>();
    List<GameObject> LaneDividers = new List<GameObject>();
    int maxnodes = 100;
    public GameObject CrossRoad;
    public GameObject BusStop;

    void Start()
    {
        graph.Nodes.Add(new Node(-500, -500, 4, 0, "none", 0));
        for(int i = 0; graph.Nodes.Count < maxnodes; i++)
        {
            ExpandNode(i);
        }
        SpawnCrossroads();
        spawnRoads();
        graph.GenerateBusStops();
        //graph.BusPath.ListAllData();
        SpawnBusStops();
    }
    void ExpandNode(int i)
    {
        Node currentNode = graph.Nodes[i];
        bool didEast = false, didWest = false, didNorth = false, didSouth = false;
        switch (currentNode.ConnectedFrom)
        {
            case "south":
                didSouth = true;
                break;
            case "north":
                didNorth = true;
                break;
            case "west":
                didWest = true;
                break;
            case "east":
                didEast = true;
                break;
            default:
                Debug.Log("No case match!");
                break;
        }
        for(int j = 0; j < currentNode.branches; j++)
        {
            int random;
            string direction;
            while (true)
            {
                random = Random.Range(0, 4);
                if (random == 0 && didSouth == false)
                {
                    didSouth = true;
                    direction = "south";
                    break;
                }
                if (random == 1 && didNorth == false)
                {
                    didNorth = true;
                    direction = "north";
                    break;
                }
                if (random == 2 && didWest == false)
                {
                    didWest = true;
                    direction = "west";
                    break;
                }
                if (random == 3 && didEast == false)
                {
                    didEast = true;
                    direction = "east";
                    break;
                }
            }
            string childConnectedFrom = "error";
            float xpos = 0, zpos = 0;
            switch (direction)
            {
                case "south":
                    childConnectedFrom = "north";
                    xpos = Random.Range(-10.0f, 11.0f)+currentNode.Position.x;
                    zpos = -(Random.Range(150.0f,251.0f))+currentNode.Position.z;
                    break;
                case "north":
                    childConnectedFrom = "south";
                    xpos = Random.Range(-10.0f,11.0f) + currentNode.Position.x;
                    zpos = Random.Range(150.0f,251.0f) + currentNode.Position.z;
                    break;
                case "west":
                    childConnectedFrom = "east";
                    xpos = -(Random.Range(150.0f,251.0f)) + currentNode.Position.x;
                    zpos = Random.Range(-10.0f,11.0f) + currentNode.Position.z;
                    break;
                case "east":
                    childConnectedFrom = "west";
                    xpos = Random.Range(150.0f,251.0f) + currentNode.Position.x;
                    zpos = Random.Range(-10.0f,11.0f) + currentNode.Position.z;
                    break;
                default:
                    Debug.Log("No case match!");
                    break;
            }
            int level = currentNode.level + 1;
            int branches = Util.GetBranchCount(level);
            int lanes = branches == 3 ? 2 : 1;
            bool invalidedge = false, corrected = false, enoughdistance = true;
            Node newNode = new Node(xpos, zpos, branches, level, childConnectedFrom, i);
            Edge newEdge = new Edge(currentNode, newNode, lanes, false,direction);
            for(int k = 0; k < graph.Edges.Count; k++)
            {
                if(graph.Edges.Count != 0)
                {
                    Edge currentEdge = graph.Edges[k];
                    invalidedge = Util.hasIntersected(currentEdge, newEdge);
                    if (enoughdistance && Util.FindDistanceToSegment(currentEdge, newNode) < 100)
                    {
                        enoughdistance = false;
                    }
                    if(invalidedge && !corrected)
                    {
                        if(Util.distance(newNode,currentEdge.From) > Util.distance(newNode, currentEdge.To))
                        {
                            if (!Util.maxBranchesReached(graph,currentEdge.To, i))
                            {
                                newEdge.To = currentEdge.To;
                                invalidedge = false;
                                corrected = true;
                            }
                            else
                            {
                                invalidedge = true;
                                break;
                            }
                        }
                        else
                        {
                            if (!Util.maxBranchesReached(graph,currentEdge.From, i))
                            {
                                newEdge.To = currentEdge.From;
                                invalidedge = false;
                                corrected = true;
                            }
                            else
                            {
                                invalidedge = true;
                                break;
                            }
                        }
                    }
                    else if (invalidedge)
                    {
                        break;
                    }
                }
            }
            if ((!invalidedge && enoughdistance) || corrected)
            {
                if (!corrected)
                {
                    currentNode.EdgesFromNode.Add(newEdge);
                    newNode.EdgesFromNode.Add(newEdge);
                    graph.Nodes.Add(newNode);
                    graph.Edges.Add(newEdge);
                }
                else
                {
                    newEdge.Oneway = true;
                    newEdge.Lanes = 1;
                    currentNode.EdgesFromNode.Add(newEdge);
                    graph.Edges.Add(newEdge);
                }
            }
        }
    }

    
    private void SpawnCrossroads()
    {
        foreach(Node node in graph.Nodes)
        {
            XRoadList.Add(Instantiate(CrossRoad, node.Position,CrossRoad.transform.rotation));
            node.CalculateWorldCorners(XRoadList[XRoadList.Count - 1].GetComponent<Renderer>());
        }
    }
    private void spawnRoads()
    {
        foreach(Edge edge in graph.Edges)
        {
            RoadList.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
            RoadList[RoadList.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetRoadMesh();
            RoadList[RoadList.Count - 1].GetComponent<Renderer>().material = RoadMaterial;
            RoadList[RoadList.Count - 1].AddComponent<BoxCollider>();
            LaneDividers.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
            LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetLaneDividerMesh();
            LaneDividers[LaneDividers.Count - 1].GetComponent<MeshCollider>().sharedMesh = LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().sharedMesh;
            LaneDividers[LaneDividers.Count - 1].GetComponent<Renderer>().enabled = false;
            LaneDividers[LaneDividers.Count - 1].tag = "LaneDivider";
            LaneDividers.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
            LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetInverseLaneDividerMesh();
            LaneDividers[LaneDividers.Count - 1].GetComponent<MeshCollider>().sharedMesh = LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().sharedMesh;
            LaneDividers[LaneDividers.Count - 1].GetComponent<Renderer>().enabled = false;
            LaneDividers[LaneDividers.Count - 1].tag = "LaneDivider";
            if(edge.Lanes == 2)
            {
                RoadList[RoadList.Count - 1].GetComponent<Renderer>().material = MultiLanes;
                LaneDividers.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetLeftLaneDivider();
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshCollider>().sharedMesh = LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().sharedMesh;
                LaneDividers[LaneDividers.Count - 1].GetComponent<Renderer>().enabled = false;
                LaneDividers[LaneDividers.Count - 1].tag = "MultiLaneDivider";
                LaneDividers.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetInverseLeftLaneDivider();
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshCollider>().sharedMesh = LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().sharedMesh;
                LaneDividers[LaneDividers.Count - 1].GetComponent<Renderer>().enabled = false;
                LaneDividers[LaneDividers.Count - 1].tag = "MultiLaneDivider";
                LaneDividers.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetRightLaneDivider();
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshCollider>().sharedMesh = LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().sharedMesh;
                LaneDividers[LaneDividers.Count - 1].GetComponent<Renderer>().enabled = false;
                LaneDividers[LaneDividers.Count - 1].tag = "MultiLaneDivider";
                LaneDividers.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetInverseRightLaneDivider();
                LaneDividers[LaneDividers.Count - 1].GetComponent<MeshCollider>().sharedMesh = LaneDividers[LaneDividers.Count - 1].GetComponent<MeshFilter>().sharedMesh;
                LaneDividers[LaneDividers.Count - 1].GetComponent<Renderer>().enabled = false;
                LaneDividers[LaneDividers.Count - 1].tag = "MultiLaneDivider";
            }

        }
    }
    private void SpawnBusStops()
    {
        foreach(Vector3 pos in graph.BusStops)
        {
            Instantiate(BusStop, pos, Quaternion.identity);
        }
    }
}

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
    public Material OneWayMaterial;
    public Material vertRoad, horRoad;
    List<GameObject> RoadList = new List<GameObject>();
    List<GameObject> XRoadList = new List<GameObject>();
    List<GameObject> LaneDividers = new List<GameObject>();
    List<GameObject> BusStops = new List<GameObject>();
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
        graph.CalculateActualBranchCount();
        SpawnCrossroads();
        spawnRoads();
        graph.GenerateBusStops();
        SpawnBusStops();
        gameObject.GetComponent<BuildingGenerator>().graph = this.graph;
        gameObject.GetComponent<BuildingGenerator>().GenerateBuildings();
        gameObject.GetComponent<ParkGenerator>().graph = this.graph;
        gameObject.GetComponent<ParkGenerator>().PlantTrees();
        AddParks();
        GameObject.FindGameObjectWithTag("Terrain").transform.localScale = new Vector3(1000 + 2*maxnodes, 1f,1000 + 2*maxnodes);
    }
    void ExpandNode(int i)
    {
        Node currentNode = graph.Nodes[i];

        currentNode.didEast = false;
        currentNode.didWest = false;
        currentNode.didNorth = false;
        currentNode.didSouth = false;
        switch (currentNode.ConnectedFrom)
        {
            case "south":
                currentNode.didSouth = true;
                break;
            case "north":
                currentNode.didNorth = true;
                break;
            case "west":
                currentNode.didWest = true;
                break;
            case "east":
                currentNode.didEast = true;
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
                if (random == 0 && currentNode.didSouth == false)
                {
                    direction = "south";
                    break;
                }
                if (random == 1 && currentNode.didNorth == false)
                {
                    direction = "north";
                    break;
                }
                if (random == 2 && currentNode.didWest == false)
                {
                    direction = "west";
                    break;
                }
                if (random == 3 && currentNode.didEast == false)
                {
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
                            if (!Util.maxBranchesReached(graph,currentEdge.To, i) && ((direction == "south" && !currentEdge.To.didNorth) || (direction == "north" && !currentEdge.To.didSouth) || (direction == "west" && !currentEdge.To.didEast) || (direction == "east" && !currentEdge.To.didWest)))
                            {
                                newEdge.To = currentEdge.To;
                                invalidedge = false;
                                corrected = true;
                                switch (direction)
                                {
                                    case "south":
                                        currentEdge.To.didNorth = true;
                                        break;
                                    case "north":
                                        currentEdge.To.didSouth = true;
                                        break;
                                    case "west":
                                        currentEdge.To.didEast = true;
                                        break;
                                    case "east":
                                        currentEdge.To.didWest = true;
                                        break;
                                    default:
                                        Debug.Log("No case match!");
                                        break;
                                }
                            }
                            else
                            {
                                invalidedge = true;
                                break;
                            }
                        }
                        else
                        {
                            if (!Util.maxBranchesReached(graph,currentEdge.From, i) && ((direction == "south" && !currentEdge.From.didNorth) || (direction == "north" && !currentEdge.From.didSouth) || (direction == "west" && !currentEdge.From.didEast) || (direction == "east" && !currentEdge.From.didWest)))
                            {
                                newEdge.To = currentEdge.From;
                                invalidedge = false;
                                corrected = true;
                                switch (direction)
                                {
                                    case "south":
                                        currentEdge.From.didNorth = true;
                                        break;
                                    case "north":
                                        currentEdge.From.didSouth = true;
                                        break;
                                    case "west":
                                        currentEdge.From.didEast = true;
                                        break;
                                    case "east":
                                        currentEdge.From.didWest = true;
                                        break;
                                    default:
                                        Debug.Log("No case match!");
                                        break;
                                }
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
                    switch (direction)
                    {
                        case "north":
                            currentNode.didNorth = true;
                            break;
                        case "south":
                            currentNode.didSouth = true;
                            break;
                        case "east":
                            currentNode.didEast = true;
                            break;
                        case "west":
                            currentNode.didWest = true;
                            break;
                        default:
                            Debug.Log("No case match!");
                            break;
                    }
                }
                else
                {
                    newEdge.Oneway = true;
                    
                    newEdge.Lanes = 1;
                    currentNode.EdgesFromNode.Add(newEdge);
                    graph.Edges.Add(newEdge);
                    switch (direction)
                    {
                        case "north":
                            currentNode.didNorth = true;
                            break;
                        case "south":
                            currentNode.didSouth = true;
                            break;
                        case "east":
                            currentNode.didEast = true;
                            break;
                        case "west":
                            currentNode.didWest = true;
                            break;
                        default:
                            Debug.Log("No case match!");
                            break;
                    }
                }
            }
        }
    }

    private void AddParks()
    {
        foreach(Edge edge in graph.Edges)
        {
            if (edge.Oneway)
            {
                Debug.Log("this oneway");
                Node node1 = edge.From;
                Node node2 = edge.To;
                Node node3;
                Node node4;
                foreach (Edge e in graph.Edges)
                {
                    if ((e.From == node1 || e.To == node1) && e != edge)
                    {
                        foreach (Edge ed in graph.Edges)
                        {
                            if (ed.From == node2 || e.To == node2 && e != edge && e != ed)
                            {
                                foreach (Edge edg in graph.Edges)
                                {
                                    if (edg != ed && edg != edge && edg != e && (edg.From == ed.From || edg.From == ed.To || edg.To == ed.From || edg.To == ed.To) && (edg.From == e.From || edg.From == e.To || edg.To == e.From || edg.To == e.To))
                                    {
                                        node3 = edg.From;
                                        node4 = edg.To;
                                        gameObject.GetComponent<ParkGenerator>().GeneratePark(node1, node2, node3, node4);
                                    }
                                }
                            }
                        }
                    }
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
            XRoadList[XRoadList.Count - 1].GetComponent<CrossRoadController>().node = node;
            if(node.didNorth && node.didSouth && !node.didWest && !node.didEast)
            {
                XRoadList[XRoadList.Count - 1].GetComponent<Renderer>().sharedMaterial = horRoad;
            }
            if(node.didEast && node.didWest && !node.didNorth && !node.didSouth)
            {
                XRoadList[XRoadList.Count - 1].GetComponent<Renderer>().sharedMaterial = vertRoad;
            }
        }
    }
    private void spawnRoads()
    {
        foreach(Edge edge in graph.Edges)
        {
            RoadList.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
            RoadList[RoadList.Count - 1].GetComponent<MeshFilter>().mesh = edge.GetRoadMesh();
            if (!edge.Oneway)
            {
                RoadList[RoadList.Count - 1].GetComponent<Renderer>().material = RoadMaterial;
            }
            else
            {
                RoadList[RoadList.Count - 1].GetComponent<Renderer>().material = OneWayMaterial;
            }
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
            BusStops.Add(Instantiate(BusStop, pos, Quaternion.identity));
        }
    }
    public void GenerateANew()
    {
        gameObject.GetComponent<CarSpawner>().StopAllCoroutines();
        foreach (GameObject obj in XRoadList)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in LaneDividers)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in BusStops)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in RoadList)
        {
            Destroy(obj);
        }
        graph = new Graph();
        foreach (GameObject obj in gameObject.GetComponent<CarSpawner>().Cars)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in gameObject.GetComponent<CarSpawner>().Buses)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in gameObject.GetComponent<ParkGenerator>().worldtrees)
        {
            Destroy(obj);
        }
        foreach(GameObject obj in gameObject.GetComponent<BuildingGenerator>().buildings)
        {        
            Destroy(obj);
        }
        Invoke("Start", 0.2f);
        gameObject.GetComponent<CarSpawner>().Invoke("startCoroutines", 0.2f);
    }
    public void setMaxNodes(string n)
    {
        if (int.Parse(n) > 0)
        {
            maxnodes = int.Parse(n);
        }      
    }
    public void setLightTime(string t)
    {
        if(int.Parse(t) > 0)
        {
            GameObject[] xroads = GameObject.FindGameObjectsWithTag("XRoad");
            foreach (GameObject xroad in xroads)
            {
                xroad.GetComponent<CrossRoadController>().LightSwitchTime = int.Parse(t);
            }
        }       
    }
}

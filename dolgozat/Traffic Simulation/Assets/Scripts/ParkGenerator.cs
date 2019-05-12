using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParkGenerator : MonoBehaviour
{
    public Graph graph;
    private Vector3 boundingBoxBL, boundingBoxBR, boundingBoxTL, boundingBoxTR;
    private Node node1, node2, node3, node4;
    private List<Node> nodelist = new List<Node>();
    public List<GameObject> trees;
    public List<GameObject> worldtrees = new List<GameObject>();
    public GameObject Fountain;
    Vector2[] poly = new Vector2[4];
    Vector2 treepoint;
    private float maxX;
    private float minX;
    private float maxZ;
    private float minZ;
    float xpos;
    float zpos;
    public void PlantTrees()
    {
        float minx = graph.Nodes.OrderByDescending(x => x.Position.x).LastOrDefault().Position.x;
        float maxx = graph.Nodes.OrderByDescending(x => x.Position.x).FirstOrDefault().Position.x;
        float minz = graph.Nodes.OrderByDescending(x => x.Position.z).LastOrDefault().Position.z;
        float maxz = graph.Nodes.OrderByDescending(x => x.Position.z).FirstOrDefault().Position.z;
        for (int i = 0; i < 5 * graph.Nodes.Count; i++)
        {
            xpos = Random.Range(minx, maxx);
            zpos = Random.Range(minz, maxz);
            treepoint = new Vector2(xpos, zpos);
            if (!Physics.CheckBox(new Vector3(treepoint.x, 6f, treepoint.y), new Vector3(5f, 5f, 5f)))
            {
                worldtrees.Add(Instantiate(trees[Random.Range(0, 3)], new Vector3(treepoint.x, 0.1f, treepoint.y), Quaternion.identity));
            }
        }
    }
    public void GeneratePark(Node node1,Node node2,Node node3, Node node4)
    {
        this.node1 = node1;
        poly[0] = new Vector2(node1.Position.x, node1.Position.z);
        this.node2 = node2;
        poly[1] = new Vector2(node2.Position.x, node2.Position.z);
        this.node3 = node3;
        poly[2] = new Vector2(node3.Position.x, node3.Position.z);
        this.node4 = node4;
        poly[3] = new Vector2(node4.Position.x, node4.Position.z);
        nodelist.Clear();
        nodelist.Add(node1);
        nodelist.Add(node2);
        nodelist.Add(node3);
        nodelist.Add(node4);
        Debug.Log(node1.Position);
        Debug.Log(node2.Position);
        Debug.Log(node3.Position);
        Debug.Log(node4.Position);
        GetBoundingBox();
        for(int i = 0; i < 100; i++)
        {
            do
            {
                xpos = Random.Range(minX, maxX);
                zpos = Random.Range(minZ, maxZ);
                treepoint = new Vector2(xpos, zpos);
            } while (!IsPointInPolygon(treepoint, poly));
            if (!Physics.CheckBox(new Vector3(treepoint.x, 6f, treepoint.y), new Vector3(5f, 5f, 5f)))
            {
                worldtrees.Add(Instantiate(trees[Random.Range(0, 3)], new Vector3(treepoint.x, 0.1f, treepoint.y), Quaternion.identity));
            }
        }
        do
        {
            xpos = Random.Range(minX, maxX);
            zpos = Random.Range(minZ, maxZ);
            treepoint = new Vector2(xpos, zpos);
        } while (!IsPointInPolygon(treepoint, poly) || Physics.CheckBox(new Vector3(treepoint.x, 6f, treepoint.y), new Vector3(5f, 5f, 5f)));
        worldtrees.Add(Instantiate(Fountain, new Vector3(treepoint.x, 0.1f, treepoint.y), Quaternion.identity));
    }
    private void GetBoundingBox()
    {
        maxX = nodelist.OrderByDescending(x => x.Position.x).FirstOrDefault().Position.x;
        minX = nodelist.OrderByDescending(x => x.Position.x).LastOrDefault().Position.x;
        maxZ = nodelist.OrderByDescending(x => x.Position.z).FirstOrDefault().Position.z;
        minZ = nodelist.OrderByDescending(x => x.Position.z).LastOrDefault().Position.z;
        boundingBoxBL = new Vector3(minX, 0.1f, minZ);
        boundingBoxBR = new Vector3(maxX, 0.1f, minZ);
        boundingBoxTL = new Vector3(minX, 0.1f, maxZ);
        boundingBoxTR = new Vector3(maxX, 0.1f, maxZ);
    }
    public bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x; endY = endPoint.y;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }
}

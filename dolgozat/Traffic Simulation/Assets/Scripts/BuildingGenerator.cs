using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public Graph graph;
    public List<GameObject> blocks = new List<GameObject>();
    public List<GameObject> houses = new List<GameObject>();
    public void GenerateBuildings()
    {
        foreach (Edge e in graph.Edges)
        {
            if(e.From.level < 4)
            {
                Vector3 theRoad = e.To.Position - e.From.Position;
                Vector3 buildingslot = theRoad / 10f;
                Vector3 side = Vector3.Cross(theRoad, Vector3.up).normalized;
                for (Vector3 offset = buildingslot;offset.magnitude < theRoad.magnitude - 20f;offset += buildingslot)
                {
                    Instantiate(blocks[Random.Range(0, 5)], e.From.Position + offset + side * 17f, Quaternion.LookRotation(e.From.Position + offset));
                    Instantiate(blocks[Random.Range(0, 5)], e.From.Position + offset - side * 17f, Quaternion.LookRotation(e.From.Position + offset));
                }
                //blocks
            }
            else
            {
                Vector3 theRoad = e.To.Position - e.From.Position;
                Vector3 buildingslot = theRoad / 10f;
                Vector3 side = Vector3.Cross(theRoad, Vector3.up).normalized;
                for (Vector3 offset = buildingslot; offset.magnitude < theRoad.magnitude - 20f; offset += buildingslot)
                {
                    Instantiate(houses[Random.Range(0, 2)], e.From.Position + offset + side * 12f, Quaternion.identity);
                    Instantiate(houses[Random.Range(0, 2)], e.From.Position + offset - side * 12f, Quaternion.identity);
                }
                //houses
            }
        }
    }
}

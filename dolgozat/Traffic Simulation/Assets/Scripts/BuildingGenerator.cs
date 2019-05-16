using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public Graph graph;
    public List<GameObject> blocks = new List<GameObject>();
    public List<GameObject> houses = new List<GameObject>();
    public List<GameObject> buildings = new List<GameObject>();
    public void GenerateBuildings()
    {
        foreach (Edge e in graph.Edges)
        {
            if(graph.Nodes.Count > 30)
            {
                if (e.From.level < graph.Edges.Max(x => x.From.level) / 1.5)
                {
                    Vector3 theRoad = e.To.Position - e.From.Position;
                    Vector3 buildingslot = theRoad / 9f;
                    Vector3 side = Vector3.Cross(theRoad, Vector3.up).normalized;
                    for (Vector3 offset = buildingslot; offset.magnitude < theRoad.magnitude - 20f; offset += buildingslot)
                    {
                        if (!Physics.CheckBox(e.From.Position + offset + side * 20f + Vector3.up * 6f, new Vector3(10f, 5f, 10f)))
                        {
                            buildings.Add(Instantiate(blocks[Random.Range(0, 5)], e.From.Position + offset + side * 20f, Quaternion.LookRotation(e.From.Position + offset)));
                            buildings[buildings.Count - 1].AddComponent<BoxCollider>();
                        }

                        if (!Physics.CheckBox(e.From.Position + offset - side * 20f + Vector3.up * 6f, new Vector3(10f, 5f, 10f)))
                        {
                            buildings.Add(Instantiate(blocks[Random.Range(0, 5)], e.From.Position + offset - side * 20f, Quaternion.LookRotation(e.From.Position + offset)));
                            buildings[buildings.Count - 1].AddComponent<BoxCollider>();
                        }


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
                        if (!Physics.CheckBox(e.From.Position + offset + side * 14f + Vector3.up * 6f, new Vector3(5f, 5f, 5f)))
                        {
                            buildings.Add(Instantiate(houses[Random.Range(0, 2)], e.From.Position + offset + side * 14f, Quaternion.identity));
                        }
                        if (!Physics.CheckBox(e.From.Position + offset - side * 14f + Vector3.up * 6f, new Vector3(5f, 5f, 5f)))
                        {
                            buildings.Add(Instantiate(houses[Random.Range(0, 2)], e.From.Position + offset - side * 14f, Quaternion.identity));
                        }


                    }
                    //houses
                }
            }
            else
            {
                Vector3 theRoad = e.To.Position - e.From.Position;
                Vector3 buildingslot = theRoad / 10f;
                Vector3 side = Vector3.Cross(theRoad, Vector3.up).normalized;
                for (Vector3 offset = buildingslot; offset.magnitude < theRoad.magnitude - 20f; offset += buildingslot)
                {
                    if (!Physics.CheckBox(e.From.Position + offset + side * 14f + Vector3.up * 6f, new Vector3(5f, 5f, 5f)))
                    {
                        buildings.Add(Instantiate(houses[Random.Range(0, 2)], e.From.Position + offset + side * 14f, Quaternion.identity));
                    }
                    if (!Physics.CheckBox(e.From.Position + offset - side * 14f + Vector3.up * 6f, new Vector3(5f, 5f, 5f)))
                    {
                        buildings.Add(Instantiate(houses[Random.Range(0, 2)], e.From.Position + offset - side * 14f, Quaternion.identity));
                    }


                }
                //houses
            }

        }
    }
}

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoadController : MonoBehaviour
{
    public Node node;
    public int LightSwitchTime = 20;
    public bool HorizontalCanCross = false;
    public bool VerticalCanCross = false;
    public Material Red;
    public Material Green;
    public GameObject NorthLight;
    public GameObject SouthLight;
    public GameObject WestLight;
    public GameObject EastLight;
    private Renderer westrenderer;
    private Renderer eastrenderer;
    private Renderer northrenderer;
    private Renderer southrenderer;
    // Start is called before the first frame update
    void Start()
    {
        if(node.branches > 2)
        {
            westrenderer = WestLight.GetComponent<Renderer>();
            eastrenderer = EastLight.GetComponent<Renderer>();
            northrenderer = NorthLight.GetComponent<Renderer>();
            southrenderer = SouthLight.GetComponent<Renderer>();
            StartCoroutine(LightSwitcher());
        }
        else
        {
            Destroy(NorthLight);
            Destroy(SouthLight);
            Destroy(WestLight);
            Destroy(EastLight);
            HorizontalCanCross = true;
            VerticalCanCross = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentInParent<CarEngine>().isInCrossRoad = true;
        other.GetComponentInParent<CarEngine>().sensorLength = 4f;
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponentInParent<CarEngine>().isInCrossRoad = false;
        other.GetComponentInParent<CarEngine>().sensorLength = 20f;
    }
    public bool CarCanGo(string direction)
    {
        if((HorizontalCanCross && (direction == "west" || direction == "east")) || (VerticalCanCross && (direction == "north" || direction == "south")))
        {
            return true;
        }
        return false;
    }
    IEnumerator LightSwitcher()
    {
        while (true)
        {
            westrenderer.material = Green;
            eastrenderer.material = Green;
            northrenderer.material = Red;
            southrenderer.material = Red;
            HorizontalCanCross = true;
            yield return new WaitForSeconds(LightSwitchTime);
            westrenderer.material = Red;
            eastrenderer.material = Red;
            HorizontalCanCross = false;
            yield return new WaitForSeconds(5);
            northrenderer.material = Green;
            southrenderer.material = Green;
            VerticalCanCross = true;
            yield return new WaitForSeconds(LightSwitchTime);
            northrenderer.material = Red;
            southrenderer.material = Red;
            VerticalCanCross = false;
            yield return new WaitForSeconds(5);
        }
    }
}

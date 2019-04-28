using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStopController : MonoBehaviour
{
    public bool busCurrentlyStopped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Bus"))
        {
            busCurrentlyStopped = true;
            other.gameObject.GetComponentInParent<BusEngine>().isWaitingAtStop = true;
            StartCoroutine(BuSStopper(other.gameObject.GetComponentInParent<BusEngine>()));
            Debug.Log("Stopped a bus");
        }
    }
    void OnTriggerExit(Collider other)
    {
        busCurrentlyStopped = false;
    }
    IEnumerator BuSStopper(BusEngine engine)
    {
        yield return new WaitForSeconds(5);
        engine.isWaitingAtStop = false;
    }


}

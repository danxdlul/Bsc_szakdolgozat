using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    private int currentNode = 0;
    public List<Vector3> Nodes;
    public float maxMotorTorque = 80f;
    public float maxBreakTorque = 350f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public bool isBraking = false;
    // Start is called before the first frame update
    private void Start()
    {
        transform.position = Nodes[0];
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        Braking();
    }
    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(Nodes[currentNode]);
        float newSteer = (relativeVector.x / relativeVector.magnitude)*maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }
    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;
        if(currentSpeed < maxSpeed)
        {
            isBraking = false;
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
        else if(currentSpeed > maxSpeed)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
        
    }
    private void CheckWaypointDistance()
    {
        if(Vector3.Distance(transform.position,Nodes[currentNode]) < 1f)
        {
            if(currentNode == Nodes.Count - 1)
            {
                Destroy(gameObject);
            }
            currentNode++;
        }else if(Vector3.Distance(transform.position, Nodes[currentNode]) < 40f)
        {
            maxSpeed = 2f;
        }
        else
        {
            maxSpeed = 100f;
        }
    }
    private void Braking()
    {
        if (isBraking)
        {
            wheelRL.brakeTorque = maxBreakTorque;
            wheelRR.brakeTorque = maxBreakTorque;
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }
}

using Assets.Scripts;
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
    public Path path;
    public float maxMotorTorque = 80f;
    public float maxBreakTorque = 350f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public bool isBraking = false;
    public float watafak = 0f;

    [Header("Sensors")]
    public float sensorLength = 20f;
    public float frontSensorPosition = 0.04f;
    public float sideSensorPosition = 0.65f;

    // Start is called before the first frame update
    private void Start()
    {
        transform.position = path.WayPoints[0];
        if(path.WayPoints.Count > 1)
        {
            transform?.LookAt(path.WayPoints[1]);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Sensors();
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        Braking();
    }
    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos.z += frontSensorPosition;
        //center
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car"))
            {
                maxSpeed = hit.distance - 10f;
            }
            else
            {
                //maxSpeed = 100f;
            }
            
        }
        
        //right
        sensorStartPos.x += sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car"))
            {
                maxSpeed = hit.distance-10f;
            }
            else
            {
                //maxSpeed = 100f;
            }
        }
       
        //left
        sensorStartPos.x -= 2 * sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car"))
            {
                maxSpeed = hit.distance - 10f;
            }
            else
            {
                //maxSpeed = 100f;
            }
        }
        
    }
    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(path.WayPoints[currentNode]);
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
        watafak = Vector3.Distance(transform.position, path.WayPoints[currentNode]);
        if (Vector3.Distance(transform.position,path.WayPoints[currentNode]) < 1f)
        {
            if(currentNode == path.WayPoints.Count - 1)
            {
                Destroy(gameObject);
            }
            currentNode++;
        }else if(Mathf.Abs(Vector3.Distance(transform.position, path.WayPoints[currentNode])) < 40.0f)
        {
            maxSpeed = 3f;
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

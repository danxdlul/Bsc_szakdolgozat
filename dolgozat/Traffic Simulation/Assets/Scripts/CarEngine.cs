using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public float maxSteerAngle = 45f;
    public float turnSpeed = 10f;
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
    public float angledSensor = 90f;
    public bool isBraking = false;
    public float watafak = 0f;
    private bool laneSteering;
    public bool isInCrossRoad = false;
    private float targetSteerAngle = 0;

    [Header("Sensors")]
    public float sensorLength = 20f;
    public Vector3 frontSensorPosition = new Vector3(0,0,1.5f);
    public Vector3 topSensorPosition = new Vector3(0, 6f, 1.5f);
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
        LerpToSteerAngle();
    }
    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        laneSteering = false;
        //center
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = hit.distance - 10f;
            }
            if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
            
        }
        
        //right
        sensorStartPos += transform.right * sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = hit.distance-10f;
            }
            if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
        }

        //angled
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angledSensor,transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
        }

        //left
        sensorStartPos -= 2 * transform.right * sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = hit.distance - 10f;
            }
            if(hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
        }

        //tops
        sensorStartPos = transform.position;
        sensorStartPos += transform.forward * topSensorPosition.z;
        sensorStartPos += transform.up * topSensorPosition.y;
        laneSteering = false;
        //center
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }

        }

        //right
        sensorStartPos += transform.right * sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
        }

        //angled
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angledSensor, transform.up) * transform.forward, out hit, 2))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
        }

        //left
        sensorStartPos -= 2 * transform.right * sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("LaneDivider"))
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = maxSteerAngle;
                    
                }
                laneSteering = true;
            }
        }
        

    }
    private void ApplySteer()
    {
        if (laneSteering && !isInCrossRoad)  return;
        Vector3 relativeVector = transform.InverseTransformPoint(path.WayPoints[currentNode]);
        float newSteer = (relativeVector.x / relativeVector.magnitude)*maxSteerAngle;
        if(laneSteering && isInCrossRoad)
        {
            newSteer += 30f;
        }
        targetSteerAngle = newSteer;
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
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
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

    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.other.CompareTag("Car"))
        {
            Invoke("DestroyMyself", 5);
        }
    }

    void DestroyMyself()
    {
        Destroy(gameObject);
    }
}

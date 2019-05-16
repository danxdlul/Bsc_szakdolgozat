using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer;
    public float maxSteerAngle = 30f;
    public float turnSpeed = 10f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public int currentNode = 0;
    public int currentWayPoint = 0;
    public Path path;
    public float maxMotorTorque = 80f;
    public float maxBreakTorque = 700f;
    public float currentSpeed;
    public float maxSpeed = 8f;
    public float angledSensor = 90f;
    public bool isBraking = false;
    public bool laneSteering;
    public bool isInCrossRoad = false;
    public bool WillTurnRight;
    public bool WillGoStraight;
    public float targetSteerAngle = 0;

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
    public void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        laneSteering = false;
        //center
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("XRoad"))
            {
                if (!hit.collider.gameObject.GetComponent<CrossRoadController>().CarCanGo(path.Edges[currentNode - 1].Direction))
                {
                    maxSpeed = 0f;
                }
            }
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car") || hit.collider.CompareTag("Bus"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = Mathf.Clamp(hit.distance - 10f,0,8);
            }
            else if (hit.collider.CompareTag("BusStop") && hit.collider.gameObject.GetComponent<BusStopController>().busCurrentlyStopped)
            {
                if(path.Edges[currentNode - 1].Lanes == 2 || !WillTurnRight)
                {
                    maxSpeed = 0f;
                }
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
            else if (hit.collider.CompareTag("Car") || hit.collider.CompareTag("Bus"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = Mathf.Clamp(hit.distance - 10f, 0, 8);
            }
        }

        //angled
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angledSensor,transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                targetSteerAngle = -5f;
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
            else if (hit.collider.CompareTag("Car") || hit.collider.CompareTag("Bus"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = Mathf.Clamp(hit.distance - 10f, 0, 8);
            }
        }

        //tops
        sensorStartPos = transform.position;
        sensorStartPos += transform.forward * topSensorPosition.z;
        sensorStartPos += transform.up * topSensorPosition.y;
        laneSteering = false;
        //angled
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angledSensor, transform.up) * transform.forward, out hit, 3))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            
            else if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }

            }
        }
        //angledleft
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-angledSensor, transform.up) * transform.forward, out hit, 3))
        {
            if (hit.collider.CompareTag("MultiLaneDivider") && !WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = -5f;
                    laneSteering = true;
                }

            }
        }
        //center
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("LaneDivider") && !laneSteering)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }
                
            }
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight && !laneSteering)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }
                
            }

        }

        //right
        sensorStartPos += transform.right * sideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, 10f))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("LaneDivider") && !laneSteering)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }
                
            }
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight && !laneSteering)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }
                
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
            else if (hit.collider.CompareTag("LaneDivider") && !laneSteering)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }
                
            }
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight && !laneSteering)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                    laneSteering = true;
                }
                
            }
        }
        

    }
    public void ApplySteer()
    {
        if (laneSteering && !isInCrossRoad)  return;
        Vector3 relativeVector = transform.InverseTransformPoint(path.WayPoints[currentWayPoint]);
        float newSteer = (relativeVector.x / relativeVector.magnitude)*maxSteerAngle;
        targetSteerAngle = newSteer;
    }
    public void Drive()
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
        if (Vector3.Distance(transform.position,path.WayPoints[currentWayPoint]) < 2.5f)
        {
            WillTurnRight = false;
            if(currentNode == path.Nodes.Count - 1)
            {
                Destroy(gameObject);
                return;
            }
            if(currentNode < path.Nodes.Count - 2)
            {
                WillTurnRight = path.WillTurnRight(currentNode);
                WillGoStraight = path.WillGoStraight(currentNode);
            }
            currentWayPoint++;
            if (!isInCrossRoad)
            {
                currentNode++;
            }
        }else if(Mathf.Abs(Vector3.Distance(transform.position, path.WayPoints[currentWayPoint])) < 40.0f && !WillGoStraight && !isBraking)
        {
            maxSpeed = 3f;
        }
        else if (!isBraking)
        {
            maxSpeed = 8f;
        }
    }
    private void Braking()
    {
        if (isBraking)
        {
            carRenderer.material.mainTexture = textureBraking;
            wheelRL.brakeTorque = maxBreakTorque;
            wheelRR.brakeTorque = maxBreakTorque;
            wheelFL.brakeTorque = maxBreakTorque;
            wheelFR.brakeTorque = maxBreakTorque;
        }
        else
        {
            carRenderer.material.mainTexture = textureNormal;
            wheelFL.brakeTorque = 0;
            wheelFR.brakeTorque = 0;
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    public void LerpToSteerAngle()
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

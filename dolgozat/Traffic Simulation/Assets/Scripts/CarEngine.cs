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
    public Path path;
    public float maxMotorTorque = 80f;
    public float maxBreakTorque = 500f;
    public float currentSpeed;
    public float maxSpeed = 8f;
    public float angledSensor = 90f;
    public bool isBraking = false;
    public float watafak = 0f;
    public bool laneSteering;
    public bool isInCrossRoad = false;
    public bool WillTurnRight;
    public bool WillGoStraight;
    private bool LastTurnRight = false;
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
            if (hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
            }
            else if (hit.collider.CompareTag("Car") || hit.collider.CompareTag("Bus"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                maxSpeed = hit.distance - 10f;
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
                maxSpeed = hit.distance-10f;
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
                maxSpeed = hit.distance - 10f;
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
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
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
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                }
                laneSteering = true;
            }
        }

        //angled
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angledSensor, transform.up) * transform.forward, out hit, 3))
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
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
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
            if (hit.collider.CompareTag("MultiLaneDivider") && WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = 5f;
                }
                laneSteering = true;
            }
        }
        if(Physics.Raycast(sensorStartPos,Quaternion.AngleAxis(-90f,transform.up)*transform.forward,out hit, 3))
        {
            if(hit.collider.CompareTag("MultiLaneDivider") && !WillTurnRight)
            {
                if (!isInCrossRoad)
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    targetSteerAngle = -5f;
                }
                laneSteering = true;
            }
        }
        

    }
    public void ApplySteer()
    {
        if (laneSteering && !isInCrossRoad)  return;
        Vector3 relativeVector = transform.InverseTransformPoint(path.WayPoints[currentNode]);
        float newSteer = (relativeVector.x / relativeVector.magnitude)*maxSteerAngle;
        if(laneSteering && isInCrossRoad && !LastTurnRight)
        {
            newSteer += 25f;
        }
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
        watafak = Vector3.Distance(transform.position, path.WayPoints[currentNode]);
        if (Vector3.Distance(transform.position,path.WayPoints[currentNode]) < 2f)
        {
            if (WillTurnRight)
            {
                LastTurnRight = true;
            }
            else
            {
                LastTurnRight = false;
            }
            WillTurnRight = false;
            if(currentNode == path.WayPoints.Count - 1)
            {
                Destroy(gameObject);
                return;
            }
            if(currentNode < path.WayPoints.Count - 2)
            {
                WillTurnRight = path.WillTurnRight(currentNode);
                WillGoStraight = path.WillGoStraight(currentNode);
            }
            currentNode++;
            if (WillTurnRight)
            {
                maxSteerAngle = 30f;
            }
            else
            {
                maxSteerAngle = 45f;
            }
            if(path.Edges[currentNode-1].Lanes == 2)
            {
                if (WillTurnRight)
                {
                    path.WayPoints[currentNode] = path.RightLaneWPs[currentNode];
                }
                else
                {
                    path.WayPoints[currentNode] = path.LeftLaneWPs[currentNode];
                }
            }
        }else if(Mathf.Abs(Vector3.Distance(transform.position, path.WayPoints[currentNode])) < 40.0f && !WillGoStraight)
        {
            maxSpeed = 3f;
        }
        else
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
        }
        else
        {
            carRenderer.material.mainTexture = textureNormal;
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

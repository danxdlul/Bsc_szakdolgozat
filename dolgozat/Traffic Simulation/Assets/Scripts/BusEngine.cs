using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusEngine : CarEngine
{
    public bool isWaitingAtStop = false;
    public Path reversePath;
    private Path temp;
    // Start is called before the first frame update
    private void Start()
    {
        transform.position = path.WayPoints[0];
        if (path.WayPoints.Count > 1)
        {
            transform?.LookAt(path.WayPoints[1]);
        }
        frontSensorPosition = new Vector3(0, 0.8f, 1.5f);
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

    void OnCollisionEnter(Collision collision)
    {
    }

    void DestroyMyself()
    {
        Destroy(gameObject);
    }
    private void CheckWaypointDistance()
    {
        watafak = Vector3.Distance(transform.position, path.WayPoints[currentNode]);
        if (Vector3.Distance(transform.position, path.WayPoints[currentNode]) < 2f)
        {
            WillTurnRight = true;
            if (currentNode == path.WayPoints.Count - 1)
            {
                temp = path;
                path = reversePath;
                reversePath = temp;
                currentNode = 0;
                transform.position = path.WayPoints[0];
                transform.LookAt(path.WayPoints[1]);
            }
            if (path.WillTurnRight(currentNode))
            {
                maxSteerAngle = 30f;
            }
            else
            {
                maxSteerAngle = 45f;
            }
            WillGoStraight = path.WillGoStraight(currentNode);
            currentNode++;
            if(path.RightLaneWPs[currentNode] != Vector3.zero)
            {
                Debug.Log(path.WayPoints[currentNode]);
                Debug.Log(path.RightLaneWPs[currentNode]);
                path.WayPoints[currentNode] = path.RightLaneWPs[currentNode];
            }
        }
        else if (Mathf.Abs(Vector3.Distance(transform.position, path.WayPoints[currentNode])) < 15.0f && !WillGoStraight)
        {
            maxSpeed = 20f;
        }
        else if(isWaitingAtStop)
        {
            maxSpeed = 0f;
        }
        else
        {
            maxSpeed = 50f;
        }
    }
}

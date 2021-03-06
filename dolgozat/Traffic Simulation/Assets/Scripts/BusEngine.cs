﻿using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusEngine : CarEngine
{
    public bool isWaitingAtStop = false;
    public Path reversePath;
    private List<Edge> test = new List<Edge>();
    private Path temp;
    // Start is called before the first frame update
    private void Start()
    {
        transform.position = path.WayPoints[0];
        if (path.WayPoints.Count > 1)
        {
            transform?.LookAt(path.WayPoints[1]);
        }
        frontSensorPosition = new Vector3(0, 0.8f, 2.8f);
    }
    public void setPaths(Path normal,Path reverse)
    {
        this.path = normal;
        this.reversePath = reverse;
        
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
        if (Vector3.Distance(transform.position, path.WayPoints[currentWayPoint]) < 3.5f)
        {
            WillTurnRight = true;
            
            WillGoStraight = path.WillGoStraight(currentNode);
            currentWayPoint++;
            if (!isInCrossRoad)
            {
                currentNode++;
            }
            if (currentNode == path.Nodes.Count)
            {
                temp = path;
                path = reversePath;
                reversePath = temp;
                currentNode = 1;
                currentWayPoint = 1;
                transform.position = path.WayPoints[0];
                transform.LookAt(path.WayPoints[1]);
                return;
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

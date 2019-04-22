using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 200f;
    public float scrollspeed = 20f;
    public float minY = 20f;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w"))
        {
            pos.z += panSpeed * pos.y/10 * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.z -= panSpeed * pos.y/10 * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * pos.y/10 * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * pos.y/10 * Time.deltaTime;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        pos.y -= scroll * scrollspeed* 400f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, 1000f);
        transform.position = pos;
    }
}

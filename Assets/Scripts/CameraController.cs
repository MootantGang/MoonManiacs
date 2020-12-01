using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraController : NetworkBehaviour
{
    /*float velX = 2f;
    float velY = 2f;

    float yaw;
    float pitch;*/

    public float cameraSpeed = 1;
    public float lookUpMax = 15f;
    public float lookDownMax = -15f;
    private Quaternion camRotation;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        /*camRotation.x -= InputManager.VerticalRotation()*cameraSpeed;
        camRotation.y += InputManager.HorizontalRotation()*cameraSpeed;*/
        camRotation.x -= Input.GetAxis("Mouse Y")*cameraSpeed;
        camRotation.y += Input.GetAxis("Mouse X")*cameraSpeed;


        camRotation.x = Mathf.Clamp(camRotation.x, lookDownMax, lookUpMax);

        transform.localRotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);

        /*yaw += velX * Input.GetAxis("Mouse X");
        pitch -= velY * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);*/
    }
}

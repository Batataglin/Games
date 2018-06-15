using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float  minDistance = 1.0f,
                                    maxDistance = 5.0f,
                                    smooth = 10.0f,
                                    distance = 0.0f,
                                    camYaw, camPitch;

    [SerializeField] private float camSpeedH, camSpeedV;

    private Vector3 dollyDirection;
    private GameObject cameraPivot;

    void Awake()
    {
        cameraPivot = cameraPivot == null ? GameObject.Find("CameraPivot") : null;

        camYaw = camPitch = 0.0f;
        camSpeedH = 200.0f;
        camSpeedV = 50.0f;

        dollyDirection = transform.localPosition.normalized;
        distance = transform.position.magnitude;
    }

    void FixedUpdate()
    {
        CameraRotation();
        CameraCollision();
    }

    // Check colision of camera with walls
    void CameraCollision()
    {
        Vector3 newCamPosition = transform.parent.TransformPoint(dollyDirection * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, newCamPosition, out hit))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDirection * distance, Time.deltaTime * smooth);
    }

    // Rotate Camera - ORBITAL -
    private void CameraRotation()
    {
        camYaw += (camSpeedH * Input.GetAxis("Mouse X")) * Time.deltaTime;

        camPitch -= (camSpeedV * Input.GetAxis("Mouse Y")) * Time.deltaTime;

        cameraPivot.transform.eulerAngles = new Vector3(camPitch, camYaw, 0.0f);

    }

}

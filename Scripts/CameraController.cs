using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// This struct contains a pivot and a its camera
    /// </summary>
    struct CameraList
    {
        public CameraList(GameObject _cameraPivot, Camera _cam)
        {
            CameraPivot = _cameraPivot;
            Cam = _cam;
        }

        public GameObject CameraPivot { get; set; }
        public Camera Cam { get; set; }
    }

    #region Private variables

    private enum CAMERA { FirstPerson, ThirdPerson }

    // This list contains camera pivot and its camera. Its use like a short cut
    private List<CameraList> _sceneCameras = new List<CameraList>();

    private Camera activeCamera;    // Set current camera

    private int valueHorizontalAxis = 1,    // This value is for check if the axis is inveted
                valueVerticalAxis = 1;      // This value is for check if the axis is inveted


    //[SerializeField]
    private float  yaw = 0.0f,             // Current camera yaw
                    pitch = 0.0f;           // Current camera pitch


    // Camera limits
    //[SerializeField]
    private float   minPitch = -80f,    // Minimum camera pitch
                    maxPitch = 80f,     // Maximum camera pitch
                    minYaw = -40f,      // Minimum camera yaw
                    maxYaw = 40f,       // Maximum camera yaw
                    startYaw,           // Store current yaw
                    startPitch;         // Store current pitch

    private Quaternion startCameraRotation; // Store the camera rotation 

    private bool getYaw, restoreCameraRotation;

    // TPP Camera colision
    private float minDistance = 1.0f,
                    maxDistance = 5.0f,
                    smooth = 10.0f,
                    distance = 0.0f;


    private Vector3 dollyDirection;

    #endregion

    #region Public variables

    public float    horizontalSensitive,
                    verticalSensitive,
                    FOV;

    public bool invertHorizontalAxis,
                invertVerticalAxis;

    /// <summary>
    /// Be sure that player body is in a different layer 
    /// </summary>
    public LayerMask layerMask;

    #endregion

    private void Awake()
    {
        List<GameObject> camPivots = new List<GameObject>();

        // Find the game objects / this can be transformed in public and placed by inspector
        camPivots.Add(GameObject.Find("PivotFirstPersonCamera"));
        camPivots.Add(GameObject.Find("PivotThirdPersonCamera"));


        for (int i = 0; i < camPivots.Count; i++)
        {
            var auxCamera = camPivots[i].GetComponentInChildren<Camera>();

            // Add cameras attached to a gameobeject to list of cameras
            _sceneCameras.Add(new CameraList(camPivots[i], auxCamera));
        }

        horizontalSensitive = 2.0f;
        verticalSensitive = 2.0f;
        FOV = 60.0f;
    }

    // Use this for initialization
    private void Start()
    {
        activeCamera = _sceneCameras[(int)CAMERA.FirstPerson].Cam;        // Set active camera - must be the inverse camera to change
        CameraChange();

        dollyDirection = activeCamera.transform.localPosition.normalized;
        distance = activeCamera.transform.position.magnitude;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // When player release the key reset variables
        if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            restoreCameraRotation = true;
        }

        // When press the key get current camera pivot rotation
        if (Input.GetKeyDown(KeyCode.LeftAlt))    
        {
            startYaw = yaw;
            startPitch = pitch;
            startCameraRotation = activeCamera.transform.parent.rotation;
        }

        // Press 'L-ALT' to Free look
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            FreeLook();
        }
        else         
        {
            if(restoreCameraRotation)
            {
                activeCamera.transform.parent.rotation = Quaternion.Lerp(activeCamera.transform.parent.rotation, startCameraRotation, 10 * Time.deltaTime);
                if(activeCamera.transform.parent.rotation == startCameraRotation)
                {
                    yaw = startYaw;
                    pitch = startPitch;
                    restoreCameraRotation = false;
                }
            }
            else
            {
                PlayerTowardCamera(); // Player rotate with mouse movement  
            }
        }

        // This function must be call in menu event
        // Invert the camera axis
        if (Input.GetKeyDown(KeyCode.I))
        {
            CheckAxisSide();
        }

        // Press 'V' to change camera in game
        if (Input.GetKeyDown(KeyCode.V))
        {
            CameraChange();
        }

        if(activeCamera == _sceneCameras[(int)CAMERA.ThirdPerson].Cam)
        {
            DetectCollision();
        }
    }

    /// <summary>
    /// Check colision with objects between Camera and Player and adjust the camera
    /// </summary>
    void DetectCollision()
    {
        Vector3 newCamPosition = activeCamera.transform.parent.TransformPoint(dollyDirection * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(activeCamera.transform.parent.position, newCamPosition, out hit, layerMask.value)) 
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        activeCamera.transform.localPosition = Vector3.Lerp(activeCamera.transform.localPosition, dollyDirection * distance, Time.deltaTime * smooth);
    }

    /// <summary>
    /// Change the active camera
    /// </summary>
    private void CameraChange()
    {
        // Set the first person camera as active
        if (activeCamera == _sceneCameras[(int)CAMERA.ThirdPerson].Cam)
        {
            _sceneCameras[(int)CAMERA.FirstPerson].CameraPivot.SetActive(true);
            _sceneCameras[(int)CAMERA.ThirdPerson].CameraPivot.SetActive(false);
            activeCamera = _sceneCameras[(int)CAMERA.FirstPerson].Cam;
        }
        else
        // Set the third person camera as active
        if (activeCamera == _sceneCameras[(int)CAMERA.FirstPerson].Cam)
        {
            _sceneCameras[(int)CAMERA.ThirdPerson].CameraPivot.SetActive(true);
            _sceneCameras[(int)CAMERA.FirstPerson].CameraPivot.SetActive(false);
            activeCamera = _sceneCameras[(int)CAMERA.ThirdPerson].Cam;
        }
    }

    /// <summary>
    /// Make player rotate toward camera is looking in horizontal
    /// </summary>
    private void PlayerTowardCamera()
    {
        GetMousePosition();

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);   // New player rotation
        activeCamera.transform.parent.eulerAngles = NewCameraPosition();
    }

    /// <summary>
    ///  Invert the camera axis
    /// </summary>
    private void CheckAxisSide()
    {
        if (invertHorizontalAxis)
        {
            valueHorizontalAxis = 1;
            invertHorizontalAxis = false;
        }
        else
        {
            valueHorizontalAxis = -1;
            invertHorizontalAxis = true;
        }

        if (invertVerticalAxis)
        {
            valueVerticalAxis = 1;
            invertVerticalAxis = false;
        }
        else
        {
            valueVerticalAxis = -1;
            invertVerticalAxis = true;
        }
    }

    /// <summary>
    /// Get the mouse position in your screen
    /// </summary>
    private void GetMousePosition()
    {
        yaw += (horizontalSensitive * valueHorizontalAxis) * Input.GetAxis("Mouse X");
        pitch -= (verticalSensitive * valueVerticalAxis) * Input.GetAxis("Mouse Y");

        // Limits the rotation of vertical axis
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

    }

    /// <summary>
    /// New position of camera base on mouse position on screen
    /// </summary>
    /// <returns> New Vector3 camera position </returns>
    private Vector3 NewCameraPosition()
    {
        return new Vector3(pitch, yaw, 0.0f);
    }

    /// <summary>
    /// Free rotation around campivot
    /// </summary>
    private void FreeLook()
    {
        GetMousePosition();

        if (activeCamera == _sceneCameras[(int)CAMERA.FirstPerson].Cam)
        {
            if (!getYaw)
                GetCurrYaw();

            yaw = Mathf.Clamp(yaw, startYaw - maxYaw, startYaw + maxYaw);
        }

        // Get the active camere search for his pivot and rotate it
        activeCamera.transform.parent.transform.eulerAngles = NewCameraPosition();
    }

    /// <summary>
    /// Take the current yaw to limit head rotation in first person perspective
    /// </summary>
    private void GetCurrYaw()
    {
        startYaw = yaw;

        getYaw = true;
    }

    /// <summary>
    /// Get the current camera position
    /// </summary>
    private void GetStartCameraPosition()
    {
        startCameraRotation = activeCamera.transform.parent.rotation;
        restoreCameraRotation = false;
    }
}

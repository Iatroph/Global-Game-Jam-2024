using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    [Header("References")]
    public Camera playerCam;
    public Transform orientation;

    [Header("Settings")]
    public float sensX = 1;
    public float sensY = 1;
    public float sensitivity = 1;
    public bool useIndividualSensitivity = false;
    public bool allowMouseInput = true;

    float xRotation;
    float yRotation;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateCameraRotation();
        if (allowMouseInput)
        {
            playerCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, playerCam.transform.rotation.z);
            orientation.localRotation = Quaternion.Euler(0, yRotation,0);
        }

    }

    private void CalculateCameraRotation()
    {
        float mouseX = 0;
        float mouseY = 0;

        if(useIndividualSensitivity)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * sensX;
            mouseY = Input.GetAxisRaw("Mouse Y") * sensY;
        }
        else
        {
            mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;
        }

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

    }
}

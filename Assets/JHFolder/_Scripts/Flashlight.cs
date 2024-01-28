using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("References")]
    public Camera playerCam;
    private Light fLight;

    [Header("Debug")]
    private bool debug;
    private bool flashLightToggle;


    private float distanceFromRayCastHit;


    private void Awake()
    {
        fLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fLight.enabled = !fLight.enabled;
        }

        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, 20))
        {
            if(hit.collider != null)
            {
                distanceFromRayCastHit = Vector3.Distance(hit.point, playerCam.transform.position);
                //Debug.DrawLine(playerCam.transform.position, hit.point, Color.red, 10);
                fLight.intensity = distanceFromRayCastHit / 2;
            }
        }
        else
        {
            distanceFromRayCastHit = 0;
            fLight.intensity = 5;
        }

        //Debug.Log(distanceFromRayCastHit);



    }
}

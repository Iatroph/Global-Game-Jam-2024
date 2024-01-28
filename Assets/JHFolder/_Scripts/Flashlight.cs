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

    public float intensityChange;

    private AudioSource source;
    private float distanceFromRayCastHit;

    public AudioClip onSound;
    public AudioClip offSound;


    private void Awake()
    {
        fLight = GetComponent<Light>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fLight.enabled = !fLight.enabled;
            if(fLight.enabled)
            {
                source.PlayOneShot(onSound);
            }
            else
            {
                source.PlayOneShot(offSound);
            }
        }

        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, 20))
        {
            if(hit.collider != null)
            {
                distanceFromRayCastHit = Vector3.Distance(hit.point, playerCam.transform.position);
                //Debug.DrawLine(playerCam.transform.position, hit.point, Color.red, 10);
                //fLight.intensity = distanceFromRayCastHit / 2;
                fLight.intensity = Mathf.Lerp(fLight.intensity, distanceFromRayCastHit / 2, intensityChange * Time.deltaTime);
            }
        }
        else
        {
            distanceFromRayCastHit = 0;
            //fLight.intensity = 10;
            fLight.intensity = Mathf.Lerp(fLight.intensity, 10, intensityChange * Time.deltaTime);

        }

        //Debug.Log(distanceFromRayCastHit);
        //Debug.Log(fLight.intensity);



    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    public Transform Door1;
    public Transform Door2;
    public Transform Door3;

    public string tagOfObject;

    [SerializeField]private bool canEnter;



    void FixedUpdate()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 3, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Doors>()) //Or get like the tag or something
            {
                TeleportToDoor();
            }
        }
        
    }

    void TeleportToDoor()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {

        }
    }
}

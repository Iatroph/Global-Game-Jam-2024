using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour
{
    [Header("Parameters")]
    public float laughterBuildUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().FadeLaughterOverlay(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().Laughter(laughterBuildUp);
            other.gameObject.GetComponent<Player>().isInGas = true;
            //other.gameObject.GetComponent<Player>().canHoldBreath = false;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().FadeLaughterOverlay(false);
            other.gameObject.GetComponent<Player>().isInGas= false;
            //other.gameObject.GetComponent<Player>().canHoldBreath = true;


        }
    }
}

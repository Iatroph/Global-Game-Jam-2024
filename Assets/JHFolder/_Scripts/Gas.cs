using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour
{
    [Header("Parameters")]
    public float laughterBuildUp;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().Laughter(laughterBuildUp);
        }
    }
}

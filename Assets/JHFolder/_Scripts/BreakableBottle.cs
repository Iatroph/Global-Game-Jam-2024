using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBottle : MonoBehaviour
{
    private Rigidbody rb;

    public bool isThrown = false;

    public GameObject bottleBreakingSound;

    public GameObject bottleBreakParticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.AddTorque(transform.right * 10);
    }

    public void ThrowBottle(Vector3 dir, float throwPower)
    {
        rb.AddForce(dir * throwPower, ForceMode.Impulse);
        isThrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isThrown && collision.transform.tag != "Player")
        {
            ContactPoint contact = collision.contacts[0];
            GameObject sound = Instantiate(bottleBreakingSound, contact.point, Quaternion.identity);
            GameObject particle = Instantiate(bottleBreakParticle, contact.point, Quaternion.identity);
            Destroy(sound, 1f);
            Destroy(gameObject);
        }
    }


}

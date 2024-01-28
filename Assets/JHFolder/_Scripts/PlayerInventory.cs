using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    public Camera playerCam;
    public Transform heldItemPosition;
    public GameObject throwingBottle;
    public GameObject heldBottle;

    [Header("UI References")]
    public TMP_Text itemHoverText;
    public TMP_Text keysText;

    [Header("Parameters")]
    public float PickUpRange = 5;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip keyPickUpSFX;
    public AudioClip bottlePickUpSFX;

    [Header("Keycodes")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Layermask")]
    public LayerMask whatIsInteractable;

    [Header("Debug")]
    public bool debug;
    public bool hasBottle;
    [SerializeField] private int keysOwned;

    // Start is called before the first frame update
    void Start()
    {
        itemHoverText.gameObject.SetActive(false);
        heldBottle.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0) && hasBottle)
        {
            heldBottle.gameObject.SetActive(false);
            GameObject bottle = Instantiate(throwingBottle, heldItemPosition.position, playerCam.transform.rotation);
            bottle.GetComponent<BreakableBottle>().ThrowBottle(playerCam.transform.forward, 10);
            hasBottle = false;
        }


        if (Input.GetKeyDown(interactKey))
        {
            PickUpRaycast();
        }

        HoverRaycast();
    }

    private void HoverRaycast()
    {
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, PickUpRange, whatIsInteractable))
        {
            if(hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    itemHoverText.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            itemHoverText.gameObject.SetActive(false);
        }

    }

    private void PickUpRaycast()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, PickUpRange, whatIsInteractable))
        {
            if (hit.collider != null)
            {
                //IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                //if (interactable != null)
                //{
                //    interactable.Interact();
                //}

                if (hit.collider.GetComponent<Key>())
                {
                    keysOwned++;
                    if(keysOwned == 3)
                    {
                        keysText.color = Color.red;
                            
                    }
                    audioSource.PlayOneShot(keyPickUpSFX, 1);
                    keysText.text = "x " + keysOwned;
                    Destroy(hit.transform.gameObject);
                }

                if (hit.collider.GetComponent<BottleItem>())
                {
                    if(!hasBottle)
                    {
                        audioSource.PlayOneShot(bottlePickUpSFX, 1);
                        hasBottle = true;
                        heldBottle.gameObject.SetActive(true);
                        Destroy(hit.transform.gameObject);

                    }
                }
            }
        }

    }
}

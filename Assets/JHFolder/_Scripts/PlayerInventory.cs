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
    public TMP_Text fadeText;
    public GameObject escapedImage;
    public GameObject deathImage;

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

    bool isTextFading = false;

    // Start is called before the first frame update
    void Start()
    {
        itemHoverText.gameObject.SetActive(false);
        heldBottle.gameObject.SetActive(false);
        escapedImage.gameObject.SetActive(false);
        fadeText.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0) && hasBottle)
        {
            heldBottle.gameObject.SetActive(false);
            GameObject bottle = Instantiate(throwingBottle, heldItemPosition.position, playerCam.transform.rotation);
            bottle.GetComponent<BreakableBottle>().ThrowBottle(playerCam.transform.forward, 12);
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
                //IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                //if (interactable != null)
                //{
                //    itemHoverText.gameObject.SetActive(true);
                //}
                //Debug.Log(hit.collider.name);
                if (hit.collider.GetComponent<Key>())
                {
                    itemHoverText.gameObject.SetActive(true);
                    itemHoverText.text = "<color=\"red\">E</color>: Pick Up";
                }

                if (hit.collider.GetComponent<BottleItem>())
                {
                    itemHoverText.gameObject.SetActive(true);
                    itemHoverText.text = "<color=\"red\">E</color>: Pick Up";
                }

                if (hit.collider.GetComponent<EscapeDoor>())
                {
                    itemHoverText.gameObject.SetActive(true);
                    itemHoverText.text = "<color=\"red\">E</color>: Open";
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

                if (hit.collider.GetComponent<EscapeDoor>())
                {
                    if(keysOwned < 3)
                    {
                        if(!isTextFading)
                        StartCoroutine(TextFade(fadeText, 2));
                    }
                    else if(keysOwned >= 3)
                    {
                        escapedImage.SetActive(true);
                        Time.timeScale = 0;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                }
            }
        }

    }

    public IEnumerator TextFade(TMP_Text image, float duration)
    {
        isTextFading = true;
        float elapsedTime = 0;
        float startValue = image.color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 1, elapsedTime / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;

        }

        yield return new WaitForSeconds(3.5f);

        elapsedTime = 0;
        startValue = image.color.a;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 0, elapsedTime / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
        isTextFading = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<EnemyMovement>() || collision.transform.tag == "Enemy")
        {
            deathImage.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

        Debug.Log(collision.transform.name);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.GetComponent<EnemyMovement>() || hit.transform.tag == "Enemy")
        {
            deathImage.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
}

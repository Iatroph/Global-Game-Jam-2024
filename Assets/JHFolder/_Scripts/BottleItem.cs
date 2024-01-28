using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleItem : MonoBehaviour, IInteractable
{
    public GameObject throwingBottlePrefab;

    public void Interact()
    {
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private bool _CanBePickedUp = true;

    public void Pickup(GameObject playerGO)
    {
        float height = transform.position.y + 1.0f;

        transform.parent = playerGO.transform;
        transform.rotation = playerGO.GetComponent<PlayerController>().GetController().transform.rotation;
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 90);

        transform.position = playerGO.transform.position;
        transform.position += playerGO.GetComponent<PlayerController>().GetController().transform.forward;
        transform.position += new Vector3(0.0f, height, 0.0f);
    }

    public bool CanBePickedUp() { return _CanBePickedUp; }
}
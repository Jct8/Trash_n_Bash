using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f);
        transform.localScale += new Vector3(0.0f, 5.0f, 0.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    private IEnumerator Jump()
    {
        rb.AddForce(new Vector3(0.0f, 10.0f, 0.0f), ForceMode.Impulse);
        yield return new WaitForSeconds(2.0f);
        rb.isKinematic = true;
    }
}

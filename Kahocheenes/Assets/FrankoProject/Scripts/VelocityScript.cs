using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityScript : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody rb;
    public Vector3 Velocity;
    public Vector3 AngularVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = rb.velocity;
        AngularVelocity = rb.angularVelocity;
        float fire1 = Input.GetAxis("Fire1");
        if (fire1 > 0)
        {
            
        }
    }
}

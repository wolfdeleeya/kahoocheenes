using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbaRayCast : MonoBehaviour
{
    // Start is called before the first frame update

        public float wheelRadius;
    public float suspensionRadius; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, -transform.up * (wheelRadius), Color.red);
    }
}

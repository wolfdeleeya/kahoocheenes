using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWheel : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Springs")]
    public float restLength;
    public float springTravel;
    public float wheelRadius;
    public float springStiff;
    public float damperStiff;
    public bool Rest = true;

    private float springForce;
    private float damperForce;
    private Vector3 suspensionForce;
    private float springLength;
    private float lastLength;
    private float minLength;
    private float maxLength;
    private float springVelocity;
    public bool isGrounded = false;

    private Rigidbody rb;
    void Start()
    {
       rb =  transform.root.GetComponent<Rigidbody>();
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    private void Update()
    {
        if (Rest)
        {
            Debug.DrawRay(transform.position, -transform.up * (restLength + wheelRadius), Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.up * (maxLength + wheelRadius), Color.green);
        }
       
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, wheelRadius + maxLength)) {

            isGrounded = true;
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiff * (restLength - springLength);
            damperForce = damperStiff * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;

            

            rb.AddForceAtPosition( suspensionForce , hit.point);

        }
        else
        {
            isGrounded = false;
        }
    }
}

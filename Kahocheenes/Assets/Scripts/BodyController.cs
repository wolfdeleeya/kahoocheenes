using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Stabilizator")] public float stability = 0.3f;
    public float speed = 2.0f;
    public bool isGrounded = false;
    public Transform CenterOfMass;

    [Header("Wheels")] public RayCastWheel[] wheels;

    [Header("velocity")] public float VelocityMag;
    public bool slowStart;
    public float forceFactor;
    public float maxVelocity = 30;
    public float minVelocity = -10;
    public float Torque;

    [Header("All Drag")] public float Drag;
    public float AngularDragY;
    public float driftPercent;

    [SerializeField] private bool getInputFromKeyboard;
    
    private Rigidbody rb;
    private float deltaTime;


    private float GravityPercent;
    private float forwardAcceleration;
    private Vector3 some;
    private float XRotate;
    private Transform _transform;

    public int ForwardInput { get; set; }
    public int SideInput { get; set; }

    private void Awake()
    {
        _transform = transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CenterOfMass.localPosition;
    }

    private void Update()
    {
        //check if Car is Grounded

        int sum = 0;
        foreach (RayCastWheel wheel in wheels)
        {
            if (wheel.isGrounded)
            {
                sum += 1;
            }
        }

        if (sum < 2)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }

        Debug.Log(sum);
    }


    private float CalcLineVel(float Time)
    {
        return 5 * Time;
    }

    private float CalcLineTime(float Vel)
    {
        return Vel / 5;
    }

    private float CalcFastVel(float Time)
    {
        return Mathf.Log(Time * forceFactor + 1, 1.07f);
    }

    private float CalcFastTime(float Vel)
    {
        return (Mathf.Pow(1.07f, Vel) - 1) / forceFactor;
    }

    private float CalcSlowVel(float Time)
    {
        return Mathf.Pow(2, Time * 1.5f) - 1;
    }

    private float CalcSlowTime(float Vel)
    {
        return Mathf.Log(Vel + 1, 2) / 1.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Proba Stabilizator
        var up = _transform.up;
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
            rb.angularVelocity
        ) * up;
        //Debug.DrawRay(new Vector3(rb.position.x, rb.position.y - 0.5f, rb.position.z), predictedUp, Color.black);
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        //Debug.DrawRay(new Vector3(rb.position.x, rb.position.y - 0.5f, rb.position.z), Vector3.up, Color.blue);
        Debug.DrawRay(new Vector3(rb.position.x, rb.position.y + 4, rb.position.z), torqueVector * speed * speed,
            Color.red);


        //rb.AddForceAtPosition(Force * input * Vector3.ProjectOnPlane(transform.forward, plane.up).normalized, new Vector3 (rb.position.x, rb.position.y,rb.position.z));

        // some = Force * input * Vector3.ProjectOnPlane(transform.forward, plane.up).normalized;
        // Debug.DrawRay(new Vector3(rb.position.x, rb.position.y, rb.position.z), some, Color.blue);


        //turn Force
        if(getInputFromKeyboard)
            SideInput = (int)Input.GetAxis("Horizontal");

        rb.AddTorque(up * (SideInput * Torque), ForceMode.VelocityChange);

        // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turn * Torque * Time.deltaTime, 0f));


        //Drift

        //angularDrag

        Vector3 locVelAng = _transform.InverseTransformDirection(rb.angularVelocity);
        locVelAng.y *= 1.0f - AngularDragY;

        rb.angularVelocity = _transform.TransformDirection(locVelAng);

        //if we dont press gas or reverse or we do when we want to slow down we activate drag
        Vector3 locVel = _transform.InverseTransformDirection(rb.velocity);

        if (isGrounded && (ForwardInput == 0 || locVel.z * ForwardInput < 0))
        {
            rb.drag = Drag;
        }
        else
        {
            rb.drag = 0;
        }

        if (isGrounded)
        {
            //forward and backward force
            if(getInputFromKeyboard)
                ForwardInput = (int)Input.GetAxis("Vertical");

            //calculate forward and backward Force

            if (ForwardInput > 0)
            {
                VelocityMag = rb.velocity.magnitude;
                if (VelocityMag < maxVelocity)
                {
                    float time = slowStart ? CalcSlowTime(VelocityMag) : CalcFastTime(VelocityMag);

                    deltaTime = Time.fixedDeltaTime;

                    float NextVelocityMag = slowStart ? CalcSlowVel(time + deltaTime) : CalcFastVel(time + deltaTime);

                    float acceleration = (NextVelocityMag - VelocityMag) / deltaTime;

                    float ForceTrue = rb.mass * acceleration;
                    Debug.Log("VelocityMag:" + VelocityMag + "Time: " + time + "DeltaTime:" + deltaTime +
                              "\nNextVelocityMag:" + NextVelocityMag + "acceleration:" + acceleration + "ForceTrue:" +
                              ForceTrue);
                    var rbPos = rb.position;
                    rb.AddForceAtPosition(ForceTrue * _transform.forward.normalized,
                        new Vector3(rbPos.x, rbPos.y, rbPos.z));
                }
            }
            else if (ForwardInput < 0)
            {
                VelocityMag = rb.velocity.magnitude;
                if (VelocityMag > minVelocity)
                {
                    float time = CalcLineTime(VelocityMag);

                    deltaTime = Time.fixedDeltaTime;
                    float NextVelocityMag = CalcLineVel(time - deltaTime);

                    float acceleration = (NextVelocityMag - VelocityMag) / deltaTime;

                    float ForceTrue = rb.mass * acceleration;
                    Debug.Log("VelocityMag:" + VelocityMag + "Time: " + time + "DeltaTime:" + deltaTime +
                              "\nNextVelocityMag:" + NextVelocityMag + "acceleration:" + acceleration + "ForceTrue:" +
                              ForceTrue);
                    var rbPos = rb.position;
                    rb.AddForceAtPosition(ForceTrue * _transform.forward.normalized,
                        new Vector3(rbPos.x, rbPos.y, rbPos.z));
                }
            }

            //drift force

            locVel = _transform.InverseTransformDirection(rb.velocity);

            var right = _transform.right;
            rb.AddForceAtPosition(locVel.x * -right * driftPercent,
                new Vector3(rb.position.x, rb.position.y, rb.position.z));
            var position = rb.position;
            Debug.DrawRay(new Vector3(position.x, position.y, position.z),
                locVel.x * -right * driftPercent, Color.red);

            //normal drag


            rb.AddForceAtPosition(locVel.z * _transform.forward * -GravityPercent,
                new Vector3(rb.position.x, rb.position.y, rb.position.z));
            Debug.DrawRay(new Vector3(rb.position.x, rb.position.y, rb.position.z),
                locVel.z * _transform.forward * -GravityPercent, Color.red);
        }
        else if (!isGrounded) //&& (transform.rotation.x > 0.10f || transform.rotation.x < -0.10f))
        {
            // Update is called once per frame

            predictedUp = Quaternion.AngleAxis(
                rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
                rb.angularVelocity
            ) * _transform.up;
            torqueVector = Vector3.Cross(predictedUp, Vector3.up);
            rb.AddTorque(torqueVector * speed * speed);

            //stop over rotate when jump 
            XRotate = _transform.rotation.x;

            //rb.AddTorque(transform.right * -rb.angularVelocity.x * 1f, ForceMode.VelocityChange);
            //transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.rotation.x, -15, 15), transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
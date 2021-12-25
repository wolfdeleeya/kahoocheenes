using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotationAnimator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void LateUpdate()
    {
        _transform.rotation *= Quaternion.Euler(Vector3.up * (rotationSpeed * Time.deltaTime));
    }
}
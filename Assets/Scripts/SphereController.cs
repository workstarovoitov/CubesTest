using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SphereController : MonoBehaviour
{
    [SerializeField] private Vector3 angularSpeed = Vector3.zero;
    private Vector3 oldSpeed = Vector3.zero;
    
    public static event Action<Vector3> OnSpeedUpdate;

    // Update is called once per frame
    void Update()
    {
        if (oldSpeed != angularSpeed)
        {
            GetComponent<Rigidbody>().angularVelocity = angularSpeed;
            oldSpeed = angularSpeed;
            OnSpeedUpdate?.Invoke(angularSpeed);
        }
    }
}

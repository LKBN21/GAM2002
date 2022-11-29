using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carcontroller : MonoBehaviour
{
    public bool isLocked;
    [Header("Wheel Colliders")]
    public WheelCollider frontWheelLeft;
    public WheelCollider frontWheelRight;
    public WheelCollider rearWheelRight;
    public WheelCollider rearWheelLeft;
    [Header("Steer settings")]
    public float steerMax = 30f;
    public float accelMax = 5000f;
    public float brakeMAx = 5000f;
    [System.NonSerialized]
    public float steer = 0f;
    [System.NonSerialized]
    public float motor = 0f;
    [System.NonSerialized]
    public float brake = 0f;

    [Space]
    public bool fakeBrake;
    public float fakeBrakeDivider = 0.95f;

    [Space]
    public bool turnHelp;
    public float turnHelpAmount = 10f;
    public bool isGrounded;
    [System.NonSerialized]
    public float mySpeed;

    [System.NonSerialized]
    public Vector3 velo;


    public virtual void Start()
    {
        Init();
    }
    public virtual void Init()
    {
        

    }
}

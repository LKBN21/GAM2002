using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARSTeering2 : MonoBehaviour
{
    public Transform path;
    public float AngleSpeed = 5f;
    private List<Transform> nodes;
    private int currentNode = 0;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public float maxMotorTongue = 80f;
    public float currentspeed;
    public float maxSpeed = 100f;
    public float brakingspeed = 2f;
    public float FullyStopped = 1f;
    public Vector3 centerOfmass;
    public float sensorLenght = 5f;
    public Vector3 MiddleFrontSensor = new Vector3(0f, 0.5f, 3f);
    public float frontsensorposition = 0.2f;
    public float frontsensorangle = 35;
    private bool avoiding = false;
    private float targetSteerangle = 0;
    public float brakingoncorner = 200f;
    public float turningspeed = 30f;
    public Vector3 velo;
    public Rigidbody _RB;
    public float Accelaration = 5000f;
    public float motor = 100f;
    [Space]
    public bool turnHelp;
    public float turnHelpAmount = 10f;
    public float turnAngleMinimum;

    public float fakeBrakeDivider = 10f;
    [System.NonSerialized]
    public float mySpeed;

    
    

    // Start is called before the first frame update
    void Start()
    {
        _RB = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = centerOfmass;
        //Debug.Log(GetComponent<Rigidbody>().centerOfMass);
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        ApplySteer();
        Drive();
        Chechwaypointdistance();
        sensor();
        LerpToSteeringAngle();

    }
    private void ApplySteer()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        relativeVector = relativeVector / relativeVector.magnitude;
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerangle = newSteer;

    }
    private void Drive()
    {
        velo = _RB.velocity;
            velo = transform.InverseTransformDirection(_RB.velocity);
        mySpeed = velo.z;

 

        if (turnHelp && Mathf.Abs(targetSteerangle) > turnAngleMinimum)
        {
            
            
                _RB.AddTorque(Vector3.up * targetSteerangle * turnHelpAmount * _RB.mass);
            
            

         
            
        }
        
        
        currentspeed = Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;
        if (currentspeed < maxSpeed)
        {
            wheelFL.motorTorque = maxMotorTongue;
            wheelFR.motorTorque = maxMotorTongue;
            wheelBL.motorTorque = maxMotorTongue;
            wheelBR.motorTorque = maxMotorTongue;
        }
        else
        {
            wheelFL.motorTorque = 0f;
            wheelFR.motorTorque = 0f;
            wheelBL.motorTorque = 0f;
            wheelBR.motorTorque = 0f;
        }

        








    }
    private void Chechwaypointdistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 2f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }

    }
    private void sensor()
    {
        RaycastHit hit;
        Vector3 sensorStartingpos = transform.position;
        sensorStartingpos += transform.forward * MiddleFrontSensor.z;
        sensorStartingpos += transform.up * MiddleFrontSensor.y;
        float avoidMultiplier = 0;
        avoiding = false;



        sensorStartingpos += transform.right * frontsensorposition;

        if (Physics.Raycast(sensorStartingpos, transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("AccPoint"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }
        }
        else if (Physics.Raycast(sensorStartingpos, Quaternion.AngleAxis(frontsensorangle, transform.up) * transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("AccPoint"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
             if (hit.collider.CompareTag("BrakePoint"))
            {
                wheelFL.brakeTorque = 300f;
                wheelFR.brakeTorque = 300f;
                avoiding = false;
            }
             if (hit.collider.CompareTag("AccPoint"))
            {
                wheelFL.brakeTorque = 0f;
                wheelFR.brakeTorque = 0f;
                avoiding = false;
            }
        }
        sensorStartingpos += -2 * transform.right * frontsensorposition;

        if (Physics.Raycast(sensorStartingpos, transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("AccPoint"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }
        else if (Physics.Raycast(sensorStartingpos, Quaternion.AngleAxis(-frontsensorangle, transform.up) * transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("AccPoint"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier = 0.5f;
            }
             if (hit.collider.CompareTag(("BrakePoint")))
            {
                wheelFL.brakeTorque = 300f;
                wheelFR.brakeTorque = 300f;
                avoiding = false;

            }
             if (hit.collider.CompareTag(("AccPoint")))
            {
                wheelFL.brakeTorque = 0f;
                wheelFR.brakeTorque = 0f;
                avoiding = false;
            }
        }

        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartingpos, transform.forward, out hit, sensorLenght))
            {
                if (!hit.collider.CompareTag("AccPoint"))
                {

                    Debug.DrawLine(sensorStartingpos, hit.point);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = 1;
                    }
                    else
                    {
                        avoidMultiplier = -1;
                    }
                }
            }
        }
        if (avoiding)
        {
            targetSteerangle = maxSteerAngle * avoidMultiplier;

        }



    }
    private void LerpToSteeringAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerangle, Time.deltaTime * AngleSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerangle, Time.deltaTime * AngleSpeed);
    }
}
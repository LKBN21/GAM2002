using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStering : MonoBehaviour
{
    public Transform path;
    public float AngleSpeed = 5f;
    private List<Transform> nodes;
    private int currentNode = 0;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    //public WheelCollider wheelBL;
    //public WheelCollider wheelBR;
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
    
   
    
    // Start is called before the first frame update
    void Start()
    {
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
        
        currentspeed = Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;
        if (currentspeed < maxSpeed)
        {
            wheelFL.motorTorque = maxMotorTongue;
            wheelFR.motorTorque = maxMotorTongue;
        }else
        {
            wheelFL.motorTorque = 0f;
            wheelFR.motorTorque = 0f;
        }
        if (currentNode >= 2)
        {
            if (currentspeed > brakingspeed)
            {
                wheelFL.brakeTorque = 100f;
                wheelFR.brakeTorque = 100f;
            }
            
            
            else
            {
                wheelFL.brakeTorque = 0f;
                wheelFR.brakeTorque = 0f;
                wheelFL.motorTorque = 0f;
                wheelFR.motorTorque = 0f;
                
            }
            
             

        }
        





    }
    private void Chechwaypointdistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
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
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }
        }
        else if (Physics.Raycast(sensorStartingpos, Quaternion.AngleAxis(frontsensorangle, transform.up) * transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }
        sensorStartingpos += -2 * transform.right * frontsensorposition;
        
        if (Physics.Raycast(sensorStartingpos, transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }
        else if (Physics.Raycast(sensorStartingpos, Quaternion.AngleAxis(-frontsensorangle, transform.up) * transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingpos, hit.point);
                avoiding = true;
                avoidMultiplier = 0.5f;
            }
        }

        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartingpos, transform.forward, out hit, sensorLenght))
            {
                if (!hit.collider.CompareTag("Terrain"))
                {

                    Debug.DrawLine(sensorStartingpos, hit.point);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = 1;
                    }else {
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

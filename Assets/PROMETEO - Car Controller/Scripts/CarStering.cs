using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStering : MonoBehaviour
{
    public Transform path;
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
    public Vector3 centerOfmass;
    public float sensorLenght = 5f;
    public float frontsensorposition = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfmass;
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

    }
    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        relativeVector = relativeVector / relativeVector.magnitude;
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
        //wheelBL.steerAngle = newSteer;
        //wheelBR.steerAngle = newSteer;
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
        sensorStartingpos.z += frontsensorposition;
        sensorStartingpos.y += frontsensorposition;
        if (Physics.Raycast(sensorStartingpos, transform.forward, out hit, sensorLenght))
        {
            Debug.DrawLine(sensorStartingpos, hit.point);
        }
        
    }
}

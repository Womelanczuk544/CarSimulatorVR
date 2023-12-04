using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }

    [SerializeField] private driveType drive;

    private InputManager IM;
    private Rigidbody rigidBody;
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] wheelMesh = new GameObject[4];
    private GameObject centerOfMass;

    public float DownForceValue = 50;
    public float KPH;
    public float motorTorque = 200;
    public float radius = 6;
    public float steeringMax = 4;
    public float brakePower = 4;

    // Start is called before the first frame update
    void Start()
    {
        getObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        addDownForce();
        animateWheels();
        moveVehicle();
        steerVehicle();
    }
    private void moveVehicle()
    {
        if(drive == driveType.frontWheelDrive)
        {
            for (int i = 0; i < wheels.Length -2; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque)/2;
            }
        }
        else if(drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque) / 2;
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque) / 4;
            }
        }
        
        KPH = rigidBody.velocity.magnitude * 3.6f;

        if (IM.handBreak)
        {
            wheels[2].brakeTorque = wheels[3].brakeTorque = brakePower;
        }
        else
        {
            wheels[2].brakeTorque = wheels[3].brakeTorque = 0;
        }

    }

    private void steerVehicle()
    {
        if(IM.horizontal >0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
        }
        else if(IM.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }
    }

    void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0; i < wheels.Length;i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation= wheelRotation;
        }
    }
    private void getObjects()
    {
        IM = GetComponent<InputManager>();
        rigidBody = GetComponent<Rigidbody>();
        centerOfMass = GameObject.Find("mass");
        rigidBody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void addDownForce() 
    {
        rigidBody.AddForce(-transform.up * DownForceValue * rigidBody.velocity.magnitude);
    }
}

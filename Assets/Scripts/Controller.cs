using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;

public class Controller : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField] private driveType drive;

    private GameObject wheelMeshes, wheelColliders;
    private InputManager IM;
    public GameManager manager;
    private Rigidbody rigidBody;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    private GameObject centerOfMass;

    [Header("Variables")]
    public float totalPower;
    public float maxRPM = 6500, minRPM;
    public float KPH;
    public float wheelsRPM;
    public float smoothTime = 0.01f;
    public float engineRPM;
    public float[] gears;
    public bool reverse = false;
    public int gearNum = 1; 
    public int isEngineRunning = 0; 
    public AnimationCurve enginePower;

    [Header("Debug")]
    public float[] slip = new float[4];

    private float radius = 4, brakePower = 50000, DownForceValue = 100f;

    // Start is called before the first frame update
    void Start()
    {
        getObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K) && isEngineRunning == 0)
        {
            Debug.Log("jo³");
            StartCoroutine(GetComponent<EngineAudion>().StartEngine());
        }
        addDownForce();
        animateWheels();
        steerVehicle();
        getObjects();
        getFriction();
        if (isEngineRunning > 0)
        {
            calculateEnginePower();
        }
        shifter();
    }

    private void calculateEnginePower()
    {
        wheelRPM();

        totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * IM.vertical;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);

        moveVehicle();
    }

    private void wheelRPM()
    {
        float sum = 0;
        int R = 0;
        for(int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0 ) ? sum / R : 0;

        if (wheelsRPM < 0 && !reverse)
        {
            reverse = true;
        }
        else if (wheelsRPM > 0 && reverse)
        {
            reverse = false;
        }
    }

    private void moveVehicle()
    {
        if(drive == driveType.frontWheelDrive)
        {
            for (int i = 0; i < wheels.Length -2; i++)
            {
                wheels[i].motorTorque = totalPower/2;
            }
        }
        else if(drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = totalPower / 2;
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = totalPower / 4;
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

    private void shifter()
    {
        //if (!isGrounded()) return;
        if (Input.GetKeyDown(KeyCode.E) && gearNum < gears.Length-1)
        {
            gearNum++;
            manager.changeGear();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q) && gearNum > 0)
        {
            gearNum--;
            manager.changeGear();
        }

    }

    private bool isGrounded()
    {
        if (wheels[0].isGrounded && wheels[1].isGrounded && wheels[2].isGrounded && wheels[3].isGrounded)
            return true;
        else
            return false;
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
        wheelColliders = GameObject.Find("WheelCollidersMain");
        wheelMeshes = GameObject.Find("WheelMeshesMain");
        wheels[0] = wheelColliders.transform.Find("_LeftFrontWheelCollider").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("_RightFrontWheelCollider").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("_LeftRearWheelCollider").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("_RightRearWheelCollider").gameObject.GetComponent<WheelCollider>();

        wheelMesh[0] = wheelMeshes.transform.Find("_LeftFrontWheel").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("_RightFrontWheel").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("_LeftRearWheel").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("_RightRearWheel").gameObject;

        centerOfMass = GameObject.Find("mass");
        rigidBody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void addDownForce() 
    {
        rigidBody.AddForce(-transform.up * DownForceValue * rigidBody.velocity.magnitude);
    }

    private void getFriction()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.forwardSlip;
        }
    }

    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(Mathf.Abs(IM.vertical), 0.5f, 1f);
        return engineRPM * gas / maxRPM;
    }
}

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
    public int gearNum = 6; 
    public int isEngineRunning = 0; 
    public AnimationCurve enginePower;
    private float smoothingFactor = 0.1f;
    private float maxSteeringAngle = 180.0f;
    public bool clutch = false;
    public Transform steeringWheel;

    [Header("Debug")]
    public float[] slip = new float[4];

    private float radius = 4, brakePower = 500000, DownForceValue = 100f;

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
            StartCoroutine(GetComponent<EngineAudio>().StartEngine());
        }
        if (isEngineRunning == 0)
        {
            GetComponent<EngineAudio>().StopEngine();
        }
        shifter();
        addDownForce();
        animateWheels();
        steerVehicle();
        getObjects();
        getFriction();
        RotateSteeringWheel(IM.horizontal);
        checkClutch();
        calculateEnginePower();
        
    }

    private void calculateEnginePower()
    {
        wheelRPM();
        if(isEngineRunning == 0) 
        {
            totalPower = 0;
            gearNum = 6;
            manager.changeGear();
        }
        else if(clutch == false)
        {
        totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * IM.vertical;
        }
        else
        {
            totalPower = 0;
        }
        float velocity = 0.0f;
        if(isEngineRunning > 0)
        {
        engineRPM = Mathf.Abs(Mathf.SmoothDamp(engineRPM, 1000 + Mathf.Abs(wheelsRPM) * 3.6f * Mathf.Abs(gears[gearNum]), ref velocity, smoothTime));
        }
        else
        {
            engineRPM = 0;
        }

        moveVehicle();
    }

    private void wheelRPM()
    {
        float sum = 0;
        int R = 0;

        // Pêtla sumuj¹ca prêdkoœci kó³
        for (int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }

        // Obliczanie wyg³adzonej wartoœci
        float currentRPM = (R != 0) ? sum / R : 0;
        wheelsRPM = Mathf.Lerp(wheelsRPM, currentRPM, smoothingFactor);

        // Sprawdzenie kierunku obrotów i ustawienie flagi
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(clutch == false)
            {
                isEngineRunning = 0;
            }
            gearNum = 1;
            manager.changeGear();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (clutch == false)
            {
                isEngineRunning = 0;
            }
            gearNum = 2;
            manager.changeGear();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (clutch == false)
            {
                isEngineRunning = 0;
            }
            gearNum = 3;
            manager.changeGear();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (clutch == false)
            {
                isEngineRunning = 0;
            }
            gearNum = 4;
            manager.changeGear();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (clutch == false)
            {
                isEngineRunning = 0;
            }
            gearNum = 5;
            manager.changeGear();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (clutch == false)
            {
                isEngineRunning = 0;
            }
            gearNum = 0;
            manager.changeGear();
            return;
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
    private void checkClutch()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            clutch = true;
        }
        else
        {
            clutch= false;
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
        steeringWheel = transform.Find("_SteeringWheel"); 
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
        return  Mathf.Abs(engineRPM * gas / maxRPM);
    }

    void RotateSteeringWheel(float rotationInput)
    {
        // Mapuj wartoœæ wejœcia liniowo na zakres od -1 do 1 na k¹t od -maxSteeringAngle do maxSteeringAngle
        float targetRotation = Mathf.Lerp(-maxSteeringAngle, maxSteeringAngle, (rotationInput + 1f) / 2f);

        // Obróæ kierownicê na podstawie docelowego k¹ta
        steeringWheel.localRotation = Quaternion.Euler(-17.5f, 180f, targetRotation);
    }
}

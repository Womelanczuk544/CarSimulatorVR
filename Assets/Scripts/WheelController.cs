using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking.Types;
using TMPro;
using System.Xml;

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
};

public class WheelController : MonoBehaviour
{
    
    public WheelColliders colliders;
    public MesheForWheels MesheForWheels;
    private Rigidbody carRB;
    public AnimationCurve steeringCurve;
    public TMP_Text rpmText;
    public TMP_Text gearText;
    public Transform rpmNeedle;
    public AnimationCurve hpToRPMCurve;
    private GearState gearState;

    public float speedClamped;
    public int currentGear;
    public int isEngineRunning;
    public float RPM;
    public float redLine;
    public float idleRPM;
    public float minNeedleRotation;
    public float maxNeedleRotation;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;  
    public float horsePower;
    public float brakePower;
    public float slipAngle;
    public float speed;
    public float sensitivity = 2f;
    public float[] gearRatios;
    public float differentialRatio;
    public float currentTorque;
    public float clutch;
    public float wheelRPM;
    public float increaseGearRPM;
    public float decreaseGearRPM;
    public float changeGearTime = 0.5f;

    void Start()
    {
        carRB= gameObject.GetComponent<Rigidbody>();    
    }

    void Update()
    {
        rpmNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLine * 1.1f)));
        rpmText.text = RPM.ToString("0,000") + "rpm";
        gearText.text = (gearState == GearState.Neutral) ? "N" : (currentGear + 1).ToString();
        speed = colliders.RRWheel.rpm * colliders.RRWheel.radius * 2f * Mathf.PI / 10f;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.deltaTime);
        ApplyWheelMovement();
        CheckInput();
        ApplySteering();
        ApplyBreak();
        ApplyHorsePower();
    }

    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        
        gasInput = Mathf.Clamp01(gasInput);
        if (Mathf.Abs(gasInput) > 0 && isEngineRunning == 0)
        {
            StartCoroutine(GetComponent<EngineAudion>().StartEngine());
            gearState = GearState.Running;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            brakeInput = 1.0f;
        }
        else
        {
            brakeInput = 0f;
        }

        steeringInput = Input.GetAxis("Horizontal");
        slipAngle = Vector3.Angle(transform.forward, carRB.velocity - transform.forward);

        
        float movingDirection = Vector3.Dot(transform.forward, carRB.velocity);
        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (Mathf.Abs(gasInput) > 0) gearState = GearState.Running;
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0;
        }
    }

    void ApplyBreak()
    {
        colliders.FRWheel.brakeTorque= brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque= brakeInput * brakePower * 0.7f;
        colliders.RRWheel.brakeTorque= brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque= brakeInput * brakePower * 0.3f;
    }

    void ApplyHorsePower()
    {
        currentTorque = CalculateTorque();
        colliders.RRWheel.motorTorque = currentTorque * gasInput;
        colliders.RLWheel.motorTorque = currentTorque * gasInput;
    }

    float CalculateTorque()
    {
        float torque = 0;
        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }
        if (gearState == GearState.Running && clutch > 0)
        {
            if (RPM > increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
            }
        }
        if (isEngineRunning > 0)
        {
            if (clutch < 0.1f)
            {
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * gasInput) + Random.Range(-50, 50), Time.deltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
                torque = (hpToRPMCurve.Evaluate(RPM / redLine) * horsePower / RPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
            }
        }
        return torque;
    }
    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(Mathf.Abs(gasInput), 0.5f, 1f);
        return RPM * gas / redLine;
    }
    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if (currentGear + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                //increase the gear
                yield return new WaitForSeconds(0.7f);
                if (RPM < increaseGearRPM || currentGear >= gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                //decrease the gear
                yield return new WaitForSeconds(0.1f);

                if (RPM > decreaseGearRPM || currentGear <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGear += gearChange;
        }

        if (gearState != GearState.Neutral)
            gearState = GearState.Running;
    }

    void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
        if(slipAngle < 120f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, carRB.velocity + transform.forward, Vector3.up);
        }
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelMovement()
    {
        UpdateWheel(colliders.FLWheel, MesheForWheels.FLWheel);
        UpdateWheel(colliders.FRWheel, MesheForWheels.FRWheel);
        UpdateWheel(colliders.RLWheel, MesheForWheels.RLWheel);
        UpdateWheel(colliders.RRWheel, MesheForWheels.RRWheel);
    }
    void UpdateWheel(WheelCollider coll , MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat; 
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}
[System.Serializable]
public class MesheForWheels
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}

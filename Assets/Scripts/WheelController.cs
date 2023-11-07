using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class WheelController : MonoBehaviour
{

    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backLeftTransform;





    public float acceleraton = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;
    

    private float currentAcceleration = 0f;
    private float currentBreakingForce = 0f;
    private float CurrentTurnAngle = 0f;

    void FixedUpdate()
    {
        currentAcceleration = acceleraton * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
        {
            currentBreakingForce= breakingForce;
        }
        else
        {
            currentBreakingForce = 0f;
        }

        frontRight.motorTorque= currentAcceleration;
        frontLeft.motorTorque= currentAcceleration;

        frontRight.brakeTorque= currentBreakingForce;
        frontLeft.brakeTorque= currentBreakingForce;

        CurrentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = CurrentTurnAngle;
        frontRight.steerAngle = CurrentTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backRight, backRightTransform);

    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }
}

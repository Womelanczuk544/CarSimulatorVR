using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionReset : MonoBehaviour
{
    public GameObject car;
    public Rigidbody rbCar;

    void Update()
    {
        checkReset();
    }

    private void checkReset()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            car.transform.position = this.transform.position;
            car.transform.rotation = this.transform.rotation;
            rbCar.velocity = Vector3.zero;
            rbCar.angularVelocity = Vector3.zero;
        }
    }
    
}

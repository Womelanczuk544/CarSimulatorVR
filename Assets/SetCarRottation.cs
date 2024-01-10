using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCarRottation : MonoBehaviour
{
    [SerializeField] Transform car;
    [SerializeField] Rigidbody carRB;
    private float acceleration = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.rotation = car.rotation;

    }
}

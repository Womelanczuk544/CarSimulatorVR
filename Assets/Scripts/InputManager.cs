using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public float vertical;
    public float horizontal;
    public bool handBreak;
    public float gasInput;
    public float sterringInput;
    public float clutchInput;
    public float brakeInput;
    public float GearR;
    public float Gear1;
    public float Gear2;
    public float Gear3;
    public float Gear4;
    public float Gear5;
    public CarInputs input;

    public void Awake()
    {
        input = new CarInputs();
    }

    public void OnEnable()
    {
        input.Enable();
        input.car.gas.performed += ApplyGassInput;
        input.car.gas.canceled += ReleaseGassInput;
        input.car.brake.performed += ApplybrakeInput;
        input.car.brake.canceled += ReleasebrakeInput;
        input.car.steeringWheel.performed += ApplySterringInput;
        input.car.steeringWheel.canceled += ReleaseSterringInput;
        input.car.GearR.performed += ApplyGearR;
        input.car.GearR.canceled += ReleaseGearR;
        input.car.Gear1.performed += ApplyGear1;
        input.car.Gear1.canceled += ReleaseGear1;
        input.car.Gear2.performed += ApplyGear2;
        input.car.Gear2.canceled += ReleaseGear2;
        input.car.Gear3.performed += ApplyGear3;
        input.car.Gear3.canceled += ReleaseGear3;
        input.car.Gear4.performed += ApplyGear4;
        input.car.Gear4.canceled += ReleaseGear4;
        input.car.Gear5.performed += ApplyGear5;
        input.car.Gear5.canceled += ReleaseGear5;
    }

    public void OnDisable()
    {
        input.Disable();
    }

    public void ApplyGassInput(InputAction.CallbackContext value)
    {
        gasInput = value.ReadValue<float>();
    }
    public void ReleaseGassInput(InputAction.CallbackContext value)
    {
        gasInput = 0;
    }
    public void ApplybrakeInput(InputAction.CallbackContext value)
    {
        brakeInput = value.ReadValue<float>();
    }
    public void ReleasebrakeInput(InputAction.CallbackContext value)
    {
        brakeInput = 0;
    }
    public void ApplySterringInput(InputAction.CallbackContext value)
    {
        sterringInput = value.ReadValue<float>();
    }
    public void ReleaseSterringInput(InputAction.CallbackContext value)
    {
        sterringInput = 0;
    }
    public void ApplyGearR(InputAction.CallbackContext value)
    {
        GearR = value.ReadValue<float>();
    }
    public void ReleaseGearR(InputAction.CallbackContext value)
    {
        GearR = 0;
    }
    public void ApplyGear1(InputAction.CallbackContext value)
    {
        Gear1 = value.ReadValue<float>();
    }
    public void ReleaseGear1(InputAction.CallbackContext value)
    {
        Gear1 = 0;
    }
    public void ApplyGear2(InputAction.CallbackContext value)
    {
        Gear2 = value.ReadValue<float>();
    }
    public void ReleaseGear2(InputAction.CallbackContext value)
    {
        Gear2 = 0;
    }
    public void ApplyGear3(InputAction.CallbackContext value)
    {
        Gear3 = value.ReadValue<float>();
    }
    public void ReleaseGear3(InputAction.CallbackContext value)
    {
        Gear3 = 0;
    }
    public void ApplyGear4(InputAction.CallbackContext value)
    {
        Gear4 = value.ReadValue<float>();
    }
    public void ReleaseGear4(InputAction.CallbackContext value)
    {
        Gear4 = 0;
    }
    public void ApplyGear5(InputAction.CallbackContext value)
    {
        Gear5 = value.ReadValue<float>();
    }
    public void ReleaseGear5(InputAction.CallbackContext value)
    {
        Gear5 = 0;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        vertical = gasInput;
        horizontal = sterringInput;
        handBreak = (brakeInput != 0)? true : false;
    }
}

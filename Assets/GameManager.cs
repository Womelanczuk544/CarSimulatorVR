using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Controller RR;

    public GameObject needle;
    private float startPosition = 39f, endPosition = -220f;
    private float desieredPositoion;

    public float vechicleSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        vechicleSpeed = RR.KPH;
        updateNeedle();
    }

    public void updateNeedle()
    {
        desieredPositoion = startPosition - endPosition;
        float temp = vechicleSpeed / 180;
        needle.transform.eulerAngles = new Vector3(RR.transform.eulerAngles.x, RR.transform.eulerAngles.y, (startPosition - temp * desieredPositoion));  
    }
}

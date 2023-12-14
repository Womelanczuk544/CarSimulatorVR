using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Controller RR;

    public GameObject needle;
    public TextMesh kph;
    public TextMesh gearNum;
    private float startPosition = 39f, endPosition = -220f;
    private float desieredPositoion;
   

    // Update is called once per frame
    void Update()
    {
        kph.text = RR.KPH.ToString("0");
        updateNeedle();
    }

    public void updateNeedle()
    {
        desieredPositoion = startPosition - endPosition;
        float temp = RR.engineRPM / 10000;
        needle.transform.eulerAngles = new Vector3(RR.transform.eulerAngles.x, RR.transform.eulerAngles.y, (startPosition - temp * desieredPositoion));  
    }
    public void changeGear()
    {
        if (RR.gearNum == 0) gearNum.text = "R";
        else gearNum.text = RR.gearNum.ToString();
    }
}

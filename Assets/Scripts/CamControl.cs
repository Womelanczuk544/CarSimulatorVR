using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    private GameObject player;
    private GameObject child;
    private GameObject cameraLookAt;
    private Controller RR;
    public float speed;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        child = player.transform.Find("camera constraint").gameObject;
        cameraLookAt = player.transform.Find("camera lookAt").gameObject;
        RR = player.GetComponent<Controller>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        follow();

    }
    private void follow()
    {
        speed = Mathf.Lerp(speed, RR.KPH / 4, Time.deltaTime);

        gameObject.transform.position = Vector3.Lerp(transform.position, child.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(cameraLookAt.gameObject.transform.position);
    }
}

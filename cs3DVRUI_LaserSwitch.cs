using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using VR3DUI_BaseDisplay;


public class cs3DVRUI_LaserSwitch : cs3DVRUI_DisplayBase
{


    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]

    public UnityEventController EventLaserIn = new UnityEventController();
    public UnityEventController EventLaserOut= new UnityEventController();



    private LineRenderer lineRenderer;
    private float counter;
    private float dist;


    Transform origin;
    Transform destination;

    public float leneDrawSpeed = 6f;
    public float leneDrawwidth = 0.01f;

    GameObject HitObj;
    public GameObject GetLaserInObject() { return HitObj; }



    void Start()
    {

        origin = transform.Find("LaserOrigin").Find("Point").GetChild(0);
        destination = transform.Find("LaserDestination").Find("Point").GetChild(0);


        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.SetWidth(leneDrawwidth, leneDrawwidth);

        lineRenderer.SetPosition(1, destination.position);
        //dist = Vector3.Distance(origin.position, destination.position);

    }



    // Update is called once per frame
    void Update()
    {


        var forw = origin.transform.TransformDirection(Vector3.forward);
        dist = Vector3.Distance(origin.position, destination.position);

        RaycastHit hit;
        Debug.DrawRay(origin.transform.position, forw * dist, Color.red);

        if (Physics.Raycast(origin.transform.position, forw * dist, out hit, 2))
        {

            Debug.Log("HIT");

            if (hit.collider.gameObject.name != "Destination")
            {
                Debug.Log("HIT:" + hit.collider.gameObject.name);
                HitObj = hit.collider.gameObject;
                SwitchOff();
                EventLaserIn.Invoke(this);
            }
            else
            {
                EventLaserOut.Invoke(this);
                SwitchOn();
            }
        }



    }



    public void SwitchOn()
    {

        lineRenderer.enabled = true;

    }

    public void SwitchOff()
    {
        lineRenderer.enabled = false;
    }


}

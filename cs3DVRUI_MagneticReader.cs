using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using VR3DUI_BaseDisplay;

public class cs3DVRUI_MagneticReader :  cs3DVRUI_DisplayBase
{


    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]

    public UnityEventController EventCheck_True = new UnityEventController();
    public UnityEventController EventCheck_False = new UnityEventController();

    public UnityEventController EventCheck_LightOff = new UnityEventController();



    public int KeyValue = 4;


    private float counter;
    private float dist;


    Transform origin;
    Transform destination;

    public float leneDrawSpeed = 6f;
    public float leneDrawwidth = 0.01f;

    GameObject HitObj;
    public GameObject GetLaserInObject() { return HitObj; }


    GameObject GreenOn;
    GameObject GreenOff;

    GameObject RedOn;
    GameObject RedOff;

    GameObject TextObj;
    Text       Test_State;
    bool Proc_Check = false;

    void Start()
    {

        origin = transform.Find("Origin").Find("point");
        destination = transform.Find("Destination").Find("point");

        GreenOn = transform.Find("Body").Find("Green_on").gameObject;
        GreenOff = transform.Find("Body").Find("Green_off").gameObject;

        RedOn = transform.Find("Body").Find("Red_on").gameObject;
        RedOff = transform.Find("Body").Find("Red_off").gameObject;

        TextObj = transform.Find("TrueFalse").gameObject;
        Test_State = TextObj.transform.Find("Text").GetComponent<Text>();


    }


    void LampGreenOn()
    {
        Proc_Check = true;
        GreenOn.SetActive(true);
        GreenOff.SetActive(false);

        Test_State.text = "Access";
        Test_State.color = Color.green;
        TextObj.SetActive(true);

        EventCheck_True.Invoke(this);
    }

    void LampGreenOff()
    {
        Proc_Check = false;
        GreenOn.SetActive(false);
        GreenOff.SetActive(true);

        TextObj.SetActive(false);
        EventCheck_LightOff.Invoke(this);
    }

    void LampRedOn()
    {
        Proc_Check = true;
        RedOn.SetActive(true);
        RedOff.SetActive(false);
        Test_State.text = "Access Denied";
        Test_State.color = Color.red;
        TextObj.SetActive(true);

        EventCheck_False.Invoke(this);
    }

    void LampRedOff()
    {
        Proc_Check = false;
        RedOn.SetActive(false);
        RedOff.SetActive(true);

        TextObj.SetActive(false);

        EventCheck_LightOff.Invoke(this);
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

            if (hit.collider.gameObject.tag == "MagneticCard" && !Proc_Check)
            {
                if (hit.collider.GetComponent<cs3DVRUI_MagneticCard>().KeyValue == KeyValue)
                {
                    //정답
                    StartCoroutine("LampGreen");
                }
                else
                {
                    //오답
                    StartCoroutine("LampRed");
                }
            }
        }

    }


    IEnumerator LampGreen()
    {
        LampGreenOn();
        yield return new WaitForSeconds(3f);
        LampGreenOff();
    }

    IEnumerator LampRed()
    {
        LampRedOn();
        yield return new WaitForSeconds(3f);
        LampRedOff();
    }


    public void SwitchOn()
    {

       

    }

    public void SwitchOff()
    {
       
    }

}

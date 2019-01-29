using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using VR3DUI_BaseDisplay;


public class cs3DVRUI_PressureGauge : cs3DVRUI_DisplayBase
{

    [Space(5, order = 0)]
    [Header("--- Controller Property", order = 1)]
    [Space(5, order = 0)]
    public GameObject Arrow;


    Transform TextObj;

    private Text PercentDis;
    [Space(5, order = 0)]

    public float AddPressureValue = 0.1f;
    public float ReleaseSpeedValue = 0.1f;
    
    private float fPressure = 0.0f;
    public float CurProgress = 0.0f;

    private bool IsInputPressure = false;

    public float GetProgress() { return CurProgress; }
    // Use this for initialization
    void Start ()
    {
        
        TextObj = Arrow.transform.GetChild(0);
        PercentDis = TextObj.transform.GetChild(0).GetComponent<Text>();
        PercentDis.text = (CurProgress * 100.0f).ToString("N0") + "%";
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (TextObj)
        {
            TextObj.localEulerAngles = new Vector3(TextObj.localEulerAngles.x, TextObj.localEulerAngles.y, fPressure);
            PercentDis.text = (CurProgress * 100.0f).ToString("N0") + "%";
        }


    }

    IEnumerator CoReleasePressure()
    {
        while (true)
        {
            if (IsInputPressure)
                break;

            yield return new WaitForSeconds(0.02f);
            if (fPressure <= 0)
            {
                fPressure = 0.0f;
                break;
            }
            else
            {
                fPressure -= ReleaseSpeedValue;
                if (fPressure <= 0f)
                    fPressure = 0f;


                CurProgress = fPressure / 270.0f;
                Arrow.transform.localEulerAngles = new Vector3(0, 0, fPressure);
            }

        }

    }


    public void Event_Push()
    {
        IsInputPressure = true;
        fPressure += AddPressureValue;
        if (fPressure >= 270.0f)
            fPressure = 270.0f;
        CurProgress = fPressure / 270.0f;
        Arrow.transform.localEulerAngles = new Vector3(0, 0, fPressure);

    }

    public void Event_Release()
    {
        IsInputPressure = false;
        StartCoroutine("CoReleasePressure");
    }
}

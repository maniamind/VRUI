using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestLight : MonoBehaviour
{

    public cs3DVRUI_BallValve BallValveCtrl = null;
    public cs3DVRUI_PressureGauge PressureGaugeCtrl = null;

    private Animator ActionObjAniCtrl = null;

    private Light Light_One = null;
    private Light Light_Two = null;

    public bool StartLight_On = false;

    public bool StartLight_SilentlyOn = false;

    private bool IsLightOn = false;

    public GameObject Warning_Obj = null;


    Color LightColor;
    // Use this for initialization
    void Start ()
    {
        Light_One = transform.Find("Point light").GetComponent<Light>();            
        Light_Two = Light_One.transform.GetChild(0).GetComponent<Light>();



        if (StartLight_On)
            SetLightOn();

        if (StartLight_SilentlyOn)
            SilentlyLightOn();

    }
	
	// Update is called once per frame
	void Update ()
    {

        if(BallValveCtrl != null)
        {
            if (Light_One)
            {
                Light_One.intensity = BallValveCtrl.GetOpendValue() * 5.5f;
                Light_Two.intensity = BallValveCtrl.GetOpendValue() * 2.5f;

            }

        }

        if (PressureGaugeCtrl != null)
        {

            if (Light_One)
            {
                if (PressureGaugeCtrl.GetProgress() <= 0f)
                    SetLightOff();                    
                    Light_One.color = new Color(PressureGaugeCtrl.GetProgress(), 1- PressureGaugeCtrl.GetProgress(), 1- PressureGaugeCtrl.GetProgress());

                    Light_Two.color = new Color(PressureGaugeCtrl.GetProgress(), 1- PressureGaugeCtrl.GetProgress(), 1- PressureGaugeCtrl.GetProgress());
            }


        }


    }

    public void SetLightOn()
    {
        if (ActionObjAniCtrl == null)
            ActionObjAniCtrl = transform.GetComponent<Animator>();


        if (IsLightOn)
            return;

        IsLightOn = true;



        ActionObjAniCtrl.Play("LightOn");

        Light_One.color = new Color(1, 1, 0);

        Light_Two.color = new Color(1, 1, 0);

        if (Warning_Obj != null)
            Warning_Obj.SetActive(false);

    }

    public void SilentlyLightOn()
    {
        if (ActionObjAniCtrl == null)
            ActionObjAniCtrl = transform.GetComponent<Animator>();


        if (IsLightOn)
            return;

        IsLightOn = true;

        LightColor = new Color(0, 0, 0);

        ActionObjAniCtrl.Play("LightOn");

        Light_One.color = LightColor;

        Light_Two.color = LightColor;

        if (Warning_Obj != null)
            Warning_Obj.SetActive(false);
    }



    public void SetLightRedOn()
    {
        if (ActionObjAniCtrl == null)
            ActionObjAniCtrl = transform.GetComponent<Animator>();


        if (IsLightOn)
            return;

        IsLightOn = true;
        ActionObjAniCtrl.Play("LightOn");

        Light_One.color = new Color(1, 0, 0);

        Light_Two.color = new Color(1, 0, 0);

        if (Warning_Obj != null)
            Warning_Obj.SetActive(true);
    }


    public void SetLightBlueOn()
    {
        if (ActionObjAniCtrl == null)
            ActionObjAniCtrl = transform.GetComponent<Animator>();


        if (IsLightOn)
            return;

        IsLightOn = true;
        ActionObjAniCtrl.Play("LightOn");

        Light_One.color = new Color(0, 0, 1);

        Light_Two.color = new Color(0, 0, 1);

        if (Warning_Obj != null)
            Warning_Obj.SetActive(false);

    }



    public void SetLightOff()
    {
        if(ActionObjAniCtrl == null)
            ActionObjAniCtrl = transform.GetComponent<Animator>();

        if (!IsLightOn)
            return;

        IsLightOn = false;
        ActionObjAniCtrl.Play("LightOff");

        if (Warning_Obj != null)
            Warning_Obj.SetActive(false);
    }



    public void AddLightValue()
    {


        ActionObjAniCtrl.Play("LightOn");

        if (LightColor.r >= 1.0f)
        {
            LightColor = new Color(1.0f, 1.0f, 0f);
            return;
        }
        else
            LightColor = new Color(LightColor.r + 0.005f, LightColor.g + 0.005f, LightColor.b);

        Light_One.color = LightColor;

        Light_Two.color = LightColor;

        if (Warning_Obj != null)
            Warning_Obj.SetActive(false);

    }

    public void SubLightValue()
    {
        if (LightColor.r <= 0.0f)
        {
            LightColor = new Color(0f, 0f, 0f);
            return;
        }
        else
            LightColor = new Color(LightColor.r - 0.005f, LightColor.g - 0.005f, LightColor.b);

        Light_One.color = LightColor;

        Light_Two.color = LightColor;

        if (Warning_Obj != null)
            Warning_Obj.SetActive(false);
    }

}

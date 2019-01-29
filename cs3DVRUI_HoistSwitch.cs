using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.Events;

using VR3DUI_BaseButton;
public class cs3DVRUI_HoistSwitch : cs3DVRUI_ButtonBase
{

    public Transform buttonUp = null;
    public Transform buttonDown = null;
    public Transform buttonStop = null;


    Vector3 OriginPos_BtnUp;
    Vector3 OriginPos_BtnDown;
    Vector3 OriginPos_BtnStop;

    private int Btn_Num; // Up :0 Down :1   stop:2

    public Vector3 Up_buttonDownDisplacement;
    [Space(10, order = 0)]

    public Vector3 Down_buttonDownDisplacement;
    [Space(10, order = 0)]

    public Vector3 Stop_buttonDownDisplacement;
    [Space(10, order = 0)]



    public UnityEventController Stop_Button_Up = new UnityEventController();
    public UnityEventController Stop_Button_Down = new UnityEventController();

    public UnityEventController UP_Button_Up = new UnityEventController();
    public UnityEventController Down_Button_Up = new UnityEventController();

    public UnityEventController Up_Button_Press = new UnityEventController();
    public UnityEventController Down_Button_Press = new UnityEventController();







    // Use this for initialization
    protected override void Start()
    {




        if (buttonUp == null)
            buttonUp = transform.Find("Btn_up");

        if (buttonDown == null)
            buttonDown = transform.Find("Btn_down");

        if (buttonStop == null)
            buttonStop = transform.Find("Btn_stop");


        OriginPos_BtnUp = buttonUp.localPosition;
        OriginPos_BtnDown = buttonDown.localPosition;
        OriginPos_BtnStop = buttonStop.localPosition;



    }

    // Update is called once per frame
    void Update()
    {
        if (PowerOn)
        {
            if (Btn_Num == 0)
                Up_Button_Press.Invoke(this);
            else if (Btn_Num == 1)
                Down_Button_Press.Invoke(this);

        }

    }


    public override void OnColliderEventPressUp(ColliderButtonEventData eventData)
    {
        PowerOn = false;



        if (Btn_Num == 0)
            UP_Button_Up.Invoke(this);
        else if (Btn_Num == 1)
            Down_Button_Up.Invoke(this);
        else
            Stop_Button_Up.Invoke(this);

    }


    public override void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {
        if (eventData.IsViveButton(m_activeButton) && pressingEvents.Add(eventData) && pressingEvents.Count == 1)
        {

            if (PowerOn)
                return;


            for(int i = 0; i < eventData.pressEnteredObjects.Count; i++)
            {
                if (eventData.pressEnteredObjects[i].tag == "Hoist_Up")
                {
                    Btn_Num = 0;
                    buttonUp.transform.localPosition = OriginPos_BtnUp + Up_buttonDownDisplacement;
                    break;
                }
                else if (eventData.pressEnteredObjects[i].tag == "Hoist_Down")
                {
                    Btn_Num = 1;
                    buttonDown.transform.localPosition = OriginPos_BtnDown + Down_buttonDownDisplacement;
                    break;
                }
                else if (eventData.pressEnteredObjects[i].tag == "Hoist_Stop")
                {
                    Btn_Num = 2;
                    buttonStop.transform.localPosition = OriginPos_BtnStop + Stop_buttonDownDisplacement;
                    Stop_Button_Down.Invoke(this);
                    break;
                }
               
            }



            PowerOn = true;

        }
    }




    public override void OnColliderEventPressExit(ColliderButtonEventData eventData)
    {
        if (pressingEvents.Count == 0)
        {
            PowerOn = false;

        }
        PowerOn = false;

        if (pressingEvents.Remove(eventData) && pressingEvents.Count == 0)
        {

            if (Btn_Num == 0)
            {

                buttonUp.transform.localPosition = OriginPos_BtnUp;
                UP_Button_Up.Invoke(this);
            }
            else if (Btn_Num == 1)
            {
                buttonDown.transform.localPosition = OriginPos_BtnDown;
                Down_Button_Up.Invoke(this);
            }
            else
            {
                buttonStop.transform.localPosition = OriginPos_BtnStop;
                Stop_Button_Up.Invoke(this);
            }

        }
            
    }

}

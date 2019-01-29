using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_Timer : cs3DVRUI_DragBase
{



    [Space(5, order = 0)]
    [Header("--- Controller Setting", order = 1)]
    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickMoveSensitivity = 1;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickfollowSpeed = 1.0f;




    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]
    public UnityEventController HandleGrabRelease = new UnityEventController();
    public UnityEventController EventTimeCountStart = new UnityEventController();
    public UnityEventController EventTimeCountComplete = new UnityEventController();


    Transform TextObj;

    private Text TimeDisplay;

    private bool TimeEditing = true;
    private float zRotCheck = 0.0f;
    private float itmeAngle = 0;

    private Vector3 ButtonSaveAngles;

    private float fTimeValue = 0.0f;
    public float GetCurTimeValue() { return fTimeValue; }

    protected override void Start()
    {

        base.Start();
        ButtonSaveAngles = HandleObject.transform.localEulerAngles;

        itmeAngle = 357f / 3600f;

        TextObj = transform.Find("TitleCanvas");
        TimeDisplay = TextObj.transform.Find("Time").GetComponent<Text>();
        TimeDisplay.text = "00:00";
    }



    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator CoRunTimer()
    {
        while (true)
        {
            if (TimeEditing)
                break;

            yield return new WaitForSeconds(1);
            RunTimer();

        }

    }


    private void RunTimer()
    {
        Vector3 _Angle = HandleObject.localEulerAngles;
        if(_Angle.y - itmeAngle <= 0)
        {
            HandleObject.localRotation = Quaternion.Euler(_Angle.x, 0, _Angle.z);
            StopCoroutine("CoRunTimer");
            TimeEditing = true;
            EventTimeCountComplete.Invoke(this);
        }
        else
            HandleObject.localRotation = Quaternion.Euler(_Angle.x, _Angle.y - itmeAngle, _Angle.z);

        
        fTimeValue = (_Angle.y/ itmeAngle);

        int  minute, second;
        minute = (int)(fTimeValue % 3600 / 60);//분을 구하기위해서 입력되고 남은값에서 또 60을 나눈다.
        second = (int)(fTimeValue % 3600 % 60);//마지막 남은 시간에서 분을 뺀 나머지 시간을 초로 계산함

        TimeDisplay.text = string.Format("{0:00}", minute) + ":" + string.Format("{0:00}", second);
    }


    protected override void OnDisable()
    {

        if (IsGrabbed && HandleGrabRelease != null)
        {
            HandleGrabRelease.Invoke(this);
        }

        eventList.Clear();

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }


    public override void OnColliderEventDragStart(ColliderButtonEventData eventData)
    {

        if (!eventData.IsViveButton(m_grabButton)) { return; }



        var casterPose = GetEventPose(eventData);
        var offsetPose = Pose.FromToPose(casterPose, new Pose(transform));

        if (alignPosition) { offsetPose.pos = Vector3.zero; }
        if (alignRotation) { offsetPose.rot = Quaternion.identity; }

        eventList.AddUniqueKey(eventData, offsetPose);

        TimeEditing = true;
        StopCoroutine("CoRunTimer");

    }


    public override void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        if (GetComponent<Rigidbody>() == null)
        {
            // if rigidbody doesn't exist, just move to eventData caster's pose
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;



            RotateStick(targetPose.rot.eulerAngles.z );

            zRotCheck = targetPose.rot.eulerAngles.z;



        }
    }




    public override void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        var released = eventData == grabbedEvent;

        if (released && HandleGrabRelease != null)
        {
            HandleGrabRelease.Invoke(this);
        }

        eventList.Remove(eventData);


        if (released)
        {
            TimeEditing = false;
            StartCoroutine("CoRunTimer");


            EventTimeCountStart.Invoke(this);
        }


    }





    public void RotateStick(float Rot)
    {

        if (zRotCheck > Rot)
        {
            float Add_Angle = HandleObject.localEulerAngles.y + (HandlefollowSpeed / 3.0f);

            if (Add_Angle >= 357)
                Add_Angle = 357;

            HandleObject.localRotation = Quaternion.Euler(ButtonSaveAngles.x, Add_Angle, ButtonSaveAngles.x);

        }
        else if (zRotCheck < Rot)
        {
            float Add_Angle = HandleObject.localEulerAngles.y - (HandlefollowSpeed / 3.0f);

            if (Add_Angle <= 0)
                Add_Angle = 0;

            HandleObject.localRotation = Quaternion.Euler(ButtonSaveAngles.x, Add_Angle, ButtonSaveAngles.x);
        }


        


    }
}

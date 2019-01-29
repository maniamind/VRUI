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

public class cs3DVRUI_MonoLever : cs3DVRUI_DragBase
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

    [Range(20, 30)]
    public float ActiveAngle = 25;
    [Space(5, order = 0)]
    public float TicInterval = 0.9f;
    public float TicValue = 2f;
    [Space(5, order = 0)]


    private float UpCurrentValue = 0;
    private float DownCurrentValue = 0;
    private float LeftCurrentValue = 0;
    private float RightCurrentValue = 0;


    private int TicTimeMode = -1; // 0 :up 1:down 2:left 3:right
    private bool IsAction = false;

    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]
    public UnityEventController HandleGrabRelease = new UnityEventController();

    public UnityEventController Event_UpActive = new UnityEventController();
    public UnityEventController Event_DownActive = new UnityEventController();
    public UnityEventController Event_LeftActive = new UnityEventController();
    public UnityEventController Event_RightActive = new UnityEventController();


    private float xPosCheck = 0.0f;
    private float zPosCheck = 0.0f;


    private bool UpDownLock = false;
    private bool LeftRightnLock = false;

    private Vector3 ButtonSaveAngles;


    protected override void Start()
    {

        base.Start();
        ButtonSaveAngles = HandleObject.transform.localEulerAngles;

        InitValue();

        StartCoroutine("CoTicTimer");
    }


    public void InitValue()
    {
        UpCurrentValue = 0;
        DownCurrentValue = 0;
        LeftCurrentValue = 0;
        RightCurrentValue = 0;

    }

    // Update is called once per frame
    void Update()
    {

        if (HandleObject.transform.localEulerAngles == ButtonSaveAngles)
        {
            UpDownLock = false;
            LeftRightnLock = false;
            IsAction = false;
        }

    }

    IEnumerator CoTicTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(TicInterval);
            if (IsAction)
                AddValue();

        }

    }


    private void AddValue()
    {
        switch(TicTimeMode)
        {
            case 0:
                UpCurrentValue = UpCurrentValue + TicValue;
                Event_UpActive.Invoke(this);
                break;

            case 1:
                DownCurrentValue = DownCurrentValue + TicValue;
                Event_DownActive.Invoke(this);
                break;

            case 2:
                LeftCurrentValue = LeftCurrentValue + TicValue;
                Event_LeftActive.Invoke(this);
                break;

            case 3:
                RightCurrentValue = RightCurrentValue + TicValue;
                Event_RightActive.Invoke(this);
                break;
        }
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




    public override void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        if (GetComponent<Rigidbody>() == null)
        {
            // if rigidbody doesn't exist, just move to eventData caster's pose
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;



            MoveStick(targetPose.pos.z, targetPose.pos.x);

            zPosCheck = targetPose.pos.z;
            xPosCheck = targetPose.pos.x;




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
            HandleObject.localRotation = Quaternion.Euler(ButtonSaveAngles.x, ButtonSaveAngles.y, ButtonSaveAngles.z);
            UpDownLock = false;
            LeftRightnLock = false;
            IsAction = false;
        }


    }





    public void MoveStick(float Zmove, float Xmove)
    {


        //Z축 이동이 크다면 Z축 이동을 시킨다.
        if (Mathf.Abs(zPosCheck - Zmove) >= Mathf.Abs(xPosCheck - Xmove))
        {
            if ((StickMoveSensitivity / 1000f) > Mathf.Abs(zPosCheck - Zmove))
                return;

            if (UpDownLock)
                return;

            LeftRightnLock = true;

            if (zPosCheck > Zmove)
            {
                float Add_Angle = HandleObject.localEulerAngles.x - (HandlefollowSpeed / 4.0f);

                if (Add_Angle > 300)
                    Add_Angle = Add_Angle - 360;

                if ((ActiveAngle * -1) >= Add_Angle)
                {
                    IsAction = true;
                    TicTimeMode = 1;
                    Add_Angle = (ActiveAngle * -1);


                }
                else
                    IsAction = false;

                HandleObject.localRotation = Quaternion.Euler(Add_Angle, 0, 0);

            }
            else if (zPosCheck < Zmove)
            {
                float Add_Angle = HandleObject.localEulerAngles.x + (HandlefollowSpeed / 4.0f);

                if (Add_Angle > 300)
                    Add_Angle = Add_Angle - 360;

                if (ActiveAngle <= Add_Angle)
                {
                    IsAction = true;
                    TicTimeMode = 0;
                    Add_Angle = ActiveAngle;

                }
                else
                    IsAction = false;

                HandleObject.localRotation = Quaternion.Euler(Add_Angle, 0, 0);

            }
        }
        else//X축 이동이 크다면 X축 이동을 시킨다.
        {

            if ((StickMoveSensitivity / 1000f) > Mathf.Abs(xPosCheck - Xmove))
                return;

            if (LeftRightnLock)
                return;

            UpDownLock = true;

            if (xPosCheck > Xmove)
            {
                float Add_Angle = HandleObject.localEulerAngles.z + (HandlefollowSpeed / 4.0f);

                if (Add_Angle > 300)
                    Add_Angle = Add_Angle - 360;

                if (ActiveAngle <= Add_Angle)
                {
                    IsAction = true;
                    TicTimeMode = 2;
                    Add_Angle = ActiveAngle;
                }
                else
                    IsAction = false;


                HandleObject.localRotation = Quaternion.Euler(0, 0, Add_Angle);

            }
            else if (xPosCheck < Xmove)
            {
                float Add_Angle = HandleObject.localEulerAngles.z - (HandlefollowSpeed / 4.0f);

                if (Add_Angle > 300)
                    Add_Angle = Add_Angle - 360;

                if ((ActiveAngle * -1) >= Add_Angle)
                {
                    IsAction = true;
                    TicTimeMode = 3;
                    Add_Angle = (ActiveAngle * -1);

                }
                else
                    IsAction = false;

                HandleObject.localRotation = Quaternion.Euler(0, 0, Add_Angle);

            }
        }

    }

}

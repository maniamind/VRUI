
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_Progress : cs3DVRUI_DragBase
{

    [Range(1, 10)]
    public float StickMoveSensitivity = 1;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public float StickfollowSpeed = 1.0f;

    private Transform Start_Pos;
    private Transform End_Pos;

    public UnityEventController HandleGrabRelease = new UnityEventController();
    public UnityEventController Event_Percent_Full = new UnityEventController();
    public UnityEventController Event_Percent_Zero = new UnityEventController();

    private float xPosCheck = 0.0f;


    private float fTotalLength;
    public float CurProgress;
    private Vector3 ButtonSavePos;

    private Text PercentDis;

    protected override void Start()
    {
        
        base.Start();

        Start_Pos = transform.Find("Start_Pos");
        End_Pos = transform.Find("End_Pos");

        PercentDis = HandleObject.GetChild(0).GetChild(0).GetComponent<Text>();

        fTotalLength = Mathf.Abs(Start_Pos.localPosition.x - End_Pos.localPosition.x);

        CurProgress = Mathf.Abs(HandleObject.transform.localPosition.x / fTotalLength);
        PercentDis.text = (CurProgress * 100.0f).ToString("N0") + "%";

        ButtonSavePos = HandleObject.transform.localPosition;
        
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
    }


    public override void OnColliderEventDragFixedUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            // if rigidbody exists, follow eventData caster using physics
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;

            // applying velocity
            var diffPos = targetPose.pos - rigid.position;
            if (Mathf.Approximately(diffPos.sqrMagnitude, 0f))
            {
                rigid.velocity = Vector3.zero;
            }
            else
            {
                rigid.velocity = diffPos / Mathf.Clamp(followingDuration, MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION);
            }

            // applying angular velocity
            float angle;
            Vector3 axis;
            (targetPose.rot * Quaternion.Inverse(rigid.rotation)).ToAngleAxis(out angle, out axis);
            while (angle > 360f) { angle -= 360f; }

            if (Mathf.Approximately(angle, 0f) || float.IsNaN(axis.x))
            {
                rigid.angularVelocity = Vector3.zero;
            }
            else
            {
                angle *= Mathf.Deg2Rad / Mathf.Clamp(followingDuration, MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION); // convert to radius speed
                if (overrideMaxAngularVelocity && rigid.maxAngularVelocity < angle) { rigid.maxAngularVelocity = angle; }
                rigid.angularVelocity = axis * angle;
            }
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


            float HandPos = Mathf.Abs(HandleObject.transform.localPosition.x);

            if (HandPos >= Mathf.Abs(Start_Pos.transform.localPosition.x) && HandPos <= Mathf.Abs(End_Pos.transform.localPosition.x))
                MoveStick(targetPose.pos.z);
            

            xPosCheck = targetPose.pos.z;



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

            float Angle = 0.0f;
            if (HandleObject.localEulerAngles.y > 250)
                Angle = HandleObject.localEulerAngles.y - 360.0f;
            else
                Angle = HandleObject.localEulerAngles.y;






        }
    }


    public void MoveStick(float moveValue)
    {

        float Add_Pos;

        if ((StickMoveSensitivity / 10000f) > Mathf.Abs(xPosCheck - moveValue))
            return;


        if (xPosCheck < moveValue)
        {
            Add_Pos = HandleObject.transform.localPosition.x - (StickfollowSpeed / 1000f);

            if (Add_Pos <= End_Pos.transform.localPosition.x)
                Add_Pos = End_Pos.transform.localPosition.x;

            HandleObject.transform.localPosition = new Vector3(Add_Pos, ButtonSavePos.y, ButtonSavePos.z);
            
        }
        else if(xPosCheck > moveValue)
        {
            Add_Pos = HandleObject.transform.localPosition.x + (StickfollowSpeed / 1000f);

            if (Add_Pos >= Start_Pos.transform.localPosition.x)
                Add_Pos = Start_Pos.transform.localPosition.x;


            HandleObject.transform.localPosition = new Vector3(Add_Pos, ButtonSavePos.y, ButtonSavePos.z);
        }


        CurProgress = Mathf.Abs(HandleObject.transform.localPosition.x / fTotalLength);
        PercentDis.text = (CurProgress * 100.0f).ToString("N0") + "%";

        if (CurProgress >= 1.0f)
            Event_Percent_Full.Invoke(this);

        if (CurProgress <= 0.0f)
            Event_Percent_Zero.Invoke(this);


    }

}

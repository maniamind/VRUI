
using System;
using UnityEngine;
using UnityEngine.Events;

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

using VR3DUI_BaseDrag;

public class cs3DVRUI_CamSwitch : cs3DVRUI_DragBase
{


    public float ActionAngle = 45.0f;
    private float MinActionAngle;
    private float CenterActionAngle;

    [Space(5, order = 0)]
    [Header("--- Controller Event", order = 1)]
    [Space(5, order = 0)]

    //GrabRelease

    public UnityEventController Event_Froward = new UnityEventController();
    public UnityEventController Event_Reverse = new UnityEventController();
    public UnityEventController Event_Stop = new UnityEventController();
    public UnityEventController HandleGrabRelease = new UnityEventController();



    private float xPosCheck = 0.0f;
    

    // Update is called once per frame
    void Update () {
		
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


        MinActionAngle = ActionAngle * -1.0f;
        CenterActionAngle = ((ActionAngle * 2.0f) / 3.0f)/2.0f;

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

            float Angle = 0.0f;
            if (HandleObject.localEulerAngles.y > 250)
                Angle = HandleObject.localEulerAngles.y - 360.0f;



            //if (Angle >= ActionAngle)
            //    Angle = ActionAngle;
            //else if (Angle <= MinActionAngle)
            //    Angle = MinActionAngle;

            //    if (Angle <= ActionAngle && Angle >= MinActionAngle)
            //       MoveStick(targetPose.pos.x);


            //xPosCheck = targetPose.pos.x;



            MoveStick(targetPose.rot.eulerAngles.z);

            xPosCheck = targetPose.rot.eulerAngles.z;


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




            if (Angle <= CenterActionAngle && Angle >= (CenterActionAngle *-1.0f)) // Center
            {
                HandleObject.localRotation = Quaternion.Euler(0, 0, 0);
                Event_Stop.Invoke(this);
            }
            else if (Angle <= ((CenterActionAngle * -1.0f)-1.0f) && Angle >= MinActionAngle) // Left
            {
                HandleObject.localRotation = Quaternion.Euler(0, MinActionAngle, 0);
                Event_Froward.Invoke(this);
            }
            else
            {
                HandleObject.localRotation = Quaternion.Euler(0, ActionAngle, 0);
                Event_Reverse.Invoke(this);
            }

            
}
    }


    public void MoveStick(float pos_x)
    {
        


        if ((HandleMoveSensitivity / 10000.0f) > Mathf.Abs(xPosCheck - pos_x))
            return;

        float Add_Angle;


        if (xPosCheck < pos_x)
        {
            Add_Angle = HandleObject.localEulerAngles.y - (HandlefollowSpeed / 4);



            float Angle = 0.0f;
            if (Add_Angle > 250)
                Angle = Add_Angle - 360.0f;
            else
                Angle = Add_Angle;

            if (Angle < MinActionAngle)
                Angle = MinActionAngle;


            HandleObject.localRotation = Quaternion.Euler(0, Angle, 0);
        }
        else if(xPosCheck > pos_x)
        {
            Add_Angle = HandleObject.localEulerAngles.y + (HandlefollowSpeed/4);

            float Angle = 0.0f;
            if (Add_Angle > 250)
                Angle = Add_Angle - 360.0f;
            else
                Angle = Add_Angle;

            if (Angle >= ActionAngle)
                Angle = ActionAngle;

            HandleObject.localRotation = Quaternion.Euler(0, Angle, 0);
        }



    }




}

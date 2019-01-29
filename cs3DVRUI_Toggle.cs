using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using UnityEngine;
using UnityEngine.Events;

public class cs3DVRUI_Toggle : MonoBehaviour
    , IColliderEventDragStartHandler
    , IColliderEventDragFixedUpdateHandler
    , IColliderEventDragUpdateHandler
    , IColliderEventDragEndHandler

{



    [Serializable]
    public class UnityEventToggle : UnityEvent<cs3DVRUI_Toggle> { }





    [Space(5, order = 0)]

    public const float MIN_FOLLOWING_DURATION = 0.02f;
    public const float DEFAULT_FOLLOWING_DURATION = 0.04f;
    public const float MAX_FOLLOWING_DURATION = 0.5f;


    [Space(5, order = 0)]
    [Header("------------ Button Setting ", order = 1)]
    [Space(1, order = 2)]

    public Transform StickObject;


    private OrderedIndexedTable<ColliderButtonEventData, Pose> eventList = new OrderedIndexedTable<ColliderButtonEventData, Pose>();

    private bool alignPosition = true;
    private bool alignRotation = true;

    [Space(5, order = 0)]

    [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
    private float followingDuration = DEFAULT_FOLLOWING_DURATION;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public int StickMoveSensitivity = 5;

    [Space(5, order = 0)]

    [Range(1.0f, 3.0f)]
    public float StickfollowSpeed = 1.0f;


    private bool overrideMaxAngularVelocity = true;


    [Space(5, order = 0)]

    [SerializeField]
    private ControllerButton m_grabButton = ControllerButton.Trigger;


    [Space(5, order = 0)]
//    public UnityEventToggle afterGrabbed = new UnityEventToggle();
  //  public UnityEventToggle beforeRelease = new UnityEventToggle();

    public UnityEventToggle Switch_On_Event = new UnityEventToggle();
    public UnityEventToggle Switch_Off_Event = new UnityEventToggle();


    private bool bSwitchOn = false;
    public bool IsSwitchOn() { return bSwitchOn; }


    public ControllerButton grabButton
    {
        get
        {
            return m_grabButton;
        }
        set
        {
            m_grabButton = value;
            // set all child MaterialChanger heighlightButton to value;
            var matChangers = ListPool<MaterialChanger>.Get();
            GetComponentsInChildren(matChangers);
            for (int i = matChangers.Count - 1; i >= 0; --i) { matChangers[i].heighlightButton = value; }
            ListPool<MaterialChanger>.Release(matChangers);
        }
    }

    public bool isGrabbed { get { return eventList.Count > 0; } }


    private float yPosCheck = 0.0f;

    public ColliderButtonEventData grabbedEvent { get { return isGrabbed ? eventList.GetLastKey() : null; } }

    private Pose GetEventPose(ColliderButtonEventData eventData)
    {
        var grabberTransform = eventData.eventCaster.transform;
        return new Pose(grabberTransform);
    }



    
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        grabButton = m_grabButton;
    }
#endif
    protected virtual void Start()
    {
        alignPosition = true;
        alignRotation = true;

    grabButton = m_grabButton;
    }

    // Update is called once per frame
    void Update () {
		
	}



    protected virtual void OnDisable()
    {
        //if (isGrabbed && beforeRelease != null)
        //{
        //    beforeRelease.Invoke(this);
        //}

        eventList.Clear();

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    public virtual void OnColliderEventDragStart(ColliderButtonEventData eventData)
    {
        if (!eventData.IsViveButton(m_grabButton)) { return; }

        var casterPose = GetEventPose(eventData);
        var offsetPose = Pose.FromToPose(casterPose, new Pose(transform));

        if (alignPosition) { offsetPose.pos = Vector3.zero; }
        if (alignRotation) { offsetPose.rot = Quaternion.identity; }

        eventList.AddUniqueKey(eventData, offsetPose);

        //if (afterGrabbed != null)
        //{
        //    afterGrabbed.Invoke(this);
        //}
    }

    public virtual void OnColliderEventDragFixedUpdate(ColliderButtonEventData eventData)
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

    public virtual void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
    {
        if (eventData != grabbedEvent) { return; }

        if (GetComponent<Rigidbody>() == null)
        {
            // if rigidbody doesn't exist, just move to eventData caster's pose
            var casterPose = GetEventPose(eventData);
            var offsetPose = eventList.GetLastValue();
            var targetPose = casterPose * offsetPose;


            if (StickObject.localEulerAngles.x > 290)
            {
                if (StickObject.localEulerAngles.x <= 360.0f && StickObject.localEulerAngles.x >= 300.0f)
                    MoveStick(targetPose.pos.y);
            }
            else
            {
                if (StickObject.localEulerAngles.x <= 0.0f && StickObject.localEulerAngles.x >= -60.0f)
                    MoveStick(targetPose.pos.y);
            }

            yPosCheck = targetPose.pos.y;

        }
    }

    private void MoveStick(float moveValue)
    {

        float Add_Angle;

        if ((StickMoveSensitivity / 10000.0f) > Mathf.Abs(yPosCheck - moveValue))
            return;

        

        if (yPosCheck > moveValue)
        {
            Add_Angle = StickObject.localEulerAngles.x - StickfollowSpeed;

            if (Add_Angle <= 300)
                Add_Angle = 300;

            StickObject.localRotation = Quaternion.Euler(Add_Angle, 0, 0);

            //손잡이를 잡고 내릴때 
            if (Add_Angle <= 320)
                SwitchOff();

        }
        else if (yPosCheck < moveValue)
        {
            Add_Angle = StickObject.localEulerAngles.x + StickfollowSpeed;

            if (Add_Angle >= 359)
                Add_Angle = 359;

            StickObject.localRotation = Quaternion.Euler(Add_Angle, 0, 0);

            //손잡이를 잡고 내릴때 
            if (Add_Angle >= 340)
                SwitchOn();

        }
    }




    public virtual void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        var released = eventData == grabbedEvent;


        eventList.Remove(eventData);


        if (released)
        {
            // 0도랑 360 체크
            if(StickObject.localEulerAngles.x > 290)
            {
                if (StickObject.localEulerAngles.x >= 330.0f)
                {
                    StickObject.localRotation = Quaternion.Euler(359, 0, 0);
                    SwitchOn();
                }
                else
                {
                    StickObject.localRotation = Quaternion.Euler(300, 0, 0);
                    SwitchOff();
                }

            }
            else
            {
                if (StickObject.localEulerAngles.x >= -30.0f)
                {
                    StickObject.localRotation = Quaternion.Euler(359, 0, 0);
                    SwitchOn();
                }
                else
                {
                    StickObject.localRotation = Quaternion.Euler(300, 0, 0);
                    SwitchOff();
                }
            }


        }
    }



    public void SwitchOn()
    {
        if (IsSwitchOn())
            return;

        bSwitchOn = true;


        if (Switch_On_Event != null)
        {
            Switch_On_Event.Invoke(this);
        }
    }

    public void SwitchOff()
    {
        if (!IsSwitchOn())
            return;

        bSwitchOn = false;

        if (Switch_Off_Event != null)
        {
            Switch_Off_Event.Invoke(this);
        }
    }
}

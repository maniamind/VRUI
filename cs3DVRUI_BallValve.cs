using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using UnityEngine;
using UnityEngine.Events;


public class cs3DVRUI_BallValve : MonoBehaviour
    , IColliderEventDragStartHandler
    , IColliderEventDragFixedUpdateHandler
    , IColliderEventDragUpdateHandler
    , IColliderEventDragEndHandler
{


    [Serializable]
    public class UnityEventBallValve : UnityEvent<cs3DVRUI_BallValve> { }

    [Space(5, order = 0)]

    public const float MIN_FOLLOWING_DURATION = 0.02f;
    public const float DEFAULT_FOLLOWING_DURATION = 0.04f;
    public const float MAX_FOLLOWING_DURATION = 0.5f;


    [Space(5, order = 0)]
    [Header("------------ Setting ", order = 1)]
    [Space(1, order = 2)]

    public Transform HandleObject;


    private OrderedIndexedTable<ColliderButtonEventData, Pose> eventList = new OrderedIndexedTable<ColliderButtonEventData, Pose>();

    private bool alignPosition = true;
    private bool alignRotation = true;

    [Space(5, order = 0)]

    [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
    private float followingDuration = DEFAULT_FOLLOWING_DURATION;

    [Space(5, order = 0)]

    [Range(1, 10)]
    public int HandleMoveSensitivity = 2;


    [Space(5, order = 0)]

    [Range(1, 5)]
    public float HandlefollowSpeed = 3;


    private bool overrideMaxAngularVelocity = true;


    [Space(5, order = 0)]

    [SerializeField]
    private ControllerButton m_grabButton = ControllerButton.Trigger;


    [Space(5, order = 0)]


    [Space(5, order = 0)]
    [Header("------------ Property", order = 1)]
    [Space(3, order = 2)]


    public float BallValve_OpenedValue;
    public float GetOpendValue() { return BallValve_OpenedValue; }
    public void SetOpendValue(float value)
    {
        if (value > 1.0f)
            value = 1.0f;

        if (value < 0.0f)
            value = 0.0f;

        BallValve_OpenedValue = value;
    }

    [Space(5, order = 0)]


    //event
    public UnityEventBallValve HandleGrabRelease = new UnityEventBallValve();






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

    private float xPosCheck = 0.0f;
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
    void Update()
    {

    }



    protected virtual void OnDisable()
    {
        if (isGrabbed && HandleGrabRelease != null)
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


            if (HandleObject.localEulerAngles.x <= 0)
            {

                if (HandleObject.localEulerAngles.x <= 0.0f && HandleObject.localEulerAngles.x >= -90.0f)
                    MoveStick(targetPose.pos.x, targetPose.pos.y);
            }
            else
            {
                if (HandleObject.localEulerAngles.x <= 360f && HandleObject.localEulerAngles.x >= 270.0f)
                    MoveStick(targetPose.pos.x, targetPose.pos.y);

            }

            xPosCheck = targetPose.pos.x;
            yPosCheck = targetPose.pos.y;


        }
    }

    private void MoveStick(float pos_x, float pos_y)
    {

        float Add_Angle;

        if ((HandleMoveSensitivity / 10000.0f) > Mathf.Abs(xPosCheck - pos_x))
            return;

        if ((HandleMoveSensitivity / 10000.0f) > Mathf.Abs(yPosCheck - pos_y))
            return;



        if (xPosCheck < pos_x && yPosCheck > pos_y)
        {
            Add_Angle = HandleObject.localEulerAngles.x + HandlefollowSpeed;

            if (Add_Angle >= 359)
                Add_Angle = 359;

            HandleObject.localRotation = Quaternion.Euler(Add_Angle, 270, 90);

        }
        else if (xPosCheck > pos_x && yPosCheck < pos_y)
        {
            Add_Angle = HandleObject.localEulerAngles.x - HandlefollowSpeed;



            if (Add_Angle <= 269)
                Add_Angle = 269;

            HandleObject.localRotation = Quaternion.Euler(Add_Angle, 270, 90);

        }

        BallValve_OpenedValue = (1.0f/90.0f) * (359-HandleObject.localEulerAngles.x);
        if (BallValve_OpenedValue > 0.97)
            BallValve_OpenedValue = 1.0f;
    }




    public virtual void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        var released = eventData == grabbedEvent;

        if (released && HandleGrabRelease != null)
        {
            HandleGrabRelease.Invoke(this);
        }

        eventList.Remove(eventData);


        if (released)
        {

            BallValve_OpenedValue = (1.0f / 90.0f) * (359 - HandleObject.localEulerAngles.x);
            if (BallValve_OpenedValue > 0.97)
                BallValve_OpenedValue = 1.0f;


        }
    }

}

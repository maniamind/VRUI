using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;




namespace VR3DUI
{

    public class cs3DVRUI_Base : MonoBehaviour
        , IColliderEventPressUpHandler
        , IColliderEventPressEnterHandler
        , IColliderEventPressExitHandler
        , IColliderEventDragStartHandler
        , IColliderEventDragFixedUpdateHandler
        , IColliderEventDragUpdateHandler
        , IColliderEventDragEndHandler
    {


        [Space(5, order = 0)]
        [Header("--- Controller Property", order = 1)]
        [Space(1, order = 2)]

        [Space(5, order = 0)]


        public Transform buttonObject;

        [Space(10, order = 0)]

        [SerializeField]
        private ControllerButton m_activeButton = ControllerButton.Trigger;



        protected OrderedIndexedTable<ColliderButtonEventData, Pose> eventList = new OrderedIndexedTable<ColliderButtonEventData, Pose>();
        private Pose GetEventPose(ColliderButtonEventData eventData)
        {
            var grabberTransform = eventData.eventCaster.transform;
            return new Pose(grabberTransform);
        }



        private HashSet<ColliderButtonEventData> pressingEvents = new HashSet<ColliderButtonEventData>();

        public ControllerButton activeButton
        {
            get
            {
                return m_activeButton;
            }
            set
            {
                m_activeButton = value;
                // set all child MaterialChanger heighlightButton to value;
                var changers = ListPool<MaterialChanger>.Get();
                GetComponentsInChildren(changers);
                for (int i = changers.Count - 1; i >= 0; --i) { changers[i].heighlightButton = value; }
                ListPool<MaterialChanger>.Release(changers);
            }
        }

        // Define Unity Event....
        [Serializable]
        public class UnityEventController : UnityEvent<cs3DVRUI_Base> { }




        protected bool PowerOn = false;
        bool IsPowerOn() { return PowerOn; }

        private Vector3 OriginPosition;

        public bool isGrabbed { get { return eventList.Count > 0; } }

        // Use this for initialization
        void Start()
        {
            OriginPosition = buttonObject.position;
        }

        // Update is called once per frame
        void Update()
        {

        }





        protected virtual void OnDisable()
        {

            eventList.Clear();

            var rigid = GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
        }




#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            activeButton = m_activeButton;
        }
#endif



        #region Press Event ------------------------------------------------------

        public virtual void OnColliderEventPressUp(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventPressUp");
        }

        public virtual void OnColliderEventPressEnter(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventPressEnter");
        }

        public virtual void OnColliderEventPressExit(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventPressExit");
        }
        #endregion


        #region Drag Event ------------------------------------------------------

        public virtual void OnColliderEventDragStart(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventDragStart");
        }

        public virtual void OnColliderEventDragFixedUpdate(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventDragFixedUpdate");
        }

        public virtual void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventDragUpdate");
        }

        public virtual void OnColliderEventDragEnd(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventDragEnd");
        }
        #endregion

    }
}
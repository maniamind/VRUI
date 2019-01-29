using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;

namespace VR3DUI_BaseButton
{
    public class cs3DVRUI_ButtonBase : MonoBehaviour
            , IColliderEventPressDownHandler
            , IColliderEventPressUpHandler
            , IColliderEventPressEnterHandler
            , IColliderEventPressExitHandler


    {

        [Space(5, order = 0)]
        [Header("--- Controller Property", order = 1)]
        [Space(1, order = 2)]

        [Space(5, order = 0)]


        public Transform buttonObject;

        public Vector3 buttonDownDisplacement;
        [Space(10, order = 0)]

        [SerializeField]
        protected ControllerButton m_activeButton = ControllerButton.Trigger;


        [Serializable]
        public class UnityEventController : UnityEvent<cs3DVRUI_ButtonBase> { }



        protected HashSet<ColliderButtonEventData> pressingEvents = new HashSet<ColliderButtonEventData>();
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


        protected bool PowerOn = false;
        bool IsPowerOn() { return PowerOn; }

        protected Vector3 OriginPosition;




        // Use this for initialization
        protected virtual void Start()
        {
 
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Press Event ------------------------------------------------------

        public virtual void OnColliderEventPressUp(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventPressUp");
        }
        public virtual void OnColliderEventPressDown(ColliderButtonEventData eventData)
        {
            Debug.Log("cs3DVRUI_Base::OnColliderEventPressDown");
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
    }
}
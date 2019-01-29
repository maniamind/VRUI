using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cs3DVRUI_Foot_Event : MonoBehaviour {


    // Use this for initialization

    cs3DVRUI_FootSwitch Controller;
    void Start()
    {
        Controller = transform.parent.GetComponent<cs3DVRUI_FootSwitch>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {

        if (other.transform.tag == "Connet")
        {
            Controller.TriggerEnter();
        }

    }

    void OnTriggerExit(Collider other)
    {

        if (other.transform.tag == "Connet")
        {
            Controller.RestoreHandle();
        }

    }

}

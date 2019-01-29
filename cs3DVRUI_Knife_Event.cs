using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cs3DVRUI_Knife_Event : MonoBehaviour {


    cs3DVRUI_KnifeSwitch Controller;
    void Start()
    {
        Controller = transform.parent.parent.GetComponent<cs3DVRUI_KnifeSwitch>();

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

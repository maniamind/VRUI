using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestDoor : MonoBehaviour
{

    bool IsDoorOpen = false;
    Animator AniCtrl;
	// Use this for initialization
	void Start ()
    {

        AniCtrl = GetComponent<Animator>();
        IsDoorOpen = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OpenDoor()
    {
        if (IsDoorOpen)
            return;
        else
            IsDoorOpen = true;



        if (AniCtrl != null)
        {
            AniCtrl.Play("Door_opne");
        }
    }

    public void CloseDoor()
    {
        if (!IsDoorOpen)
            return;
        else
            IsDoorOpen = false;

        if (AniCtrl != null)
        {
            AniCtrl.Play("Door_close");
        }
    }

}

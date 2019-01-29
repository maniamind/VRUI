using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestCrane : MonoBehaviour
{


    Animation AnCrtrl;
    public float fMoveTime = 0.0f;
	// Use this for initialization
	void Start () {

        AnCrtrl = transform.GetComponent<Animation>();

        //AnCrtrl.Play("CraneUp");
        //AnCrtrl.Stop();


        // Set up some state; 

        DownValue();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpValue()
    {

        if (AnCrtrl["CraneUp"].length < 5.2f)
        {
            Debug.Log("Max----------------------");
            return;
        }
        fMoveTime = fMoveTime + 0.01f;


        AnCrtrl["CraneUp"].time = fMoveTime;
        AnCrtrl["CraneUp"].enabled = true;
        // Sample animations now. 
        AnCrtrl.Sample();
        AnCrtrl["CraneUp"].enabled = false;
        AnCrtrl["CraneUp"].speed = 0;
        AnCrtrl.Play("CraneUp", PlayMode.StopAll);
    }

    public void DownValue()
    {
        if (fMoveTime <= 0.0f)
            return;

        fMoveTime = fMoveTime - 0.01f;
        AnCrtrl["CraneUp"].time = fMoveTime;
        AnCrtrl["CraneUp"].enabled = true;
        // Sample animations now. 
        AnCrtrl.Sample();
        AnCrtrl["CraneUp"].enabled = false;
        AnCrtrl["CraneUp"].speed = 0;
        AnCrtrl.Play("CraneUp", PlayMode.StopAll);
    }
}

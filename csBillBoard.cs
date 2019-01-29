using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csBillBoard : MonoBehaviour {

    private Text mLength;
	// Use this for initialization
	void Start ()
    {
        mLength = GetComponentInChildren<Text>();
    }
	


	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(Camera.main.transform);
	}
}

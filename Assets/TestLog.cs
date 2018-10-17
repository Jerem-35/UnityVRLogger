using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLog : MonoBehaviour {

    public string ReadMe = "Press A, B and C to generate Log messages \n\n";


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Log "+Time.time);
        }
		if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.LogError("ErrorLog"); 
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.LogWarning("WarningLog");
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRotation_Test : MonoBehaviour {

    GameObject mPlayer;

	// Use this for initialization
	void Start () {
       mPlayer = GameObject.Find("PlayerObj_B");
    }
	
	// Update is called once per framea
	void Update () {

        Quaternion targetRot = Quaternion.identity;
        targetRot.SetLookRotation(mPlayer.transform.position - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.01f);

  

        //Quaternion q = Quaternion.identity;
        //q.SetLookRotation(mPlayer.transform.position - transform.position,Vector3.up);
        //transform.rotation = q;
        
	}
}

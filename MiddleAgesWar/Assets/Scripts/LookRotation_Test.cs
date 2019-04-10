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
        //gameObject.transform.LookAt(mPlayer.transform);
        //Vector3 toLook = Vector3.Lerp(transform.position, mPlayer.transform.position - transform.position, 0.5f);

        //Quaternion q = Quaternion.identity;
        //q.SetLookRotation(toLook);
        //transform.rotation = q;

  

        //Quaternion q = Quaternion.identity;
        //q.SetLookRotation(mPlayer.transform.position - transform.position,Vector3.up);
        //transform.rotation = q;
        
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    Rigidbody mRigid;

    float speed;

    Vector3 mDirection;

	// Use this for initialization
	void Start () {
        speed = 20;
        mRigid = gameObject.GetComponent<Rigidbody>();
        mDirection = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    gameObject.transform.Translate(Vector3.left * Time.deltaTime * speed);
        //}

        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
        //}

        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    gameObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    gameObject.transform.Translate(Vector3.back * Time.deltaTime * speed);
        //}

        //mDirection = Vector3.zero;

        mDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            mDirection = mDirection + Vector3.left;
            //mRigid.MovePosition(gameObject.transform.position+(Vector3.left * Time.deltaTime * speed));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            mDirection = mDirection + Vector3.right;
            //mRigid.MovePosition(gameObject.transform.position + (Vector3.right * Time.deltaTime * speed));
        }


        if (Input.GetKey(KeyCode.UpArrow))
        {
            mDirection = mDirection + Vector3.forward;
            //mRigid.MovePosition(gameObject.transform.position + (Vector3.forward * Time.deltaTime * speed));
        }


        if (Input.GetKey(KeyCode.DownArrow))
        {
            mDirection = mDirection + Vector3.back;
            //mRigid.MovePosition(gameObject.transform.position + (Vector3.back * Time.deltaTime * speed));
        }

        //if(mDirection == Vector3.zero)
        //  mRigid.velocity = Vector3.zero;
        //mRigid.AddForce(mDirection * speed * Time.deltaTime);
        // mRigid.MovePosition(mDirection * speed * Time.deltaTime);

        mRigid.MovePosition(gameObject.transform.position + (mDirection * speed * Time.deltaTime));
    
    }

    

}

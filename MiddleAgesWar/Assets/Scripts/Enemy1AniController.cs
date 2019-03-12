using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1AniController : MonoBehaviour {

    Animator mAnimator;
    Enemy1Controller mEnemy1Ctrl;

	// Use this for initialization
	void Start () {
        mAnimator = gameObject.GetComponent<Animator>();
        mEnemy1Ctrl = gameObject.GetComponentInParent<Enemy1Controller>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SlowMotion()
    {
        mAnimator.speed = 0.1f;
    }

    void SpeedInitialize()
    {
        mAnimator.speed = 1.0f;
    }

    void AttackEnd()
    {
        mEnemy1Ctrl.SendMessage("AttackEnd");
    }
}

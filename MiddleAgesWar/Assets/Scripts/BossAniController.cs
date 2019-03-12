using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAniController : MonoBehaviour {

    BossController mBossCtrl;

	// Use this for initialization
	void Start () {
        mBossCtrl = gameObject.GetComponentInParent<BossController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //스킬이 끝난 후 호출 되는 함수.
    void SkillEnd(int attackType)
    {
        /*
         어택 타입에 따른 쿨타임 초기화.
         1 : 기본공격
         2 : 슬래쉬공격
         3 : 점프공격
         기본공격은 모든 스킬이 끝날때마다 항상 초기화 해줌.
         */
        switch (attackType)
        {
            case 1:
                mBossCtrl.SendMessage("SkillEnd", 1);
                break;
            case 2:
                mBossCtrl.SendMessage("SkillEnd", 2);
                break;
            case 3:
                mBossCtrl.SendMessage("SkillEnd", 3);
                break;

        }
        
    }

    // 캐스팅 시작.
    void CastingStart()
    {
        mBossCtrl.SendMessage("CastingStart");
        
    }

    // 캐스팅 끝.
    void CastingEnd(int attackType)
    {
        /*
         점프공격일때는 jumpPoint지정을 위해 별도의 함수 호출.
         */
        mBossCtrl.SendMessage("CastingEnd", attackType);

    }

    void JumpVelocityZero()
    {
        mBossCtrl.SendMessage("JumpVelocityZero");
    }
}

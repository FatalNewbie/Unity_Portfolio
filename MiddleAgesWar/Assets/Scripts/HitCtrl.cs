using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCtrl : MonoBehaviour {

    [SerializeField] bool mBossNormalAttack;
    [SerializeField] bool mBossSlashAttack;
    [SerializeField] bool mBossJumpAttack;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // 공격자가 플레이어일때 이게 지금 이 오브젝트랑 부모 오브젝트의 태그를 전부 "Player"로 했는데 크게 문제가 없으려나...
        // 다른 씬에서 플레이어를 태그로 불러올때 태그가 제일 하단부에 있는 오브젝트가 가져와짐. 오브젝트들 전부 Player로 통일하면 안될듯..
        // 플레이어가 공격할때 공격당하는 것들은 전부 여기에. 밑에도 마찬가지.
        // gameObject가 공격하는 오브젝트, other이 공격당하는 오브젝트.
        //Debug.Log(other.tag);
        if (gameObject.tag == "PlayerSword")
        {
            if (other.tag == "Enemy")
            {
                other.GetComponentInParent<Enemy1Controller>().GetDamaged(gameObject.GetComponentInParent<PlayerController>().GetAttackDamage());
            }

            if (other.tag == "MainGateLever")
            {
                other.SendMessage("OpenMainGate");
            }

            if (other.tag == "Boss")
            {
                other.GetComponentInParent<BossController>().GetDamaged(gameObject.GetComponentInParent<PlayerController>().GetAttackDamage());
            }

        }

        else if (gameObject.tag == "Boss")
        {
            if (other.tag == "PlayerObj")
            {
                if (mBossNormalAttack)
                    other.GetComponent<PlayerController>().GetDamaged(gameObject.GetComponentInParent<BossController>().GetAttackDamage("normal"));
                else if (mBossSlashAttack)
                    other.GetComponent<PlayerController>().GetDamaged(gameObject.GetComponentInParent<BossController>().GetAttackDamage("slash"));
                else
                {
                    other.GetComponent<PlayerController>().GetDamaged(gameObject.GetComponentInParent<BossController>().GetAttackDamage("jump"));
                    other.GetComponent<PlayerController>().PlayerFall();
                }
            }
        }

        else if (gameObject.tag == "Enemy")
        {
            if (other.tag == "PlayerObj")
                other.GetComponent<PlayerController>().GetDamaged(gameObject.GetComponentInParent<Enemy1Controller>().GetAttackDamage());
        }

       
        
    }
}


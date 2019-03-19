using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy1Controller : MonoBehaviour {

    NavMeshAgent mNavMeshAgent;
    Transform mPlayer;
    Animator mAnimator;
    Slider mHpSlider;
    [SerializeField] GameObject mHitParticlePrefab;              // 히트 파티클 프리팹

    Enemy1State mEnemy1State;                   // 몬스터 상태 변수
    bool mIsDead;                               // 사망 플래그 변수
    bool mIsAttack;                             // 공격 플래그 변수
    [SerializeField] float mAttackCoolTime;     // 공격 쿨타임 설정
    float mAttackCoolCur;                       // 공격 쿨타임
    [SerializeField] float mHp;                 // 체력
    [SerializeField] int mEnemy1AtkDamage;      // 공격 데미지
    [SerializeField] float mSight;              // 몬스터 시야 범위
    float mFullHp;                              // 체력 감소 퍼센트 계산하기 위해 필요한 hp최대치를 저장.
    

    enum Enemy1State
    {
        IDLE,
        CHASE,
        ATTACK,
        DAMAGED,
        DIE
    }


	// Use this for initialization
	void Start () {
        mNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        mAnimator = gameObject.GetComponentInChildren<Animator>();
        mPlayer = GameObject.Find("PlayerObj_B").transform;

        // 플래그 변수와 몬스터 상태 초기화.
        mIsDead = false;
        mIsAttack = false;
        mEnemy1State = Enemy1State.IDLE;
        
       // 공격 쿨타임 초기화.
        mAttackCoolCur = 0;
        
        
        //체력바 컴포넌트 가져와서 값을 1로 설정.
        mHpSlider = gameObject.GetComponentInChildren<Slider>();
        mHpSlider.value = 1.0f;

        // 체력 최대치를 mFullHp에 저장.
        mFullHp = mHp;
        
    }
	
	// Update is called once per frame
	void Update () {

        if(mIsDead)
            return;

        // 몬스터 체력이 0이 되었을시 사망.
        if (mHp <= 0)
        {
            // 플래그 변수 변경, 네비메쉬 정지, 소멸 코루틴 시작
            // 공격 모션중에 애니메이터의 속도를 늦춰 슬로우를 주는 곳이 있는데 그때 죽으면 죽는 모션도 슬로우 처리 됨. 그래서 애니메이터 속도를 다시 올려줌.
            mAnimator.speed = 1.0f;
            mIsDead = true;
            mAnimator.SetBool("Death", true);
            mNavMeshAgent.enabled = false;
            StartCoroutine(Extinction());
            return;
            // 죽은 후에 모든 동작을 차단해야 하는데
            // 지금 그냥 애니메이터 끄고, Update함수 바로 return되게 mIsDead변수만 바꿔줬는데 이게 차후에
            // 문제가 생길지도 모르겠음.. 이거와 관련된 오류가 난다면 나중에 이 컴퍼넌트 Active를 false로 해볼것.
        }

        

        // 몬스터의 상태 판단.
        SetEnemy1State();

        // 일반공격 쿨타임 증가
        mAttackCoolCur += Time.deltaTime;

        // 상태의 따른 행동.
        switch (mEnemy1State)
        {
            case Enemy1State.IDLE:
                mNavMeshAgent.enabled=false;
                mAnimator.SetBool("Walk", false);
                mAnimator.SetBool("Slash", false);
                mAnimator.SetBool("Idle", true);
                break;
            case Enemy1State.CHASE:
                mNavMeshAgent.enabled = true;
                mAnimator.SetBool("Idle", false);
                mAnimator.SetBool("Slash", false);
                mAnimator.SetBool("Walk", true);
                mNavMeshAgent.SetDestination(mPlayer.position);
                break;
            case Enemy1State.ATTACK:
                mNavMeshAgent.enabled = false;
                mAnimator.SetBool("Walk", false);
                mAnimator.SetBool("Idle", false);
                mAnimator.SetBool("Slash", true);
                break;
            case Enemy1State.DAMAGED:
                mNavMeshAgent.enabled = false;
                break;
            case Enemy1State.DIE:
                mNavMeshAgent.enabled = false;
                break;

        }


    }

    void SetEnemy1State()
    {
        // 거리가 NavMesh의 정지거리보다 멀고, 시야 안에 들어오면 추격상태.
        if (Vector3.Distance(gameObject.transform.position, mPlayer.position) > mNavMeshAgent.stoppingDistance &&
            Vector3.Distance(gameObject.transform.position, mPlayer.position) < mSight &&
            mIsAttack == false)
        {
            mEnemy1State = Enemy1State.CHASE;
        }

        // 쿨타임이 끝났으면 공격상태.
        else if (mAttackCoolCur > mAttackCoolTime && mIsAttack == false &&
            Vector3.Distance(gameObject.transform.position, mPlayer.position) <= mNavMeshAgent.stoppingDistance
            )
        {
            // 몹이 공격할때 플레이어 방향으로 확 돌도록 설정해놨는데 어색하면 바꿔줄것.
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            gameObject.transform.rotation = Quaternion.LookRotation(mPlayer.position - gameObject.transform.position);
            mEnemy1State = Enemy1State.ATTACK;
            mIsAttack = true;
        }


        // IDLE상태.
        else if (mIsAttack == false)
        {
            mEnemy1State = Enemy1State.IDLE;
        }
    }

    // 공격끝났을때 BossAniController에 의해 호출됨. 공격변수 초기화하고, 쿨타임 초기화함.
    void AttackEnd()
    {
        mIsAttack = false;
        mAttackCoolCur = 0.0f;
        
    }

    // 죽을때 실행되고 지정된 시간 후에 오브젝트 제거.
    IEnumerator Extinction()
    {
        yield return new WaitForSeconds(3.7f);
        Destroy(gameObject);
        
    }

    // 데미지 입었을때 호출 되는 변수. 강 AttackCollider에 붙은 Hit Ctrl에 의해 호출됨. 체력 계산후 체력바 갱신.
    public void GetDamaged(int num)
    {
        if (mIsDead)
            return;
        //else if (mIsDamaged)
        //    return;

        mHp -= num;
        mHpSlider.value = mHp/mFullHp;
        
        
        GameObject go = Instantiate(mHitParticlePrefab,gameObject.transform);
        StartCoroutine(ParticleRoutine(go));
        
    }

    IEnumerator ParticleRoutine(GameObject go)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(go);
    }

    public int GetAttackDamage()
    {
        return mEnemy1AtkDamage;
    }
}


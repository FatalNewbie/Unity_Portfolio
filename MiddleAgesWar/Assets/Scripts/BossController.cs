using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{

    /*
    XXXXCoolTime       : 쿨타임. 
    XXXXCoolingCnt     : 쿨링 카운트. 공격시 쿨링 카운트를 0으로 만든 후, 쿨링카운트가 쿨타임과 같아질때까지 카운트 함. 
    */
    [SerializeField] float mNormalAttackCoolTime;
    float mNormalAttackCoolingCnt;
    [SerializeField] float mSlashCoolTime;
    float mSlashCoolingCnt;
    [SerializeField] float mJumpAttackCoolTime;
    float mJumpAttackCoolingCnt;

    // 점프공격의 점프 높이와 점프 속도.
    [SerializeField] float mJumpAttackHeight;
    [SerializeField] float mJumpAttackSpeed;

    // 각 공격 데미지
    [SerializeField] int mNormalAttackDamage;
    [SerializeField] int mSlashAttackDamage;
    [SerializeField] int mJumpAttackDamage;

    // 보스 체력
    [SerializeField] int mBossHp;

    NavMeshAgent mNavMeshAg;
    GameObject mPlayer;
    GameObject mBoss;
    Animator mAnimator;
    BossState mBossState;
    bool IsAttacking;                   // 공격중을 나타내는 변수.
    bool IsDeath;                       // 생사여부 부울변수.
    
    Vector3 mJumpAttackPoint;           // 점프공격시 점프로 날아갈 위치.
    GameObject mJumpAttackRange;
    Rigidbody mRgb;

    Vector3 mvJumpMid;                   // 점프공격에 필요한 벡터. 보스의 위치와 점프로 날아갈 위치의 중간을 나타냄.
    Vector3 mvJumpCos;                   // 점프공격에 필요한 벡터. x좌표를 계산하는데 필요한 벡터로 Cos계산에 필요함. 보스의 위치와 점프로 날아갈 위치의 중간에서 보스의 위치와 점프로 날아갈 위치를 번갈아가며 가르킴.
    float mAngle;                         // 점프공격 Cos에 들어갈 각.
    bool mJumpAttackEnd;                  // 점프공격이 끝났는지 판별하는 변수.

    enum BossState
    {
        IDLE,               // IDLE상태
        TRACING,            // 추적상태
        PUNCH,              // 일반공격
        SLASH,              // 강한공격
        ROAR,               // 함성지르기
        JUMPATTACKREADY,    // 점프어택 준비
        JUMPATTACK          // 점프어택
    }
    
    // Use this for initialization
    void Start()
    {
        mNavMeshAg = gameObject.GetComponentInChildren<NavMeshAgent>();
        mAnimator = gameObject.GetComponentInChildren<Animator>();
        mPlayer = GameObject.Find("PlayerObj_B");
        mBoss = GameObject.Find("BossObj");
        mBossState = BossState.IDLE;
        IsAttacking = false;
        IsDeath = false;
        //////////////////////////////////////////////////////////////////
        // 시작시 쿨링카운트 조정용
        mNormalAttackCoolingCnt = 0.0f;
        mSlashCoolingCnt = 0.0f;
        mJumpAttackCoolingCnt = 0.0f;
        //////////////////////////////////////////////////////////////////
        //
        mJumpAttackPoint = new Vector3(0, 0, 0);
        mJumpAttackRange = GameObject.Find("MonsterJumpAttackRange");
        mRgb = gameObject.GetComponentInChildren<Rigidbody>();
        mAngle = 0.0f;
        mJumpAttackEnd = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {

        if(IsDeath)
            return;

        if (mBossHp <= 0)
        {
            IsDeath = true;
            mAnimator.SetBool("IsDeath",true);
        }

        

        // 쿨링카운트 증가
        mNormalAttackCoolingCnt += Time.deltaTime;
        mSlashCoolingCnt += Time.deltaTime;
        mJumpAttackCoolingCnt += Time.deltaTime;

        //상황을 판별해 적절한 mBossState 부여.
        StateDetermination();

        //mBossState = BossState.IDLE;

        // mBossState에 맞는 행동 취함.
       BossAction();
        
        

    }


    // 매 프레임마다 몬스터의 State를 결정해주는 함수.
    void StateDetermination()
    {

        // 공격 중 아닐 때 플레이어 방향 바라보기
        if (!IsAttacking)
        {
            Quaternion targetRot = Quaternion.identity;
            targetRot.SetLookRotation(mPlayer.transform.position - mBoss.transform.position);
            mBoss.transform.rotation = Quaternion.Lerp(mBoss.transform.rotation, targetRot, 0.08f);
        }



        //mBoss.LookAt(mPlayer);





        //transform.rotation.SetLookRotation(Vector3.Lerp(mBoss.transform.localEulerAngles,mBoss.transform.position - mPlayer.transform.position, 0.9f));


        // 점프 공격 체크
        if (!(IsAttacking) && // 공격중이 아닐때
            (mJumpAttackCoolingCnt >= mJumpAttackCoolTime)
            )
        {
            mBossState = BossState.JUMPATTACKREADY;
        }

        // 추적 체크
        else if (Vector3.Distance(mBoss.transform.position, mPlayer.transform.position) >= mNavMeshAg.stoppingDistance && // 나브메쉬 정지거리보다 멀때 &&
            !(IsAttacking) // 공격중이 아닐때
            )
        {
            mBossState = BossState.TRACING;
        }

        // 슬래쉬 공격 체크
        else if (Vector3.Distance(mBoss.transform.position, mPlayer.transform.position) <= mNavMeshAg.stoppingDistance && // 나브메쉬 정지거리보다 가까울때 &&
            (mSlashCoolingCnt >= mSlashCoolTime) && // 현재 쿨타임이 정해진 쿨타임을 충족했을 때  &&
            !(IsAttacking) // 공격중이 아닐때
            )
        {
            mBossState = BossState.SLASH;
        }

        // 일반공격 체크
        else if (Vector3.Distance(mBoss.transform.position, mPlayer.transform.position) <= mNavMeshAg.stoppingDistance && // 나브메쉬 정지거리보다 가까울때 &&
            (mNormalAttackCoolingCnt >= mNormalAttackCoolTime) && // 현재 쿨타임이 정해진 쿨타임을 충족했을 때 &&
            !(IsAttacking) // 공격중이 아닐때
            )
        {
            mBossState = BossState.PUNCH;
        }

        // 아무것도 아닐때 아이들.
        else if (!(IsAttacking))
        {
            mBossState = BossState.IDLE;
        }
    }

    void BossAction()
    {
        switch (mBossState)
        {
            case BossState.IDLE:

                mNavMeshAg.enabled = false;

                mAnimator.SetInteger("State", 0);

                break;
            case BossState.TRACING:
                mNavMeshAg.enabled = true;
                mAnimator.SetInteger("State", 1);
                mNavMeshAg.SetDestination(mPlayer.transform.position);
                //mNormalAttackCoolingCnt = mNormalAttackCoolTime;
                break;
            case BossState.PUNCH:
                IsAttacking = true;
                mNavMeshAg.enabled = false;
                mAnimator.SetBool("IsPunch", true);
                break;
            case BossState.SLASH:

                IsAttacking = true;
                mNavMeshAg.enabled = false;
                mAnimator.SetBool("IsSlash", true);

                break;
            case BossState.ROAR:
                IsAttacking = true;
                mNavMeshAg.enabled = false;
                break;
            case BossState.JUMPATTACKREADY:
                if (!IsAttacking)
                    StartCoroutine(JumpReady());
                mJumpAttackEnd = false;
                IsAttacking = true;
                mNavMeshAg.enabled = false;
                mAnimator.SetBool("IsJumpAttack", true);
                break;
            case BossState.JUMPATTACK:

                if (!mJumpAttackEnd)
                {
                    //점프어택 메소드
                    JumpAttack();
                }
                break;
        }
    }

    //스킬이 끝난 후 호출 되는 함수.
    void SkillEnd(int attackType)
    {

        IsAttacking = false;
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
                mAnimator.SetBool("IsPunch", false); // 애니메이터 변수 false.
                mNormalAttackCoolingCnt = 0.0f; // 기본공격 쿨타임 초기화.
                break;
            case 2:
                mAnimator.SetBool("IsSlash", false);
                mSlashCoolingCnt = 0.0f; // 슬래쉬 공격 쿨타임 초기화.
                mNormalAttackCoolingCnt = 0.0f;
                break;
            case 3:
                mAnimator.SetBool("IsJumpAttack", false);
                mJumpAttackRange.GetComponent<MeshRenderer>().enabled = false;
                mJumpAttackCoolingCnt = 0.0f;
                mNormalAttackCoolingCnt = 0.0f;
                mRgb.velocity = new Vector3(0, 0, 0);
                break;

        }



        StateDetermination ();
    }

    // 캐스팅 시작.
    // 애니메이터 속도를 늦춤. 애니메이터 이벤트 쪽에서 호출되는 함수임.
    // 점프공격중에 3번 호출이 되는데 처음에 웅크리는 때랑 체공중일때, 착지후 자연스럽게 하기 위해 속도를 늦추기위해 애니메이터 이벤트로 호출함.
    void CastingStart()
    {
        mAnimator.speed = 0.2f;
    }

    // 캐스팅 끝.
    void CastingEnd(int attackType)
    {
        /*
         점프공격일때는 jumpPoint지정을 위해 별도의 함수 호출.
         */

        mAnimator.speed = 1.0f;

        if (attackType == 5)
        {
            mJumpAttackPoint = mPlayer.transform.position;
            //JumpAttackCalculate();

        }

    }

    // 웅크리는 모션을 위해 1.5초 후에 mBossState를 JUMPATTACK로 바꿔줌.
    // 여기서 뛸 위치를 설정.
    IEnumerator JumpReady()
    {
        yield return new WaitForSeconds(1.5f);
        mBossState = BossState.JUMPATTACK;

        // 점프할 위치를 가리키는 벡터에 플레이어의 포지션을 가져옴.
        mJumpAttackPoint = mPlayer.transform.position;
        // 점프할 위치를 바라봄.
        mBoss.transform.rotation = Quaternion.LookRotation(mJumpAttackPoint - mBoss.transform.position);
        // 필요한 벡터 계산.
        mvJumpMid = (mBoss.transform.position + mPlayer.transform.position) / 2f;
        mvJumpCos = mBoss.transform.position - mvJumpMid;

        //점프 공격 범위를 옮김.
        mJumpAttackRange.transform.position = mJumpAttackPoint;
        mJumpAttackRange.GetComponent<MeshRenderer>().enabled = true;

        //점프 시작이므로 늦춰놓았던 애니메이션 속도를 다시 정상으로.
        mAnimator.speed = 1.0f;
        
        

    }


    void JumpAttack()
    {
        

        if (mBoss.transform.position.y > 0.0f)
        {
            mBoss.transform.position = mvJumpCos * Mathf.Cos(mAngle) + mJumpAttackHeight * Vector3.up * Mathf.Sin(mAngle) + mvJumpMid;
            mAngle += mJumpAttackSpeed * Time.deltaTime;
        }
        else
        {
            mJumpAttackEnd = true;
            // 체공중에 늦춰놓았던 스피드를 다시 정상으로 돌림.
            mAnimator.speed = 1.0f;

           


            StartCoroutine(JumpAttackEnd());
        }

    }

    // 땅에 착지 후에 잠깐 대기 시간을 갖기 위해 코루틴 돌릶.
    IEnumerator JumpAttackEnd()
    {
        yield return new WaitForSeconds(2.0f);

        // 다시 공격 루틴을 돌리기 위해서 공격중을 false로 전환.
        IsAttacking = false;

        // 점프어택 완료를 애니메이터에게 알려줌.
        mAnimator.SetBool("IsJumpAttack", false);
        
        // 바닥에 빨간색 표시 제거.
        mJumpAttackRange.GetComponent<MeshRenderer>().enabled = false;

        // 공격들 쿨타임 초기화.
        mJumpAttackCoolingCnt = 0.0f;
        mNormalAttackCoolingCnt = 0.0f;

        // 완전 끝나고 늦춰진 속도를 다시 전환.
        mAnimator.speed = 1.0f;

        // angle을 0으로.
        mAngle = 0;
    }

    public int GetAttackDamage(string type)
    {
        if (type == "normal")
        {
            return mNormalAttackDamage;
        }
        else if (type == "slash")
        {
            return mSlashAttackDamage;

        }
        else if (type == "jump")
        {
            return mJumpAttackDamage;
        }

        return 0;

    }

    public void GetDamaged(int Damage)
    {
        mBossHp -= Damage;
    }

    //void JumpVelocityZero()
    //{
    //    mRgb.velocity = new Vector3(0, 0, 0);
    //}

    //void JumpAttackCalculate()
    //{
    //    mJumpAttackRange.transform.position = mJumpAttackPoint;
    //    mJumpAttackRange.GetComponent<MeshRenderer>().enabled = true;

    //    Vector3 jumpDirection = (mPlayer.position - mBoss.position).normalized;
    //    float mDistance = Vector3.Distance(mPlayer.position, mBoss.position);
    //    float time = 70.0f;
    //    mRgb.velocity = jumpDirection * mDistance / time * 60.0f;
    //}


}

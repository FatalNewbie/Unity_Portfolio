using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] float mMoveSpeed;              // 캐릭터 이동속도
    float mRotSpeed;                                // 캐릭터 회전속도
    Animator mPlayerAnimator;                       // 캐릭터 애니메이터
    bool mIsDead;                                   //  
    bool mIsFall;                                   // 넘어져 있는지 확인하는 변수.
    float mNormalAtkCnt;                            // 노말어택
    float mPowerAtkCnt;                             //
    bool mIsAttack;                                 // 공격중 판단하는 변수
    int mComboAtkState;                             // 콤보어택 현 상황 판단용 변수
    StickObj mStick;                                // 조이스틱
    Transform mPlayer = null;                              // 플레이어 위치
    Vector3 mToLook;                                // 
    AtkBtnObj mAtkBtn;                              // 공격 버튼
    int a = 0;                                      //
    int b = 0;                                      //
    int c = 0;                                      //
    [SerializeField] int mAttackDamage;             // 공격 데미지
    [SerializeField] int mPlayerHp;                 // 플레이어 체력
    Rigidbody mRigid;                               // 플레이어 리지드바디
    [SerializeField] Camera mMainCamera = null;                             // 메인카메라
    Vector3 exToLook;                                 // 마지막으로 바라본 방향. 조이스틱을 터치하지 않을때 마지막으로 바라본 곳을 계속 바라보게 하기 위함.

    private void Awake()
    {
        mIsDead = false;
        // mMoveSpeed = 5.0f;
        // mRotSpeed = 150.0f;
        mNormalAtkCnt = 0.0f;
        mPowerAtkCnt = 0.0f;
        mIsAttack = false;
        mComboAtkState = 1;
        mToLook = new Vector3(0, 0, 0);
        mAtkBtn = GameObject.Find("AtkBtn").GetComponent<AtkBtnObj>();
        mIsFall = false;
    }

    // Use this for initialization
    void Start () {
        mPlayerAnimator = GetComponentInChildren<Animator>();
        if (mPlayerAnimator == null)
            Debug.Log("mPlayerAnimator is null");

        GameObject go = GameObject.Find("StickBG");
        if (go == null)
            Debug.Log("InGameFrame is null");
        mStick = go.GetComponent<StickObj>();

        mPlayer = GameObject.Find("PlayerModel").GetComponent<Transform>();
        if (mPlayer == null)
            Debug.Log("mPlayer is null");

        mMainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (mMainCamera == null)
            Debug.Log("mMainCamera is null");

        mRigid = gameObject.GetComponent<Rigidbody>();

        exToLook = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {

        if (mIsDead)
            return;

        if (mPlayerHp <= 0.0f)
        {
            mIsDead = true;
            mPlayerAnimator.SetBool("Death", true);
            return;
        }

        //mz, mx 변화에 따라 방향 측정
        //float mz = Input.GetAxis("Vertical");
        //float mx = Input.GetAxis("Horizontal");

        if (mIsFall)
            return;

#if UNITY_EDITOR
        float x = mStick.GetVerVal();
        float y = mStick.GetHorVal();
        Vector3 toGo = new Vector3(y, 0, x);

        if (Input.GetKey(KeyCode.W))
            toGo += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            toGo += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            toGo += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            toGo += Vector3.right;

#else
        float x = mStick.GetVerVal();
        float y = mStick.GetHorVal();
        Vector3 toGo = new Vector3(y, 0, x);
#endif



            if (toGo != Vector3.zero)
        {
            mToLook = toGo;
            exToLook = toGo;
        }
        else
        {
            mToLook = exToLook;
        }
        toGo.Normalize();
        mToLook.Normalize();
        
        if (!mIsAttack)
        {

            mRigid.velocity = Quaternion.Euler(0, -(mMainCamera.GetComponent<MainCameraController>().GetmDelta()+90), 0) * toGo * mMoveSpeed * Time.deltaTime;
            
            //mRigid.AddForce(toGo  * mMoveSpeed * Time.deltaTime);
            //mPlayer.Rotate(new Vector3(0,0,1) * mRotSpeed * Time.deltaTime);
            //mPlayer.rotation = Quaternion.LookRotation(mToLook);
            mPlayer.rotation = Quaternion.LookRotation(Quaternion.Euler(0, -(mMainCamera.GetComponent<MainCameraController>().GetmDelta()+90), 0) * mToLook);
            
            //mRigid.velocity = Quaternion.Euler(0, 10, 0) * toGo;

            

        }

        // 원본.------------------------------------------------------------------------------
        //if (!mIsAttack)
        //{
        //    transform.Translate(toGo * mMoveSpeed * Time.deltaTime);
        //    //mPlayer.Rotate(new Vector3(0,0,1) * mRotSpeed * Time.deltaTime);
        //    mPlayer.rotation = Quaternion.LookRotation(mToLook);

        //}
        // 여기까지 --------------------------------------------------------------------------

        //if (mz < 0)
        //{
        //    if (!mIsAttack)
        //    {
        //        mPlayer.Translate(Vector3.forward * mz * mMoveSpeed * Time.deltaTime);
        //        mPlayer.Rotate(Vector3.up * mx * mRotSpeed * Time.deltaTime);
        //    }
        //}
        //else
        //{
        //    if (!mIsAttack)
        //    {
        //        mPlayer.Translate(Vector3.forward * mz * mMoveSpeed * Time.deltaTime);
        //        mPlayer.Rotate(Vector3.up * mx * mRotSpeed * Time.deltaTime);
        //    }
        //}



        if (toGo == Vector3.zero || mIsAttack)
        {
            mPlayerAnimator.SetInteger("State", 0);
        }
        else
        {
            mPlayerAnimator.SetInteger("State", 1);
        }

        // 일반 공격
        if (mAtkBtn.IsClick && (!mIsAttack))
        {
            mIsAttack = true;
            SlashState();
            // 공격 버튼이 눌렸으니 velocity를 0로 만들어줌.
            mRigid.velocity = new Vector3(0,0,0);
        }

        // 특수 공격
        //if (Input.GetButtonUp("Fire2") && (!mIsAttack))
        //{
        //    mIsAttack = true;
        //    mComboAtkState = 4;
        //}

        //if (mIsAttack)
        //    switch (mComboAtkState)
        //    {
        //        case 1:
        //            Slash_1();
        //            break;
        //        case 2:
        //            Slash_2();
        //            break;
        //        case 3:
        //            Slash_3();
        //            break;
        //        case 4:
        //            PowerSlash();
        //            break;
        //        default:
        //            break;
        //    }


        //        Debug.Log(mNormalAtkCnt);





    }

    void SlashState()
    {

        // 공격중 상태변수 true로.
        mIsAttack = true;
        // 애니메이터의 공격중 변수를 true로.
        mPlayerAnimator.SetBool("Attacking", true);
        /* 현재 캐릭터의 슬래쉬 단계 상태를 받아옴.
         0 : 공격중이 아니었음.
         1 : 1번째 콤보공격을 하는중.
         2 : 2번째 콤보공격을 하는중.
         3 : 3번째 콤보공격을 하는중.
         */
        int nowSlashState = mPlayerAnimator.GetInteger("Attack");


        if (nowSlashState == 0)
        {
            mPlayerAnimator.SetInteger("Attack", ++nowSlashState);
            mAtkBtn.IsClick = false;
        }
        else if (nowSlashState == 1 && mAtkBtn.IsClick == true)
        {
            mPlayerAnimator.SetInteger("Attack", ++nowSlashState);
            mAtkBtn.IsClick = false;
        }
        else if (nowSlashState == 2 && mAtkBtn.IsClick == true)
        {
            mPlayerAnimator.SetInteger("Attack", ++nowSlashState);
            mAtkBtn.IsClick = false;
        }
        else
        {
            mIsAttack = false;
            mPlayerAnimator.SetBool("Attacking", false);
            mPlayerAnimator.SetInteger("Attack", 0);
            mAtkBtn.IsClick = false;
        }



    }


    // 공격버튼으로 공격
    void Slash_1()
    {
        mPlayerAnimator.SetBool("Attacking", true);
        mNormalAtkCnt += Time.deltaTime;
        mPlayerAnimator.SetInteger("Attack", 1);
        Debug.Log(mAtkBtn.IsClickAgain);
        if ((mNormalAtkCnt >= 0.2f) && (mNormalAtkCnt <= 1.35f) && mAtkBtn.IsClickAgain)
        {
            mComboAtkState = 2;
            mNormalAtkCnt = 0.0f;
            mAtkBtn.IsClickAgain = false;
            //mAtkBtn.IsClick = false;
        }
        if (mNormalAtkCnt > 1.35f)
        {
            mPlayerAnimator.SetBool("Attacking", false);
            mIsAttack = false;
            mNormalAtkCnt = 0.0f;
            mAtkBtn.IsClickAgain = false;
            mAtkBtn.IsClick = false;
            
        }
    }

    void Slash_2()
    {
        mNormalAtkCnt += Time.deltaTime;
        mPlayerAnimator.SetInteger("Attack", 2);
        if ((mNormalAtkCnt <= 1.2f) && (Input.GetButtonUp("Fire1")))
        {
            mComboAtkState = 3;
            mNormalAtkCnt = 0.0f;
            mAtkBtn.IsClickAgain = false;
        }
        if (mNormalAtkCnt > 1.2f)
        {
            mPlayerAnimator.SetBool("Attacking", false);
            mComboAtkState = 1;
            mIsAttack = false;
            mNormalAtkCnt = 0.0f;
            mAtkBtn.IsClickAgain = false;
            mAtkBtn.IsClick = false;
        }
    }

    void Slash_3()
    {
        mNormalAtkCnt += Time.deltaTime;
        mPlayerAnimator.SetInteger("Attack", 3);
        if (mNormalAtkCnt > 1.00f)
        {
            mPlayerAnimator.SetBool("Attacking", false);
            mNormalAtkCnt = 0.0f;
            mComboAtkState = 1;
            mIsAttack = false;
            mAtkBtn.IsClickAgain = false;
            mAtkBtn.IsClick = false;
        }
    }

    // 마우스 클릭으로 공격
    //void Slash_1()
    //{
    //    mNormalAtkCnt += Time.deltaTime;
    //    mPlayerAnimator.SetInteger("Attack", 1);
    //    mPlayerAnimator.SetBool("Attacking", true);
    //    if ((mNormalAtkCnt >= 0.2f )&&(mNormalAtkCnt <= 1.35f) && (Input.GetButtonUp("Fire1")))
    //    {
    //        mComboAtkState = 2;
    //        mNormalAtkCnt = 0.0f;
    //    }
    //    if (mNormalAtkCnt > 1.35f)
    //    {
    //        mIsAttack = false;
    //        mPlayerAnimator.SetBool("Attacking", false);
    //        mNormalAtkCnt = 0.0f;
    //    }
    //}

    //void Slash_2()
    //{
    //    mNormalAtkCnt += Time.deltaTime;
    //    mPlayerAnimator.SetInteger("Attack", 2);
    //    if ((mNormalAtkCnt <= 1.2f) && (Input.GetButtonUp("Fire1")))
    //    {
    //        mComboAtkState = 3;
    //        mNormalAtkCnt = 0.0f;
    //    }
    //    if (mNormalAtkCnt > 1.2f)
    //    {
    //        mComboAtkState = 1;
    //        mIsAttack = false;
    //        mPlayerAnimator.SetBool("Attacking", false);
    //        mNormalAtkCnt = 0.0f;
    //    }
    //}

    //void Slash_3()
    //{
    //    mNormalAtkCnt += Time.deltaTime;
    //    mPlayerAnimator.SetInteger("Attack", 3);
    //    if (mNormalAtkCnt > 1.00f)
    //    {
    //        mNormalAtkCnt = 0.0f;
    //        mComboAtkState = 1;
    //        mIsAttack = false;
    //        mPlayerAnimator.SetBool("Attacking", false);
    //    }
    //}

    void PowerSlash()
    {
        mPlayerAnimator.SetInteger("PowerAttack", 1);
        mPlayerAnimator.SetBool("Attacking", true);
        mNormalAtkCnt += Time.deltaTime;
        if (mNormalAtkCnt > 2.00f)
        {
            mNormalAtkCnt = 0.0f;
            mIsAttack = false;
            mPlayerAnimator.SetBool("Attacking", false);
            mPlayerAnimator.SetInteger("PowerAttack", 0);
        }
    }

    public int GetAttackDamage()
    {
        return mAttackDamage;
    }

    public void GetDamaged(int num)
    {
        mPlayerHp -= num;
   
    }

    // 플레이어가 몬스터의 점프공격 범위 안에서 점프공격을 당했을때 호출되는 함수.
    public void PlayerFall()
    {
        mRigid.velocity = new Vector3(0, 0, 0); // 리지드바디의 velocity를 0으로.
        mIsFall = true;                         // 넘어짐 플래그 함수 true로.
        mPlayerAnimator.SetBool("Fall", true);  // 애니메이터의 Fall 함수를 true로.
    }

    void PlayerStandUp()
    {
        mIsFall = false;
        mPlayerAnimator.SetBool("Fall", false);
    }
}

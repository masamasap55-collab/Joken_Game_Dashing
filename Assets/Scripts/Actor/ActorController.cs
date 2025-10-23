using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// アクター操作・制御クラス
/// </summary>
public class ActorController : MonoBehaviour
{
    //変数宣言部
    private bool isWalking = false;

    //オブジェクト・コンポーネント参照
    private Rigidbody2D rigidbody2D; //このオブジェクトのRigidbody2Dコンポーネントを参照。(まだ入れ物で中身は空っぽ)
    private SpriteRenderer spriteRenderer; //spriteRenderコンポーネントを参照。
    private ActorGroundSensor groundSensor; //接地判定を見るscriptコンポーネントを参照。
    private ActorSprite actorSprite; //アクタースプライト設定クラス
    public CameraController cameraController; //カメラ制御クラス (今回はGetComponentをしてないのでこの変数にInspectorからセット参照してる)
    public GameObject weaponBulletPrefab; //弾プレハブ
    private AudioSource audioSource; //オーディオ   

    //設定項目
    [Header("true:足滑るモード")]
    public bool icyGroundMode;

    //　体力変数
    [HideInInspector] public int nowHP; // 現在HP
    [HideInInspector] public int maxHP; // 最大HP

    //移動関連変数
    [HideInInspector] public float xSpeed; //このオブジェクトのx方向の移動速度(xベクトルの値)
    [HideInInspector] public bool rightFacing; //このオブジェクトの向いてる向き(右:true 左: false)
    private float remainJumpTime; // 空中でのジャンプ入力残り受付時間

    //その他変数
    private float remainStuckTime; //残り硬直時間(0以上だと行動できない)
    private float invincibleTime; //残り無敵時間(秒)
    [HideInInspector] public bool isDefeat; // true:撃破された(ゲームオーバー)
    [HideInInspector] public bool inWaterMode; //true:水中モード（メソッドから変更）
    public float stepInterval = 0.1f; //足音の間隔
    private Coroutine footstepCoroutine;

    //各種効果音
    [Header("ジャンプ音")] public AudioClip jumpSound;
    [Header("やられ音")] public AudioClip damagedSound;
    [Header("あるき音")] public AudioClip walkSound;

    //定数定義
    private const int InitialHP = 20; //　初期HP(最大HP)
    private const float InvicibleTime = 2.0f; // 被ダメージ直後の無敵時間(秒)
    private const float StuckTime = 0.5f; // 被ダメージ直後の硬直時間(秒)
    private const float KnockBack_X = 2.5f; // 被ダメージ時ノックバック(x方向)
    private const float WaterModeDeceletate_X = 0.8f; // 水中でのX方向速度倍率
    private const float WaterModeDeceletate_Y = 0.92f; //水中でのy方向速度倍率  

    // {Start} オブジェクト有効時　１回だけ実行されるメソッド
    void Start()
    {
        //コンポーネント参照取得。(ここで初めてこのオブジェクトのコンポーネントが変数に入る)
        rigidbody2D = GetComponent<Rigidbody2D>(); //これでrigidbody2D === このオブジェクトのコンポーネント となっている。(ポインタ参照みたいなもん)
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundSensor = GetComponentInChildren<ActorGroundSensor>(); // InChildrenで子オブジェクトからコンポーネントを参照取得。
        actorSprite = GetComponent<ActorSprite>();
        audioSource = GetComponent<AudioSource>();

        //配下コンポーネントを初期化　(先にActorControllerを初期化したいからここでActorSpriteを初期化している)
        actorSprite.Init(this); //thisでこのscriptをコンポーネントとして渡せる。

        //カメラ初期位置
        cameraController.SetPosition(transform.position);

        //変数の初期化
        rightFacing = true; //最初は右向き
        nowHP = maxHP = InitialHP; //　初期HP
    }

    // {Update}　１フレームごとに一度ずつ実行されるメソッド
    void Update()
    {
        //撃破された後なら終了
        if (isDefeat)
            return;

        //　無敵時間が残っているなら減少
        if (invincibleTime > 0.0f)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0.0f)
            { //　無敵時間終了時処理
                actorSprite.EndBlinking(); //点滅終了
            }
        }
        //硬直時間処理
        if (remainStuckTime > 0.0f)
        {//硬直時間減少
            remainStuckTime -= Time.deltaTime;
            if (remainStuckTime <= 0.0f)
            {// スタン時間終了時処理
                actorSprite.stuckMode = false;
            }
            else
                return;
        }



        //左右移動のメソッドを呼び出し
        MoveUpdate();
        //ジャンプのメソッドを呼び出し
        JumpUpdate();

        //攻撃入力処理
        StartShotAction();

        //坂道で滑らなくなる処理
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation; //rigidbody2Dの回転を常時停止
        if (groundSensor.isGround && !Input.GetKey(KeyCode.UpArrow)) //地上にいるとき　かつ　ジャンプ中でないとき
        {
            if (rigidbody2D.velocity.y > 0.0f) //ジャンプ以外はy方向の移動はしないようにする。(ずっと地面にはりつけている感じ)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
            }
            if (Mathf.Abs(xSpeed) < 0.1f && !icyGroundMode) //地上で移動を止めたらその場で止まる。
            {
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
        }

        //カメラに自身の座標を渡し続ける
        cameraController.SetPosition(transform.position);
    }

    #region 移動関連
    /// <summary>
    /// 左右移動のメソッド
    /// </summary>
    private void MoveUpdate()
    {
        bool moving = false;
        //右
        if (Input.GetKey(KeyCode.RightArrow))
        {
            xSpeed = 6.0f;
            rightFacing = true; //右向きのフラグon
                                //spriteRendererコンポーネントにflipXというものがあり、trueだと初期の画像を反転
            spriteRenderer.flipX = false;
            moving = true;
        }
        //左
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            xSpeed = -6.0f;
            rightFacing = false; //右向きのフラグoff
                                 //trueだと画像反転
            spriteRenderer.flipX = true;
            moving = true;
        }
        //何もしてない
        else
        {
            xSpeed = 0.0f;
        }

        // 足音の再生制御
        if (moving && !isWalking)
        {
            isWalking = true;
            footstepCoroutine = StartCoroutine(PlayFootsteps());
        }
        else if (!moving && isWalking)
        {
            isWalking = false;
            if (footstepCoroutine != null)
                StopCoroutine(footstepCoroutine);
        }
    }

    private void JumpUpdate()
    {
        //空中でのジャンプ入力受付時間を経過秒数で減少
        if (remainJumpTime > 0.0f)
            remainJumpTime -= Time.deltaTime; //Time.deltaTimeで1Fの経過秒数を参照



        //ジャンプ操作
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {//ジャンプ開始
            //接地していないなら終了(水中であれば続行)
            if (!groundSensor.isGround && !inWaterMode)
                return;

            //効果音
            audioSource.PlayOneShot(jumpSound);
            //ジャンプ力を計算
            float jumpPower = 10.0f;
            //rigidbody2Dの速度ベクトルにx軸、y軸のベクトルを新しく代入(jumpPowerは物理演算の重力の影響でどんどん下がっていく)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpPower);

            //飛んでから、0.25秒受付時間設定
            remainJumpTime = 0.25f;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {//ジャンプ中

            if (remainJumpTime <= 0.0f)
                return; //受付時間が残ってないなら終了
            if (groundSensor.isGround)
                return; //接地してるなら終了

            //ジャンプ力加算を計算
            float jumpAddPower = 30.0f * Time.deltaTime; //Time.deltaTimeでFPSによらずに数値の秒数加算が可能。
            //ジャンプ力加算を適応
            rigidbody2D.velocity += new Vector2(0.0f, jumpAddPower);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {//ジャンプ入力終了
            remainJumpTime = -1.0f;
        }



    }

    //{FixedUpdate} 物理演算をするたびに実行されるメソッド
    private void FixedUpdate()
    {
        //velocity=速度 現在の
        // 移動速度ベクトルを取得(x,yどっちも)
        Vector2 velocity = rigidbody2D.velocity;
        // 変数velocityのxベクトルにxSpeedの値を代入。
        velocity.x = xSpeed;
        //　氷床ステージなら接地時に滑るような速度設定にする
        if (icyGroundMode && groundSensor.isGround)
            velocity.x = Mathf.Lerp(xSpeed, rigidbody2D.velocity.x, 0.99f);

        //水中モードでの速度
        if (inWaterMode)
        {
            velocity.x *= WaterModeDeceletate_X;
            velocity.y *= WaterModeDeceletate_Y;
        }

        // このオブジェクトのrigidbody2Dコンポーネントにvelocityを適応
        rigidbody2D.velocity = velocity;
    }

    /// <summary>
    /// 水中モードをセットする
    /// </summary>
    /// <param name="mode">true:水中にいる</param>
    public void SetWaterMode(bool mode)
    {
        //水中モード
        inWaterMode = mode;
        // 水中での重力
        if (inWaterMode)
        {
            rigidbody2D.gravityScale = 0.3f;
        }
        else
        {
            rigidbody2D.gravityScale = 1.0f;
        }
    }

    #endregion

    #region 戦闘関連

    ///<summary>
    /// ダメージを受ける際に呼び出される
    ///</sumary>
    /// <param name="damage">ダメージ量</param>
    public void Damaged(int damage)
    {
        //撃破された後なら終了
        if (isDefeat)
            return;

        //もし無敵時間中ならダメージ無効
        if (invincibleTime > 0.0f)
            return;

        //ダメージ処理
        nowHP -= damage;
        //効果音
        audioSource.PlayOneShot(damagedSound);

        //HP0ならゲームオーバー処理
        if (nowHP <= 0)
        {
            isDefeat = true;
            // 被撃破演出開始
            actorSprite.StartDefeatAnim();
            //物理演算を停止
            rigidbody2D.velocity = Vector2.zero;
            xSpeed = 0.0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        //スタン硬直
        remainStuckTime = StuckTime;
        actorSprite.stuckMode = true;

        //ノックバック処理
        //ノックバック力・方向決定
        float knockBackPower = KnockBack_X;
        if (rightFacing)
            knockBackPower *= -1.0f;
        //　ノックバック適用
        xSpeed = knockBackPower;

        //無敵時間発生
        invincibleTime = InvicibleTime;
        if (invincibleTime > 0.0f)
            actorSprite.StartBlinking();
    }





    /// <summary>
    /// 戦闘ボタン入力時処理
    /// </summary>
    public void StartShotAction()
    {
        //　攻撃ボタンが入力されていないなら終了
        if (!Input.GetKeyDown(KeyCode.Z))
            return;

        // このメソッド内で選択武器別のメソッドの呼び分けやエネルギー消費処理を行う。
        // 現在は初期武器のみなのでShotAction_Normalを呼び出すだけ
        ShotAction_Normal();
    }

    /// <summary>
    /// 射撃アクション：通常攻撃
    /// </summary>
    private void ShotAction_Normal()
    {
        //　弾の方向を取得
        float bulletAngle = 0.0f; //右向き
        // アクターが左向きなら弾も左向きに進む
        if (!rightFacing)
            bulletAngle = 180.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate( // オブジェクト新規生成
            weaponBulletPrefab, //生成するオブジェクトのプレハブ
            transform.position, //生成したオブジェクトの初期座標
            Quaternion.identity);  //初期Rotation(傾き)
        //弾丸設定
        obj.GetComponent<ActorNormalShot>().Init(
            12.0f,  //速度
            bulletAngle, // 角度
            1, //ダメージ量
            5.0f); //存在時間
    }
    
    /// <summary>
    /// 効果音のループメソッド
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayFootsteps()
    {
        while (isWalking)
        {
            // 地面にいる時だけ足音を鳴らす（空中で鳴らさないように）
            if (groundSensor.isGround)
                audioSource.PlayOneShot(walkSound);

            yield return new WaitForSeconds(stepInterval); // 0.1秒待つ
        }
    }

    #endregion


}

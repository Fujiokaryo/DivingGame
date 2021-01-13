using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Coffee.UIExtensions;
public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed;

    [Header("落下速度")]
    public float fallSpeed;

    [Header("着水判定用。trueなら着水済")]
    public bool inWater;

    public enum AttitudeType
    {
        Straight,
        Prone,
    }

    [Header("現在のキャラの姿勢")]
    public AttitudeType attitudeType;

    
    private Rigidbody rb; 

    private Vector3 straightRotation = new Vector3(180, 0, 0);

    private float x;
    private float z;

    private int score;

    private Vector3 proneRotation = new Vector3(-90, 0, 0);

    private float attitudeTimer;
    private float chargeTime = 2.0f;

    public bool isCharge;

    private Animator anim;

    [SerializeField, Header("水しぶきのエフェクト")]
    private GameObject splashEffectPrefab = null;

    [SerializeField, Header("水しぶきのSE")]
    private AudioClip splashSE = null;

    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Button btnChangeAttitude;

    [SerializeField]
    private Image imgGauge;

    [SerializeField]
    private ShinyEffectForUGUI shinyEffect;

    [SerializeField]
    private Transform limitLeftBottom;

    [SerializeField]
    private Transform limitRightTop;

    [SerializeField]
    private FloatingJoystick joystick;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //初期の姿勢を設定（頭を水平方向に向ける
        transform.eulerAngles = straightRotation;

        attitudeType = AttitudeType.Straight;

        btnChangeAttitude.onClick.AddListener(ChangeAttitude);

        btnChangeAttitude.interactable = false;

        anim = GetComponent<Animator>();
    }

   
    // Update is called once per frame
    void FixedUpdate()
    {
        if (inWater == false)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            x = joystick.Horizontal;
            z = joystick.Vertical;

            //velocityに新しい値を代入して移動
            rb.velocity = new Vector3(x * moveSpeed, -fallSpeed, z * moveSpeed);
        }
          
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Water" && inWater == false)
        {
            inWater = true;

            //水しぶきのエフェクトを生成
            GameObject effect = Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y, effect.transform.position.z - 0.5f);

            Destroy(effect, 2.0f);

            AudioSource.PlayClipAtPoint(splashSE, transform.position);

            StartCoroutine(OutOfWater());
        }

        if (col.gameObject.tag == "FlowerCircle")
        {
            //Debug.Log("花輪ゲット");

            score += col.transform.parent.GetComponent<FlowerCircle>().point;

            txtScore.text = score.ToString();

            //画面に表示されている得点表示を更新する処理を追加する

        }



    }
    /// <summary>
    /// 水面に顔を出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator OutOfWater()
    {
        yield return new WaitForSeconds(1.0f);

        rb.isKinematic = true;

        transform.eulerAngles = new Vector3(-30, 180, 0);

        transform.DOMoveY(4.7f, 1.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && inWater == false)
        {
            ChangeAttitude();
        }

        LimitMoveArea();

        if(isCharge == false && attitudeType == AttitudeType.Straight)
        {
            attitudeTimer += Time.deltaTime;

        

            if (inWater == false)
            {
                imgGauge.DOFillAmount(attitudeTimer / chargeTime, 0.1f);
            }

            btnChangeAttitude.interactable = false;

            if(attitudeTimer >= chargeTime)
            {
                attitudeTimer = chargeTime;

                //チャージ完了状態
                isCharge = true;
                //ボタンを活性化
                if (isCharge == true && inWater == false)
                {
                    btnChangeAttitude.interactable = true;
                }

                shinyEffect.Play(0.5f);
            }
        }

        //姿勢が伏せの状態
        if(attitudeType == AttitudeType.Prone)
        {
            attitudeTimer -= Time.deltaTime;

            //ゲージ表示を更新
            if (inWater == false)
            {
                imgGauge.DOFillAmount(attitudeTimer / chargeTime, 0.1f);
            }

            //タイマーチャージが０になったら
            if(attitudeTimer <= 0)
            {
                //タイマーをリセットして再度計測できる状態にする
                attitudeTimer = 0;

                //ボタンを非活性化
                btnChangeAttitude.interactable = false;

                //強制的に直滑降に戻す
                ChangeAttitude();
            }
        }

       
    }

    private void ChangeAttitude()
    {
        switch (attitudeType)
        {
            //現在の姿勢が直滑降だったら
            case AttitudeType.Straight:

                //未チャージ状態（チャージ中）なら
                if(isCharge == false)
                {
                    return;
                }

                //チャージ状態を未チャージ状態にする
                isCharge = false;
                //姿勢を伏せに変更
                attitudeType = AttitudeType.Prone;
                //キャラクターを回転させて伏せ状態にする
                transform.DORotate(proneRotation, 0.25f, RotateMode.WorldAxisAdd);
                //空気抵抗値を上げて減速させる
                rb.drag = 25.0f;
                //ボタンの子オブジェクトの画像を回転させる
                btnChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), 0.25f);

                anim.SetBool("Prone", true);

                break;

            case AttitudeType.Prone:

                attitudeType = AttitudeType.Straight;

                transform.DORotate(straightRotation, 0.25f);

                rb.drag = 0f;

                btnChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 90), 0.25f);

                anim.SetBool("Prone", false);

                break;
        }
    }
    /// <summary>
    /// 移動範囲の確認と制限
    /// </summary>
    private void LimitMoveArea()
    {
        //現在のXの位置が移動範囲内に収まっているか確認し、超えていた場合には（左端）か上限に合わせる
        float limitX = Mathf.Clamp(transform.position.x, limitLeftBottom.position.x, limitRightTop.position.x);

        //現在のZの位置が移動範囲内に収まっているか確認し、超えていた場合には下限（手前側）か上限（奥側）に合わせる
        float limitZ = Mathf.Clamp(transform.position.z, limitLeftBottom.position.z, limitRightTop.position.z);

        //制限値ないになるように位置情報を更新
        transform.position = new Vector3(limitX, transform.position.y, limitZ);
    }

    /// <summary>
    /// キャラの落下と移動を一時停止
    /// </summary>
    public void StopMove()
    {
        rb = GetComponent<Rigidbody>();

        //キャラのゲームオブジェクトを物理演算の影響を受けない状態にする
        rb.isKinematic = true;

        //キャラの速度を０にして停止する
        rb.velocity = Vector3.zero;
    }

    /// <summary>
    /// キャラの落下と移動を再開
    /// </summary>
    public void ResumeMove()
    {
        //キャラのゲームオブジェクトを物理演算の影響を受ける状態に戻す
        rb.isKinematic = false;

        //キャラに落下速度を設定する
        rb.velocity = new Vector3(0, -fallSpeed, 0);
    }

    /// <summary>
    /// スコアを半分にする
    /// </summary>
    public void HalveScore()
    {
        //スコアを半分にする
        score = Mathf.CeilToInt(score * 0.5f);

        Debug.Log("スコア半分 :" + score);

        //画面のスコア表示を更新
        txtScore.text = score.ToString();
    }

    public void DampigDrag(float airResistance)
    {
        //空気抵抗の値を更新
        rb.drag = airResistance;

        //3秒かけて空気抵抗の値を0にする
        DOTween.To(() => rb.drag, (x) => rb.drag = x, 0, 3.0f)
            .OnComplete(() =>
            {
                if (transform.rotation.x != 1)
                {
                    transform.DORotate(straightRotation, 0.25f);
                }
            });
    }
}

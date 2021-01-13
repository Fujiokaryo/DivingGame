using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Transform goal;

    [SerializeField]
    private Text txtDistance;

    [SerializeField]
    private CameraContoroller cameraContoroller;

    [SerializeField]
    private ResultpopUp resultpopUp;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField, Header("ステージをランダム生成する場合にはチェックする")]
    private bool isRandomStaging;

    [SerializeField, Header("移動する花輪の割合"), Range(0, 100)]
    private int movingFlowaerCirclePercent;

    [SerializeField, Header("大きさが変化する花輪の割合"), Range(0, 100)]
    private int scalingFlowerCirclePercent;

    [SerializeField]
    private FlowerCircle flowerCirclePrefab;

    [SerializeField]
    private Transform limitLeftBottom;

    [SerializeField]
    private Transform limitRightTop;

    [SerializeField]
    private Slider sliderAltimeter;

    [SerializeField]
    private SkyboxChanger skyboxChanger;

    [SerializeField, Header("障害物とアイテムをランダムに生成する場合にはチェックする")]
    private bool isRandomObjects;

    [SerializeField, Header("障害物とアイテムのプレファブ登録")]
    private GameObject[] randomObjPrefabs;

    private float startPos;

    private float distance;
    private bool isGoal;

    private void Awake()
    {
        //Skyboxの変更
        skyboxChanger.ChangeSkybox();

    }


    // Update is called once per frame
    void Update()
    {

        if(isGoal == true)
        {
            return;
        }

        distance = player.transform.position.y - goal.position.y;

        //Debug.Log(distance.ToString("F2"));

        txtDistance.text = distance.ToString("F2");

        

        //高度計用のキャラのアイコンの位置を更新
        sliderAltimeter.DOValue(distance / startPos, 0.1f);
        //Debug.Log(distance / startPos);
        //Debug.Log(startPos);


        if(distance <= 0)
        {
            isGoal = true;

            txtDistance.text = 0.ToString("F2");

            cameraContoroller.SetDefaultCamera();

            resultpopUp.DisplayResult();

            audioManager.PlayBGM(AudioManager.BgmType.GameClear);
        }
    }

    IEnumerator Start()
    {
        //スタート地点取得
        startPos = player.transform.position.y;

        //キャラの移動を一時停止（キー入力も受け付けない）
        player.StopMove();

        //Updateを止める
        isGoal = true;

        //花輪をランダムで配置する場合
        if(isRandomStaging)
        {
            //花輪の生成処理を行う。この処理が終了するまで次の処理を中断する
            yield return StartCoroutine(CreateRandomStage());
        }

        //障害物とアイテムをランダムで配置する場合
        if(isRandomObjects)
        {
            //障害物とアイテムをランダムに生成して配置する
            yield return StartCoroutine(CreateRandomObjects());
        }

        //Updateを再開
        isGoal = false;

        //キャラの移動を再開（キー入力受付開始）
        player.ResumeMove();

        Debug.Log(isGoal);
    }


    private IEnumerator CreateRandomStage()
    {
        //花輪の高さのスタート位置
        float flowerHeight = goal.position.y;
        Debug.Log(flowerHeight);

        //花輪を生成した数
        int count = 0;
        Debug.Log("初期の花輪のスタート位置　：" + flowerHeight);

        //花輪の高さがキャラの位置に到達するまでループ処理を行って花輪を生成する。キャラの位置に到達したらループを終了する
        while (flowerHeight <= player.transform.position.y)
        {
            //花輪の高さを加算(float型のrandom.rangeメソッドは10.0fを含む）
            flowerHeight += Random.Range(5.0f, 10.0f);

            //Debug.Log("現在の花輪の生成位置 :" + flowerHeight);

            //花輪の位置を設定して生成
            FlowerCircle flowerCircle = Instantiate(flowerCirclePrefab, new Vector3(Random.Range(limitLeftBottom.position.x, limitRightTop.position.x), flowerHeight, Random.Range(limitLeftBottom.position.z, limitRightTop.position.z)), Quaternion.identity);

            //花輪の初期設定を呼び出す。引数には評価後の戻り値を利用する。この時、移動するかどうか、大きさを変えるかどうかの情報を引数として渡す
            flowerCircle.SetUpMovingFlowerCircle(Random.Range(0, 100) <= movingFlowaerCirclePercent, Random.Range(0, 100) <= scalingFlowerCirclePercent);

            //花輪の生成数を加算
            count++;

            //Debug.Log("花輪の合計生成数　：" + count);

            //1フレームだけ中断（この処理を入れないと無限ループしてunityがフリーズする
            yield return null;
        }
        
        //Debug.Log("ランダムステージ完成");
    }
   
    private IEnumerator CreateRandomObjects()
    {
        //ステージの長さ
        float height = goal.position.y;
        int count = 0;

        while(height <= player.transform.position.y)
        {
            height += Random.Range(10, 15);

            //位置を設定して生成
            Instantiate(randomObjPrefabs[Random.Range(0, randomObjPrefabs.Length)], new Vector3(Random.Range(limitLeftBottom.position.x, limitRightTop.position.x), height, Random.Range(limitLeftBottom.position.z, limitRightTop.position.z)), Quaternion.identity);

            count++;

            yield return null;
        }
    }
}

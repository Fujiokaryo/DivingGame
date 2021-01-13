using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FlowerCircle : MonoBehaviour
{
    [Header("花輪通過時の得点")]
    public int point;

    [SerializeField]
    private BoxCollider boxCollider;

    [SerializeField]
    private GameObject effectPrefab;

    [SerializeField]
    private AudioClip flowerSE; //花輪獲得時のSE

    [SerializeField, Header("移動させる場合スイッチを入れる")]
    private bool isMoveing;

    [SerializeField, Header("移動時間")]
    private float duration;

    [SerializeField, Header("移動距離")]
    private float moveDistance;

    [SerializeField, Header("移動する時間と距離をランダムにする場合"), Range(0, 100)]
    private int randomMovvingPercent;

    [SerializeField, Header("移動時間のランダム幅")]
    private Vector2 durationRange;

    [SerializeField, Header("移動距離のランダム幅")]
    private Vector2 moveDistanceRange;

    [SerializeField, Header("大きさの設定")]
    private float[] flowerSizes;

    [SerializeField, Header("点数の倍率")]
    private float[] pointRate;

        void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

        if(isMoveing)
        {
            //前後にループ移動させる
            transform.DOMoveZ(transform.position.z + moveDistance, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }

    //花輪から見て、他のゲームオブジェクトが侵入してきたとき
    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Water")
        {
            return;
        }

        //花輪のBoxcolliderのスイッチをオフにして重複判定を阻止
        boxCollider.enabled = false;

        //花輪をキャラの子オブジェクトにする
        transform.SetParent(other.transform);

        //花輪をくぐった際の演出
        StartCoroutine(PlayGetEffect());
    }

    private IEnumerator PlayGetEffect()
    {

        //SE再生
        AudioSource.PlayClipAtPoint(flowerSE, transform.position);

        //DOTweenのSequenceを宣言して利用できるようにする
        Sequence sequence = DOTween.Sequence();

        //Appendを実行すると、引数でDOTweenの処理を実行できる。花輪のscaleを1秒かけて０にして見えなくする
        sequence.Append(transform.DOScale(Vector3.zero, 1.0f));

        //joinを実行することでAppendと一緒にDOTweenの処理を行える。花輪が1秒かけて見えなくなるのと一緒にプレイヤーの位置に花輪を移動させる
        sequence.Join(transform.DOLocalMove(Vector3.zero, 1.0f));

        //1秒処理を中断（待機）する
        yield return new WaitForSeconds(1.0f);

        //エフェクトを生成して、instantiateメソッドの戻り値をeffect変数に代入
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

        //エフェクトの高さを調整する
        effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y - 1.5f, effect.transform.position.z);

        Destroy(effect, 1.0f);

        //花輪を1秒後に破棄
        Destroy(gameObject, 1.0f);
    }
    /// <summary>
    /// 移動する花輪の設定
    /// </summary>
    public void SetUpMovingFlowerCircle(bool isMoveing, bool isScaleChanging)
    {
        //移動する花輪か、通常の花輪かの設定
        this.isMoveing = isMoveing;

        //移動する場合
        if(this.isMoveing)
        {
            //ランダムな移動時間や距離を使うか、戻り値を持つメソッドを利用して判定
            if(DetectRandomMovingFromPercent())
            {
                //ランダムの場合には、移動距離と距離のランダム設定を行う
                ChangeRandomMoveParameters();
            }
        }

        //花輪の大きさを変更する場合
        if(isScaleChanging)
        {
            //大きさを変更
            ChangeRandomScales();
        }
    }

    /// <summary>
    /// 移動時間と移動距離をランダムにするか判定　trueの場合はランダムとする
    /// </summary>
    /// <returns></returns>
    private bool DetectRandomMovingFromPercent()
    {
        //処理結果をbool値で戻す。randomMovingPercentの値よりも大きければfalse　同じか小さければtrue
        return Random.Range(0, 100) <= randomMovvingPercent;
    }
    /// <summary>
    /// ランダム値を取得して移動
    /// </summary>
    private void ChangeRandomMoveParameters()
    {
        //移動時間をランダム値の範囲で設定
        duration = Random.Range(durationRange.x, durationRange.y);

        //移動距離をランダム値の範囲で設定
        moveDistance = Random.Range(moveDistanceRange.x, moveDistanceRange.y);
    }

    private void ChangeRandomScales()
    {
        //ランダム値の範囲内で大きさを設定
        int index = Random.Range(0, flowerSizes.Length);

        //大きさを変更
        transform.localScale *= flowerSizes[index];

        //点数を変更
        point = Mathf.CeilToInt(point * pointRate[index]);
    }
}

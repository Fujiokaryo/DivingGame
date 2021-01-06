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
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    //花輪から見て、他のゲームオブジェクトが侵入してきたとき
    private void OnTriggerEnter(Collider other)
    {
        //花輪のBoxcolliderのスイッチをオフにして重複判定を阻止
        boxCollider.enabled = false;

        //花輪をキャラの子オブジェクトにする
        transform.SetParent(other.transform);

        //花輪をくぐった際の演出
        StartCoroutine(PlayGetEffect());
    }

    private IEnumerator PlayGetEffect()
    {
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

}

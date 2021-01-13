using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ItemTranpoline : MonoBehaviour
{
    private BoxCollider boxCol;

    [SerializeField, Header("跳ねた時の空気抵抗値")]
    private float airResistance;
    void Start()
    {
        boxCol = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //指定されたゲームオブジェクトが接触した場合には判定を行わない
        if(boxCol.gameObject.tag == "Water" || boxCol.gameObject.tag == "FlowerCircle")
        {
            return;
        }

        //侵入してきたゲームオブジェクトがPlayerControllerスクリプトを持っていたら取得
        if(other.gameObject.TryGetComponent(out PlayerController player))
        {
            //バウンドさせる
            Bound(player);
        }
    }

    private void Bound(PlayerController player)
    {
        //コライダーをオフにして重複判定を防止する
        boxCol.enabled = false;

        //キャラを上空にバウンドさせる（操作は可能
        player.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(800, 1000), ForceMode.Impulse);

        //キャラを回転させる
        player.transform.DORotate(new Vector3(90, 1080, 0), 1.5f, RotateMode.FastBeyond360)
            .OnComplete(() =>
            {
                //しばらくの間落下速度をゆっくりにする
                player.DampigDrag(airResistance);
            });

        Destroy(gameObject);
    }
}

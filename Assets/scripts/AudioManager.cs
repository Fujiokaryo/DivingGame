using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField, Header("BGM用オーディオファイル")]
    private AudioClip[] bgms = null;

    private AudioSource audioSource; //BGM再生用コンポーネント

    public enum BgmType
    {
        Main,     //自動的に０が割り振られている
        GameClear //動揺に1が自動的に割り振られている
    }
    void Start()
    {
        //コンポーネントを取得して代入する
        audioSource = GetComponent<AudioSource>();

        //ゲーム中のBGM を再生
        PlayBGM(BgmType.Main);
    }

    // Update is called once per frame
    public void PlayBGM(BgmType bgmType)
    {
        //BGM停止
        audioSource.Stop();

        //再生するBGM を設定する
        audioSource.clip = bgms[(int)bgmType];

        //BGM再生
        audioSource.Play();

        Debug.Log("再生中のBGM :" + bgmType);
    }
}

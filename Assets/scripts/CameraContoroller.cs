using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraContoroller : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private Camera selfishCamera; //自撮りカメラのCameraコンポーネント代入用

    [SerializeField]
    private Camera fpsCamera; //一人称カメラのCameraコンポーネント代入用

    [SerializeField]
    private Button btnChangeCamera;　//カメラの制御用ボタンのBottunコンポーネントの代入用

    private int cameraIndex;　//現在適用しているカメラの通し番号
    private Camera mainCamera;　//MainCameraゲームオブジェクトのCameraコンポーネント代入用

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - playerController.transform.position;

        //mainCamera Tagを持つゲームオブジェクト（MainCameraゲームオブジェクト）のCameraコンポーネントを取得して代入
        mainCamera = Camera.main;

        //ボタンのOnClickイベントにメソッドを登録
        btnChangeCamera.onClick.AddListener(ChangeCamera);

        //カメラを初期カメラ（三人称カメラ）に戻す
        SetDefaultCamera();
    }

    // Update is called once per frame
    void Update()
    {

        if(playerController.inWater == true)
        {
            return;
        }

        if(playerController != null)
        {
            transform.position = playerController.transform.position + offset;
        }

    }

    /// <summary>
    /// カメラを変更（ボタンを押す度に呼び出される）
    /// </summary>
    private void ChangeCamera()
    {
        //現在のカメラの通し番号に応じて、次のカメラを用意して切り替える
        switch(cameraIndex)
        {
            case 0:
                cameraIndex++;
                mainCamera.enabled = false;
                fpsCamera.enabled = true;
                break;

            case 1:
                cameraIndex++;
                fpsCamera.enabled = false;
                mainCamera.enabled = true;
                break;

            case 2:
                cameraIndex = 0;
                selfishCamera.enabled = false;
                mainCamera.enabled = true;
                break;
        }
    }

    /// <summary>
    /// カメラを初期カメラ（三人称カメラ）に戻す
    /// </summary>
    public void SetDefaultCamera()
    {
        cameraIndex = 0;

        mainCamera.enabled = true;
        fpsCamera.enabled = false;
        selfishCamera.enabled = false;
    }
}

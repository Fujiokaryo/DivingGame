using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField]
    private Material[] skyboxMaterials; //skybox用のマテリアルを登録するための配列

    [SerializeField, Header("Skybox設定用の指定値。999の場合ランダムにする")]
    private int skyboxMaterialIndex;


    public void ChangeSkybox()
    {
        //ランダム設定の場合
        if(skyboxMaterialIndex == 999)
        {
            //skyboxをランダムな要素番号のマテリアルのskyboxに変更
            RenderSettings.skybox = skyboxMaterials[RandomSelectIndexOfSkyboxMaterials()]; 
        }
        else
        {
            //Skyboxを指定された要素番号のマテリアルのSkyboxに変更
            RenderSettings.skybox = skyboxMaterials[skyboxMaterialIndex];
        }
        Debug.Log("Skybox 変更");
    }

    public int RandomSelectIndexOfSkyboxMaterials()
    {
        return Random.Range(0, skyboxMaterials.Length);
    }
}

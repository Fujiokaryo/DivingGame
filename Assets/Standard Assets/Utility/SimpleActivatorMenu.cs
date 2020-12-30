using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 618
namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // An incredibly simple menu which, when given references
        // to gameobjects in the scene
        private Text camSwitchButton;
        public GameObject[] objects;


        private int m_CurrentActiveObject;

        public Text GetCamSwitchButton()
        {
            return camSwitchButton;
        }

        public void SetCamSwitchButton(Text value)
        {
            camSwitchButton = value;
        }

        private void OnEnable()
        {
            // active object starts from first in array
            m_CurrentActiveObject = 0;
            GetCamSwitchButton().text = objects[m_CurrentActiveObject].name;
        }


        public void NextCamera()
        {
            int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextactiveobject);
            }

            m_CurrentActiveObject = nextactiveobject;
            GetCamSwitchButton().text = objects[m_CurrentActiveObject].name;
        }
    }
}

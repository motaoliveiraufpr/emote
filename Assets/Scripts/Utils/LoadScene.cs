using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Emote.Utils
{
    public class LoadScene : MonoBehaviour
    {
        public Button m_Button;
        public string m_Scene;

        void Start()
        {
            if (m_Button == null)
                m_Button = GetComponent<Button>();

            m_Button.onClick.AddListener(delegate
            {
                if (!string.IsNullOrEmpty(m_Scene))
                    SceneManager.LoadScene(m_Scene, LoadSceneMode.Single);
            });
        }
    }
}
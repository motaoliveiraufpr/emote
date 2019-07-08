using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Emote.Utils
{
    public class Exit : MonoBehaviour
    {
        public Button m_Button;

        void Start()
        {
            if (m_Button == null)
                m_Button = GetComponent<Button>();

            m_Button.onClick.AddListener(delegate
            {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        }
    }
}
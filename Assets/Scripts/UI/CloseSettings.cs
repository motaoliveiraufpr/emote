using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Emote.Utils
{
    public class CloseSettings : MonoBehaviour
    {
        public Button m_Button;
        public Canvas m_Canvas;

        void Start()
        {
            if (m_Button == null)
                m_Button = GetComponent<Button>();

            m_Button.onClick.AddListener(delegate
            {
                if (m_Canvas) m_Canvas.enabled = false;
            });
        }
    }
}
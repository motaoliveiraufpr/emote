using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Emote.Utils
{
    public class OnEnterKey : MonoBehaviour
    {
        public Button m_Button;

        void Start()
        {
            if (m_Button == null)
                m_Button = GetComponent<Button>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                if (m_Button)
                    m_Button.onClick.Invoke();
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using TMPro;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class CountDown : MonoBehaviour
    {
        public float m_TimeLeft = 5.0f;
        public float m_WaitToStart = 0.0f;
        public string m_WaitText = "Tudo bem, vamos começar!";

        [HideInInspector]
        public bool m_IsCompleted
        {
            get
            {
                return m_Completed;
            }
        }

        private float m_CurrentTime;
        private TextMeshProUGUI m_Text;
        private bool m_Completed = false;

        private void Start()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
            if (!m_Text)
            {
                return;
            }
        }

        public IEnumerator StartCountDown()
        {
            m_Completed = false;
            m_CurrentTime = m_TimeLeft;

            SetText(m_WaitText);

            yield return new WaitForSeconds(m_WaitToStart);

            while (m_CurrentTime >= 0)
            {
                SetText(m_CurrentTime.ToString());
                yield return new WaitForSeconds(1.0f);
                m_CurrentTime--;
            }

            SetText("");
            m_Completed = true;
        }

        public void SetText(string _text)
        {
            if (m_Text && m_Text.IsActive())
            {
                m_Text.text = _text;
            }
        }

        public void ResetStatus()
        {
            m_Completed = false;
        }
    }
}

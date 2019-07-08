using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Emote.Utils;
using Emote.Database;
using Emote.Models;
using UnityEngine.SceneManagement;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class StartSessionManager : MonoBehaviour
    {
        public Button m_Button;
        public InputField m_Text;
        public string m_NextScene = "PhotoScene";

        private void Start()
        {
            if (m_Button == null)
            {
                m_Button = GetComponent<Button>();
                if (m_Button == null)
                {
                    return;
                }
            }

            // start session time counter
            EmoteSession.time.Start();

            m_Button.onClick.AddListener(delegate
            {
                OnClick();
            });
        }

        private void OnClick()
        {
            // set a new session
            EmoteSession.session = new Session();
            if (m_Text)
            {
                if (!string.IsNullOrEmpty(m_Text.text))
                {
                    EmoteSession.session.reference = m_Text.text;
                }
                else
                {
                    EmoteSession.session.reference = (System.Guid.NewGuid()).ToString();
                }
            }
            else
            {
                EmoteSession.session.reference = (System.Guid.NewGuid()).ToString();
            }
            DatabaseManager.m_Session = EmoteSession.session;
            EmoteSession.session.id = DatabaseManager.m_Session.id;

            if (!string.IsNullOrEmpty(m_NextScene))
            {
                SceneManager.LoadScene(m_NextScene, LoadSceneMode.Single);
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
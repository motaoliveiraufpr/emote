using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class AudioPlayerButtons : MonoBehaviour
    {
        public AudioSource m_AudioPlayer;
        public Button m_Pause;
        public Button m_Replay;

        public AudioQuizManager m_AudioManager;

        private int m_PlayerCount = 0;

        void Start()
        {
            if (m_AudioManager == null) return;

            if (m_Pause)
            {
                m_Pause.enabled = false;
                m_Pause.onClick.AddListener(OnPauseClick);
            }
            if (m_Replay)
            {
                m_Replay.enabled = false;
                m_Replay.onClick.AddListener(OnReplayClick);
            }
        }

        private void Update()
        {
            if (m_AudioManager && m_AudioManager.m_PlayCount > 0)
            {
                if (m_AudioPlayer.isPlaying)
                {
                    m_Pause.enabled = true;
                    m_Replay.enabled = false;
                }
                else
                {
                    m_Pause.enabled = false;
                    m_Replay.enabled = true;
                }
            }
        }

        void OnPauseClick()
        {
            m_AudioPlayer.Pause();
        }

        void OnReplayClick()
        {
            m_AudioPlayer.Stop();
            m_AudioManager.PlayAudio();
        }
    }
}
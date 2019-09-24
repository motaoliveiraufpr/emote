#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0162

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
    public class VideoPlayerButtonsManager : MonoBehaviour
    {
        public VideoPlayer m_VideoPlayer;
        public Button m_Pause;
        public Button m_Replay;

        public VideoQuizManager m_VideoManager;

        private int m_PlayerCount = 0;

        void Start()
        {
            if (m_VideoPlayer == null) return;

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
            if (m_VideoManager && m_VideoManager.m_PlayCount > 0)
            {
                if (m_VideoPlayer.isPlaying)
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
            m_VideoPlayer.Pause();
        }

        void OnReplayClick()
        {
            m_VideoPlayer.Stop();
            m_VideoPlayer.Play();
        }
    }
}
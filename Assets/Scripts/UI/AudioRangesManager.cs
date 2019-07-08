using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emote.Database;
using Emote.Models;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class AudioRangesManager : MonoBehaviour
    {
        public Slider m_MinRange;
        public Slider m_MaxRange;

        public Text m_MinText
        {
            get
            {
                return m_MinRange.GetComponentInChildren<Text>();
            }
        }

        public Text m_MaxText
        {
            get
            {
                return m_MaxRange.GetComponentInChildren<Text>();
            }
        }

        void Start()
        {
            if (m_MinRange == null || m_MaxRange == null)
            {
                return;
            }

            var audioRanges = DatabaseManager.m_MinMaxAudios;
            m_MinRange.value = audioRanges.min_audios;
            m_MaxRange.value = audioRanges.max_audios;

            if (m_MinRange.value > m_MaxRange.value)
            {
                m_MinRange.value = m_MaxRange.value;
            }

            // set max range as number of audios in database
            m_MaxRange.value = (int)DatabaseManager.m_Audios.Count;

            m_MinRange.onValueChanged.AddListener(delegate
            {
                OnChanged();
            });

            m_MaxRange.onValueChanged.AddListener(delegate
            {
                OnChanged();
            });

            UpdateCaption();
        }

        void OnChanged()
        {
            if (m_MaxRange.value > DatabaseManager.m_Audios.Count)
            {
                m_MaxRange.value = DatabaseManager.m_Audios.Count;
            }

            if (m_MinRange.value > m_MaxRange.value)
            {
                m_MinRange.value = m_MaxRange.value;
            }

            UpdateCaption();

            Models.AudioSettings audioSettings = new Models.AudioSettings();
            audioSettings.min_audios = (int)m_MinRange.value;
            audioSettings.max_audios = (int)m_MaxRange.value;

            DatabaseManager.m_MinMaxAudios = audioSettings;
        }

        void UpdateCaption()
        {
            int index = m_MinText.text.IndexOf(":");
            if (index > 0)
            {
                m_MinText.text = m_MinText.text.Substring(0, index);
                m_MinText.text += (": " + m_MinRange.value.ToString());
            }
            index = m_MaxText.text.IndexOf(":");
            if (index > 0)
            {
                m_MaxText.text = m_MaxText.text.Substring(0, index);
                m_MaxText.text += (": " + m_MaxRange.value.ToString());
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Affdex;
using Emote.Avatar;
using Emote.Database;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class RandomAvatarTextureManager : MonoBehaviour
    {
        public Toggle m_Toggle;
        public AvatarListener m_Listener;
        public Detector m_Detector;

        private void Start()
        {
            if (m_Toggle == null)
            {
                m_Toggle = GetComponent<Toggle>();
                if (m_Toggle == null)
                {
                    return;
                }
            }

            bool randomTexture = DatabaseManager.m_RandomAvatarTexture;
            m_Toggle.isOn = randomTexture;
            if (m_Listener && m_Detector)
            {
                m_Detector.StopDetector();
                m_Listener.SetRandomTexture(randomTexture);
                m_Detector.StartDetector();
            }

            m_Toggle.onValueChanged.AddListener(delegate
            {
                OnValueChanged(m_Toggle);
            });
        }

        #region Device States
        private void SetRandomTexture(bool random)
        {
            if (m_Listener && m_Detector)
            {
                m_Detector.StopDetector();
                m_Listener.SetRandomTexture(random);
                m_Detector.StartDetector();
            }
            DatabaseManager.m_RandomAvatarTexture = random;
        }
        #endregion

        #region Handlers
        private void OnValueChanged(Toggle change)
        {
            SetRandomTexture(change.isOn);
        }
        #endregion
    }
}
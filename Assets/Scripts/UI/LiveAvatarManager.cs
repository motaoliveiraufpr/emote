using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emote.Database;
using Emote.Avatar;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class LiveAvatarManager : MonoBehaviour
    {
        public Toggle m_Toggle;
        public AvatarListener m_AvatarListener;

        void Start()
        {
            if (m_Toggle == null)
            {
                m_Toggle = GetComponent<Toggle>();
                if (m_Toggle == null)
                {
                    return;
                }
            }

            bool liveAvatar = DatabaseManager.m_LiveAvatar;
            m_Toggle.isOn = liveAvatar;

            m_Toggle.onValueChanged.AddListener(delegate
            {
                OnValueChanged(m_Toggle);
            });
        }

        #region Handlers
        private void OnValueChanged(Toggle change)
        {
            DatabaseManager.m_LiveAvatar = change.isOn;
            if (change.isOn)
            {
                if (m_AvatarListener)
                {
                    m_AvatarListener.ResetWeights(true);
                }
            }
        }
        #endregion
    }
}
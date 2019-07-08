using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Utils
{
    public class VersionControlInfo : MonoBehaviour
    {
        private TextMeshProUGUI m_Version;
        public bool m_Beta = true;

        void Start()
        {
            m_Version = GetComponent<TextMeshProUGUI>();
            if (m_Version)
            {
                m_Version.SetText("v" + Application.version + (m_Beta ? " Beta" : ""));
            }
        }
    }
}
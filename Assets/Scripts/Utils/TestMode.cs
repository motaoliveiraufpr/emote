using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emote.Utils;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class TestMode : MonoBehaviour
    {
        public SwitchAnim m_Switch;

        void Start()
        {
            EmoteSession.trainingMode = false;

            if (m_Switch == null)
                m_Switch = GetComponent<SwitchAnim>();
        }

        void Update()
        {
            if (m_Switch && m_Switch.isOn)
                EmoteSession.trainingMode = true;
            else
                EmoteSession.trainingMode = false;
        }
    }
}
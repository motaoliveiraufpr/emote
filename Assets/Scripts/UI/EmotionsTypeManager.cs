using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Affdex;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class EmotionsTypeManager : MonoBehaviour
    {
        public Dropdown m_Dropdown;
        List<string> m_DropOptions;

        void Start()
        {
            if (m_Dropdown == null)
            {
                m_Dropdown = GetComponent<Dropdown>();
                if (m_Dropdown == null)
                {
                    return;
                }
            }

            PopulateExpression();
        }

        void PopulateExpression()
        {
            m_Dropdown.ClearOptions();

            m_DropOptions = new List<string>();
            List<Emotions> m_Emotions = new List<Emotions>();
            m_Emotions.Add(Emotions.Joy);
            m_Emotions.Add(Emotions.Fear);
            m_Emotions.Add(Emotions.Disgust);
            m_Emotions.Add(Emotions.Sadness);
            m_Emotions.Add(Emotions.Anger);
            m_Emotions.Add(Emotions.Surprise);
            m_Emotions.Add(Emotions.Neutral);

            foreach (var emotion in m_Emotions)
            {
                m_DropOptions.Add(QuizOptionsManager.GetTraslatedEmotion(emotion));
            }

            m_Dropdown.AddOptions(m_DropOptions);
        }
    }
}
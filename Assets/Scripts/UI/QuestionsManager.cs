using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emote.Database;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class QuestionsManager : MonoBehaviour
    {
        public GameObject m_ToggleObject;
        public GameObject m_SliderObject;
        public Text m_SliderText;

        [HideInInspector]
        public Toggle m_Toggle
        {
            get
            {
                return m_ToggleObject.GetComponent<Toggle>();
            }
        }

        [HideInInspector]
        public Slider m_Slider
        {
            get
            {
                return m_SliderObject.GetComponent<Slider>();
            }
        }

        void Start()
        {
            if (m_ToggleObject == null || m_SliderObject == null)
            {
                return;
            }

            bool randomQuestions = DatabaseManager.m_RandomQuestions;
            if (randomQuestions)
            {
                m_SliderObject.SetActive(true);
            }
            else
            {
                m_Toggle.isOn = false;
            }

            int maxQuestions = DatabaseManager.m_MaxQuestions;
            m_Slider.value = maxQuestions;
            if (m_SliderText)
            {
                m_SliderText.text = maxQuestions.ToString();
            }

            m_Toggle.onValueChanged.AddListener(delegate
            {
                OnValueChanged(m_Toggle);
            });
            m_Slider.onValueChanged.AddListener(delegate
            {
                OnValueChanged(m_Slider);
            });
        }

        public int GetRandomQuestionsNumber()
        {
            return UnityEngine.Random.Range((int)m_Slider.minValue, (int)DatabaseManager.m_MaxQuestions);
        }

        #region Handlers
        private void OnValueChanged(Toggle change)
        {
            if (change.isOn)
            {
                m_SliderObject.SetActive(true);
            }
            else
            {
                m_SliderObject.SetActive(false);
            }
            DatabaseManager.m_RandomQuestions = change.isOn;
        }

        private void OnValueChanged(Slider change)
        {
            if (m_SliderText)
            {
                m_SliderText.text = ((int)change.value).ToString();
            }
            DatabaseManager.m_MaxQuestions = (int)change.value;
        }
        #endregion
    }
}
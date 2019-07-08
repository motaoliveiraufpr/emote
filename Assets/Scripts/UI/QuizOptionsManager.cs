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
    public class QuizOptionsManager : MonoBehaviour
    {

        private KeyCode[][] keyCodes = {
            new KeyCode[] { KeyCode.Alpha1, KeyCode.Keypad1 },
            new KeyCode[] { KeyCode.Alpha2, KeyCode.Keypad2 },
            new KeyCode[] { KeyCode.Alpha3, KeyCode.Keypad3 },
            new KeyCode[] { KeyCode.Alpha4, KeyCode.Keypad4 },
            new KeyCode[] { KeyCode.Alpha5, KeyCode.Keypad5 },
            new KeyCode[] { KeyCode.Alpha6, KeyCode.Keypad6 },
            new KeyCode[] { KeyCode.Alpha7, KeyCode.Keypad7 }
        };

        public GameObject m_TogglePrefab;
        public Button m_NextButton;

        public Toggle[] m_ToggleList
        {
            get
            {
                return GetComponentsInChildren<Toggle>();
            }
        }
        private List<GameObject> m_ToggleObjectList = new List<GameObject>();

        void Start()
        {
            if (m_TogglePrefab == null)
            {
                return;
            }
        }

        public static string GetTraslatedEmotion(Emotions emotion)
        {
            switch (emotion)
            {
                case Emotions.Joy:
                    return "Alegria";
                case Emotions.Fear:
                    return "Medo";
                case Emotions.Disgust:
                    return "Nojo";
                case Emotions.Sadness:
                    return "Tristeza";
                case Emotions.Anger:
                    return "Raiva";
                case Emotions.Surprise:
                    return "Surpresa";
                default:
                    return "Neutro";
            }
        }

        public void AddToggle(Emotions[] emotions)
        {
            m_ToggleObjectList.Clear();
            int i = 1;
            foreach (var emotion in emotions)
            {
                AddToggle(i + ". " + GetTraslatedEmotion(emotion));
                i++;
            }
        }

        public Toggle AddToggle(string label = null)
        {
            GameObject _object = Instantiate<GameObject>(m_TogglePrefab);
            Toggle toggle = _object.GetComponent<Toggle>();

            RectTransform transform = _object.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(125, 32);

            m_ToggleObjectList.Add(_object);

            if (toggle)
            {
                toggle.isOn = false;
                toggle.onValueChanged.AddListener(delegate
                {
                    OnValueChanged(toggle);
                });

                if (!string.IsNullOrEmpty(label))
                {
                    Text[] _label = toggle.GetComponentsInChildren<Text>();
                    if (_label.Length > 0)
                    {
                        for (int i = 0; i < _label.Length; i++)
                        {
                            _label[i].resizeTextMinSize = 55;
                            _label[i].resizeTextMaxSize = 55;
                            _label[i].text = label;
                        }
                    }
                }

                toggle.transform.SetParent(this.transform);

                return toggle;
            }

            return null;
        }

        private void Update()
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i][0]) || Input.GetKeyDown(keyCodes[i][1]))
                {
                    GameObject toggleRefs = m_ToggleObjectList[i];
                    if (toggleRefs)
                    {
                        Toggle toggle = toggleRefs.GetComponent<Toggle>();

                        foreach (var _toggle in m_ToggleObjectList)
                        {
                            _toggle.GetComponent<Toggle>().isOn = false;
                        }
                        if (toggle) toggle.isOn = true;
                    }
                }
            }
        }

        private void OnValueChanged(Toggle change)
        {
            bool status = change.isOn;
            foreach (var toggle in m_ToggleList)
            {
                if (toggle != change)
                {
                    toggle.isOn = false;
                }
            }
            change.isOn = status;
            if (change.isOn)
            {
                EmoteSession.enableNextKey = true;
                m_NextButton.enabled = true;
            }
        }

        public void ClearToggles()
        {
            foreach (Object toggle in m_ToggleObjectList)
            {
                Destroy(toggle);
            }
            m_ToggleObjectList.Clear();
        }
    }
}
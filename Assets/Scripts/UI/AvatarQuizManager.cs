using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Emote.Database;
using Affdex;
using Emote.Avatar;
using Emote.Models;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class AvatarQuizManager : MonoBehaviour
    {
        public static class QuizState
        {
            public static int index;
            public static Emotions answer;
            public static string answered;
            public static Emotions[] options;
            public static List<Emotions> used = new List<Emotions>();
        }

        public Toggle m_Toggle;
        public GameObject m_QuizPanel;
        public GameObject m_RandomQuestions;
        public GameObject m_RandomQuestionsNumber;
        public AvatarListener m_AvatarListener;
        public Button m_NextButton;
        public string m_NextScene;

        private int questionsNumber = 0;
        private Emotions[] emotionsList;

        public StoreDuration m_Duration
        {
            get
            {
                return GetComponent<StoreDuration>();
            }
        }

        public PipelineType pipelineType
        {
            get
            {
                List<PipelineType> types = DatabaseManager.m_PipelineType;
                foreach(var type in types)
                {
                    if (type.type == "AVATAR")
                    {
                        return type;
                    }
                }
                return null;
            }
        }

        public Toggle randomQuestions
        {
            get
            {
                if (m_RandomQuestions)
                {
                    return m_RandomQuestions.GetComponent<Toggle>();
                }
                return null;
            }
        }

        public Slider randomQuestionsNumber
        {
            get
            {
                if (m_RandomQuestionsNumber)
                {
                    return m_RandomQuestionsNumber.GetComponent<Slider>();
                }
                return null;
            }
        }

        public QuizOptionsManager OptionManager
        {
            get
            {
                if (m_QuizPanel)
                {
                    return m_QuizPanel.GetComponentInChildren<QuizOptionsManager>();
                }
                return null;
            }
        }

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

            if (m_NextButton == null && m_QuizPanel)
            {
                m_NextButton = m_QuizPanel.GetComponentInChildren<Button>();
            }

            if (m_NextButton)
            {
                m_NextButton.onClick.AddListener(delegate
                {
                    OnNextClick();
                });
            }

            m_Toggle.onValueChanged.AddListener(delegate
            {
                OnValueChanged(m_Toggle);
            });

            bool quizEnabled = DatabaseManager.m_QuizEnabled;
            m_Toggle.isOn = quizEnabled;

            if (DatabaseManager.m_RandomQuestions)
            {
                if (quizEnabled && randomQuestions && randomQuestions.isOn)
                {
                    if (randomQuestionsNumber)
                    {
                        int number = (int)randomQuestionsNumber.value;
                        if (m_QuizPanel)
                        {
                            m_QuizPanel.GetComponent<Canvas>().enabled = true;
                        }

                        var questionsManager = m_RandomQuestions.GetComponent<QuestionsManager>();
                        if (questionsManager)
                        {
                            questionsNumber = questionsManager.GetRandomQuestionsNumber();
                            emotionsList = AvatarEmotions.GetRandomEmotions(questionsNumber);

                            // start quiz handler
                            QuizState.index = 0;
                            EmotionsQuizHandler();
                        }
                    }
                }
            }
            else
            {
                emotionsList = AvatarEmotions.GetStaticEmotions();

                // start quiz handler
                QuizState.index = 0;
                EmotionsQuizHandler();
            }
        }

        private void EmotionsQuizHandler()
        {
            if (m_AvatarListener)
            {
                m_AvatarListener.SetCurrentEmotion(emotionsList[QuizState.index]);
            }

            if (m_NextButton)
            {
                // disable next button
                m_NextButton.enabled = false;
            }

            // generate array of answers
            QuizState.answer = emotionsList[QuizState.index];
            QuizState.options = new Emotions[7];
            QuizState.options[6] = emotionsList[QuizState.index];

            QuizState.used.Clear();
            QuizState.used.Add(emotionsList[QuizState.index]);
            if (EmoteSession.randomOptions)
            {
                for (int i = 0; i < 7; i++)
                {
                    Emotions temp = AvatarEmotions.GetRandomEmotion();
                    while (QuizState.used.Contains(temp))
                    {
                        temp = AvatarEmotions.GetRandomEmotion();
                    }
                    QuizState.used.Add(temp);
                    QuizState.options[i] = temp;
                }
                AvatarQuizManager.Shuffle(QuizState.options);
            }
            else
            {
                Emotions[] temp = AvatarEmotions.GetStaticEmotions();
                for (int i = 0; i < 7; i++)
                {
                    QuizState.options[i] = temp[i];
                }
            }

            // set answer options
            if (OptionManager)
            {
                OptionManager.ClearToggles();
                OptionManager.AddToggle(QuizState.options);
            }

            // increment index
            QuizState.index++;
        }

        public static void Shuffle(Emotions[] emotions)
        {
            // Knuth shuffle algorithm
            for (int t = 0; t < emotions.Length; t++)
            {
                Emotions tmp = emotions[t];
                int r = Random.Range(t, emotions.Length);
                emotions[t] = emotions[r];
                emotions[r] = tmp;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                if (OptionManager)
                {
                    bool active = false;
                    foreach (var toggle in OptionManager.m_ToggleList)
                    {
                        if (toggle.isOn) active = true;
                    }

                    if (active) OnNextClick();
                }
            }
        }

        #region Handlers
        private void OnNextClick()
        {
            if (QuizState.index < emotionsList.Length)
            {
                if (EmoteSession.enableNextKey)
                {
                    if (!EmoteSession.trainingMode)
                    {
                        Answer answer = new Answer();
                        answer.pipeline_type_id = pipelineType.id;
                        answer.session_id = EmoteSession.session.id;
                        answer.correct = QuizOptionsManager.GetTraslatedEmotion(QuizState.answer);
                        answer.file = "";

                        if (OptionManager)
                        {
                            foreach (var toggle in OptionManager.m_ToggleList)
                            {
                                if (toggle.isOn)
                                {
                                    Text text = toggle.GetComponentInChildren<Text>();
                                    if (text)
                                    {
                                        QuizState.answered = text.text;
                                    }
                                    break;
                                }

                            }
                        }
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[0-9\.]");
                        answer.answer = regex.Replace(QuizState.answered, "");

                        // insert new answer
                        DatabaseManager.m_Answers = answer;
                    }
                    EmoteSession.enableNextKey = false;
                    EmotionsQuizHandler();
                }
            }
            else
            {
                if (EmoteSession.enableNextKey)
                {
                    m_Duration.StoreData();
                    m_NextButton.enabled = false;

                    // exit the application
#if UNITY_EDITOR
                    if (!string.IsNullOrEmpty(m_NextScene))
                    {
                        EmoteSession.enableNextKey = false;
                        SceneManager.LoadScene(m_NextScene);
                    }
                    else
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                if (!string.IsNullOrEmpty(m_NextScene))
                    SceneManager.LoadScene(m_NextScene);
                else
                    Application.Quit();
#endif
                }
            }
        }

        private void OnValueChanged(Toggle change)
        {
            if (change.isOn)
            {
                m_QuizPanel.GetComponent<Canvas>().enabled = true;
            } else
            {
                m_QuizPanel.GetComponent<Canvas>().enabled = false;
            }
            DatabaseManager.m_QuizEnabled = change.isOn;
        }
#endregion
    }
}
using System.IO;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Affdex;
using Emote.Database;
using Emote.Models;
using System.Collections;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class EmotionQuizManager : MonoBehaviour
    {
        public static class QuizState
        {
            public static int index;
            public static Emotions answer;
            public static string answered;
            public static Emotions[] options;
            public static List<Emotions> used = new List<Emotions>();
        }

        public VideoPlayer m_VideoPlayer;
        RenderTexture m_RenderTexture;

        public List<MEmotions> m_EmotionList;
        public GameObject m_QuizPanel;
        public Button m_NextButton;
        public static string m_EmotionsPath = "StaticFiles\\Emotions\\";
        public string m_NextScene = "TakePhoto";
        public CaptureDeviceManager m_DeviceManager;

        [HideInInspector]
        public int m_PlayCount = 0;

        public StoreDuration m_Duration
        {
            get
            {
                return GetComponent<StoreDuration>();
            }
        }

        public RawImage m_Image
        {
            get
            {
                return m_VideoPlayer.gameObject.GetComponent<RawImage>();
            }
        }

        public EmotionsListener m_Listener
        {
            get
            {
                return GetComponent<EmotionsListener>();
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

        public PipelineType pipelineType
        {
            get
            {
                List<PipelineType> types = DatabaseManager.m_PipelineType;
                foreach (var type in types)
                {
                    if (type.type == "EMOTION")
                    {
                        return type;
                    }
                }
                return null;
            }
        }

        public static string GetCombinedPath(string path)
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _path = Directory.GetParent(Application.dataPath).ToString();
            _path = Path.Combine(_path, path);
            return _path;
#else
            return "";
#endif
        }

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            m_EmotionsPath = GetCombinedPath(m_EmotionsPath);
#endif
            if (m_VideoPlayer == null)
            {
                m_VideoPlayer = GetComponentInChildren<VideoPlayer>();
                if (m_VideoPlayer == null) return;
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

            PopulateEmotions();

            // start quiz handler
            QuizState.index = 0;
            EmotionsQuizHandler();
        }

        public List<MEmotions> GetTrainingEmotions()
        {
            string path = "StaticFiles\\Training\\Emotions";
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            path = GetCombinedPath(path);
#endif
            string[] files = FileUtils.GetFileNames(path);

            List<MEmotions> emotions = new List<MEmotions>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext == ".mp4")
                {
                    MEmotions emotion = new MEmotions();
                    emotion.emotion = Random.Range(0, Emotions.GetNames(typeof(Emotions)).Length);
                    emotion.file = Path.GetFileName(file);

                    emotions.Add(emotion);
                }
            }

            return emotions;
        }

        void PopulateEmotions()
        {
            bool random = DatabaseManager.m_RandomQuestions;

            if (random)
                m_EmotionList = GetRandomEmotions();
            else
                m_EmotionList = GetStaticEmotions();
        }

        public List<MEmotions> GetStaticEmotions()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingEmotions();
            }
            return DatabaseManager.m_Emotions;
        }

        public List<MEmotions> GetRandomEmotions()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingEmotions();
            }
            MEmotionsSettings emotionSettings = DatabaseManager.m_MinMaxEmotions;

            int min = emotionSettings.min_emotions;
            int max = emotionSettings.max_emotions;

            List<MEmotions> disponibleEmotions = DatabaseManager.m_Emotions;
            List<MEmotions> selectedEmotions = new List<MEmotions>();
            int randomEmotions = 0;
            if (disponibleEmotions.Count <= 0 || max <= 0)
            {
                return new List<MEmotions>();
            }
            else
            {
                int _min = (min <= 0) ? 1 : min;
                randomEmotions = UnityEngine.Random.Range(_min, max);
            }

            for (int i = 0; i < randomEmotions; i++)
            {
                int index = UnityEngine.Random.Range(0, disponibleEmotions.Count);
                selectedEmotions.Add(disponibleEmotions[index]);
            }

            return selectedEmotions;
        }

        private void Update()
        {
            if (m_VideoPlayer.isPlaying)
                m_NextButton.enabled = false;

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

        void PlayVideo()
        {
            if (m_RenderTexture == null)
            {
                m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            }

            m_Image.texture = m_RenderTexture;

            m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            m_VideoPlayer.targetTexture = m_RenderTexture;

            m_VideoPlayer.Play();
            m_PlayCount++;
        }

        void SetCurrentEmotion(MEmotions emotion)
        {
            EmoteSession.current_resource_id = emotion.id;
            string path = m_EmotionsPath + emotion.file;
            if (EmoteSession.trainingMode)
            {
                path = "StaticFiles\\Training\\Emotions\\" + emotion.file;
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
                path = GetCombinedPath(path);
#endif
            }

            if (File.Exists(path))
            {
                path = "file://" + path;

                m_VideoPlayer.url = path;
                PlayVideo();
            }
        }

        void EmotionsQuizHandler()
        {
            SetCurrentEmotion(m_EmotionList[QuizState.index]);

            if (m_NextButton)
            {
                // disable next button
                m_NextButton.enabled = false;
            }

            // generate array of answers
            QuizState.answer = (Emotions)m_EmotionList[QuizState.index].emotion;
            QuizState.options = new Emotions[7];
            QuizState.options[6] = (Emotions)m_EmotionList[QuizState.index].emotion;

            QuizState.used.Clear();
            QuizState.used.Add(QuizState.answer);
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

        #region Handlers
        void OnNextClick()
        {
            if (QuizState.index < m_EmotionList.Count)
            {
                if (EmoteSession.enableNextKey && !m_VideoPlayer.isPlaying)
                {
                    if (!Emote.Utils.EmoteSession.trainingMode)
                    {
                        Answer answer = new Answer();
                        answer.pipeline_type_id = pipelineType.id;
                        answer.session_id = EmoteSession.session.id;
                        answer.correct = QuizOptionsManager.GetTraslatedEmotion(QuizState.answer);
                        answer.file = m_EmotionList[QuizState.index].file;

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

                        if (m_VideoPlayer.isPrepared && !m_VideoPlayer.isPlaying)
                        {
                            m_Listener.SaveCachedData(DatabaseManager.m_LastAnswer);
                        }
                    }
                    EmoteSession.enableNextKey = false;
                    EmotionsQuizHandler();
                }
            }
            else
            {
                if (EmoteSession.enableNextKey && !m_VideoPlayer.isPlaying)
                {
                    m_Duration.StoreData();
                    m_NextButton.enabled = false;
                    if (EmoteSession.trainingMode)
                    {
                        if (m_DeviceManager)
                        {
                            m_DeviceManager.m_Device.Stop();
                        }

                        EmoteSession.enableNextKey = false;
                        SceneManager.LoadScene("ModeScene", LoadSceneMode.Single);
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(m_NextScene))
                        {
                            if (m_DeviceManager)
                            {
                                m_DeviceManager.m_Device.Stop();
                            }

                            EmoteSession.enableNextKey = false;
                            SceneManager.LoadScene(m_NextScene, LoadSceneMode.Single);
                        }
                        else
                        {
                            Application.Quit();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
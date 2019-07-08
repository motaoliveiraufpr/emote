using System.IO;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Affdex;
using Emote.Database;
using Emote.Models;
using Emote.Utils;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class VideoQuizManager : MonoBehaviour
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

        public List<Videos> m_VideoList;
        public GameObject m_QuizPanel;
        public Button m_NextButton;
        public static string m_VideosPath = "StaticFiles\\Videos\\";
        public string m_NextScene = "AudioScene";
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
                    if (type.type == "VIDEO")
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
            m_VideosPath = GetCombinedPath(m_VideosPath);
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

            PopulateVideos();

            // start quiz handler
            QuizState.index = 0;
            EmotionsQuizHandler();
        }

        void PopulateVideos()
        {
            bool random = DatabaseManager.m_RandomQuestions;

            if (random)
                m_VideoList = GetRandomVideos();
            else
                m_VideoList = GetStaticVideos();
        }

        public List<Videos> GetTrainingVideos()
        {
            string path = "StaticFiles\\Training\\Videos";
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            path = GetCombinedPath(path);
#endif
            string[] files = FileUtils.GetFileNames(path);

            List<Videos> videos = new List<Videos>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext == ".mp4")
                {
                    Videos video = new Videos();
                    video.emotion = Random.Range(0, Emotions.GetNames(typeof(Emotions)).Length);
                    video.file = Path.GetFileName(file);

                    videos.Add(video);
                }
            }

            return videos;
        }

        public List<Videos> GetStaticVideos()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingVideos();
            }
            return DatabaseManager.m_Videos;
        }

        public List<Videos> GetRandomVideos()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingVideos();
            }

            VideoSettings videoSettings = DatabaseManager.m_MinMaxVideos;

            int min = videoSettings.min_videos;
            int max = videoSettings.max_videos;

            List<Videos> disponibleVideos = DatabaseManager.m_Videos;
            List<Videos> selectedVideos = new List<Videos>();
            int randomVideos = 0;
            if (disponibleVideos.Count <= 0 || max <= 0)
            {
                return new List<Videos>();
            }
            else
            {
                int _min = (min <= 0) ? 1 : min;
                randomVideos = UnityEngine.Random.Range(_min, max);
            }

            for (int i = 0; i < randomVideos; i++)
            {
                int index = UnityEngine.Random.Range(0, disponibleVideos.Count);
                selectedVideos.Add(disponibleVideos[index]);
            }

            return selectedVideos;
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

        void SetCurrentVideo(Videos video)
        {
            string path = m_VideosPath + video.file;
            if (EmoteSession.trainingMode)
            {
                path = "StaticFiles\\Training\\Videos\\" + video.file;
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
            SetCurrentVideo(m_VideoList[QuizState.index]);

            if (m_NextButton)
            {
                // disable next button
                m_NextButton.enabled = false;
            }

            // generate array of answers
            QuizState.answer = (Emotions)m_VideoList[QuizState.index].emotion;
            QuizState.options = new Emotions[7];
            QuizState.options[6] = (Emotions)m_VideoList[QuizState.index].emotion;

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
        void OnNextClick()
        {
            if (QuizState.index < m_VideoList.Count)
            {
                if (EmoteSession.enableNextKey && !m_VideoPlayer.isPlaying)
                {
                    if (!Emote.Utils.EmoteSession.trainingMode)
                    {
                        Answer answer = new Answer();
                        answer.pipeline_type_id = pipelineType.id;
                        answer.session_id = EmoteSession.session.id;
                        answer.correct = QuizOptionsManager.GetTraslatedEmotion(QuizState.answer);
                        answer.file = m_VideoList[QuizState.index].file;

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
                if (EmoteSession.enableNextKey && !m_VideoPlayer.isPlaying)
                {
                    m_Duration.StoreData();
                    m_NextButton.enabled = false;
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
#endregion
    }
}
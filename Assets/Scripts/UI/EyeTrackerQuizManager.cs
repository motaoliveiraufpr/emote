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
    public class EyeTrackerQuizManager : MonoBehaviour
    {
        public enum ResourceType
        {
            VIDEO, IMAGE, UNKNOW
        }

        public Canvas m_QuizCanvas;

        public static class QuizState
        {
            public static int index;
            public static Emotions answer;
            public static string answered;
            public static Emotions[] options;
            public static List<Emotions> used = new List<Emotions>();
        }

        public GameObject m_VideoQuad;
        public GameObject m_ImageQuad;

        public VideoPlayer m_VideoPlayer;
        RenderTexture m_RenderTexture;

        public List<EyeTracker> m_ResourceList;
        public GameObject m_QuizPanel;
        public Button m_NextButton;
        public static string m_ResourcesPath = "StaticFiles\\EyeTracking\\";
        public string m_NextScene = "";

        [HideInInspector]
        public int m_PlayCount = 0;

        [HideInInspector]
        public bool m_TimeFlag = false;

        private ResourceType m_ResourceType = ResourceType.UNKNOW;

        public EyeTrackingListener m_TrackerListener
        {
            get
            {
                return GetComponent<EyeTrackingListener>();
            }
        }

        public StoreDuration m_Duration
        {
            get
            {
                return GetComponent<StoreDuration>();
            }
        }

        public RawImage m_RawImage
        {
            get
            {
                return m_ImageQuad.GetComponentInChildren<RawImage>();
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
                    if (type.type == "EYETRACKING")
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
#endif
            return "";
        }

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            m_ResourcesPath = GetCombinedPath(m_ResourcesPath);
#endif
            m_QuizCanvas.enabled = false;
            if (m_VideoPlayer == null)
            {
                m_VideoPlayer = GetComponentInChildren<VideoPlayer>();
                if (m_VideoPlayer == null) return;
            }

            m_VideoPlayer.loopPointReached += EndReached;

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

            PopulateResources();

            // start quiz handler
            QuizState.index = 0;
            ResourceQuizHandler();
        }

        public List<EyeTracker> GetTrainingResources()
        {
            string path = "StaticFiles\\Training\\EyeTracking";
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            path = GetCombinedPath(path);
#endif
            string[] files = FileUtils.GetFileNames(path);

            List<EyeTracker> resources = new List<EyeTracker>();
            foreach (var file in files)
            {
                EyeTracker resource = new EyeTracker();
                resource.emotion = Random.Range(0, Emotions.GetNames(typeof(Emotions)).Length);
                resource.file = Path.GetFileName(file);

                resources.Add(resource);
            }

            return resources;
        }

        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            if (m_QuizCanvas)
                m_QuizCanvas.enabled = true;

            if (m_TrackerListener) m_TrackerListener.m_StoreThePresence = false;
        }

        void PopulateResources()
        {
            bool random = DatabaseManager.m_RandomQuestions;

            if (random)
                m_ResourceList = GetRandomResources();
            else
                m_ResourceList = GetStaticResources();
        }

        public List<EyeTracker> GetStaticResources()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingResources();
            }
            return DatabaseManager.m_Resources;
        }

        public List<EyeTracker> GetRandomResources()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingResources();
            }
            EyeTrackerSettings resourceSettings = DatabaseManager.m_MinMaxResources;

            int min = resourceSettings.min_resources;
            int max = resourceSettings.max_resources;

            List<EyeTracker> disponibleResources = DatabaseManager.m_Resources;
            List<EyeTracker> selectedResources = new List<EyeTracker>();
            int randomResources = 0;
            if (disponibleResources.Count <= 0 || max <= 0)
            {
                return new List<EyeTracker>();
            }
            else
            {
                int _min = (min <= 0) ? 1 : min;
                randomResources = UnityEngine.Random.Range(_min, max);
            }

            for (int i = 0; i < randomResources; i++)
            {
                int index = UnityEngine.Random.Range(0, disponibleResources.Count);
                selectedResources.Add(disponibleResources[index]);
            }

            return selectedResources;
        }

        private void Update()
        {
            if (m_ResourceType == ResourceType.VIDEO)
            {
                if (m_VideoPlayer.isPlaying)
                {
                    m_NextButton.enabled = false;
                    m_QuizCanvas.enabled = false;
                }
                else
                {
                    //m_QuizCanvas.enabled = true;
                }
            } else
            {
                m_NextButton.enabled = true;
            }

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

        void SetCurrentResource(EyeTracker resource)
        {
            string path = m_ResourcesPath + resource.file;
            if (EmoteSession.trainingMode)
            {
                path = "StaticFiles\\Training\\EyeTracking\\" + resource.file;
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
                path = GetCombinedPath(path);
#endif
            }

            if (File.Exists(path))
            {
                if (Path.GetExtension(path) == ".mp4")
                {
                    m_ResourceType = ResourceType.VIDEO;
                    m_VideoQuad.SetActive(true);

                    path = "file://" + path;

                    m_VideoPlayer.url = path;
                    PlayVideo();
                } else
                {
                    m_ResourceType = ResourceType.IMAGE;
                    m_ImageQuad.SetActive(true);

                    Texture2D texture = new Texture2D(2, 2);
                    byte[] rawData = File.ReadAllBytes(path);
                    texture.LoadImage(rawData);

                    if (m_RawImage)
                    {
                        m_RawImage.texture = texture;
                    }

                    m_TimeFlag = false;
                    StartCoroutine(AwaitSeconds());
                }
            }
        }

        IEnumerator AwaitSeconds()
        {
            yield return new WaitForSeconds(Emote.Utils.EmoteSession.awaitSecondsToShowImages);
            if (m_QuizCanvas) m_QuizCanvas.enabled = true;
            if (m_TrackerListener) m_TrackerListener.m_StoreThePresence = false;
            m_TimeFlag = true;
        }

        void ResourceQuizHandler()
        {
            if (m_TrackerListener) m_TrackerListener.m_StoreThePresence = true;

            if (m_VideoPlayer) m_VideoPlayer.Stop();
            if (m_QuizCanvas) m_QuizCanvas.enabled = false;

            m_VideoQuad.SetActive(false);
            m_ImageQuad.SetActive(false);

            SetCurrentResource(m_ResourceList[QuizState.index]);

            if (m_NextButton)
            {
                // disable next button
                m_NextButton.enabled = false;
            }

            // generate array of answers
            QuizState.answer = (Emotions)m_ResourceList[QuizState.index].emotion;
            QuizState.options = new Emotions[7];
            QuizState.options[6] = (Emotions)m_ResourceList[QuizState.index].emotion;

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
                Debug.Log(temp);
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

        void TakeScreenImage(string file = "default")
        {
            string session = (Emote.Utils.EmoteSession.session.id >= 0) ? Emote.Utils.EmoteSession.session.id.ToString() : "default";
            string path = GetCombinedPath("StaticFiles\\ScreenCapture\\EyeTracking\\" + session);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            ScreenCapture.CaptureScreenshot(path + "\\" + file);
        }

        #region Handlers
        void OnNextClick()
        {
            m_VideoQuad.SetActive(false);
            m_ImageQuad.SetActive(false);
            if (QuizState.index < m_ResourceList.Count)
            {
                if (EmoteSession.enableNextKey)
                {
                    if ((m_ResourceType == ResourceType.IMAGE && m_TimeFlag == true) || (m_ResourceType == ResourceType.VIDEO && !m_VideoPlayer.isPlaying))
                    {
                        if (!Emote.Utils.EmoteSession.trainingMode)
                        {
                            Answer answer = new Answer();
                            answer.pipeline_type_id = pipelineType.id;
                            answer.session_id = EmoteSession.session.id;
                            answer.correct = QuizOptionsManager.GetTraslatedEmotion(QuizState.answer);
                            answer.file = m_ResourceList[QuizState.index].file;

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

                            var lastId = DatabaseManager.m_LastAnswer.id;
                            TakeScreenImage("ref" + DatabaseManager.m_LastAnswer.id.ToString() + ".png");

                            if (m_TrackerListener)
                            {
                                m_TrackerListener.UpdateGazeDataRef();
                            }
                        }
                        EmoteSession.enableNextKey = false;
                        ResourceQuizHandler();
                    }
                }
            }
            else
            {
                if (EmoteSession.enableNextKey)
                {
                    if ((m_ResourceType == ResourceType.IMAGE && m_QuizCanvas.enabled == true) || (m_ResourceType == ResourceType.VIDEO && !m_VideoPlayer.isPlaying))
                    {
                        m_NextButton.enabled = false;
                        m_Duration.StoreData();
                        m_TrackerListener.SaveGazeData();
                        if (EmoteSession.trainingMode)
                        {
                            EmoteSession.enableNextKey = false;
                            SceneManager.LoadScene("ModeScene", LoadSceneMode.Single);
                        }
                        else
                        {
                            // stop session timer count
                            EmoteSession.time.Stop();
                            if (!EmoteSession.trainingMode)
                            {
                                EmoteSession.session.time = EmoteSession.current_time;
                                DatabaseManager.m_Session = EmoteSession.session;
                            }

                            if (!string.IsNullOrEmpty(m_NextScene))
                            {
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
        }
        #endregion
    }
}

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Affdex;
using Emote.Database;
using Emote.Models;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class AudioQuizManager : MonoBehaviour
    {
        public static class QuizState
        {
            public static int index;
            public static Emotions answer;
            public static string answered;
            public static Emotions[] options;
            public static List<Emotions> used = new List<Emotions>();
        }

        public AudioSource m_AudioPlayer;
        RenderTexture m_RenderTexture;

        public List<Audios> m_AudioList;
        public GameObject m_QuizPanel;
        public Button m_NextButton;
        public static string m_AudiosPath = "StaticFiles\\Audios\\";
        public string m_NextScene = "TakePhoto";
        public CaptureDeviceManager m_DeviceManager;

        private AudioClip m_AudioClip;

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
                return m_AudioPlayer.gameObject.GetComponent<RawImage>();
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
                    if (type.type == "AUDIO")
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
            m_AudiosPath = GetCombinedPath(m_AudiosPath);
#endif
            if (m_AudioPlayer == null)
            {
                m_AudioPlayer = GetComponentInChildren<AudioSource>();
                if (m_AudioPlayer == null) return;
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

            PopulateAudios();

            // start quiz handler
            QuizState.index = 0;
            EmotionsQuizHandler();
        }

        void PopulateAudios()
        {
            bool random = DatabaseManager.m_RandomQuestions;

            if (random)
                m_AudioList = GetRandomAudios();
            else
                m_AudioList = GetStaticAudios();
        }

        public List<Audios> GetTrainingAudios()
        {
            string path = "StaticFiles\\Training\\Audios";
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            path = GetCombinedPath(path);
#endif
            string[] files = FileUtils.GetFileNames(path);

            List<Audios> audios = new List<Audios>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext == ".wav")
                {
                    Audios audio = new Audios();
                    audio.emotion = Random.Range(0, Emotions.GetNames(typeof(Emotions)).Length);
                    audio.file = Path.GetFileName(file);

                    audios.Add(audio);
                }
            }

            return audios;
        }

        public List<Audios> GetStaticAudios()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingAudios();
            }
            return DatabaseManager.m_Audios;
        }

        public List<Audios> GetRandomAudios()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingAudios();
            }
            Models.AudioSettings audioSettings = DatabaseManager.m_MinMaxAudios;

            int min = audioSettings.min_audios;
            int max = audioSettings.max_audios;

            List<Audios> disponibleAudios = DatabaseManager.m_Audios;
            List<Audios> selectedAudios = new List<Audios>();
            int randomAudios = 0;
            if (disponibleAudios.Count <= 0 || max <= 0)
            {
                return new List<Audios>();
            }
            else
            {
                int _min = (min <= 0) ? 1 : min;
                randomAudios = UnityEngine.Random.Range(_min, max);
            }

            for (int i = 0; i < randomAudios; i++)
            {
                int index = UnityEngine.Random.Range(0, disponibleAudios.Count);
                selectedAudios.Add(disponibleAudios[index]);
            }

            return selectedAudios;
        }

        public void PlayAudio()
        {
            if (m_AudioClip)
            {
                m_AudioPlayer.PlayOneShot(m_AudioClip);
                m_PlayCount++;
            }
        }

        void SetCurrentAudio(Audios audio)
        {
            string path = m_AudiosPath + audio.file;
            if (EmoteSession.trainingMode)
            {
                path = "StaticFiles\\Training\\Audios\\" + audio.file;
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
                path = GetCombinedPath(path);
#endif
            }

            if (File.Exists(path))
            {
                path = "file://" + path;

                WWW response = new WWW(path);
                while (!response.isDone) ;
                m_AudioClip = response.GetAudioClip();

                PlayAudio();
            }
        }

        void EmotionsQuizHandler()
        {
            try
            {
                if (m_AudioPlayer.isPlaying)
                {
                    m_AudioPlayer.Stop();
                }

                SetCurrentAudio(m_AudioList[QuizState.index]);

                if (m_NextButton)
                {
                    // disable next button
                    m_NextButton.enabled = false;
                }

                // generate array of answers
                QuizState.answer = (Emotions)m_AudioList[QuizState.index].emotion;
                QuizState.options = new Emotions[7];
                QuizState.options[6] = (Emotions)m_AudioList[QuizState.index].emotion;

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
                //QuizState.index++;
            } catch (System.IndexOutOfRangeException e)
            {
                nextStep();
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
        void OnNextClick()
        {
            if (QuizState.index < m_AudioList.Count)
            {
                if (EmoteSession.enableNextKey && !m_AudioPlayer.isPlaying)
                {
                    if (!Emote.Utils.EmoteSession.trainingMode)
                    {
                        Answer answer = new Answer();
                        answer.pipeline_type_id = pipelineType.id;
                        answer.session_id = EmoteSession.session.id;
                        answer.correct = QuizOptionsManager.GetTraslatedEmotion(QuizState.answer);
                        answer.file = m_AudioList[QuizState.index].file;

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
                    QuizState.index++;

                    EmotionsQuizHandler();
                }
            }
            else
            {
                if (EmoteSession.enableNextKey && !m_AudioPlayer.isPlaying)
                {
                    nextStep();
                }
            }
        }

        private void nextStep()
        {
            m_Duration.StoreData();
            m_NextButton.enabled = false;
            if (!string.IsNullOrEmpty(m_NextScene))
            {
                if (m_DeviceManager)
                {
                    m_DeviceManager.secureDeviceStop();
                    //m_DeviceManager.m_Device.Stop();
                }
                EmoteSession.enableNextKey = false;
                SceneManager.LoadScene(m_NextScene, LoadSceneMode.Single);
            }
            else
            {
                Application.Quit();
            }
        }
#endregion
    }
}
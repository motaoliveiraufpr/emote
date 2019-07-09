using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Emote.Database;
using Emote.Models;
using Affdex;
using Emote.Utils;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class ImageQuizManager : MonoBehaviour
    {
        public static class QuizState
        {
            public static int index;
            public static Emotions answer;
            public static string answered;
            public static Emotions[] options;
            public static List<Emotions> used = new List<Emotions>();
        }

        public List<Images> m_ImageList;
        public GameObject m_QuizPanel;
        public Button m_NextButton;
        public static string m_ImagesPath = "StaticFiles\\Images\\";
        public string m_NextScene = "VideoScene";
        public CaptureDeviceManager m_DeviceManager;

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
                return GetComponentInChildren<RawImage>();
            }
        }

        public PipelineType pipelineType
        {
            get
            {
                List<PipelineType> types = DatabaseManager.m_PipelineType;
                foreach (var type in types)
                {
                    if (type.type == "IMAGE")
                    {
                        return type;
                    }
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

            PopulateImages();

            // start quiz handler
            QuizState.index = 0;
            EmotionsQuizHandler();
        }

        void PopulateImages()
        {
            bool random = DatabaseManager.m_RandomQuestions;

            if (random)
                m_ImageList = GetRandomImages();
            else
                m_ImageList = GetStaticImages();
        }

        void EmotionsQuizHandler()
        {
            SetCurrentImage(m_ImageList[QuizState.index]);

            if (m_NextButton)
            {
                // disable next button
                m_NextButton.enabled = false;
            }

            // generate array of answers
            QuizState.answer = (Emotions)m_ImageList[QuizState.index].emotion;
            QuizState.options = new Emotions[7];
            QuizState.options[6] = (Emotions)m_ImageList[QuizState.index].emotion;

            QuizState.used.Clear();
            if (EmoteSession.randomOptions)
            {
                QuizState.used.Add(QuizState.answer);
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

        void SetCurrentImage(Images image)
        {
            string path = m_ImagesPath + image.file;
            if (EmoteSession.trainingMode)
            {
                path = "StaticFiles\\Training\\Images\\" + image.file;
            }
            Texture2D texture = new Texture2D(2, 2);

            if (File.Exists(path))
            {
                byte[] rawData = File.ReadAllBytes(path);
                texture.LoadImage(rawData);

                if (m_RawImage)
                {
                    m_RawImage.texture = texture;
                }
            }
        }

        public static int GetRandomImagesLength(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public List<Images> GetStaticImages()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingImages();
            }
            return DatabaseManager.m_Images;
        }

        public List<Images> GetTrainingImages()
        {
            string path = "StaticFiles\\Training\\Images";
            string[] files = FileUtils.GetFileNames(path);

            List<Images> images = new List<Images>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (ext == ".jpg") {
                    Images image = new Images();
                    image.emotion = Random.Range(0, Emotions.GetNames(typeof(Emotions)).Length);
                    image.file = Path.GetFileName(file);

                    images.Add(image);
                }
            }

            return images;
        }

        public List<Images> GetRandomImages()
        {
            if (EmoteSession.trainingMode)
            {
                return GetTrainingImages();
            }

            ImageSettings imageSettings = DatabaseManager.m_MinMaxImages;

            int min = imageSettings.min_images;
            int max = imageSettings.max_images;

            List<Images> disponibleImages = DatabaseManager.m_Images;
            List<Images> selectedImages = new List<Images>();
            int randomImages = 0;
            if (disponibleImages.Count <= 0 || max <= 0)
            {
                return new List<Images>();
            }
            else
            {
                int _min = (min <= 0) ? 1 : min;
                randomImages = UnityEngine.Random.Range(_min, max);
            }

            for (int i = 0; i < randomImages; i++)
            {
                int index = UnityEngine.Random.Range(0, disponibleImages.Count);
                selectedImages.Add(disponibleImages[index]);
            }

            return selectedImages;
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
            if (QuizState.index < m_ImageList.Count)
            {
                if (EmoteSession.enableNextKey)
                {
                    if (!Emote.Utils.EmoteSession.trainingMode)
                    {
                        Answer answer = new Answer();
                        answer.pipeline_type_id = pipelineType.id;
                        answer.session_id = EmoteSession.session.id;
                        answer.correct = QuizOptionsManager.GetTraslatedEmotion(QuizState.answer);
                        answer.file = m_ImageList[QuizState.index].file;

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
            }
        }
#endregion
    }
}
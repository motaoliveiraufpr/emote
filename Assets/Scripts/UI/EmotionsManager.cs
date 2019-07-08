using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using Emote.Models;
using Emote.Database;
using GracesGames.SimpleFileBrowser.Scripts;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class EmotionsManager : MonoBehaviour
    {
        public Button m_AddEmotion;
        public static string m_EmotionsPath = "StaticFiles\\Emotions\\";
        public Animator m_FilePopup;

        // Use the file browser prefab
        public GameObject m_FileBrowsePrefab;

        // Define a file extension
        public string[] m_FileExtensions = { "mp4" };

        public bool m_PortraitMode = false;
        public EmotionsTypeManager m_TypeManager;
        public GameObject m_ListObjectPanel;
        public GameObject m_ListObjectPrefab;

        private string m_File;
        private int m_MaxEmotions = 3;
        private List<MEmotions> m_Emotions;

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _path = Directory.GetParent(Application.dataPath).ToString();
            _path = Path.Combine(_path, m_EmotionsPath);
            m_EmotionsPath = _path;
#endif
            if (m_AddEmotion == null)
            {
                return;
            }

            if (!m_FileBrowsePrefab)
            {
                m_AddEmotion.enabled = false;
            }

            m_AddEmotion.onClick.AddListener(delegate
            {
                OnClick();
            });

            m_Emotions = DatabaseManager.m_Emotions;
            PopulateEmotions();
        }

        void Update()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_File))
                {
                    // clone video
                    if (File.Exists(m_File))
                    {
                        if (Directory.Exists(m_EmotionsPath))
                        {
                            string copyTo = m_EmotionsPath + Path.GetFileName(m_File);
                            if (File.Exists(copyTo))
                            {
                                m_File = null;
                                return;
                            }
                            File.Copy(m_File, copyTo);
                        }
                        else
                        {
                            Directory.CreateDirectory(m_EmotionsPath);
                            File.Copy(m_File, m_EmotionsPath);
                        }
                    }

                    List<MEmotions> emotions = new List<MEmotions>();

                    MEmotions emotion = new MEmotions();
                    emotion.file = Path.GetFileName(m_File);
                    emotion.emotion = m_TypeManager.m_Dropdown.value;

                    emotions.Add(emotion);
                    DatabaseManager.m_Emotions = emotions;
                    AddEmotion(emotion);

                    m_File = null;
                }
            }
            catch (Exception exception)
            {
                m_File = null;
                Debug.LogError(exception.Message);
                return;
            }
        }

        void OnClick()
        {
            m_FilePopup.Play("Fade-in");
            OpenFileBrowser(FileBrowserMode.Load);
        }

        // Open a file browser to save and load files
        private void OpenFileBrowser(FileBrowserMode fileBrowserMode)
        {
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(m_FileBrowsePrefab, transform);
            fileBrowserObject.name = "Emote Browser";

            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(m_PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
            if (fileBrowserMode == FileBrowserMode.Load)
            {
                fileBrowserScript.OpenFilePanel(m_FileExtensions);
                // Subscribe to OnFileSelect event (call LoadFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += LoadFileUsingPath;
            }
        }

        // Loads a file using a path
        private void LoadFileUsingPath(string path)
        {
            if (path.Length != 0)
            {
                m_File = path;
            }
            else
            {
                Debug.Log("Invalid path given");
            }
        }

        void PopulateEmotions()
        {
            int count = 0;
            foreach (var emotion in m_Emotions)
            {
                if (count <= m_MaxEmotions)
                {
                    AddEmotion(emotion);
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        void AddEmotion(MEmotions emotion)
        {
            if (m_ListObjectPanel)
            {
                GameObject instance = Instantiate(m_ListObjectPrefab);
                if (instance != null)
                {
                    var manager = instance.GetComponent<EmotionListManager>();
                    if (manager)
                    {
                        manager.m_IntanceRef = instance;
                        manager.m_CurrentEmotion = emotion;
                    }
                }
                instance.transform.SetParent(m_ListObjectPanel.transform);
            }
        }
    }
}
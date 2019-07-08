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
    public class AudiosManager : MonoBehaviour
    {
        public Button m_AddAudio;
        public static string m_AudiosPath = "StaticFiles\\Audios\\";
        public Animator m_FilePopup;

        // Use the file browser prefab
        public GameObject m_FileBrowsePrefab;

        // Define a file extension
        public string[] m_FileExtensions = { "wav" };

        public bool m_PortraitMode = false;
        public EmotionsTypeManager m_TypeManager;
        public GameObject m_ListObjectPanel;
        public GameObject m_ListObjectPrefab;

        private string m_File;
        private int m_MaxAudios = 3;
        private List<Audios> m_Audios;

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _path = Directory.GetParent(Application.dataPath).ToString();
            _path = Path.Combine(_path, m_AudiosPath);
            m_AudiosPath = _path;
#endif
            if (m_AddAudio == null)
            {
                return;
            }

            if (!m_FileBrowsePrefab)
            {
                m_AddAudio.enabled = false;
            }

            m_AddAudio.onClick.AddListener(delegate
            {
                OnClick();
            });

            m_Audios = DatabaseManager.m_Audios;
            PopulateAudios();
        }

        void Update()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_File))
                {
                    // clone audio
                    if (File.Exists(m_File))
                    {
                        if (Directory.Exists(m_AudiosPath))
                        {
                            string copyTo = m_AudiosPath + Path.GetFileName(m_File);
                            if (File.Exists(copyTo))
                            {
                                m_File = null;
                                return;
                            }
                            File.Copy(m_File, copyTo);
                        }
                        else
                        {
                            Directory.CreateDirectory(m_AudiosPath);
                            File.Copy(m_File, m_AudiosPath);
                        }
                    }

                    List<Audios> audios = new List<Audios>();

                    Audios audio = new Audios();
                    audio.file = Path.GetFileName(m_File);
                    audio.emotion = m_TypeManager.m_Dropdown.value;

                    audios.Add(audio);
                    DatabaseManager.m_Audios = audios;
                    AddAudio(audio);

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

        void PopulateAudios()
        {
            int count = 0;
            foreach (var audio in m_Audios)
            {
                if (count <= m_MaxAudios)
                {
                    AddAudio(audio);
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        void AddAudio(Audios audio)
        {
            if (m_ListObjectPanel)
            {
                GameObject instance = Instantiate(m_ListObjectPrefab);
                if (instance != null)
                {
                    var manager = instance.GetComponent<AudioListManager>();
                    if (manager)
                    {
                        manager.m_IntanceRef = instance;
                        manager.m_CurrentAudio = audio;
                    }
                }
                instance.transform.SetParent(m_ListObjectPanel.transform);
            }
        }
    }
}
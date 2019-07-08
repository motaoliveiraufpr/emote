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
    public class VideosManager : MonoBehaviour
    {
        public Button m_AddVideo;
        public static string m_VideosPath = "StaticFiles\\Videos\\";
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
        private int m_MaxVideos = 3;
        private List<Videos> m_Videos;

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _path = Directory.GetParent(Application.dataPath).ToString();
            _path = Path.Combine(_path, m_VideosPath);
            m_VideosPath = _path;
#endif
            if (m_AddVideo == null)
            {
                return;
            }

            if (!m_FileBrowsePrefab)
            {
                m_AddVideo.enabled = false;
            }

            m_AddVideo.onClick.AddListener(delegate
            {
                OnClick();
            });

            m_Videos = DatabaseManager.m_Videos;
            PopulateVideos();
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
                        if (Directory.Exists(m_VideosPath))
                        {
                            string copyTo = m_VideosPath + Path.GetFileName(m_File);
                            if (File.Exists(copyTo))
                            {
                                m_File = null;
                                return;
                            }
                            File.Copy(m_File, copyTo);
                        }
                        else
                        {
                            Directory.CreateDirectory(m_VideosPath);
                            File.Copy(m_File, m_VideosPath);
                        }
                    }

                    List<Videos> videos = new List<Videos>();

                    Videos video = new Videos();
                    video.file = Path.GetFileName(m_File);
                    video.emotion = m_TypeManager.m_Dropdown.value;

                    videos.Add(video);
                    DatabaseManager.m_Videos = videos;
                    AddVideo(video);

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

        void PopulateVideos()
        {
            int count = 0;
            foreach (var video in m_Videos)
            {
                if (count <= m_MaxVideos)
                {
                    AddVideo(video);
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        void AddVideo(Videos video)
        {
            if (m_ListObjectPanel)
            {
                GameObject instance = Instantiate(m_ListObjectPrefab);
                if (instance != null)
                {
                    var manager = instance.GetComponent<VideoListManager>();
                    if (manager)
                    {
                        manager.m_IntanceRef = instance;
                        manager.m_CurrentVideo = video;
                    }
                }
                instance.transform.SetParent(m_ListObjectPanel.transform);
            }
        }
    }
}
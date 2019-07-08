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
    public class EyeTrackingManager : MonoBehaviour
    {
        public Button m_AddResource;
        public static string m_ResourcesPath = "StaticFiles\\EyeTracking\\";
        public Animator m_FilePopup;

        // Use the file browser prefab
        public GameObject m_FileBrowsePrefab;

        // Define a file extension
        public string[] m_FileExtensions = { "mp4", "jpg" };

        public bool m_PortraitMode = false;
        public EmotionsTypeManager m_TypeManager;
        public GameObject m_ListObjectPanel;
        public GameObject m_ListObjectPrefab;

        private int m_MaxResources = 3;
        private string m_File;
        private List<EyeTracker> m_Resources;

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _path = Directory.GetParent(Application.dataPath).ToString();
            _path = Path.Combine(_path, m_ResourcesPath);
            m_ResourcesPath = _path;
#endif
            if (m_AddResource == null)
            {
                return;
            }

            if (!m_FileBrowsePrefab)
            {
                m_AddResource.enabled = false;
            }

            m_AddResource.onClick.AddListener(delegate
            {
                OnClick();
            });

            m_Resources = DatabaseManager.m_Resources;
            PopulateResources();
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
                        if (Directory.Exists(m_ResourcesPath))
                        {
                            string copyTo = m_ResourcesPath + Path.GetFileName(m_File);
                            if (File.Exists(copyTo))
                            {
                                m_File = null;
                                return;
                            }
                            File.Copy(m_File, copyTo);
                        }
                        else
                        {
                            Directory.CreateDirectory(m_ResourcesPath);
                            File.Copy(m_File, m_ResourcesPath);
                        }
                    }

                    List<EyeTracker> resources = new List<EyeTracker>();

                    EyeTracker resource = new EyeTracker();
                    resource.file = Path.GetFileName(m_File);
                    resource.emotion = m_TypeManager.m_Dropdown.value;

                    resources.Add(resource);
                    DatabaseManager.m_Resources = resources;
                    AddResource(resource);

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

        void PopulateResources()
        {
            int count = 0;
            foreach (var resource in m_Resources)
            {
                if (count <= m_MaxResources)
                {
                    AddResource(resource);
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        void AddResource(EyeTracker resource)
        {
            if (m_ListObjectPanel)
            {
                GameObject instance = Instantiate(m_ListObjectPrefab);
                if (instance != null)
                {
                    var manager = instance.GetComponent<EyeTrackerListManager>();
                    if (manager)
                    {
                        manager.m_IntanceRef = instance;
                        manager.m_CurrentResource = resource;
                    }
                }
                instance.transform.SetParent(m_ListObjectPanel.transform);
            }
        }
    }
}
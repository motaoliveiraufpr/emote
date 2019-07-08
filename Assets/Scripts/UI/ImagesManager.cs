using UnityEngine;
using UnityEngine.UI;
using GracesGames.SimpleFileBrowser.Scripts;
using Emote.Database;
using Emote.Models;
using System.Collections.Generic;
using System.IO;
using System;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class ImagesManager : MonoBehaviour
    {
        public Button m_AddImage;
        public static string m_ImagesPath = "StaticFiles\\Images\\";
        public Animator m_FilePopup;

        // Use the file browser prefab
        public GameObject m_FileBrowsePrefab;

        // Define a file extension
        public string[] m_FileExtensions = { "png", "jpg" };

        public bool m_PortraitMode = false;
        public EmotionsTypeManager m_TypeManager;
        public GameObject m_ListObjectPanel;
        public GameObject m_ListObjectPrefab;

        private string m_File;
        private int m_MaxImages = 3;
        private List<Images> m_Images;
        private GameObject fileBrowserObject;

        void Start()
        {
            if (m_AddImage == null)
            {
                return;
            }

            if (!m_FileBrowsePrefab)
            {
                m_AddImage.enabled = false;
            }

            m_AddImage.onClick.AddListener(delegate
            {
                OnClick();
            });

            m_Images = DatabaseManager.m_Images;
            PopulateImages();
        }

        void Update()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_File))
                {
                    // clone image
                    if (File.Exists(m_File))
                    {
                        if (Directory.Exists(m_ImagesPath))
                        {
                            string copyTo = m_ImagesPath + Path.GetFileName(m_File);
                            if (File.Exists(copyTo))
                            {
                                m_File = null;
                                return;
                            }
                            File.Copy(m_File, copyTo);
                        }
                        else
                        {
                            Directory.CreateDirectory(m_ImagesPath);
                            File.Copy(m_File, m_ImagesPath);
                        }
                    }

                    List<Images> images = new List<Images>();

                    Images image = new Images();
                    image.file = Path.GetFileName(m_File);
                    image.emotion = m_TypeManager.m_Dropdown.value;

                    images.Add(image);
                    DatabaseManager.m_Images = images;
                    AddImage(image);

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

        void PopulateImages()
        {
            int count = 0;
            foreach (var image in m_Images)
            {
                if (count <= m_MaxImages)
                {
                    AddImage(image);
                    count++;
                }
                else
                {
                    break;
                }
            }
        }

        void AddImage(Images image)
        {
            if (m_ListObjectPanel)
            {
                GameObject instance = Instantiate(m_ListObjectPrefab);
                if (instance != null)
                {
                    var manager = instance.GetComponent<ImageListManager>();
                    if (manager)
                    {
                        manager.m_IntanceRef = instance;
                        manager.m_CurrentImage = image;
                    }
                }
                instance.transform.SetParent(m_ListObjectPanel.transform);
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
            fileBrowserObject = Instantiate(m_FileBrowsePrefab, transform);
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
    }
}
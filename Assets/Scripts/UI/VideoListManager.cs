using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using Emote.Models;
using System.IO;
using Emote.Database;
using TMPro;
using Affdex;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class VideoListManager : MonoBehaviour
    {
        public Videos m_CurrentVideo;
        public GameObject m_IntanceRef;
        public static string m_VideosPath = "StaticFiles\\Videos\\";
        public GameObject m_VideoPlayerPrefab;

        private GameObject m_PlayerRef;
        private VideoPlayer m_Player;
        private RenderTexture m_RenderTexture;

        public class ImagePreviewHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public VideoPlayer m_Player;

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (m_Player)
                {
                    if (m_Player.isPlaying) m_Player.Stop();
                    m_Player.Play();
                }
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                m_Player.Stop();
            }
        }

        public RawImage m_RawImage
        {
            get
            {
                return GetComponentInChildren<RawImage>();
            }
        }

        public TextMeshProUGUI m_Text
        {
            get
            {
                return GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        public Button m_Button
        {
            get
            {
                return GetComponentInChildren<Button>();
            }
        }

        void Start()
        {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _path = Directory.GetParent(Application.dataPath).ToString();
            _path = Path.Combine(_path, m_VideosPath);
            m_VideosPath = _path;
#endif
            if (m_VideoPlayerPrefab)
            {
                m_PlayerRef = Instantiate(m_VideoPlayerPrefab);
                if (m_PlayerRef)
                {
                    m_Player = m_PlayerRef.GetComponent<UnityEngine.Video.VideoPlayer>();
                }
            }

            StartCoroutine(PutImage());
            PutText();

            if (m_Button)
            {
                m_Button.onClick.AddListener(delegate
                {
                    RemoveItem();
                });
            }

            if (m_RawImage)
            {
                ImagePreviewHandler ihandler = m_RawImage.gameObject.AddComponent<ImagePreviewHandler>();
                ihandler.m_Player = m_Player;
            }
        }

        void PutText()
        {
            if (m_CurrentVideo != null)
            {
                TextMeshProUGUI text = m_Text;
                if (text)
                {
                    text.text = string.Format
                    (
                        "Nome: {0}\nExpressão: {1}\nCriado em: {2}",
                        m_CurrentVideo.file,
                        QuizOptionsManager.GetTraslatedEmotion((Emotions)m_CurrentVideo.emotion),
                        m_CurrentVideo.created_at.ToShortDateString()
                    );
                }
            }
        }

        IEnumerator PutImage()
        {
            if (m_CurrentVideo != null)
            {
                string path = m_VideosPath + m_CurrentVideo.file;

                if (File.Exists(path))
                {
                    if (m_Player)
                    {
                        m_Player.url = "file://" + path;
                        m_Player.playOnAwake = false;
                        m_Player.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.None;
                    }
                    else
                    {
                        yield return null;
                    }

                    if (m_RenderTexture == null)
                    {
                        m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                    }

                    var mainTexture = m_RawImage;
                    if (mainTexture)
                    {
                        mainTexture.texture = m_RenderTexture;
                        m_Player.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
                        m_Player.targetTexture = m_RenderTexture;
                        m_Player.Play();
                    }

                    yield return new WaitForSecondsRealtime(1.0f);
                    m_Player.Pause();
                    //Destroy(m_PlayerRef);
                }
            }
        }

        void RemoveItem()
        {
            if (m_IntanceRef)
            {
                string path = m_VideosPath + m_CurrentVideo.file;

                Destroy(m_IntanceRef);

                List<Videos> videos = new List<Videos>();
                videos.Add(m_CurrentVideo);
                DatabaseManager.RemoveVideos(videos);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
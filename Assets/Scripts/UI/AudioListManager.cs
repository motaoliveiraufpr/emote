using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
    public class AudioListManager : MonoBehaviour
    {
        public Audios m_CurrentAudio;
        public GameObject m_IntanceRef;
        public static string m_AudiosPath = "StaticFiles\\Audios\\";
        public GameObject m_AudioPlayerPrefab;

        private GameObject m_PlayerRef;
        private AudioSource m_Player;
        private AudioClip m_AudioClip;
        private RenderTexture m_RenderTexture;

        private ImagePreviewHandler ihandler;

        public class ImagePreviewHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public AudioSource m_Player;
            public string m_Path;
            private AudioClip m_AudioClip;

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (m_Player && !string.IsNullOrEmpty(m_Path))
                {
                    if (m_AudioClip == null)
                    {
                        WWW response = new WWW(m_Path);
                        while (!response.isDone) ;
                        m_AudioClip = response.GetAudioClip();

                    }
                    m_Player.PlayOneShot(m_AudioClip);
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
            _path = Path.Combine(_path, m_AudiosPath);
            m_AudiosPath = _path;
#endif
            if (m_AudioPlayerPrefab)
            {
                m_PlayerRef = Instantiate(m_AudioPlayerPrefab);
                if (m_PlayerRef)
                {
                    m_Player = m_PlayerRef.GetComponent<AudioSource>();
                }
            }

            if (m_RawImage)
            {
                ihandler = m_RawImage.gameObject.AddComponent<ImagePreviewHandler>();
                ihandler.m_Player = m_Player;
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
        }

        void PutText()
        {
            if (m_CurrentAudio != null)
            {
                TextMeshProUGUI text = m_Text;
                if (text)
                {
                    text.text = string.Format
                    (
                        "Nome: {0}\nExpressão: {1}\nCriado em: {2}",
                        m_CurrentAudio.file,
                        QuizOptionsManager.GetTraslatedEmotion((Emotions)m_CurrentAudio.emotion),
                        m_CurrentAudio.created_at.ToShortDateString()
                    );
                }
            }
        }

        IEnumerator PutImage()
        {
            if (m_CurrentAudio != null)
            {
                string path = m_AudiosPath + m_CurrentAudio.file;

                if (File.Exists(path))
                {
                    if (m_Player)
                    {
                        path = "file://" + path;
                        m_Player.playOnAwake = false;

                        ihandler.m_Path = path;
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
                        // mainTexture.texture = m_RenderTexture;
                    }
                }
            }
        }

        void RemoveItem()
        {
            if (m_IntanceRef)
            {
                string path = m_AudiosPath + m_CurrentAudio.file;

                Destroy(m_IntanceRef);

                List<Audios> audios = new List<Audios>();
                audios.Add(m_CurrentAudio);
                DatabaseManager.RemoveAudios(audios);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
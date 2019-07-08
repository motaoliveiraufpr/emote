using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public class ImageListManager : MonoBehaviour
    {
        public Images m_CurrentImage;
        public GameObject m_IntanceRef;
        public static string m_ImagesPath = "StaticFiles\\Images\\";

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
            PutImage();
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
            if (m_CurrentImage != null)
            {
                TextMeshProUGUI text = m_Text;
                if (text)
                {
                    text.text = string.Format
                    (
                        "Nome: {0}\nExpressão: {1}\nCriado em: {2}",
                        m_CurrentImage.file,
                        QuizOptionsManager.GetTraslatedEmotion((Emotions)m_CurrentImage.emotion),
                        m_CurrentImage.created_at.ToShortDateString()
                    );
                }
            }
        }

        void PutImage()
        {
            if (m_CurrentImage != null)
            {
                string path = m_ImagesPath + m_CurrentImage.file;

                Texture2D texture = new Texture2D(2, 2);
                if (File.Exists(path))
                {
                    byte[] rawData = File.ReadAllBytes(path);

                    texture.LoadImage(rawData);

                    var mainTexture = m_RawImage;
                    if (mainTexture)
                    {
                        mainTexture.texture = texture;
                    }
                }
            }
        }

        void RemoveItem()
        {
            if (m_IntanceRef)
            {
                string path = m_ImagesPath + m_CurrentImage.file;

                Destroy(m_IntanceRef);

                List<Images> images = new List<Images>();
                images.Add(m_CurrentImage);
                DatabaseManager.RemoveImages(images);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
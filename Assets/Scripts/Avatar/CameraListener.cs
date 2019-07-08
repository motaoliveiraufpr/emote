using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Affdex;
using Emote.Utils;
using Emote.Avatar;
using Emote.Database;
using System;
using System.IO;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
public class CameraListener : ImageResultsListener
{
    public GameObject m_Preview;
    public GameObject m_Camera;
    public Emote.Utils.CountDown m_CountDown;
    public AvatarGenerator m_Generator;
    public string m_LoadSceneAfter = "AvatarScene";
    public bool m_SaveSelfie = true;
    public string m_SelfiePath = EmoteSession.selfie_path;

    #region Detected/Not detected marks
    public GameObject m_Detected;
    public GameObject m_NotDetected;
    #endregion

    private Coroutine m_CountDownRoutine;

    #region Custom properties
    private RawImage m_Frame
    {
        get
        {
            return m_Preview.GetComponent<RawImage>();
        }
    }

    private EmoteCameraInput m_CameraInput
    {
        get
        {
            if (m_Camera)
            {
                return m_Camera.GetComponent<EmoteCameraInput>();
            }
            return null;
        }
    }

    private Detector m_Detector
    {
        get
        {
            if (m_Camera)
            {
                return m_Camera.GetComponent<Detector>();
            }
            return null;
        }
    }
    #endregion

    #region Handlers
    public override void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found the face: " + faceId);

        if (m_Detected && m_NotDetected)
        {
            m_NotDetected.SetActive(false);
            m_Detected.SetActive(true);
        }

        /* start countdown */
        if (m_Detector)
        {
            m_CountDownRoutine = StartCoroutine(m_CountDown.StartCountDown());
        }
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face: " + faceId);

        if (m_CountDownRoutine != null)
        {
            StopCoroutine(m_CountDownRoutine);
            m_CountDown.SetText("");
        }

        if (m_Detected && m_NotDetected)
        {
            m_Detected.SetActive(false);
            m_NotDetected.SetActive(true);
        }
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
    }
    #endregion

    private void Start()
    {
        if (m_Detected && m_NotDetected)
        {
            m_Detected.SetActive(false);
            m_NotDetected.SetActive(true);
        }
    }

    private void Update()
    {
        if (m_CountDown && m_CountDown.m_IsCompleted)
        {
            m_Detector.enabled = false;

            // take image and send to create new avatar
            Texture2D texture = TakeCurrentFrame();

            PauseWebCamTexture();
            if (texture != null)
            {
                byte[] data = texture.EncodeToJPG(100);

                // save photo to database here
                if (m_SaveSelfie)
                {
                    try
                    {
                        if (!EmoteSession.trainingMode)
                        {
                            if (!string.IsNullOrEmpty(m_SelfiePath))
                            {
                                if (!Directory.Exists(m_SelfiePath))
                                {
                                    Directory.CreateDirectory(m_SelfiePath);
                                }

                                string fileName = EmoteSession.session.reference + ".jpg";
                                File.WriteAllBytes(m_SelfiePath + fileName, data);

                                EmoteSession.session.selfie = fileName;
                                DatabaseManager.m_Session = EmoteSession.session;
                            }
                        }
                    } catch(Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }

                StartCoroutine(m_Generator.CreateNewAvatar(data));
            }

            // reset countdown status
            m_CountDown.ResetStatus();
        }

        // load scene when completed
        if (!string.IsNullOrEmpty(CurrentAvatar.code))
        {
            if (CurrentAvatar.state == AvatarState.COMPLETED)
            {
                if (!string.IsNullOrEmpty(m_LoadSceneAfter))
                {
                    SceneManager.LoadScene(m_LoadSceneAfter, LoadSceneMode.Single);
                }
            }
        }
    }

    private void PauseWebCamTexture()
    {
        if (m_CameraInput.WebCamTexture.isPlaying)
        {
            m_CameraInput.WebCamTexture.Pause();
        }
    }

    private void StopWebCamTexture()
    {
        if (m_CameraInput.WebCamTexture.isPlaying)
        {
            m_CameraInput.WebCamTexture.Stop();
        }
    }

    private Texture2D TakeCurrentFrame()
    {
        WebCamTexture w_Texture;
        Texture2D texture;

        if (m_Frame != null)
        {
            w_Texture = (WebCamTexture) m_Frame.mainTexture;
            if (w_Texture != null)
            {
                texture = new Texture2D(w_Texture.width, w_Texture.height);
                texture.SetPixels(w_Texture.GetPixels());
                texture.Apply();

                return texture;
            }
        }

        return null;
    }
}
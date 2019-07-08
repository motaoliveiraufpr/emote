using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Affdex;
using Emote.Database;
using Emote.Models;
using System;
using UnityEngine.UI;
using Emote.Utils;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
public class EmotionsListener : ImageResultsListener
{
    #region Detected/Not detected marks
    public GameObject m_Detected;
    public GameObject m_NotDetected;
    #endregion

    public EmotionQuizManager m_Manager
    {
        get
        {
            return GetComponent<EmotionQuizManager>();
        }
    }

    List<Emote.Models.Emotion> emotionList = new List<Emotion>();

    #region Handlers
    public override void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found the face: " + faceId);

        if (m_Detected && m_NotDetected)
        {
            m_NotDetected.SetActive(false);
            m_Detected.SetActive(true);
        }
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face: " + faceId);

        if (m_Detected && m_NotDetected)
        {
            m_Detected.SetActive(false);
            m_NotDetected.SetActive(true);
        }
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
        if (faces.Count > 0 && !Emote.Utils.EmoteSession.trainingMode)
        {
            if (m_Manager && m_Manager.m_VideoPlayer.isPlaying)
            {
                foreach (KeyValuePair<int, Face> pair in faces)
                {
                    int faceID = pair.Key;
                    Face face = pair.Value;

                    Emote.Models.Emotion emotion = new Emote.Models.Emotion();
                    emotion.session_id = Emote.Utils.EmoteSession.session.id;
                    emotion.created_at = DateTime.Now;

                    foreach (Emotions _emotion in Enum.GetValues(typeof(Emotions)))
                    {
                        float value;
                        face.Emotions.TryGetValue(_emotion, out value);

                        switch (_emotion)
                        {
                            case Emotions.Joy:
                                emotion.Joy = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Fear:
                                emotion.Fear = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Disgust:
                                emotion.Disgust = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Sadness:
                                emotion.Sadness = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Anger:
                                emotion.Anger = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Surprise:
                                emotion.Surprise = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Contempt:
                                emotion.Contempt = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                            case Emotions.Valence:
                                emotion.Valence = NormalizeValue(value, 100.0f, 100.0f);
                                break;
                            case Emotions.Engagement:
                                emotion.Engagement = NormalizeValue(value, 0.0f, 100.0f);
                                break;
                        }
                    }

                    emotionList.Add(emotion);
                }
            }
        }
    }
    #endregion

    float NormalizeValue(float value, float min, float max)
    {
        if (value < min) value = min;
        if (value > max) value = max;
        return value;
    }

    public void SaveCachedData()
    {
        DatabaseManager.m_Emotion = emotionList;
    }

    public void SaveCachedData(Answer aref)
    {
        if (aref != null)
        {
            foreach (var emotion in emotionList)
            {
                emotion.answers_id = aref.id;
                emotion.emotions_id = EmoteSession.current_resource_id;
            }
            DatabaseManager.m_Emotion = emotionList;
        }
    }
}

using Affdex;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kalagaan.BlendShapesPresetTool;
using Emote.Utils;
using Emote.Database;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Avatar
{
    public class AvatarListener : ImageResultsListener
    {
        private float[] expressions;
        private int count = 0;
        private int m_CurrentEmotion = -1;

        public GameObject m_Head;
        public GameObject m_Full;
        public AvatarType m_Type = AvatarType.HEAD;
        public AvatarTextureCollection m_TextureCollection;

        public GameObject m_DetectedFrame;
        public GameObject m_UnDetectedFrame;

        public Text m_TextArea;
        public Boolean m_ShowDebug;

        public Toggle m_LiveAvatar;
        public Toggle m_RandomAvatar;
        public Toggle m_EnableQuiz;
        public Toggle m_RandomQuestions;
        public Slider m_MaxQuestions;

        public AvatarEmotions m_AvatarEmotions;

        public GameObject[] m_AvatarObjs;

        public BlendShapesPresetController m_Blendshapes
        {
            get
            {
                BlendShapesPresetController controller;

                switch (m_Type)
                {
                    case AvatarType.HEAD:
                        m_Full.SetActive(false);
                        m_Head.SetActive(true);
                        controller = m_Head.GetComponentInChildren<BlendShapesPresetController>();
                        break;
                    case AvatarType.FULL:
                        m_Head.SetActive(false);
                        m_Full.SetActive(true);
                        controller = m_Full.GetComponentInChildren<BlendShapesPresetController>();
                        break;
                    default:
                        m_Head.SetActive(false);
                        m_Full.SetActive(false);
                        controller = null;
                        break;
                }

                return controller;
            }
        }

        public void Start()
        {
            expressions = new float[System.Enum.GetValues(typeof(Expressions)).Length];

            m_Head.SetActive(false);
            m_Full.SetActive(false);

            if (m_TextureCollection == null)
            {
                m_TextureCollection = GetComponent<AvatarTextureCollection>();
            }

            AvatarType type = (AvatarType)DatabaseManager.m_AvatarType;
            m_Type = type;

            //  if avatar is generated load texture
            if (!string.IsNullOrEmpty(CurrentAvatar.code))
            {
                if (CurrentAvatar.state == AvatarState.COMPLETED)
                {
                    Texture2D texture = AvatarManager.GetTexture(CurrentAvatar.code);
                    SetTexture(texture);
                }
            }
            else
            {
                if (m_RandomAvatar)
                {
                    SetRandomTexture(m_RandomAvatar.isOn);
                }
            }
        }

        public void SetCurrentEmotion(Emotions emotion)
        {
            m_CurrentEmotion = (int)emotion;
        }

        public void SetType(AvatarType type)
        {
            m_Type = type;
        }

        public void SetTexture(Texture2D texture)
        {
            GameObject _object;
            SkinnedMeshRenderer _mesh;

            switch (m_Type)
            {
                case AvatarType.HEAD:
                    _object = m_Head.transform.GetChild(0).gameObject;
                    if (_object)
                    {
                        _mesh = _object.GetComponent<SkinnedMeshRenderer>();
                        _mesh.material.mainTexture = texture;
                    }
                    break;
                case AvatarType.FULL:
                    _object = m_Full.transform.GetChild(0).gameObject;
                    if (_object)
                    {
                        _mesh = _object.GetComponent<SkinnedMeshRenderer>();
                        _mesh.material.mainTexture = texture;
                    }
                    break;
            }
        }

        public void SetRandomEmotion(Emotions emotion)
        {
            m_CurrentEmotion = -1; // reset emotions
            switch(emotion)
            {
                case Emotions.Joy:
                    expressions = m_AvatarEmotions.Joy;
                    break;
                case Emotions.Fear:
                    expressions = m_AvatarEmotions.Fear;
                    break;
                case Emotions.Disgust:
                    expressions = m_AvatarEmotions.Disgust;
                    break;
                case Emotions.Sadness:
                    expressions = m_AvatarEmotions.Sadness;
                    break;
                case Emotions.Anger:
                    expressions = m_AvatarEmotions.Anger;
                    break;
                case Emotions.Surprise:
                    expressions = m_AvatarEmotions.Surprise;
                    break;
                case Emotions.Neutral:
                    expressions = m_AvatarEmotions.Neutral;
                    break;
            }
            MapRoutine(false, true);
        }

        public void SetRandomTexture(bool random)
        {
            if (random)
            {
                Texture2D texture = m_TextureCollection.GetRandomTexture(m_Type);
                SetTexture(texture);
            }
        }

        public override void onFaceFound(float timestamp, int faceId)
        {
            if (m_DetectedFrame && m_UnDetectedFrame)
            {
                m_UnDetectedFrame.SetActive(false);
                m_DetectedFrame.SetActive(true);
            }
            Debug.Log("Found the face");
        }

        public override void onFaceLost(float timestamp, int faceId)
        {
            if (m_DetectedFrame && m_UnDetectedFrame)
            {
                m_UnDetectedFrame.SetActive(true);
                m_DetectedFrame.SetActive(false);
            }
            Debug.Log("Lost the face");
        }

        public override void onImageResults(Dictionary<int, Face> faces)
        {
            if (faces.Count > 0)
            {
                DebugFeatureViewer dfv = GameObject.FindObjectOfType<DebugFeatureViewer>();

                foreach (KeyValuePair<int, Face> pair in faces)
                {
                    int faceID = pair.Key;
                    Face face = pair.Value;

                    // Debug only
                    if (dfv != null && m_ShowDebug)
                    {
                        dfv.ShowFace(face);

                        m_TextArea.text = face.ToString();
                        m_TextArea.CrossFadeColor(Color.white, 0.2f, true, false);
                    }

                    if (count <= 0)
                    {
                        MapRoutine(face);
                        count++;
                    }
                    else
                    {
                        // use live avatar
                        if (m_LiveAvatar && m_LiveAvatar.isOn)
                        {
                            ResetWeights();
                            MapRoutine(face);
                        }
                        else
                        {
                            if (m_CurrentEmotion > -1)
                            {
                                SetRandomEmotion((Emotions)m_CurrentEmotion);
                            }
                        }
                    }
                }
            }
            else
            {
                m_TextArea.CrossFadeColor(new Color(1, 0.7f, 0.7f), 0.2f, true, false);
            }
        }

        void MapRoutine(bool normalize = false, bool unify = false)
        {
            if (normalize)
            {
                normalizeExpressionVector();
            }
            mapExpressions(unify);
        }

        void MapRoutine(Face face)
        {
            foreach (Expressions expression in Enum.GetValues(typeof(Expressions)))
            {
                face.Expressions.TryGetValue(expression, out expressions[(int)expression]);
            }
            normalizeExpressionVector();
            mapExpressions();
        }

        void normalizeExpressionVector()
        {
            for (int key = 0; key < expressions.Length; ++key)
            {
                expressions[key] /= 100;
                expressions[key] = (float)(Math.Truncate((double)expressions[key] * 100.0) / 100.0);
            }
        }

        public void ResetWeights(bool unify = false)
        {
            if (m_Blendshapes)
            {
                foreach (Expressions expression in Enum.GetValues(typeof(Expressions)))
                {
                    m_Blendshapes.SetWeight(Enum.GetName(typeof(Expressions), expression), 0.0f);
                }
                if (unify)
                {
                    foreach (ExtraPresets expression in Enum.GetValues(typeof(ExtraPresets)))
                    {
                        m_Blendshapes.SetWeight(Enum.GetName(typeof(ExtraPresets), expression), expressions[(int)expression]);
                    }
                }
            }
        }

        void mapExpressions(bool unify = false)
        {
            if (m_Blendshapes)
            {
                foreach (Expressions expression in Enum.GetValues(typeof(Expressions)))
                {
                    m_Blendshapes.SetWeight(Enum.GetName(typeof(Expressions), expression), expressions[(int)expression]);
                }
                if (unify)
                {
                    foreach (ExtraPresets expression in Enum.GetValues(typeof(ExtraPresets)))
                    {
                        m_Blendshapes.SetWeight(Enum.GetName(typeof(ExtraPresets), expression), expressions[(int)expression]);
                    }
                }
            }
        }
    }
}
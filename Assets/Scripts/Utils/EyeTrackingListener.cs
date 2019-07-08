using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using Emote.Models;
using Emote.Database;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class EyeTrackingListener : MonoBehaviour
    {
        #region Detected/Not detected marks
        public GameObject m_Detected;
        public GameObject m_NotDetected;
        #endregion

        List<GazeRawData> m_GazeDataList = new List<GazeRawData>();
        List<GazeRawData> m_FinalData = new List<GazeRawData>();

        public bool m_StoreThePresence = false;

        private void Start()
        {
            //InvokeRepeating("StoreGazeData", 1.0f, 1.0f);
        }

        void Update()
        {
            VerifyPresence();
        }

        public void SaveGazeData()
        {
            DatabaseManager.m_GazeRawData = m_FinalData;
        }

        public void UpdateGazeDataRef()
        {
            Answer answer = DatabaseManager.m_LastAnswer;
            if (answer.id >= 0)
            {
                for (int i = 0; i < m_GazeDataList.Count; i++) m_GazeDataList[i].answer_id = answer.id;

                m_FinalData.AddRange(m_GazeDataList);
                m_GazeDataList.Clear();
            }
        }

        private void StoreGazeData()
        {
            GazePoint point = TobiiAPI.GetGazePoint();
            if (point.IsValid)
            {
                GazeRawData gazeData = new GazeRawData();
                gazeData.x = Mathf.RoundToInt(point.Screen.x);
                gazeData.y = Mathf.RoundToInt(point.Screen.y);
                gazeData.value = 1;

                //DatabaseManager.m_GazeRawData = gazeData;
                m_GazeDataList.Add(gazeData);
            }
        }

        private void VerifyPresence()
        {
            if (TobiiAPI.GetUserPresence() == UserPresence.Present)
            {
                if (m_Detected && m_NotDetected)
                {
                    m_Detected.SetActive(true);
                    m_NotDetected.SetActive(false);
                }

                if (m_StoreThePresence)
                    StoreGazeData();
            }
            else
            {
                if (m_Detected && m_NotDetected)
                {
                    m_Detected.SetActive(false);
                    m_NotDetected.SetActive(true);
                }
            }
        }
    }
}
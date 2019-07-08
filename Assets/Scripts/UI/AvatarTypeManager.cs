using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Emote.Utils;
using Emote.Avatar;
using Affdex;
using Emote.Database;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
public class AvatarTypeManager : MonoBehaviour {
    public Dropdown m_Dropdown;
    public AvatarListener m_Listener;
    public Detector m_Detector;

    #region Types
    List<string> m_DropOptions;
    #endregion

    void Start () {
        if (m_Dropdown == null)
        {
            m_Dropdown = GetComponent<Dropdown>();
            if (m_Dropdown == null)
            {
                return;
            }
        }

        PopulateTypes();

        AvatarType type = (AvatarType)DatabaseManager.m_AvatarType;
        m_Dropdown.value = (int)type;
        if (m_Detector && m_Listener)
        {
            m_Detector.StopDetector();
            m_Listener.SetType(type);
            m_Detector.StartDetector();
        }

        m_Dropdown.onValueChanged.AddListener(delegate
        {
            OnValueChanged(m_Dropdown);
        });
    }

    #region Device States
    private void SetType(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            var type = (AvatarType)Enum.Parse(typeof(AvatarType), name);
            if (m_Detector && m_Listener)
            {
                m_Detector.StopDetector();
                m_Listener.SetType(type);
                m_Detector.StartDetector();
            }
            DatabaseManager.m_AvatarType = (int)type;
        }
    }

    private void PopulateTypes()
    {
        m_Dropdown.ClearOptions();

        m_DropOptions = new List<string>();

        foreach (var type in Enum.GetNames(typeof(AvatarType)))
        {
            m_DropOptions.Add(type);
        }

        m_Dropdown.AddOptions(m_DropOptions);
    }
    #endregion

    #region Handlers
    private void OnValueChanged(Dropdown change)
    {
        SetType(m_DropOptions[change.value]);
    }
    #endregion
}

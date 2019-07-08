using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emote.Database;
using System.IO;

public class ExportData : MonoBehaviour {
    public static string m_Path = "ExportedData\\";
    public Button m_Button;

    // Use this for initialization
    void Start () {
#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
        string _path = Directory.GetParent(Application.dataPath).ToString();
        _path = Path.Combine(_path, m_Path);
        m_Path = _path;
#endif

        if (!m_Button)
        {
            m_Button = GetComponent<Button>();
        }

        m_Button.onClick.AddListener(delegate
        {
            DatabaseManager.ExportCSV(m_Path);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

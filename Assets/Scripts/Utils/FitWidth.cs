using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitWidth : MonoBehaviour {
    public LayoutElement m_LayoutElement;

	void Start () {
        m_LayoutElement = GetComponent<LayoutElement>();
        if (m_LayoutElement == null)
            return;
	}
	
	void Update () {
        if (m_LayoutElement)
        {
            m_LayoutElement.minWidth = Screen.width;
        }
	}
}

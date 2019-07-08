using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileBrowserExit : MonoBehaviour {

    public Canvas m_Canvas;
    public Button m_Button;
    public Animator m_Animator;

	void Start () {
        if (m_Button == null)
            m_Button = GetComponent<Button>();

        m_Button.onClick.AddListener(delegate
        {
            if (m_Animator)
            {
                m_Animator.Play("Fade-out");
            }
            if (m_Canvas)
            {
                foreach (Transform child in m_Canvas.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        });
	}
}

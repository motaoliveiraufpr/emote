using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Utils
{
    public class InfoManager : MonoBehaviour
    {
        public GameObject[] m_InfoRefs;
        public GameObject m_Preview;

        void Start()
        {
            if (m_InfoRefs == null)
            {
                m_InfoRefs = GameObject.FindGameObjectsWithTag("Info");
            }
            if (m_Preview != null)
            {
                m_Preview.SetActive(true);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) && m_InfoRefs != null)
            {
                foreach (GameObject _object in m_InfoRefs)
                {
                    SwapObjectVisibility(_object);
                }
            }
        }

        private void SwapObjectVisibility(GameObject _object)
        {
            _object.SetActive(!_object.activeSelf);
        }
    }
}
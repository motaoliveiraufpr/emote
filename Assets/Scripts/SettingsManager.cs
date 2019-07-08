using UnityEngine;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Utils
{
    public class SettingsManager : MonoBehaviour
    {
        public GameObject[] m_SettingsRefs;

        void Start()
        {
            if (m_SettingsRefs == null)
            {
                m_SettingsRefs = GameObject.FindGameObjectsWithTag("Settings");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && m_SettingsRefs != null)
            {
                foreach (GameObject _object in m_SettingsRefs)
                {
                    SwapObjectVisibility(_object);
                }
            }
        }

        private void SwapObjectVisibility(GameObject _object)
        {
            //_object.SetActive(!_object.activeSelf);
            Canvas canvas = _object.GetComponent<Canvas>();
            if (canvas)
            {
                canvas.enabled = !canvas.enabled;
            }
        }
    }
}
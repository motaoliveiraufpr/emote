using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Affdex;
using Emote.Database;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class CaptureDeviceManager : MonoBehaviour
    {

        public Dropdown m_Dropdown;
        public CameraInput m_CameraInput;
        public Detector m_Detector;
        public bool m_StartWebCamOnReady = true;

        #region Devices
        WebCamTexture m_WebCamTexture;
        WebCamDevice[] m_Devices;
        List<string> m_DropOptions;
        #endregion

        public WebCamTexture m_Device
        {
            get
            {
                return m_WebCamTexture;
            }
        }

        public void secureDeviceStop()
        {
            if (m_CameraInput && m_WebCamTexture)
            {
                m_Detector.StopDetector();
                m_WebCamTexture.Stop();
            }
        }

        void Start()
        {
            if (m_Dropdown == null)
            {
                m_Dropdown = GetComponent<Dropdown>();
                if (m_Dropdown == null)
                {
                    return;
                }
            }

            PopulateDevices();

            /* play first device */
            m_WebCamTexture = new WebCamTexture();
            var currentDevice = DatabaseManager.m_CaptureDevice;
            if (string.IsNullOrEmpty(currentDevice))
            {
                currentDevice = m_Devices[0].name;
            }
            if (!string.IsNullOrEmpty(currentDevice))
            {
                if (m_CameraInput)
                {
                    m_Detector.StopDetector();
                    m_CameraInput.SelectCamera(true, currentDevice);
                    m_Detector.StartDetector();

                    var index = m_DropOptions.FindIndex(delegate (string str)
                    {
                        return currentDevice == str;
                    });
                    if (index >= 0)
                    {
                        m_Dropdown.value = index;
                    }
                }
                else
                {
                    if (m_StartWebCamOnReady)
                    {
                        m_WebCamTexture.deviceName = currentDevice;
                        m_WebCamTexture.Play();
                    }
                }
            }

            m_Dropdown.onValueChanged.AddListener(delegate
            {
                OnValueChanged(m_Dropdown);
            });
        }

        #region Handlers
        private void OnValueChanged(Dropdown change)
        {
            PlayDevice(m_DropOptions[change.value]);
        }
        #endregion

        #region Device States
        private void PlayDevice(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (m_CameraInput)
                {
                    m_Detector.StopDetector();
                    m_CameraInput.SelectCamera(true, name);
                    m_Detector.StartDetector();
                }
                else
                {
                    m_WebCamTexture.Stop();
                    m_WebCamTexture.deviceName = name;
                    m_WebCamTexture.Play();
                }
                DatabaseManager.m_CaptureDevice = name;
            }
        }

        private void PopulateDevices()
        {
            m_Devices = WebCamTexture.devices;
            m_Dropdown.ClearOptions();

            m_DropOptions = new List<string>();
            foreach (var device in m_Devices)
            {
                m_DropOptions.Add(device.name);
            }

            m_Dropdown.AddOptions(m_DropOptions);
        }
        #endregion
    }
}
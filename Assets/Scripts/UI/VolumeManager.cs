using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
public class VolumeManager : MonoBehaviour {
    public VideoPlayer m_VideoPlayer;
    public AudioSource m_AudioPlayer;

    public Slider m_Slider;

	void Start () {
        if (m_VideoPlayer == null)
            m_VideoPlayer = GetComponent<VideoPlayer>();
        if (m_AudioPlayer == null)
            m_AudioPlayer = GetComponent<AudioSource>();

        if (m_Slider == null)
            m_Slider = GetComponent<Slider>();

        OnValueChanged(m_Slider);
        m_Slider.onValueChanged.AddListener(delegate
        {
            OnValueChanged(m_Slider);
        });
	}

    void OnValueChanged(Slider changed)
    {
        if (m_VideoPlayer)
        {
            int acount = m_VideoPlayer.audioTrackCount;
            for(int i=0; i<acount; i++)
            {
                m_VideoPlayer.SetDirectAudioVolume((ushort)i, changed.value);
            }
        }
        if (m_AudioPlayer) m_AudioPlayer.volume = changed.value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Emote.Utils
{
    public class AutoplayAnimator : MonoBehaviour
    {
        public Animator m_Animator;

        void Start()
        {
            if (m_Animator == null)
                m_Animator = GetComponent<Animator>();

            m_Animator.Play("Fade-in");
        }
    }
}
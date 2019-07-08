using UnityEngine;
using System.Collections;


namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {

        [ExecuteInEditMode]
        public class BlendShapesPresetAnimator : MonoBehaviour
        {
            public BlendShapesPresetController m_bptc;
            public bool[] m_enableAnimation = new bool[20];
            public float m_weight0 = 0;
            public float m_weight1 = 0;
            public float m_weight2 = 0;
            public float m_weight3 = 0;
            public float m_weight4 = 0;
            public float m_weight5 = 0;
            public float m_weight6 = 0;
            public float m_weight7 = 0;
            public float m_weight8 = 0;
            public float m_weight9 = 0;
            public float m_weight10 = 0;
            public float m_weight11 = 0;
            public float m_weight12 = 0;
            public float m_weight13 = 0;
            public float m_weight14 = 0;
            public float m_weight15 = 0;
            public float m_weight16 = 0;
            public float m_weight17 = 0;
            public float m_weight18 = 0;
            public float m_weight19 = 0;
            public float m_weight20 = 0;

            void Awake()
            {
                if (m_bptc == null)
                {
                    m_bptc = GetComponent<BlendShapesPresetController>();
                    for (int i = 0; i < m_enableAnimation.Length; ++i)
                        m_enableAnimation[i] = true;
                }
            }


            public void SetWeight(int id, float weight)
            {
                if (m_bptc.m_presetState.Count > id && m_enableAnimation[id])
                    m_bptc.SetWeight(id, weight);
            }

            void Update()
            {
                if (m_bptc == null) return;
                int i = 0;
                SetWeight(i++, m_weight0);
                SetWeight(i++, m_weight1);
                SetWeight(i++, m_weight2);
                SetWeight(i++, m_weight3);
                SetWeight(i++, m_weight4);
                SetWeight(i++, m_weight5);
                SetWeight(i++, m_weight6);
                SetWeight(i++, m_weight7);
                SetWeight(i++, m_weight8);
                SetWeight(i++, m_weight9);
                SetWeight(i++, m_weight10);
                SetWeight(i++, m_weight11);
                SetWeight(i++, m_weight12);
                SetWeight(i++, m_weight13);
                SetWeight(i++, m_weight14);
                SetWeight(i++, m_weight15);
                SetWeight(i++, m_weight16);
                SetWeight(i++, m_weight17);
                SetWeight(i++, m_weight18);
                SetWeight(i++, m_weight19);
                SetWeight(i++, m_weight20);
            }
        }
    }
}
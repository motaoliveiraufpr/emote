using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {
        public class HeadController : MonoBehaviour
        {
            public Transform m_headBone;
            public Transform m_CharacterOrientation;
            public Transform m_LookAtTarget;

            public Kalagaan.BlendShapesPresetTool.BlendShapesPresetControllerBase m_bspm;

            [Range(0f, 1f)]
            public float m_lookAtWeight = 0f;

            public float m_reactionSpeed = 2f;

            public int m_parentMax = 0;
            public AnimationCurve m_curve = new AnimationCurve();

            public Vector2 m_blinkTimerMinMax = new Vector2(2f, 5f);
            float m_blinkTimer;
            float m_blinkStartTime;
            bool m_blinkTrigger = false;

            public bool m_eyeTracking = true;

            Quaternion[] m_original;
            Quaternion[] m_lastRotation;
            Transform[] m_IKChain;
            Vector3 m_targetPosition;
            Vector3 m_eyesTargetPosition;
            Vector3 m_noTarget;

            // Use this for initialization
            void Start()
            {
                //initialize data for the IK
                m_original = new Quaternion[m_parentMax + 1];
                m_lastRotation = new Quaternion[m_parentMax + 1];
                m_IKChain = new Transform[m_parentMax + 1];

                Transform t = m_headBone;
                for (int i = m_original.Length - 1; i >= 0; --i)
                {
                    m_original[i] = t.localRotation;
                    m_lastRotation[i] = t.rotation;
                    m_IKChain[i] = t;
                    t = t.parent;
                }

                m_noTarget = m_headBone.position + m_headBone.forward * 10f;
                m_targetPosition = m_LookAtTarget.position;                                
            }

            public void SetEyeTracking( bool On )
            {
                m_eyeTracking = On;
            }

            void LateUpdate()
            {
                if (Vector3.Dot((m_LookAtTarget.position - m_headBone.position).normalized, m_CharacterOrientation.forward) > -.5
                    && Vector3.Dot((m_LookAtTarget.position - m_headBone.position).normalized, Vector3.up) < .9
                    && Vector3.Dot((m_LookAtTarget.position - m_headBone.position).normalized, Vector3.up) > -.9 )
                {
                    m_targetPosition = Vector3.Lerp(m_targetPosition, m_LookAtTarget.position, Time.deltaTime * m_reactionSpeed);
                }
                else
                {
                    m_targetPosition = Vector3.Lerp(m_targetPosition, m_noTarget, Time.deltaTime * .1f);
                }

                if (m_headBone != null && m_LookAtTarget != null)
                {
                    Quaternion q;

                    //if( Vector3.Dot((m_targetPosition-m_headBone.position).normalized, m_headBone.forward) < .995f )
                    for (int i = 0; i < m_IKChain.Length; ++i)
                    {
                        float f = (float)i / (float)m_IKChain.Length;
                        m_IKChain[i].localRotation = m_original[i];
                        q = m_IKChain[i].rotation;
                        m_IKChain[i].LookAt(m_targetPosition);
                        m_IKChain[i].rotation = Quaternion.Lerp(q, m_IKChain[i].rotation, m_curve.Evaluate(f) * m_lookAtWeight);
                        m_IKChain[i].rotation = Quaternion.Lerp(m_lastRotation[i], m_IKChain[i].rotation, Time.deltaTime * m_reactionSpeed);
                        m_lastRotation[i] = m_IKChain[i].rotation;
                    }
                    
                }
                EyeControl();
            }

            void EyeControl()
            {

                m_eyesTargetPosition = Vector3.Lerp(m_eyesTargetPosition, m_targetPosition, Time.deltaTime * m_reactionSpeed * 10f);
                float dotDir = Vector3.Dot(m_headBone.forward, (m_eyesTargetPosition - m_headBone.position).normalized);

                if (dotDir > 0)
                {
                    float dotRL = Vector3.Dot(m_headBone.right, (m_eyesTargetPosition - m_headBone.position).normalized);
                    if (!m_eyeTracking) dotRL = 0f;

                    if (dotRL > 0)
                    {
                        m_bspm.SetWeight("Eyes_R", Mathf.Clamp01(dotRL * 2f));
                        m_bspm.SetWeight("Eyes_L", 0f);
                    }
                    else
                    {
                        m_bspm.SetWeight("Eyes_R", 0f);
                        m_bspm.SetWeight("Eyes_L", Mathf.Clamp01(-dotRL * 2f));
                    }

                    float dotUD = Vector3.Dot(m_headBone.up, (m_eyesTargetPosition - m_headBone.position).normalized);
                    if (!m_eyeTracking) dotUD = 0f;

                    if (dotUD > 0)
                    {
                        m_bspm.SetWeight("Eyes_U", Mathf.Clamp01(dotUD * 2f));
                        m_bspm.SetWeight("Eyes_D", 0f);
                    }
                    else
                    {
                        m_bspm.SetWeight("Eyes_U", 0f);
                        m_bspm.SetWeight("Eyes_D", Mathf.Clamp01 (- dotUD * 2f));
                    }
                }
                else
                {
                    m_bspm.GetPresetState("Eyes_L").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_L").weight, Time.deltaTime * 10f);
                    m_bspm.GetPresetState("Eyes_R").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_R").weight, Time.deltaTime * 10f);
                    m_bspm.GetPresetState("Eyes_D").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_D").weight, Time.deltaTime * 10f);
                    m_bspm.GetPresetState("Eyes_U").weight = Mathf.Lerp(0f, m_bspm.GetPresetState("Eyes_U").weight, Time.deltaTime * 10f);
                }

                if (m_blinkTrigger)
                {
                    float f = Mathf.Sin((Time.time - m_blinkStartTime) * 10f);
                    m_bspm.GetPresetState("Blink").weight = Mathf.Clamp01(f);

                    if (f < 0)
                    {
                        m_blinkTrigger = false;
                        m_blinkTimer = m_blinkTimerMinMax.x + Random.value * (m_blinkTimerMinMax.y - m_blinkTimerMinMax.x) + Time.time;
                    }
                }

                if (!m_blinkTrigger && Time.time > m_blinkTimer)
                {
                    m_blinkTrigger = true;
                    m_blinkStartTime = Time.time;
                }
            }
        }
    }
}
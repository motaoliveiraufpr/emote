using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {

        public class BlendShapesPresetControllerBase : MonoBehaviour
        {
            //internal states for blending
            [System.Serializable]
            public class BlendShapeState
            {
                public string name = "";
                public float weight = 0;
                public float modifierCount = 0;
                public float totalWeight = 0;
                public bool refresh = false;
            }

            //instance weights
            [System.Serializable]
            public class PresetState
            {
                public string name;
                public float weight = 0;                
            }

            public static string version = "1.0.2";

            public BlendShapesPresetTemplateBase m_blendShapePreset;
            public SkinnedMeshRenderer m_smr;
            public List<SkinnedMeshRenderer> m_skinnedMeshRenderers = new List<SkinnedMeshRenderer>();

            public BlendShapeState[] m_shapeState;
            public List<PresetState> m_presetState = new List<PresetState>();

            //public float[] m_weights;

            public bool m_onLateUpdate = true;
            public bool m_showEditionWarning = true;
            public bool m_showAdvancedParameters = false;

            public List<BlendShapesPresetControllerBase> m_childControllers = new List<BlendShapesPresetControllerBase>();

            public void Init()
            {
                m_smr = GetComponent<SkinnedMeshRenderer>();
                if (m_smr != null)
                {
                    m_shapeState = new BlendShapeState[m_smr.sharedMesh.blendShapeCount];

                    for (int i = 0; i < m_shapeState.Length; ++i)
                        m_shapeState[i] = new BlendShapeState();
                }
                else
                {
                    Debug.LogError("SkinnedMesh not found");
                }
                /*
                if (m_weights.Length != m_blendShapePreset.m_presets.Count)
                    m_weights = new float[m_blendShapePreset.m_presets.Count];
                   */


                if (m_blendShapePreset != null)
                {
                    while (m_presetState.Count < m_blendShapePreset.m_presets.Count)
                    {
                        m_presetState.Add(new PresetState());
                    }
                }

                /*
                //Check preset order
                for(int i=0; i< m_presetState.Count; ++i)
                {
                    if(m_blendShapePreset.m_presets[i].name != m_presetState[i].name )
                    {

                    }
                }
                */

            }



            void Start()
            {
                if (m_blendShapePreset == null)
                    return;

                Init();
            }


            public void Update()
            {
                if(!m_onLateUpdate)
                    UpdateBlendShapes();
            }

            public void LateUpdate()
            {
                if(m_onLateUpdate)
                    UpdateBlendShapes();
            }



            public void UpdateBlendShapes()
            {
                if (m_blendShapePreset == null)
                    return;

                //------------------------
                //blend                
                for (int i = 0; i < m_blendShapePreset.m_presets.Count; ++i)
                {
                    BlendShapesPresetTemplateBase.PresetData sp = m_blendShapePreset.m_presets[i];
                    if (sp.blendMode == BlendShapesPresetTemplateBase.eBlendMode.BLEND)
                    {
                        for (int j = 0; j < sp.m_blendShapes.Count; ++j)
                        {
                            int id = m_smr.sharedMesh.GetBlendShapeIndex(sp.m_blendShapes[j].name);

                            if (m_shapeState[id].modifierCount == 0)
                            {
                                m_shapeState[id].weight = 0;
                                m_shapeState[id].totalWeight = 0;
                            }

                            if (m_shapeState[id].weight == 0)
                                m_shapeState[id].weight = sp.m_blendShapes[j].weight * m_presetState[i].weight;
                            else
                            {
                                //m_shapeState[id].weight = m_shapeState[id].weight * .5f + sp.m_shapes[j].weight * m_presetState[i].weight * .5f ;

                                float f = 1f;
                                if(sp.m_blendShapes[j].weight * m_presetState[i].weight < m_shapeState[id].weight)                                
                                    f = sp.m_blendShapes[j].weight * m_presetState[i].weight / m_shapeState[id].weight;
                                else
                                    f = 1- sp.m_blendShapes[j].weight * m_shapeState[id].weight / m_presetState[i].weight;

                                m_shapeState[id].weight = m_shapeState[id].weight * (1f-f) + sp.m_blendShapes[j].weight * m_presetState[i].weight * f;

                                m_shapeState[id].weight = Mathf.Clamp01(m_shapeState[id].weight);
                            }

                            m_shapeState[id].refresh = true;
                            m_shapeState[id].totalWeight += m_presetState[i].weight;
                            //m_shapeState[id].modifierCount += m_presetState[i].weight;                            
                            if(sp.m_blendShapes[j].weight * m_presetState[i].weight > 0)
                                m_shapeState[id].modifierCount++;
                        }
                    }                    
                }

                for (int i = 0; i < m_shapeState.Length; ++i)
                {
                    if (m_shapeState[i].refresh)
                    {
                        //if (m_shapeState[i].modifierCount > 0)
                        //    m_shapeState[i].weight /= (float)m_shapeState[i].modifierCount;
                            //m_shapeState[i].weight *= (float)m_shapeState[i].totalWeight;
                    }
                }
                    /*
                    for (int i = 0; i < m_blendShapePreset.m_presets.Count; ++i)
                    {
                        BlendShapePreset.ShapePreset sp = m_blendShapePreset.m_presets[i];
                        if (sp.blendMode == BlendShapePreset.eBlendMode.BLEND)
                        {
                            for (int j = 0; j < sp.m_shapes.Count; ++j)
                            {
                                int id = m_smr.sharedMesh.GetBlendShapeIndex(sp.m_shapes[j].name);
                                if(m_shapeState[id].modifierCount>0)
                                    m_shapeState[id].weight /= (float)m_shapeState[id].modifierCount;
                            }
                        }
                    }*/

                    //Min
                    for (int i = 0; i < m_blendShapePreset.m_presets.Count; ++i)
                {
                    BlendShapesPresetTemplateBase.PresetData sp = m_blendShapePreset.m_presets[i];
                    if (sp.blendMode == BlendShapesPresetTemplateBase.eBlendMode.MIN)
                    {
                        for (int j = 0; j < sp.m_blendShapes.Count; ++j)
                        {
                            int id = m_smr.sharedMesh.GetBlendShapeIndex(sp.m_blendShapes[j].name);
                            m_shapeState[id].weight = Mathf.Min(m_shapeState[id].weight, sp.m_blendShapes[j].weight * m_presetState[i].weight);
                            m_shapeState[id].refresh = true;
                        }
                    }
                }


                //Max
                for (int i = 0; i < m_blendShapePreset.m_presets.Count; ++i)
                {
                    BlendShapesPresetTemplateBase.PresetData sp = m_blendShapePreset.m_presets[i];
                    if (sp.blendMode == BlendShapesPresetTemplateBase.eBlendMode.MAX)
                    {
                        for (int j = 0; j < sp.m_blendShapes.Count; ++j)
                        {
                            int id = m_smr.sharedMesh.GetBlendShapeIndex(sp.m_blendShapes[j].name);
                            m_shapeState[id].weight = Mathf.Max(m_shapeState[id].weight, sp.m_blendShapes[j].weight * m_presetState[i].weight);
                            m_shapeState[id].refresh = true;
                        }
                    }
                }


                //Override
                for (int i = 0; i < m_blendShapePreset.m_presets.Count; ++i)
                {
                    BlendShapesPresetTemplateBase.PresetData sp = m_blendShapePreset.m_presets[i];
                    if (sp.blendMode == BlendShapesPresetTemplateBase.eBlendMode.OVERRIDE)
                    {
                        for (int j = 0; j < sp.m_blendShapes.Count; ++j)
                        {
                            int id = m_smr.sharedMesh.GetBlendShapeIndex(sp.m_blendShapes[j].name);
                            m_shapeState[id].weight = sp.m_blendShapes[j].weight * m_presetState[i].weight;
                            m_shapeState[id].refresh = true;
                        }
                    }
                }


                for (int i = 0; i < m_shapeState.Length; ++i)
                {

                    if (m_shapeState[i].refresh)
                    {
                        //if(m_shapeState[i].modifierCount < 1f)
                        m_smr.SetBlendShapeWeight(i, m_shapeState[i].weight * 100f);
                        //else
                        //    m_smr.SetBlendShapeWeight(i, m_shapeState[i].weight / m_shapeState[i].modifierCount * 100f);

                        for( int smr = 0; smr< m_skinnedMeshRenderers.Count; ++smr )
                        {
                            if(m_skinnedMeshRenderers[smr].sharedMesh.blendShapeCount > i)
                                m_skinnedMeshRenderers[smr].SetBlendShapeWeight(i, m_shapeState[i].weight * 100f);
                        }


                    }
                    m_shapeState[i].refresh = false;
                    m_shapeState[i].weight = 0;
                    m_shapeState[i].totalWeight = 0;
                    m_shapeState[i].modifierCount = 0;
                }                
            }


            public void ClearBlendShapesWeights()
            {
                if(m_smr == null )
                    m_smr = GetComponent<SkinnedMeshRenderer>();

                for (int i = 0; i < m_smr.sharedMesh.blendShapeCount; ++i)
                    m_smr.SetBlendShapeWeight(i, 0f);
            }



            public int GetPresetStateId( string name )
            {
                for (int i = 0; i < m_blendShapePreset.m_presets.Count; ++i)
                    if (m_blendShapePreset.m_presets[i].name == name)
                        return i;

                return -1;
            }

            public PresetState GetPresetState(int id)
            {
                if (m_presetState.Count > id && id >= 0)
                    return m_presetState[id];
                return null;
            }


            public PresetState GetPresetState(string name)
            {
                return GetPresetState( GetPresetStateId(name) );
            }


            public void SetWeight(string name, float weight )
            {
                PresetState ps = GetPresetState(GetPresetStateId(name));
                if(ps!=null)
                    ps.weight = weight;
                else if(Application.isEditor)
                {
                    Debug.LogWarning( "Blendshape preset name not found : " + name);
                }
            }


            public void SetWeight(int id, float weight)
            {
                PresetState ps = GetPresetState(id);
                if (ps != null)
                    ps.weight = weight;
                else if (Application.isEditor)
                {
                    Debug.LogWarning("Blendshape preset id out of range : " + id );
                }
            }


            public float GetWeight(string name)
            {
                PresetState ps = GetPresetState(GetPresetStateId(name));
                if (ps != null)
                    return ps.weight;
                else if (Application.isEditor)
                {
                    Debug.LogWarning("Blendshape preset name not found : " + name);
                }
                return -1;
            }


            public float GetWeight(int id)
            {
                PresetState ps = GetPresetState(id);
                if (ps != null)
                    return ps.weight;
                else if (Application.isEditor)
                {
                    Debug.LogWarning("Blendshape preset name not found : " + name);
                }
                return -1;
            }

        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {
        public class Demo : MonoBehaviour
        {
            public BlendShapesPresetController m_female;
            public BlendShapesPresetController m_male;
            public BlendShapesPresetTemplate[] m_presets;
            public Slider m_ikSlider;
            public Text m_textBlendShapesWeights1;
            public Text m_textBlendShapesWeights2;
            public Image m_presetTemplate;


            HeadController m_femaleHC;
            HeadController m_maleHC;

            SkinnedMeshRenderer m_smrFemale;
            SkinnedMeshRenderer m_smrMale;

            bool m_femaleActivated = true;
            List<GameObject> m_presetListItems = new List<GameObject>();
            Dictionary<Slider, BlendShapesPresetControllerBase.PresetState> m_maleSliderToBS = new Dictionary<Slider, BlendShapesPresetControllerBase.PresetState>();
            Dictionary<Slider, BlendShapesPresetControllerBase.PresetState> m_femaleSliderToBS = new Dictionary<Slider, BlendShapesPresetControllerBase.PresetState>();

            void Start()
            {
                m_femaleHC = m_female.transform.parent.GetComponent<HeadController>();
                m_maleHC = m_male.transform.parent.GetComponent<HeadController>();

                m_smrFemale = m_female.GetComponent<SkinnedMeshRenderer>();
                m_smrMale = m_male.GetComponent<SkinnedMeshRenderer>();

                m_presetTemplate.gameObject.SetActive(false);

                InitPresetSliders();
            }

            void InitPresetSliders()
            {
                BlendShapesPresetController bpc = m_femaleActivated ? m_female : m_male;

                m_female.ClearBlendShapesWeights();
                m_male.ClearBlendShapesWeights();

                string[] automaticWeights = { "Eyes_U", "Eyes_D", "Eyes_R", "Eyes_L", "Blink" };

                for (int i = 0; i < m_presetListItems.Count; ++i)
                {
                    Destroy(m_presetListItems[i]);
                }
                m_presetListItems.Clear();

                m_maleSliderToBS.Clear();
                m_femaleSliderToBS.Clear();

                for ( int i=0; i< bpc.m_blendShapePreset.m_presets.Count; ++i )
                {
                    if (!automaticWeights.Contains(bpc.m_blendShapePreset.m_presets[i].name))
                    {
                        GameObject go = Instantiate(m_presetTemplate.gameObject, m_presetTemplate.transform.parent) as GameObject;
                        go.GetComponentInChildren<Text>().text = bpc.m_blendShapePreset.m_presets[i].name;
                        
                        Slider sld = go.GetComponentInChildren<Slider>();
                        m_maleSliderToBS.Add(sld, m_male.m_presetState[i]);
                        m_femaleSliderToBS.Add(sld, m_female.m_presetState[i]);
                        sld.value = bpc.m_presetState[i].weight;
                        //bpc.m_presetState[i].weight = 0f;
                        sld.onValueChanged.AddListener((v) =>
                        {
                            m_maleSliderToBS[sld].weight = v;
                            m_femaleSliderToBS[sld].weight = v;
                        }
                        );                        
                        go.SetActive(true);
                        m_presetListItems.Add(go);
                    }
                }
            }
                


            void Update()
            {
                m_femaleHC.m_lookAtWeight = m_ikSlider.value;
                m_maleHC.m_lookAtWeight = m_ikSlider.value;

                SkinnedMeshRenderer smr = m_femaleActivated ? m_smrFemale : m_smrMale;

                m_textBlendShapesWeights1.text = "<b>Blend shapes</b>\n";

                for( int i=0; i< smr.sharedMesh.blendShapeCount; ++i )
                {
                    string w = smr.sharedMesh.GetBlendShapeName(i).Replace("Expressions_", "")+"\n";
                    //w += "\t" + Mathf.Floor(smr.GetBlendShapeWeight(i)) + "%\n";

                    m_textBlendShapesWeights1.text += w;
                }

                m_textBlendShapesWeights2.text = "";

                for (int i = 0; i < smr.sharedMesh.blendShapeCount; ++i)
                {
                    string w = "\t" + Mathf.Floor(smr.GetBlendShapeWeight(i)) + "%\n";
                    m_textBlendShapesWeights2.text += w;
                }

            }


            public void SwitchFace( Dropdown dpd )
            {
                m_female.transform.parent.gameObject.SetActive(dpd.value==0);
                m_male.transform.parent.gameObject.SetActive(dpd.value == 1);
                m_femaleActivated = dpd.value == 0;
                InitPresetSliders();
            }


            public void SwitchPresets(Dropdown dpd)
            {
                m_female.m_blendShapePreset = m_presets[dpd.value];
                m_male.m_blendShapePreset = m_presets[dpd.value];
                InitPresetSliders();
            }
        }
    }
}
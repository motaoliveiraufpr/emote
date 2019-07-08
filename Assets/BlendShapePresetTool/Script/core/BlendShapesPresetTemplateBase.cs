using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {
        public class BlendShapesPresetTemplateBase : ScriptableObject
        {
            [System.Serializable]
            public enum eBlendMode
            {
                BLEND =0,
                MAX,
                MIN,
                OVERRIDE
            }

            [System.Serializable]
            public class BlendShapeData
            {
                public string name;
                public float weight;
            }

            [System.Serializable]
            public class PresetData
            {
                public string name = "New preset";
                [HideInInspector]                
                public List<BlendShapeData> m_blendShapes = new List<BlendShapeData>();
                public eBlendMode blendMode;
            }

            public List<PresetData> m_presets = new List<PresetData>();
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {

        [CustomEditor(typeof(BlendShapesPresetAnimator))]
        public class BlendShapesPresetAnimatorEditor : Editor
        {


            public override void OnInspectorGUI()
            {
                BlendShapesPresetAnimator anm = target as BlendShapesPresetAnimator;

                if (anm == null) return;
                if (anm.m_bptc == null)
                {
                    anm.m_bptc = EditorGUILayout.ObjectField("Blenshape preset controller", anm.m_bptc, typeof(BlendShapesPresetController), true) as BlendShapesPresetController;
                    return;
                }

                SerializedObject so = new UnityEditor.SerializedObject(anm);

                for (int i = 0; i < anm.m_bptc.m_presetState.Count; ++i)
                {
                    UnityEditor.SerializedProperty weight = so.FindProperty("m_weight" + i);
                    if (weight != null)
                    {
                        if (i < anm.m_bptc.m_blendShapePreset.m_presets.Count)
                        {
                            EditorGUILayout.BeginHorizontal();
                            anm.m_enableAnimation[i] = EditorGUILayout.ToggleLeft(anm.m_bptc.m_blendShapePreset.m_presets[i].name, anm.m_enableAnimation[i], GUILayout.MaxWidth(200));
                            weight.floatValue = EditorGUILayout.Slider(weight.floatValue, 0f, 1f);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                so.ApplyModifiedProperties();
            }
        }
    }
}
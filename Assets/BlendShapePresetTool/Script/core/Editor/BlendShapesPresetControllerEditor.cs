using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using System.IO;

namespace Kalagaan
{
    namespace BlendShapesPresetTool
    {
        [CustomEditor(typeof(BlendShapesPresetControllerBase), true)]
        public class BlendShapePresetControllerEditor : Editor
        {
            

            int m_editID = -1;
            bool m_reorder = false;

            public bool CheckGameObject(BlendShapesPresetControllerBase pm)
            {
                if (m_smr == null)
                {
                    EditorGUILayout.HelpBox("Require a skinnedMeshRenderer", MessageType.Error);
                    return false;
                }

                if (m_smr.sharedMesh == null)
                {
                    EditorGUILayout.HelpBox("SkinnedMeshRenderer must have a mesh", MessageType.Error);
                    return false;
                }

                if (m_smr.sharedMesh.blendShapeCount == 0)
                {
                    EditorGUILayout.HelpBox("SkinnedMeshRenderer has no blendshape", MessageType.Warning);
                    return false;
                }

                if (pm == null) return false;
                if (pm.m_blendShapePreset == null) return false;

                return true;
            }

            SkinnedMeshRenderer m_smr;
            //float[] m_lastState;

            Texture2D m_iconEdit;
            Texture2D m_logo;
            public void OnEnable()
            {
                m_iconEdit = Resources.Load("BSPT_Edit", typeof(Texture2D)) as Texture2D;
                m_logo = Resources.Load("BSPT_Logo", typeof(Texture2D)) as Texture2D;
            }


            
            private ReorderableList list;
            public void InitReorderableList()
            {
                BlendShapesPresetControllerBase pm = target as BlendShapesPresetControllerBase;
                SerializedObject so = new UnityEditor.SerializedObject(pm.m_blendShapePreset);
                list = new ReorderableList(so,
                so.FindProperty("m_presets"),
                true, false, false, false);


                list.drawElementCallback =
                    (Rect rect, int index, bool isActive, bool isFocused) => {
                        var element = list.serializedProperty.GetArrayElementAtIndex(index);
                        rect.y += 2;
                                                
                        EditorGUI.LabelField(
                           new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                           element.FindPropertyRelative("name").stringValue);

                        pm.m_presetState[index].weight = EditorGUI.Slider(
                           new Rect(rect.x + 60, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight),
                           pm.m_presetState[index].weight, 0f,1f);

                    };
            }
            

            public override void OnInspectorGUI()
            {
                BlendShapesPresetControllerBase pc = target as BlendShapesPresetControllerBase;

                Color guiCol = GUI.color;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent(m_logo));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(-20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent(BlendShapesPresetControllerBase.version), EditorStyles.centeredGreyMiniLabel);
                GUILayout.EndHorizontal();

                if (m_smr == null)
                    m_smr = pc.GetComponent<SkinnedMeshRenderer>();
                pc.Init();


                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Presets template", EditorStyles.boldLabel);
                pc.m_blendShapePreset = EditorGUILayout.ObjectField("Template", pc.m_blendShapePreset, typeof(BlendShapesPresetTemplateBase), true) as BlendShapesPresetTemplateBase;

                if( !pc.m_showEditionWarning)
                    pc.m_showEditionWarning = EditorGUILayout.Toggle("Show warning", pc.m_showEditionWarning);
                GUILayout.EndVertical();

                

                if (!CheckGameObject(pc)) return;

                //GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Presets", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUI.color = m_reorder ? Color.yellow : guiCol;
                if (pc.m_blendShapePreset.m_presets.Count > 1)
                {
                    if (GUILayout.Button("Reorder"))
                    {
                        int aswr = 0;

                        if (!m_reorder && pc.m_showEditionWarning)
                            aswr = EditorUtility.DisplayDialogComplex("Edit preset", "The preset will be modified.\nAll the controller that share it will be impacted.", "Ok", "Ok, don't display warnings", "Cancel");
                        if (aswr == 1) pc.m_showEditionWarning = false;

                        if (aswr != 2)
                            m_reorder = !m_reorder;

                        if (m_reorder)
                        {
                            InitReorderableList();
                        }
                        else
                        {
                            EditorUtility.SetDirty(pc.m_blendShapePreset);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
                GUILayout.EndHorizontal();


                GUI.color = guiCol;
                if (m_reorder)
                {
                    if (list == null)
                        InitReorderableList();

                    serializedObject.Update();
                    list.DoLayoutList();
                    serializedObject.ApplyModifiedProperties();
                    pc.UpdateBlendShapes();
                }

                else
                {
                    for (int i = 0; i < pc.m_blendShapePreset.m_presets.Count; ++i)
                    {
                        GUI.color = (m_editID != i && m_editID != -1) ? Color.grey : guiCol;

                        if (pc.m_blendShapePreset.m_presets.Count < i)
                            break;

                        if (m_editID == i)
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();


                        if (m_editID == i)
                        {

                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            pc.m_blendShapePreset.m_presets[i].name = EditorGUILayout.TextField("Name", pc.m_blendShapePreset.m_presets[i].name);
                            pc.m_blendShapePreset.m_presets[i].blendMode = (BlendShapesPresetTemplateBase.eBlendMode)EditorGUILayout.EnumPopup("Blend mode", pc.m_blendShapePreset.m_presets[i].blendMode);
                            pc.m_presetState[i].weight = EditorGUILayout.Slider("weight", pc.m_presetState[i].weight, 0f, 1f);
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            pc.m_presetState[i].weight = EditorGUILayout.Slider(pc.m_blendShapePreset.m_presets[i].name, pc.m_presetState[i].weight, 0f, 1f);
                        }


                        //Blend mode
                        string blendMode = "B";
                        switch( pc.m_blendShapePreset.m_presets[i].blendMode )
                        {
                            case BlendShapesPresetTemplateBase.eBlendMode.MAX:
                                blendMode = "+";
                                break;
                            case BlendShapesPresetTemplateBase.eBlendMode.MIN:
                                blendMode = "-";
                                break;
                            case BlendShapesPresetTemplateBase.eBlendMode.OVERRIDE:
                                blendMode = "O";
                                break;
                        }
                        GUILayout.Label(blendMode, EditorStyles.centeredGreyMiniLabel, GUILayout.Width(14));

                        //GUI.color = m_editID == i ? Color.yellow : EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label.normal.textColor;
                        

                        GUI.enabled = !Application.isPlaying;

                        if (m_editID != i && m_editID != -1)
                            GUI.enabled = false;

                        {
                            GUILayout.BeginVertical();
                            GUI.color = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label.normal.textColor;
                            if (GUILayout.Button(new GUIContent(m_iconEdit), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                bool edit = true;
                                if (m_editID != i && pc.m_showEditionWarning)
                                {
                                    int aswr = EditorUtility.DisplayDialogComplex("Edit preset", "The preset will be modified.\nAll the controller that share it will be impacted.", "Ok", "Ok, don't display warnings", "Cancel");
                                    if (aswr == 1) pc.m_showEditionWarning = false;
                                    if (aswr == 2) edit = false;
                                    //Debug.Log("answer " + aswr );
                                }

                                if (edit)
                                {
                                    if (m_editID == i)
                                    {
                                        EditorUtility.SetDirty(pc.m_blendShapePreset);
                                        AssetDatabase.SaveAssets();
                                    }

                                    m_editID = m_editID == i ? -1 : i;
                                    pc.m_presetState[i].weight = 1;
                                    pc.UpdateBlendShapes();
                                }
                            }


                            if (m_editID == i)
                            {
                                GUI.color = Color.white;
                                GUILayout.Space(10);
                                if (GUILayout.Button("X", EditorStyles.centeredGreyMiniLabel))
                                {
                                    bool delete = EditorUtility.DisplayDialog("Delete preset",
                                        "the preset '" + pc.m_blendShapePreset.m_presets[i].name + "' will be deleted.\n"
                                        + "All the controllers that share the preset reference will be impacted.", "Delete", "Cancel");
                                    if (delete)
                                    {
                                        pc.m_blendShapePreset.m_presets.RemoveAt(i--);
                                        m_editID = -1;
                                        break;
                                    }
                                }
                            }

                            GUILayout.EndVertical();
                        }

                        GUI.color = guiCol;
                        GUI.enabled = true;

                        GUILayout.EndHorizontal();

                        if (m_editID == i)
                        {
                            for (int sId = 0; sId < m_smr.sharedMesh.blendShapeCount; ++sId)
                            {
                                string BlendShapeName = m_smr.sharedMesh.GetBlendShapeName(sId);
                                BlendShapesPresetTemplateBase.BlendShapeData sd = pc.m_blendShapePreset.m_presets[i].m_blendShapes.Find(s => s.name == BlendShapeName);
                                float v = m_smr.GetBlendShapeWeight(sId) / 100f;
                                if (sd != null)
                                {
                                    v = sd.weight;
                                    GUI.color = sd.weight != 0 ? Color.yellow : Color.magenta;
                                }
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(15);
                                float w = EditorGUILayout.Slider(BlendShapeName, v, 0, 1);
                                if (sd != null)
                                {
                                    if (GUILayout.Button("X", GUILayout.Width(20)))
                                    {
                                        m_smr.SetBlendShapeWeight(sId, 0);
                                        pc.m_blendShapePreset.m_presets[i].m_blendShapes.Remove(sd);
                                    }
                                }
                                else
                                {
                                    GUILayout.Space(20);
                                }

                                GUILayout.EndHorizontal();

                                GUI.color = guiCol;



                                if (v != w)
                                {
                                    if (sd == null)
                                    {
                                        sd = new BlendShapesPresetTemplateBase.BlendShapeData();
                                        sd.name = BlendShapeName;
                                        pc.m_blendShapePreset.m_presets[i].m_blendShapes.Add(sd);
                                    }
                                    sd.weight = w;
                                    m_smr.SetBlendShapeWeight(sId, sd.weight * 100);
                                }


                            }

                            /*
                            for (int j = 0; j < pm.m_blendShapePreset.m_presets[i].m_shapes.Count; ++j)
                            {
                                BlendShapePreset.ShapeData sd = pm.m_blendShapePreset.m_presets[i].m_shapes[j];
                                sd.name = EditorGUILayout.TextField(pm.m_blendShapePreset.m_presets[i].m_shapes[j].name);
                                sd.weight = EditorGUILayout.Slider("Shape weight", sd.weight, 0f, 1f);

                            }*/
                        }

                        if (m_editID == i)
                            GUILayout.EndVertical();

                        //if (GUI.changed)
                        {
                            pc.Init();
                            pc.UpdateBlendShapes();
                        }
                        if (GUI.changed)
                            Undo.RegisterCompleteObjectUndo(pc, "Save preset");
                    }
                }

                GUILayout.EndVertical();

                               

                GUILayout.BeginVertical(EditorStyles.helpBox);
                if (GUILayout.Button("New Preset"))
                {
                    pc.m_blendShapePreset.m_presets.Add(new BlendShapesPresetTemplateBase.PresetData());
                    EditorUtility.SetDirty(pc.m_blendShapePreset);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Clear blendshapes weights"))
                {
                    pc.ClearBlendShapesWeights();
                }

                


                GUILayout.EndVertical();


                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if ( GUILayout.Button( (pc.m_showAdvancedParameters?"Hide":"Show") + " advanced parameters", EditorStyles.centeredGreyMiniLabel) )
                {
                    pc.m_showAdvancedParameters = !pc.m_showAdvancedParameters;
                }
                EditorGUILayout.EndHorizontal();


                if (pc.m_showAdvancedParameters)
                {

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label("Additionnal Skinned Mesh renderers", EditorStyles.boldLabel);
                    for (int i = 0; i < pc.m_skinnedMeshRenderers.Count; ++i)
                    {
                        GUILayout.BeginHorizontal();

                        bool blendShapeCountOk = pc.m_skinnedMeshRenderers[i].sharedMesh.blendShapeCount == pc.m_smr.sharedMesh.blendShapeCount;

                        GUI.backgroundColor = blendShapeCountOk? Color.white:Color.red;

                        GUILayout.Label(pc.m_skinnedMeshRenderers[i].gameObject.name  + (blendShapeCountOk ?"": " / Error\n  ->  Blendshapes count doesn't match") );
                                             

                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            pc.m_skinnedMeshRenderers.RemoveAt(i--);
                        }

                        GUI.backgroundColor = Color.white;
                        GUILayout.EndHorizontal();
                    }
                    SkinnedMeshRenderer empty = EditorGUILayout.ObjectField(null, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
                    if (empty != null && !pc.m_skinnedMeshRenderers.Contains(empty) && empty != pc.m_smr)
                    {
                        pc.m_skinnedMeshRenderers.Add(empty);
                    }


                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label("Manuel Bastioni Lab importer", EditorStyles.boldLabel);
                    if (GUILayout.Button("Load single expression"))
                    {
                        string filepath = EditorUtility.OpenFilePanel("Load Manuel Bastioni Lab Expression", "", "json");
                        LoadMBLabExpression(pc, filepath);

                        EditorUtility.SetDirty(pc.m_blendShapePreset);
                        AssetDatabase.SaveAssets();
                    }

                    if (GUILayout.Button("Load expressions folder"))
                    {
                        string folderpath = EditorUtility.OpenFolderPanel("Load Manuel Bastioni Lab Expressions folder", "", "");
                        string[] filespath = Directory.GetFiles(folderpath, "*.json");

                        for( int i=0; i< filespath.Length; ++i )
                            LoadMBLabExpression(pc, filespath[i]);

                        EditorUtility.SetDirty(pc.m_blendShapePreset);
                        AssetDatabase.SaveAssets();
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(20);
                }


               

                //DrawDefaultInspector();
            }
            
            


            class jsonElement
            {
                public string key = "";
                public string content = "";
                public List<jsonElement> elements = null;
                public jsonElement parent = null;


                public jsonElement Find(string searchKey)
                {
                    if (key == searchKey)
                        return this;

                    if (elements != null)
                    {
                        for (int i = 0; i < elements.Count; ++i)
                        {
                            jsonElement f = elements[i].Find(searchKey);
                            if (f != null) return f;
                        }
                    }
                    return null;
                }



                public void ParseContent()
                {
                    if (content.Trim() == "")
                        return;

                    if (content.Trim()[0] != '{')
                        return;
                    elements = new List<jsonElement>();

                    bool readKey = false;
                    bool readContent = false;
                    int subContentDepth = 0;
                    jsonElement currentElmt = null;

                    char[] json = content.Trim().ToCharArray();
                    for (int i = 1; i < json.Length; ++i)
                    {
                        if (json[i] == '{')subContentDepth++;
                        if (json[i] == '}')subContentDepth--;                            
                                                
                        if (!readContent)
                        {
                            //read key
                            if (readKey && json[i] != '"') currentElmt.key += json[i];
                            if (json[i] == '"')
                            {
                                if (!readKey)
                                {
                                    elements.Add(new jsonElement());
                                    currentElmt = elements[elements.Count - 1];
                                }
                                readKey = !readKey;
                            }
                        }

                        //readContent content
                        if (json[i] == ',' && subContentDepth == 0) readContent = false;
                        if (json[i] == '}' && subContentDepth == 0) readContent = false;
                        if (readContent) currentElmt.content += json[i];
                        if (json[i] == ':' && subContentDepth == 0 ) readContent = true;
                    }
                    
                    for (int i = 0; i < elements.Count; ++i)                    
                        elements[i].ParseContent();
                }


            }


            jsonElement ParseJson( string filepath )
            {
                jsonElement element = new jsonElement();
                element.key = "root";
                StreamReader sr = File.OpenText(filepath);                
                element.content = sr.ReadToEnd();
                element.ParseContent();
                element = element.Find("structural");                
                return element;
            }


            void LoadMBLabExpression( BlendShapesPresetControllerBase pc, string filepath )
            {                
                
                if (filepath == "")
                    return;
                List<jsonElement> elements = ParseJson(filepath).elements;


                List<string> blendShapeNames = new List<string>();

                for (int i = 0; i < pc.m_smr.sharedMesh.blendShapeCount; ++i)
                    blendShapeNames.Add(pc.m_smr.sharedMesh.GetBlendShapeName(i));

                string name = Path.GetFileNameWithoutExtension(filepath);

                BlendShapesPresetTemplateBase.PresetData pd = null;

                for ( int i=0; i< pc.m_blendShapePreset.m_presets.Count; ++i )
                    if (pc.m_blendShapePreset.m_presets[i].name == name )
                    {
                        pd = pc.m_blendShapePreset.m_presets[i];
                        break;
                    }

                if (pd == null)
                {
                    pc.m_blendShapePreset.m_presets.Add(new BlendShapesPresetTemplateBase.PresetData());
                    pd = pc.m_blendShapePreset.m_presets[pc.m_blendShapePreset.m_presets.Count - 1];
                    pd.name = name;
                }
                else
                {
                    pd.m_blendShapes.Clear();
                }

                

                //get blendshape weights
                for (int i = 0; i < elements.Count; ++i)
                {                   
                   
                    float w = 0;                    
                    try
                    {                        
                        w = float.Parse(elements[i].content);
                        string sfx = w >= .5 ? "_max" : "_min";
                        w = w >= .5 ? (w - .5f) * 2f : (.5f - w) * 2f;                        
                        //Debug.Log(elements[i].key + sfx);
                                                
                        if (blendShapeNames.Contains(elements[i].key + sfx))
                        {                            
                            //int idx = blendShapeNames.IndexOf(elements[i].key + sfx);
                            pd.m_blendShapes.Add(new BlendShapesPresetTemplateBase.BlendShapeData());
                            pd.m_blendShapes[pd.m_blendShapes.Count - 1].name = elements[i].key + sfx;
                            pd.m_blendShapes[pd.m_blendShapes.Count - 1].weight = w;
                        }
                    }
                    catch
                    {                        
                    }  
                    
                }
                
            }
        }
    }
}

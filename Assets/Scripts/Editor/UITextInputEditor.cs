using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UITextInput), true)]
[CanEditMultipleObjects]
public class UITextInputEditor : Editor
{
    UITextInput input;
    static SerializedObject soUiInput;
    SerializedProperty propLabel;

    int fontSize;
    bool placeholderChanged;
    bool useLabel;
    string placeholderDefault;

    private void OnEnable()
    {
        soUiInput = serializedObject;
        propLabel = soUiInput.FindProperty("labelText");
        input = (UITextInput)target;
        input.ShowLabelFromEditor();
    }

    public override void OnInspectorGUI()
    {
        input = (UITextInput)target;

        CustomUIBase.Title("Input Text by ASGD", Resources.Load<Font>("Fonts/Sans Mateo 2 Semi Bold"));

        serializedObject.Update();

        var styleBack = new GUIStyle(GUI.skin.textField);
        styleBack.padding = new RectOffset(10, 10, 10, 10);

        using (new EditorGUILayout.VerticalScope(styleBack))
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(soUiInput.FindProperty("type"));

                CustomUIBase.Div(5);

                EditorGUILayout.PropertyField(soUiInput.FindProperty("hasLabel"));
                useLabel = soUiInput.FindProperty("hasLabel").boolValue;
                if (useLabel)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.PropertyField(propLabel);
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("labelFont"));
                    }
                }

                var styleList = new GUIStyle();
                styleList.padding = new RectOffset(15, 5, 0, 0);
                using (new EditorGUILayout.VerticalScope(styleList))
                {
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("linkedObjects"));
                }
                GUILayout.Space(5);
            }

            input.showVariables = CustomUIBase.Foldout("Variables", input.showVariables, 17, TextAnchor.MiddleCenter);

            if (input.showVariables)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("_interactable"));

                    CustomUIBase.ScalePart(soUiInput);

                    EditorGUILayout.PropertyField(soUiInput.FindProperty("hasHighlight"));
                    if(soUiInput.FindProperty("hasHighlight").boolValue)
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("highlight"));

                    CustomUIBase.Subtitle("Placeholder / Input", 14);

                    EditorGUILayout.PropertyField(soUiInput.FindProperty("placeholder"));
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("inputText"));
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("inputGhost"));

                    CustomUIBase.Div(5);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("fontInput"));
                        var temp = fontSize;
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("fontSize"), new GUIContent(""), GUILayout.Width(60));
                    }

                    EditorGUILayout.PropertyField(soUiInput.FindProperty("placeholderDefault"));
                    placeholderChanged = placeholderDefault != soUiInput.FindProperty("placeholderDefault").stringValue ? true : false;
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("prefix"));
                }
            }
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            input.ChangeLabelFromEditor(input.getLabel(), useLabel);
            if (placeholderChanged) placeholderDefault = input.SetPlaceholderFromEditor();
            if (soUiInput.FindProperty("hasHighlight").boolValue) input.SetHighlightFromEditor();
            input.ChangeFontFromEditor();
        }
    }
}

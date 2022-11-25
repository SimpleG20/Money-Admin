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
        input.showLabel();
    }

    public override void OnInspectorGUI()
    {
        input = (UITextInput)target;

        CustomUIBase.Title("Input Text by ASGD");

        serializedObject.Update();

        var styleBack = new GUIStyle(GUI.skin.textField);
        styleBack.padding = new RectOffset(10, 10, 10, 10);

        using (new EditorGUILayout.VerticalScope(styleBack))
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(soUiInput.FindProperty("type"));
                EditorGUILayout.PropertyField(soUiInput.FindProperty("_interactable"));

                CustomUIBase.ScalePart(soUiInput);

                CustomUIBase.Div(5);

                CustomUIBase.Subtitle("Label", 15);

                GUILayout.Space(5);

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
                GUILayout.Space(5);

                CustomUIBase.Subtitle("Placeholder/Input", 15);

                GUILayout.Space(5);

                EditorGUILayout.PropertyField(soUiInput.FindProperty("placeholder"));
                EditorGUILayout.PropertyField(soUiInput.FindProperty("placeholderDefault"));
                placeholderChanged = placeholderDefault != soUiInput.FindProperty("placeholderDefault").stringValue ? true : false;
                EditorGUILayout.PropertyField(soUiInput.FindProperty("inputText"));
                EditorGUILayout.PropertyField(soUiInput.FindProperty("inputGhost"));
                GUILayout.Space(5);

                CustomUIBase.Div(5);

                var styleList = new GUIStyle();
                styleList.padding = new RectOffset(15, 5, 0, 0);
                using (new EditorGUILayout.VerticalScope(styleList))
                {
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("linkedObjects"));
                }
            }

            input.showVariables = CustomUIBase.Foldout("Variables", input.showVariables, 17, TextAnchor.MiddleCenter);

            if (input.showVariables)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("prefix"));

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("font"));
                        var temp = fontSize;
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("fontSize"), new GUIContent(""), GUILayout.Width(60));
                    }
                    EditorGUILayout.PropertyField(soUiInput.FindProperty("hasHighlight"));
                    if(soUiInput.FindProperty("hasHighlight").boolValue)
                        EditorGUILayout.PropertyField(soUiInput.FindProperty("highlight"));
                }
            }
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            input.ChangeLabel(input.getLabel(), useLabel);
            if (placeholderChanged) placeholderDefault = input.ChangePlaceholder();
            if (soUiInput.FindProperty("hasHighlight").boolValue) input.UpdateHighlight();
            input.ChangeFont();
        }
    }
}

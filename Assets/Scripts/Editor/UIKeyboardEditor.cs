using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Enums;
using UnityEngine.UIElements;
using static UnityEditor.MaterialProperty;

[CustomEditor(typeof(KeyboardPad))]
[CanEditMultipleObjects]
public class UIKeyboardEditor : Editor
{
    static SerializedObject so;
    SerializedProperty propType;

    string labelPast;
    bool showExtraParams = true, labelChanged;
    static GUIStyle styleList;

    private void OnEnable()
    {
        so = serializedObject;

        propType = so.FindProperty("function");
        labelPast = so.FindProperty("labelText").stringValue;

        styleList = new GUIStyle();
        styleList.padding = new RectOffset(15, 5, 0, 0);
    }
    public override void OnInspectorGUI()
    {
        KeyboardPad button = (KeyboardPad)target;
        CustomUIBase.Title("UI Keyboard by ASGD");

        var styleBack = new GUIStyle(GUI.skin.textField);
        styleBack.padding = new RectOffset(10, 10, 10, 10);

        so.Update();

        using (new GUILayout.VerticalScope(styleBack))
        {

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                #region Part One
                EditorGUILayout.PropertyField(propType);

                CustomUIBase.Div(5);

                string[] variables = new string[] { "labelComponent", "labelText" };
                string[] names = new string[] { "", "" };
                CustomUIBase.PartOfSomeVariablesRelated(so, true, "hasLabel", variables, names, 275, 100);

                labelChanged = labelPast != so.FindProperty("labelText").stringValue ? true : false;

                EditorGUILayout.PropertyField(so.FindProperty("textToWrite"));

                if (so.FindProperty("hasLabel").boolValue) CustomUIBase.Div(5);

                using (new GUILayout.VerticalScope(styleList))
                {
                    #region Linked Objects
                    EditorGUILayout.PropertyField(so.FindProperty("linkedObjects"));
                    #endregion
                }
                #endregion

                GUILayout.Space(10);

                #region Part Two
                button.showVariables = CustomUIBase.Foldout("Variables", button.showVariables, 17, TextAnchor.MiddleCenter);

                if (button.showVariables)
                {
                    //Ripple and Highlight
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.PropertyField(so.FindProperty("_interactable"));
                        EditorGUILayout.PropertyField(so.FindProperty("selected"));

                        GUILayout.Space(3);

                        CustomUIBase.Subtitle("Transformations", 15);

                        CustomUIBase.ScalePart(so);

                        var ripple = so.FindProperty("useRipple");
                        EditorGUILayout.PropertyField(ripple);
                        if (ripple.boolValue == true)
                        {
                            EditorGUI.indentLevel++;
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                                EditorGUILayout.PropertyField(so.FindProperty("rippleSource"));
                            EditorGUI.indentLevel--;
                        }

                        GUILayout.Space(3);

                        CustomUIBase.Subtitle("Highlight", 15);

                        if (!so.FindProperty("temporarilyHighlight").boolValue) EditorGUILayout.PropertyField(so.FindProperty("toggle"));
                        if (!so.FindProperty("toggle").boolValue) EditorGUILayout.PropertyField(so.FindProperty("temporarilyHighlight"));
                        EditorGUILayout.PropertyField(so.FindProperty("highlightSource"));
                        GUILayout.Space(3);
                    }
                }
                #endregion

                GUILayout.Space(5);
            }
        }

        if(so.ApplyModifiedProperties())
        {
            if (labelChanged)
            {
                labelPast = button.ChangeLabel();
            }
        }
    }
}

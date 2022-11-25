using UnityEditor;
using UnityEngine;
using static Enums;

[CustomEditor(typeof(UIType), true)]
[CanEditMultipleObjects]
public class UITypeEditor : Editor
{
    static SerializedObject so;
    SerializedProperty propType;

    string labelPast;
    bool showExtraParams = true, labelChanged;
    static GUIStyle styleList;

    private void OnEnable()
    {
        so = serializedObject;
        propType = so.FindProperty("typeUi");
        labelPast = so.FindProperty("labelText").stringValue;

        styleList = new GUIStyle();
        styleList.padding = new RectOffset(15, 0, 0, 0);
    }
    public override void OnInspectorGUI()
    {
        UIType button = (UIType)target;
        CustomUIBase.Title("UI Button by ASGD");

        serializedObject.Update();

        var styleBack = new GUIStyle(GUI.skin.textField);
        styleBack.padding = new RectOffset(10, 10, 10, 10);

        
        
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

            //CustomUIBase.Div(5);

            if (propType.intValue > 1) CustomUIBase.LabeledDiv("Other Params", GUI.skin.label);

            #region Part Three
            if(button.family == FamilyUI.GENERIC)
            {
                switch (button.typeUi)
                {
                    case TypesUI.GENERIC:
                        EditorGUILayout.PropertyField(so.FindProperty("clickEvent"), new GUIContent("On Click Event"), true);
                        break;
                    case TypesUI.DESTINATION:
                        showExtraParams = DestinationParams(showExtraParams);
                        break;
                    case TypesUI.PAGE:
                        showExtraParams = PageParams(showExtraParams);
                        break;
                    case TypesUI.HOLD:
                        showExtraParams = HoldParams(showExtraParams);
                        break;
                }
            }
            #endregion
        }

        if (serializedObject.ApplyModifiedProperties()) 
        {
            if (labelChanged) 
            { 
                labelPast = button.ChangeLabel();
            }
        }
    }
    public static bool DestinationParams(bool display)
    {
        display = CustomUIBase.Foldout("Destination Button Parameters", display);

        if (display)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(so.FindProperty("destinationBtParams.target"));
                EditorGUILayout.PropertyField(so.FindProperty("destinationBtParams.managment"));
            }
        }
        return display;
    }
    public static bool PageParams(bool display)
    {
        display = CustomUIBase.Foldout("Page Button Parameters", display, 13);

        if (display)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                string[] variables = new string[] { "pageBtParams.title" };
                string[] names = new string[] { "Title" };
                CustomUIBase.PartOfSomeVariablesRelated(so, false, "pageBtParams.multiplePages", variables, names, 250, 110);

                if (so.FindProperty("pageBtParams.multiplePages").boolValue) CustomUIBase.ShowList(so, "pageBtParams.pages", styleList);
                EditorGUILayout.PropertyField(so.FindProperty("pageBtParams.direction"));
            }
        }
        return display;
    }
    public static bool HoldParams(bool display)
    {
        display = CustomUIBase.Foldout("Hold Button Parameters", display, 13);

        if (display)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(so.FindProperty("holdBtParams.type"));
                EditorGUILayout.Toggle("Released", so.FindProperty("holdBtParams.released").boolValue);

                if (so.FindProperty("holdBtParams.type").intValue != 0)
                {
                    EditorGUILayout.PropertyField(so.FindProperty("holdBtParams.min"));
                    EditorGUILayout.PropertyField(so.FindProperty("holdBtParams.max"));
                }
            }
        }
        return display;
    }
}


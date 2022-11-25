using System;
using UnityEditor;
using UnityEngine;

public static class CustomUIBase
{
    public static bool Foldout(string title, bool display, int fontSize = 12, TextAnchor anchor = TextAnchor.MiddleLeft)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.label).font;
        style.alignment = anchor;
        style.fontSize = fontSize;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 25;

        if (anchor == TextAnchor.MiddleLeft) style.contentOffset = new Vector2(20f, -2f);
        else if(anchor == TextAnchor.MiddleCenter) style.contentOffset = new Vector2(-20f, -2f);

        var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x, rect.y + 3.5f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }
    public static bool Title(string title)
    {
        var titleCentered = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22};
        EditorGUILayout.LabelField(title, titleCentered, GUILayout.Height(30));

        GUILayout.Label("", GUI.skin.horizontalSlider, GUILayout.Height(10));
        GUILayout.Space(10);

        return true;
    }
    public static bool Subtitle(string subtitle, int sizeFont)
    {
        //GUILayout.Label("", GUI.skin.horizontalSlider, GUILayout.Height(10));

        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.label).font;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = sizeFont;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 25;
        style.contentOffset = new Vector2(-20f, -2f);

        var rect = GUILayoutUtility.GetRect(16, 22f, style);
        GUI.Box(rect, subtitle, style);


        //var centered = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerCenter, fontSize = sizeFont};
        //EditorGUILayout.LabelField(subtitle, centered, GUILayout.Height(25));

        //GUILayout.Label("", GUI.skin.horizontalSlider, GUILayout.Height(10));
        return true;
    }

    public static void ShowList(SerializedObject so, string variable, GUIStyle style)
    {
        using(new EditorGUILayout.VerticalScope(style))
            EditorGUILayout.PropertyField(so.FindProperty(variable));
    }
    public static bool Div(int height)
    {
        //GUILayout.Space(3);
        GUILayout.Label("", GUI.skin.horizontalSlider, GUILayout.Height(height));
        GUILayout.Space(10);
        return true;
    }
    public static bool LabeledDiv(string label, GUIStyle skin, TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        GUILayout.Space(5);
        var styleHeader = new GUIStyle(skin);
        styleHeader.alignment = anchor;
        styleHeader.fontSize = 17;
        GUILayout.Label(label, styleHeader);
        GUILayout.Space(5);

        return true;
    }

    public static void ScalePart(SerializedObject so)
    {
        using (new EditorGUILayout.VerticalScope())
        {
            var propScale = so.FindProperty("scale");
            var show = false;
            EditorGUILayout.PropertyField(propScale);
            show = propScale.boolValue;

            if (show)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("On Hover", GUILayout.MaxWidth(250), GUILayout.MinWidth(75));
                        EditorGUILayout.PropertyField(so.FindProperty("scaleOnHover"), new GUIContent(""), GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
                    }
                    GUILayout.Button("", GUILayout.Width(5), GUILayout.Height(25));
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("On Click", GUILayout.MaxWidth(250), GUILayout.MinWidth(75));
                        EditorGUILayout.PropertyField(so.FindProperty("scaleOnClick"), new GUIContent(""), GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
                    }
                    GUILayout.Button("", GUILayout.Width(5), GUILayout.Height(25));
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Scale", GUILayout.MaxWidth(250), GUILayout.MinWidth(50));
                        EditorGUILayout.PropertyField(so.FindProperty("scaleSize"), new GUIContent(""));
                    }
                }
            }
        }
    }

    public static void PartOfSomeVariablesRelated(SerializedObject so, bool displayWhen, string mainVariable,string[] variables, string[] desiredNames, int max = 50, int min = 20)
    {
        using (new EditorGUILayout.VerticalScope())
        {
            var prop = so.FindProperty(mainVariable);
            EditorGUILayout.PropertyField(prop);
            var show = false;
            show = prop.boolValue;

            if (show == displayWhen)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int i = 0; i < variables.Length; i++)
                    {
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            if (desiredNames[i] != "") EditorGUILayout.LabelField(desiredNames[i], GUILayout.MaxWidth(250), GUILayout.MinWidth(75));
                            EditorGUILayout.PropertyField(so.FindProperty(variables[i]), new GUIContent(""), GUILayout.MaxWidth(max), GUILayout.MinWidth(min));
                        }
                    }
                }
            }
        }
    }
}
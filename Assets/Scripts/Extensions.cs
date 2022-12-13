using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static Enums;

public static class Extensions 
{
    public async static void ChangeAlpha(this CanvasGroup obj, float valueDesired)
    {
        int sinal = valueDesired >= obj.alpha ? 1 : -1;
        obj.alpha += sinal * 0.1f;

        await Task.Delay(100);

        if (obj.alpha < valueDesired) { obj.ChangeAlpha(valueDesired); return; }
        else { obj.alpha = valueDesired; Debug.Log("Finalizou"); return; }
    }

    public static int PositionOfTheOnlyActived(this List<Toggle> list)
    {
        for(int i=0; i < list.Count; i++)
        {
            if (list[i].isOn) return i;
        }
        return 0;
    }
    public static T PickRandom<T>(this List<T> obj)
    {
        return obj[Random.Range(0, obj.Count)];
    }
    public static List<T> getList<T>(this List<GameObject> list)
    {
        if (list == null) return null;

        List<T> ret = new List<T>();
        foreach (GameObject obj in list)
        {
            if(obj.GetComponent<T>() != null) ret.Add(obj.GetComponent<T>());
        }

        return ret;
    }
    public static List<T> getChildsWithComponentT<T>(this Transform transform)
    {
        List<T> ret = new List<T>();

        foreach(Transform child in transform)
        {
            if(child.GetComponent<T>() != null)
                ret.Add(child.GetComponent<T>());
            if(child.childCount != 0)
            {
                var list = child.getChildsWithComponentT<T>();
                foreach (T component in list)
                    ret.Add(component);
            }
        }

        return ret;
    }
    public static void DeleteChildren(this Transform transform)
    {
        foreach (Transform child in transform) Object.Destroy(child.gameObject);
    }


    #region String
    public static void LoopStringFadingTMPro(this TextMeshProUGUI text)
    {
        LeanTween.cancel(text.gameObject);
        LeanTween.value(text.gameObject, 0, 1, 0.6f).setLoopPingPong().setOnUpdate((value) =>
        {
            var color = text.color;
            color.a = value;
            text.color = color;
        });
    }
    public static void CheckCommaSituationTMPro(this TextMeshProUGUI text)
    {
        if (text.text.Contains(","))
        {
            var builder = new StringBuilder();
            if(text.text.Length - text.text.IndexOf(",") < 2)
            {
                builder.Append(text.text);
                builder.Append("00");
                text.text = builder.ToString();
            }
            else if(text.text.Length - text.text.IndexOf(",") < 3)
            {
                builder.Append(text.text);
                builder.Append("0");
                text.text = builder.ToString();
            }
        }
    }
    public static string CheckCommaSituation(this string text)
    {
        if (text.Contains(","))
        {
            var builder = new StringBuilder();
            if (text.Length - text.IndexOf(",") < 2)
            {
                builder.Append(text);
                builder.Append("00");
                text = builder.ToString();
            }
            else if (text.Length - text.IndexOf(",") < 3)
            {
                builder.Append(text);
                builder.Append("0");
                text = builder.ToString();
            }
        }
        return text;
    }
    public static string MoneyFormat(this string text)
    {
        var builder = new StringBuilder();
        builder.Append("R$ ");
        if (text.Contains(","))
        {
            if (text.Length - text.IndexOf(",") < 2)
            {
                builder.Append(text);
                builder.Append("00");
            }
            else if (text.Length - text.IndexOf(",") < 3)
            {
                builder.Append(text);
                builder.Append("0");
            }
            else builder.Append(text);
        }
        else
        {
            builder.Append(text);
            builder.Append(",00");
        }
        return builder.ToString();
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

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

    public static void getValues(this Vector3 vector, out float x, out float y, out float z)
    {
        x = vector.x; 
        y = vector.y;
        if (vector.GetType() == typeof(Vector3)) z = vector.z;
        else z = 0;
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

    public static void setPlaceholderUpdated(this UITextInput ui, string value)
    {
        StringBuilder stringBuilder= new StringBuilder();

        if (value == "" || value == "0") { ui.placeholder.text = ui.placeholderDefault; return; }

        stringBuilder.Append(ui.prefix);
        stringBuilder.Append(value);

        ui.NeedSufix(stringBuilder.ToString());

        ui.placeholderDefault = ui.prefix + value + ui.sufix;
    }

    public static void DeleteChildren(this Transform transform)
    {
        foreach (Transform child in transform) Object.Destroy(child.gameObject);
    }

    public static bool CommaRule(this string input, string toAppend, TypeInputValue typeInput)
    {
        var length = input.Length;
        if (input.Contains(","))
        {
            if (length - input.IndexOf(",") >= 3) return false;
            if (toAppend == "," && typeInput == TypeInputValue.Float) return false;
        }
        else
        {
            if (typeInput == TypeInputValue.Int) return false;
        }

        return true;
    }
    public static void LoopStringFading(this TextMeshProUGUI text, string waiting)
    {
        LeanTween.cancel(text.gameObject);
        LeanTween.value(text.gameObject, 0, 1, 0.6f).setLoopPingPong().setOnUpdate((value) =>
        {
            var color = text.color;
            if (text.text != waiting || Keyboard.leave)
            {
                color.a = 1;
                text.color = color;
                LeanTween.cancel(text.gameObject);
                return;
            }
            color.a = value;
            text.color = color;
        });
    }
}

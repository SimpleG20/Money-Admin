using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TypeInputValue { Int, Float, String }

[RequireComponent(typeof(CanvasGroup), typeof(Animator))]
public class UITextInput : UIElement, IPointerClickHandler
{
    public TypeInputValue type;
    public string prefix, sufix, placeholderDefault;
    public TMP_FontAsset labelFont;

    public TextMeshProUGUI inputText, inputGhost;
    public TextMeshProUGUI placeholder;
    private string placeholderString;
    
    public TMP_FontAsset font;
    public int fontSize;
    public bool hasHighlight;
    private bool waitingInput;

    private Vector3 prevTransform;

    private InputValue inputValue;
    private GameObject leaveButton;
    private GameObject highlight;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.variablesSet && start);

        labelComponent.text = labelText;
        labelComponent.font = labelFont;

        if (inputText.text == "")
        {
            placeholder.gameObject.SetActive(true);
            inputText.gameObject.SetActive(false);
        }
        else
        {
            placeholder.gameObject.SetActive(false);
            inputText.gameObject.SetActive(true);
        }
    }
    public override void Init()
    {
        base.Init();

        if (hasHighlight)
        {
            highlight = inputText.transform.GetChild(0).gameObject;
            highlight.gameObject.SetActive(false);
        }

        inputValue = new InputValue(type);
    }

    public void Default()
    {
        placeholder.text = placeholderDefault;
        Unselected();
    }

    #region get and set
    public InputValue getInputValue() => inputValue;
    #endregion

    #region Selection
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        Selected();
    }
    
    public override void Selected()
    {
        base.Selected();

        placeholder.gameObject.SetActive(false);
        placeholderString = placeholder.text;
        InputShowingSituation(true);

        if (hasHighlight) highlight.SetActive(true);
        SelectedSecondPart();
    }
    private void SelectedSecondPart()
    {
        DisplayKeyboard();
        StartCoroutine(AdjustTransform());
        WaitingInput();
        DisplayLeaveButton();
    }
    public void WaitingInput()
    {
        if (waitingInput) return;

        inputGhost.gameObject.SetActive(false);
        waitingInput = true;

        var waiting = inputText.text;
        if (waiting == "") waiting = "...";

        inputText.text = waiting;
        inputText.LoopStringFading(waiting);
        /*LeanTween.value(inputText.gameObject, 0, 1, 0.6f).setLoopPingPong().setOnUpdate((value) =>
        {
            var color = inputText.color;
            if (inputText.text != waiting || Keyboard.leave) 
            {
                color.a = 1;
                inputText.color = color;
                waitingInput = false;
                LeanTween.cancel(inputText.gameObject); 
                return; 
            }
            color.a = value;
            inputText.color = color;
        });*/
    }
    private void DisplayKeyboard()
    {
        GameManager.Instance.canSetInteractable = false;
        Keyboard.Instance.ShowKeyboard(type, this, inputValue, linkedObjects);
    }
    private void DisplayLeaveButton()
    {
        if (leaveButton == null) leaveButton = GameManager.Instance.getLeaveInput();
        leaveButton.SetActive(true);
    }
    private IEnumerator AdjustTransform()
    {
        prevTransform = transform.position;
        if (transform.position.y < Screen.height / 2)
        {
            var offset = Mathf.Abs(transform.position.y - Screen.height / 2);
            ScenesManagment.Instance.getCurrentScene().getAnimator().enabled = false;
            LeanTween.moveY(ScenesManagment.Instance.getCurrentScene().gameObject, (Screen.height / 2) + offset, 0.5f);
        }

        yield return new WaitUntil(() => Keyboard.leave);
        if (prevTransform != transform.position)
        {
            LeanTween.moveY(ScenesManagment.Instance.getCurrentScene().gameObject, (Screen.height / 2), 0.5f).setOnComplete(() => {
                ScenesManagment.Instance.getCurrentScene().getAnimator().enabled = true;
            }); ;
        }
    }

    public override void Unselected()
    {
        base.Unselected();

        if (hasHighlight) highlight.SetActive(false);

        HandlingPrefixAndSufix();
        
        placeholder.gameObject.SetActive(true);
        inputText.gameObject.SetActive(false);
    }
    #endregion

    #region Input Text
    internal bool NeedSufix(string input)
    {
        if(input.Contains(",") && type == TypeInputValue.Float)
        {
            var distance = input.Length - input.IndexOf(",");
            if (distance == 2) { sufix = "0"; return true; }
            else if (distance == 1) { sufix = "00"; return true; }
            
            sufix = ""; 
            return false;
        }
        else if(!input.Contains(",") && type == TypeInputValue.Float)
        {
            sufix = ",00";
            return true;
        }

        return false;
    }
    private void HandlingPrefixAndSufix()
    {
        if (inputText.text == "..." || inputText.text == "")
        {
            placeholderString = placeholderDefault;
            placeholder.text = placeholderString;
            inputText.text = "";
            inputGhost.text = "";
            return;
        }
        StringBuilder builder = new StringBuilder();

        if (type != TypeInputValue.String)
        {
            if (type == TypeInputValue.Int)
            {
                int result;
                int.TryParse(inputText.text, out result);
                if (prefix == "" && result < 10) prefix = result == 0 ? "00": "0";
            }
            else
            {
                float result;
                float.TryParse(inputText.text, out result);
                if (prefix == "" && result < 10) prefix = result == 0 ? "00" : "0";
            }
        }
        builder.Append(prefix);
        builder.Append(inputText.text);

        if (NeedSufix(builder.ToString())) builder.Append(sufix);

        placeholder.text = placeholderString = builder.ToString();
    }
    private void InputShowingSituation(bool value)
    {
        inputText.gameObject.SetActive(value);
    }
    public void ChangingInputsTexts(string s)
    {
        inputText.text = s;

        if (s.CommaRule("_", type))
        {
            inputGhost.gameObject.SetActive(true);
            inputGhost.fontSizeMax = inputText.fontSize;
            inputGhost.LoopStringFading(inputGhost.text);
        }
        else
        {
            LeanTween.cancel(inputGhost.gameObject);
            inputGhost.gameObject.SetActive(false);
        }
    }
    #endregion

    #region ContextMenu
    public void ChangeFont()
    {
        if (inputText == null) inputText = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        if (placeholder == null) placeholder = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        if (hasLabel) labelComponent.font = labelFont;

        inputText.font = font;
        placeholder.font = font;

        if(inputText.enableAutoSizing) inputText.fontSizeMax = fontSize;
        else inputText.fontSize = fontSize;
        if(placeholder.enableAutoSizing) placeholder.fontSizeMax = fontSize;
        else placeholder.fontSize = fontSize;
    }
    public void ChangeLabel(string label, bool useLabel = true)
    {
        if (!useLabel)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            return;
        }
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        if (labelComponent == null) transform.GetChild(0).GetChild(0).TryGetComponent(out labelComponent);
        labelComponent.text = label;
    }
    public void showLabel()
    {
        if (!hasLabel) return;

        if (labelComponent == null) transform.GetChild(0).GetChild(0).TryGetComponent(out labelComponent);
        labelComponent.gameObject.SetActive(true);
        labelComponent.text = labelText;
        labelComponent.font = labelFont;
    }
    public string ChangePlaceholder()
    {
        if(placeholder == null) transform.GetChild(0).GetChild(1).TryGetComponent(out placeholder);
        placeholder.text = placeholderDefault;
        return placeholder.text;
    }
    #endregion
}

[Serializable]
public class InputValue
{
    public int intValue;
    public float floatValue;
    public string stringValue;
    public float incrementValue;

    private TypeInputValue typeValue;

    public InputValue(TypeInputValue _type)
    {
        typeValue = _type;
    }
    public TypeInputValue getType() => typeValue;
}

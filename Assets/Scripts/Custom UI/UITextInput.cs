using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

[RequireComponent(typeof(CanvasGroup), typeof(Animator))]
public class UITextInput : UIElement, IPointerClickHandler
{
    public TypeInputValue type;
    public string prefix, sufix;
    public string placeholderDefault;

    [SerializeField] protected TextMeshProUGUI inputText, inputGhost;
    [SerializeField] protected TextMeshProUGUI placeholder;
    private string placeholderString;
    
    public TMP_FontAsset labelFont;
    public TMP_FontAsset fontInput;
    public int fontSize;
    public bool hasHighlight;
    [SerializeField] protected GameObject highlight;


    private Vector3 prevTransform;
    private InputValue inputValue;
    private GameObject leaveButton;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.variablesSet && start);

        if(labelComponent != null)
        {
            labelComponent.text = labelText;
            labelComponent.font = labelFont;
        }

        placeholder.gameObject.SetActive(true);
        inputText.gameObject.SetActive(false);
    }
    public override void Init()
    {
        base.Init();

        if (hasHighlight)
        {
            highlight = transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
            highlight.gameObject.SetActive(false);
        }

        inputValue = inputValue ?? new InputValue(type);
        if(placeholder.text != placeholderDefault)
        {
            switch (type)
            {
                case TypeInputValue.Int:
                    int.TryParse(placeholder.text, out inputValue.intValue);
                    break;
                case TypeInputValue.Float:
                    float.TryParse(placeholder.text, out inputValue.floatValue);
                    break;
                default:
                    inputValue.stringValue = placeholder.text;
                    break;
            }
        }
    }

    public void Default()
    {
        placeholder.text = placeholderDefault;
        Unselected();
    }

    #region get and set
    public InputValue getInputValue() => inputValue;
    public string getPlaceholderSrting() => placeholderString;
    public string getInputString() => inputText.text;
    public GameObject getInputGhostOB() => inputGhost.gameObject;
    public void setPlaceholder(string s, TypeInputValue type, bool money = false)
    {
        if(inputValue == null) inputValue = new InputValue(this.type);

        switch (type)
        {
            case TypeInputValue.Int:
                int.TryParse(s, out inputValue.intValue);
                break;
            case TypeInputValue.Float:
                float.TryParse(s, out inputValue.floatValue);
                break;
            default:
                inputValue.stringValue = s;
                break;
        }

        if (s == "" || s == "0")
        {
            placeholder.text = placeholderDefault;
            return;
        }

        if (money) s = s.MoneyFormatForString();

        placeholder.text = s;
        placeholderString = s;
    }
    public void setInputText(string s)
    {
        inputText.text = s;
    }
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
    private void WaitingInput()
    {
        if (Keyboard.waitingInput) return;

        var waiting = new StringBuilder();
        if (placeholderString == placeholderDefault) waiting.Append(inputText.text);
        else 
        {
            waiting.Append(placeholderString);
            waiting.Remove(0, prefix.Length);
            inputText.text = waiting.ToString(); 
        }

        if (waiting.ToString() != "")
        {
            ChangingInputsTexts(waiting.ToString());
            return;
        }

        if (waiting.ToString() == "")
        {
            waiting.Clear();
            waiting.Append("...");
        }
        inputText.text = waiting.ToString();
        CheckInputText();
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
                if (prefix == "" && result < 10 && !inputText.text.Contains(",")) 
                    prefix = result == 0 ? "00": "0";
            }
            else
            {
                float result;
                float.TryParse(inputText.text, out result);
                if (prefix == "" && result < 10 && !inputText.text.Contains(",")) 
                    prefix = result == 0 ? "00" : "0";
            }
        }
        builder.Append(prefix);
        builder.Append(inputText.text);

        if (NeedSufix(builder.ToString())) builder.Append(sufix);

        placeholder.text = placeholderString = builder.ToString();
    }
    private void WaitingToInitializeInput()
    {
        DisableContinueWriting();
        Keyboard.waitingInput = true;

        LeanTween.cancel(inputText.gameObject);
        LeanTween.value(inputText.gameObject, 0, 1, 0.6f).setLoopPingPong().setOnUpdate((value) =>
        {
            var color = inputText.color;
            if (!Keyboard.waitingInput || Keyboard.leave)
            {
                color.a = 1;
                inputText.color = color;
                LeanTween.cancel(inputText.gameObject);
                return;
            }
            color.a = value;
            inputText.color = color;
        });
    }
    private void InputShowingSituation(bool value)
    {
        inputText.gameObject.SetActive(value);
    }
    public void ChangingInputsTexts(string s)
    {
        inputText.text = s;

        if (IsWritingMore(s, type)) EnableContinueWriting();
        else CheckInputText();
    }


    public void CheckInputText()
    {
        if (inputText.text == "" || inputText.text == "...")
        {
            inputText.text = "...";
            WaitingToInitializeInput();
        }
        else
        {
            if(IsWritingMore(inputText.text, type)) EnableContinueWriting();
        }
    }
    private bool IsWritingMore(string input, TypeInputValue typeInput)
    {
        var length = input.Length;

        if (input.Contains(","))
        {
            if (length - input.IndexOf(",") >= 3) return false;
        }
        else
        {
            if (input == "" || input == "...") return false;
            if (typeInput == TypeInputValue.String)
            {
                if (length > 30) return false;
            }
            else
            {
                if (length > 10 && typeInput == TypeInputValue.Int) return false;
                if (length > 12 && typeInput == TypeInputValue.Float) return false;
            }
        }

        return true;
    }
    public void DisableContinueWriting()
    {
        LeanTween.cancel(inputGhost.gameObject);
        inputGhost.gameObject.SetActive(false);
        inputText.transform.parent.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(0, 0, 0, 0);
    }
    private void EnableContinueWriting()
    {
        inputGhost.gameObject.SetActive(true);
        inputGhost.fontSizeMax = inputText.fontSize;
        inputGhost.LoopStringFadingTMPro();
        inputText.transform.parent.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(30, 52, 0, 0);
    }
    #endregion

    #region ContextMenu
    public void SetHighlightFromEditor()
    {
        highlight = transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
    }
    public void ChangeFontFromEditor()
    {
        if (inputText == null) inputText = transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        if (placeholder == null) placeholder = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        if (hasLabel) labelComponent.font = labelFont;

        inputText.font = fontInput;
        placeholder.font = fontInput;

        if(inputText.enableAutoSizing) inputText.fontSizeMax = fontSize;
        else inputText.fontSize = fontSize;
        if(placeholder.enableAutoSizing) placeholder.fontSizeMax = fontSize;
        else placeholder.fontSize = fontSize;
    }
    public void ChangeLabelFromEditor(string label, bool useLabel = true)
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
    public void ShowLabelFromEditor()
    {
        if (!hasLabel) return;

        if (labelComponent == null) transform.GetChild(0).GetChild(0).TryGetComponent(out labelComponent);
        labelComponent.gameObject.SetActive(true);
        labelComponent.text = labelText;
        labelComponent.font = labelFont;
    }
    public string SetPlaceholderFromEditor()
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

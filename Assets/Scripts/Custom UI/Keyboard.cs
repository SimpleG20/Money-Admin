using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using static Enums;
using System.Linq;
using System;

public class Keyboard : UIElement
{
    public static bool holding, waitingInput;
    public static bool leave;
    public static bool fixedUpper;
    private static bool _toUpper, keyboardShowing;
    private static bool toUpper
    {
        get => _toUpper;
        set
        {
            _toUpper = value;
            ChangeLettersSize(value);
        }
    }
    private static TypeInputValue typeInput;

    private static UITextInput inputField;
    private static InputValue inputValue;

    [SerializeField] UIElement iconsGroup;

    static Transform _transform;
    static List<UIElement> linkedElements = new List<UIElement>();

    public static KeyboardPad capslock;
    static List<KeyboardPad> letters;
    static List<KeyboardPad> numbers;

    private static Keyboard _instance;
    public static Keyboard Instance
    {
        get
        {
            if (_instance == null) _instance = GameObject.Find("Teclado").GetComponent<Keyboard>();
            return _instance;
        }
    }

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (keyboardShowing)
        {
            if (Input.GetKeyDown(KeyCode.Backspace)) { Backspace(""); return; }
            else if (Input.GetKeyDown(KeyCode.Space)) { Typing("-"); return; }
            else if (Input.GetKeyDown(KeyCode.CapsLock)) { CapsLock(""); return; }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) { GameManager.Instance.LeaveInputSection(); return; }
            if (Input.anyKeyDown)
            {
                Typing(Input.inputString);
            }
        }
    }

    #region Visualization
    public bool ShowKeyboard(TypeInputValue type, UITextInput _input, InputValue _inputValue, List<GameObject> elements)
    {
        //groupPrevValue = iconsGroup.getCanvasGroup().alpha;
        keyboardShowing = true;
        leave = false;

        linkedElements.Clear();
        linkedElements = elements.getList<UIElement>();

        foreach (UIElement e in linkedElements)
        {
            e.setInteractable(false);
        }

        typeInput = type;
        if (typeInput == TypeInputValue.String) iconsGroup.getAnimator().SetBool("Letras", true);
        else iconsGroup.getAnimator().SetBool("Numeros", true);

        inputField = _input;
        inputValue = _inputValue;
        return (inputField != null && inputValue != null);
    }
    public void HideKeyboard()
    {
        if (!animator.enabled) animator.enabled = true;
        keyboardShowing = false;
        leave = true;
        //if (groupPrevValue == 1) iconsGroup.ShowUi();

        foreach (UIElement e in linkedElements) e.setInteractable(true);

        if (typeInput == TypeInputValue.String) iconsGroup.getAnimator().SetBool("Letras", false);
        else iconsGroup.getAnimator().SetBool("Numeros", false);

        inputField = null;
        inputValue = null;
    }
    public void MoveKeyboard()
    {
        if (animator.enabled) animator.enabled = false;
        transform.position = Input.mousePosition;
    }
    #endregion

    #region Functions
    public void CapsLock(string s)
    {
        if (fixedUpper) { fixedUpper = toUpper = false; capslock.DisabledHighlight(); }
        else if (toUpper) { fixedUpper = true; capslock.EnableHighlight(); }
        else toUpper = true;
    }
    public void Backspace(string s)
    {
        StringBuilder builder = new StringBuilder(inputField.getInputString());
        if (builder.ToString() == "..." || builder.ToString() == "") return;
        
        if (builder.ToString().Length - 1 >= 0)
        {
            builder.Remove(builder.Length - 1, 1);
            SetInputValue(builder.ToString());
        }

        if (builder.ToString() == "")
        {
            inputField.DisableContinueWriting();
            inputField.setInputText("...");
            inputField.CheckInputText();
            return;
        }

        inputField.ChangingInputsTexts(builder.ToString());
    }
    public void Clear(string s)
    {
        StringBuilder builder = new StringBuilder(inputField.getInputString());
        if (builder.ToString().Length - 1 >= 0 && builder.ToString() != "...")
        {
            builder.Clear();
            SetInputValue(builder.ToString());
        }

        inputField.setInputText("...");
        inputField.CheckInputText();
    }
    public void Typing(string s)
    {
        if (inputField == null) return;
        if (inputField.getInputString() == "...") inputField.setInputText("");
        
        waitingInput = false;
        LeanTween.cancel(inputField.gameObject);

        StringBuilder builder = new StringBuilder(inputField.getInputString());
        
        s = (toUpper && typeInput == TypeInputValue.String) ? s.ToUpper() : s;
        if(InputTextLimit(builder.ToString(), s)) builder.Append(s);

        //print(builder.ToString());
        inputField.ChangingInputsTexts(builder.ToString());

        if (toUpper && !fixedUpper) toUpper = false;
        SetInputValue(builder.ToString());
    }
    private void SetInputValue(string s)
    {
        StringBuilder builder = new StringBuilder(s);
        while (builder.ToString().Contains("_")) builder.Remove(builder.Length - 1, 1);

        s = builder.ToString();
        switch(typeInput)
        {
            case TypeInputValue.String:
                inputValue.stringValue = s;
                //print(inputValue.stringValue);
                return;
            case TypeInputValue.Int:
                int.TryParse(s, out inputValue.intValue);
                //print(inputValue.intValue);
                return;
            case TypeInputValue.Float:
                float.TryParse(s, out inputValue.floatValue);
                //print(inputValue.floatValue);
                return;
        }
    }
    #endregion

    private static void ChangeLettersSize(bool value)
    {
        if (letters == null) letters = _transform.GetChild(2).getChildsWithComponentT<KeyboardPad>();

        letters = letters.Where(t => t.getType() == KeyboardFunction.Write).ToList();

        if(value)
            foreach (UIButton ui in letters) ui.setLabel(ui.getLabel().ToUpper());
        else
            foreach (UIButton ui in letters) ui.setLabel(ui.getLabel().ToLower());
    }
    private bool InputTextLimit(string input, string toAppend)
    {
        var length = input.Length;

        if (typeInput == TypeInputValue.String)
        {
            if (length > 30) return false;
        }
        else
        {
            char c = toAppend[0];
            if (typeInput == TypeInputValue.Int)
            { if (length > 10 || !Char.IsDigit(c)) return false; }
            else
            { if (length > 12 || (!Char.IsDigit(c) && c != ',')) return false; }
        }

        if (input.Contains(","))
        {
            if (length - input.IndexOf(",") >= 3) return false;
            if (toAppend == "," && typeInput == TypeInputValue.Float) return false;
            if (typeInput == TypeInputValue.Int) return false;
        }
        return true;
    }


    [ContextMenu("Show Letters")]
    void ShowLetters()
    {
        foreach(UIButton ui in letters) ui.ChangeLabelFromEditor();
        foreach (UIButton ui in numbers) ui.ChangeLabelFromEditor();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Threading;

public class Keyboard : UIElement
{
    public static bool holding;
    private static bool toUpper = false;
    private static TypeInputValue typeInput;

    private static UITextInput inputField;
    private static InputValue inputValue;

    [SerializeField] UIElement iconsGroup;
    [SerializeField] List<UIButton> letters;
    [SerializeField] List<UIButton> numbers;

    public static bool leave;
    private static List<UIElement> linkedElements = new List<UIElement>();

    private static Keyboard _instance;
    public static Keyboard Instance
    {
        get
        {
            if (_instance == null) _instance = GameObject.Find("Teclado").GetComponent<Keyboard>();
            return _instance;
        }
    }

    private CancellationTokenSource tokenSource;

    public override void Init()
    {
        base.Init();
        tokenSource = new CancellationTokenSource();
    }

    public bool ShowKeyboard(TypeInputValue type, UITextInput _input, InputValue _inputValue, List<GameObject> elements)
    {
        //groupPrevValue = iconsGroup.getCanvasGroup().alpha;
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

    #region Functions
    public void CapsLock(string s)
    {
        toUpper = !toUpper;
        if (toUpper)
        {
            foreach (UIButton ui in letters) ui.setLabel(ui.getLabel().ToUpper());
        }
        else
        {
            foreach (UIButton ui in letters) ui.setLabel(ui.getLabel().ToLower());
        }
    }
    public void Backspace(string s)
    {
        StringBuilder builder = new StringBuilder(inputField.inputText.text);
        if (builder.ToString().Length - 1 >= 0 && builder.ToString() != "...")
        {
            if (builder.ToString().Contains("_")) builder.Remove(builder.Length - 2, 2);
            else builder.Remove(builder.Length - 1, 1);

            SetInputValue(builder.ToString());
        }

        if (builder.ToString() == "" || builder.ToString() == "...")
        {
            inputField.inputGhost.gameObject.SetActive(false);
            inputField.inputText.text = "...";
            inputField.WaitingInput();
            return;
        }

        inputField.ChangingInputsTexts(builder.ToString());
    }
    public void Typing(string s)
    {
        if (inputField == null) return;
        if (inputField.inputText.text == "...") inputField.inputText.text = "";

        StringBuilder builder = new StringBuilder(inputField.inputText.text);
        if (builder.ToString().Contains("_")) builder.Remove(builder.Length - 1, 1);

        s = (toUpper && typeInput == TypeInputValue.String) ? s.ToUpper() : s;
        if(InputTextLimit(builder.ToString(), s)) builder.Append(s);

        inputField.ChangingInputsTexts(builder.ToString());
        SetInputValue(builder.ToString());
        //inputField.inputText.text = builder.ToString();

        /*if (holding) return;
        else
        {
            s = "_";
            if (InputTextLimit(builder.ToString(), s)) builder.Append(s);
            inputField.inputText.text = builder.ToString();

            
        }*/
    }
    private void SetInputValue(string s)
    {
        StringBuilder builder = new StringBuilder(s);
        while (builder.ToString().Contains("_")) builder.Remove(builder.Length - 1, 1);

        s = builder.ToString();
        print(s);
        switch(typeInput)
        {
            case TypeInputValue.String:
                inputValue.stringValue = s;
                print(inputValue.stringValue);
                return;
            case TypeInputValue.Int:
                int.TryParse(s, out inputValue.intValue);
                print(inputValue.intValue);
                return;
            case TypeInputValue.Float:
                float.TryParse(s, out inputValue.floatValue);
                print(inputValue.floatValue);
                return;
        }
    }
    #endregion

    private bool InputTextLimit(string input, string toAppend)
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


    [ContextMenu("Show Letters")]
    void ShowLetters()
    {
        foreach(UIButton ui in letters) ui.ChangeLabel();
        foreach (UIButton ui in numbers) ui.ChangeLabel();
    }
}

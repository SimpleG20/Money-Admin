using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Enums;

public class KeyboardPad : UIButton, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public readonly FamilyUI family = FamilyUI.KEYBOARD;
    [SerializeField] protected KeyboardFunction function;
    [SerializeField] protected string textToWrite = "";

    private delegate void func(string s);

    private Dictionary<KeyboardFunction, func> functions;

    private IEnumerator Start()
    {
        SetLinkedObjs();

        yield return new WaitUntil(() => start);
        interactable = true;

        if (isSelected()) ChangeHighlights();
    }
    public override void Init()
    {
        base.Init();
        ObjectRef = gameObject;
        setFunctions();
    }
    public override void getUiButton()
    {
        return;
    }

    private void setFunctions()
    {
        func f1 = Keyboard.Instance.CapsLock;
        func f2 = Keyboard.Instance.Backspace;
        func f3 = Keyboard.Instance.Typing;
        func f4 = GameManager.Instance.LeaveInputSection;

        functions = new Dictionary<KeyboardFunction, func>()
            {   { KeyboardFunction.CapsLock, f1},
                { KeyboardFunction.Backspace, f2},
                { KeyboardFunction.Write, f3},
                { KeyboardFunction.Space, f3},
                { KeyboardFunction.Enter, f4}
            };
    }
    public async void Function()
    {
        if (functions == null) setFunctions();

        //functions[function](textToWrite);
        functions[function](textToWrite); 
        await Task.Delay(250, m_TokenSource.Token);
        if(m_TokenSource.IsCancellationRequested) return;

        if(numClicks > 1) { numClicks = 1; return; }

        if (Keyboard.holding) Function();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        numClicks++;
        /*if (function == KeyboardFunction.Enter || function == KeyboardFunction.CapsLock)
            Function();*/
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (function != KeyboardFunction.Enter && function != KeyboardFunction.CapsLock)
            Keyboard.holding = true;

        Selected();
        Transformations();
        Function();
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Unselected();
        Keyboard.holding = false;
    }


    public void setWrittenTex(string text) => textToWrite = text;
    public string getWrittenText() => textToWrite;
    public KeyboardFunction getType() => function;

    protected override IEnumerator WaitStopHolding()
    {
        yield return new WaitUntil(() => !Keyboard.holding);
        LeanTween.scale(gameObject, Vector2.one, 0.15f).setOnComplete(() => LeanTween.cancel(gameObject)); ;
    }

    public override string ChangeLabelFromEditor()
    {
        if (!hasLabel) return "";

        if (labelComponent == null) labelComponent = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        labelComponent.text = labelText;
        if (function == KeyboardFunction.Write) setWrittenTex(labelText);

        return labelComponent.text;
    }


    public override void GetParent()
    {
        throw new System.NotImplementedException();
    }
}

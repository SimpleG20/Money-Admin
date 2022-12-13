using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public abstract class UIButton : UIElement
{
    protected int numClicks;
    [SerializeField] protected bool toggle;
    [SerializeField] protected bool useRipple;
    [SerializeField] protected bool temporarilyHighlight;
    protected bool highlightEnabled;

    [SerializeField] protected GameObject highlightSource;
    [SerializeField] protected Image rippleSource;
    
    protected GameObject ObjectRef;
    protected AbstractButton m_Button;
    protected CancellationTokenSource m_TokenSource;


    public override void Init()
    {
        base.Init();
        SetLinkedObjs();

        m_TokenSource = new CancellationTokenSource();
    }
    public abstract void getUiButton();
    public void SetLinkedObjs()
    {
        if (linkedObjects == null) linkedObjects = new List<GameObject>();

        if (linkedObjects.Count == 0) linkedObjects.Add(gameObject);
    }

    protected void Transformations()
    {
        if (scaleOnClick)
        {
            if (Keyboard.holding) LeanTween.scale(gameObject, Vector2.one * scaleSize, 0.05f).setOnComplete(() => StartCoroutine(WaitStopHolding()));
            else LeanTween.scale(gameObject, Vector2.one * scaleSize, 0.15f).setOnComplete(BackNormalScale);
        }
        if (useRipple) SetRipple();

        ChangeHighlights();
    }
    protected void SetRipple()
    {
        rippleSource.transform.position = Input.mousePosition;
        ResetRipple();
        LeanTween.value(0, 1, 0.75f).setOnUpdate((value) =>
        {
            if (numClicks > 1) { numClicks = 1; return; }
            rippleSource.gameObject.transform.localScale = Vector3.one * value;
            var color = rippleSource.color;
            color.a = 1 - value;
            rippleSource.color = color;
        });
    }
    protected void ResetRipple()
    {
        var color = rippleSource.color;
        color.a = 0.8f;
        rippleSource.color = color;
        rippleSource.transform.localScale = Vector2.zero;
    }
    protected void BackNormalScale()
    {
        LeanTween.scale(ObjectRef, Vector2.one, 0.15f).setOnComplete(() => LeanTween.cancel(ObjectRef));
    }
    protected abstract IEnumerator WaitStopHolding();

    internal async void ChangeHighlights()
    {
        OthersHighlights();
        if (toggle)
        {
            if (highlightEnabled)
            {
                DisabledHighlight();
            }
            else EnableHighlight();
        }
        else
        {
            if (Keyboard.fixedUpper) return;

            EnableHighlight();
            await Task.Delay(500, m_TokenSource.Token);

            if (m_TokenSource.IsCancellationRequested) return;
            DisabledHighlight();
        }
    }
    internal void EnableHighlight()
    {
        if (highlightSource == null) return;
        highlightSource.SetActive(true);
        highlightEnabled = true;
    }
    internal void DisabledHighlight()
    {
        if (highlightSource == null) return;
        highlightSource.SetActive(false);
        highlightEnabled = false;
    }
    private void OthersHighlights()
    {
        foreach (GameObject button in linkedObjects)
        {
            if (button.GetComponent<UIButton>() == null) continue;

            if (button != gameObject) button.GetComponent<UIButton>().DisabledHighlight();
        }
    }


    [ContextMenu("Instantiate Ripple")]
    public void setRipple()
    {
        if (!useRipple) return;
        if (ObjectRef.GetComponent<Mask>() == null) ObjectRef.AddComponent<Mask>();

        var circle = Resources.Load<Texture2D>("Textures/Border/Radial/256px/Radial Filled 256px");
        var temp = Instantiate(new GameObject("Ripple"), ObjectRef.transform);
        temp.AddComponent<Image>();
        rippleSource = temp.GetComponent<Image>();
        rippleSource.sprite = Sprite.Create(circle, new Rect(0, 0, circle.width, circle.height), Vector2.one * 0.5f);
        rippleSource.rectTransform.sizeDelta = Vector2.one * ObjectRef.transform.GetComponent<RectTransform>().rect.width * 2;
        rippleSource.type = Image.Type.Simple;

        Color color;
        ColorUtility.TryParseHtmlString("#D9D9D9", out color);
        color.a = 0.8f;
        rippleSource.color = color;
        rippleSource.transform.localScale = Vector2.zero;
    }
    public abstract string ChangeLabelFromEditor();
    public void showLabel()
    {
        if (!hasLabel) return;

        labelComponent.gameObject.SetActive(true);
        labelComponent.text = labelText;
    }
    public abstract void GetParent();
}

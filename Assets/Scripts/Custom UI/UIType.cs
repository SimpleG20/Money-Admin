using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

public class UIType : UIButton, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Variables
    public readonly FamilyUI family = FamilyUI.GENERIC;
    public TypesUI typeUi;

    private delegate void clickFunction();
    private delegate void initFunction(GameObject g);
    private Dictionary<TypesUI, clickFunction> genericClick;
    private Dictionary<TypesUI, initFunction> initFunctions;
    #endregion

    #region Params
    [SerializeField] protected DestinationButton destinationBtParams = new DestinationButton();
    [SerializeField] protected PageButton pageBtParams = new PageButton();
    [SerializeField] protected HoldButton holdBtParams = new HoldButton();
    [SerializeField] protected UnityEvent clickEvent;
    #endregion

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => start);
        interactable = true;

        getUiButton();
        if (m_Button != null)
        {
            if (isSelected()) ChangeHighlights();
        }
    }
    public override void Init()
    {
        base.Init();
        setFunctions();
        ChangeLabelFromEditor();

        ObjectRef = gameObject;
        initFunctions[typeUi](gameObject);
        m_TokenSource = new System.Threading.CancellationTokenSource();
    }


    private void Update()
    {
        if (typeUi != TypesUI.HOLD) return;
        if (!selected || holdBtParams.getRelease()) return;

        holdBtParams.Function();
    }

    #region Get and Set
    public DestinationButton getDestinationParams() => destinationBtParams;
    public PageButton getPageParams() => pageBtParams;
    public HoldButton getHoldParams() => holdBtParams;
    public override void getUiButton()
    {
        switch (typeUi)
        {
            case TypesUI.DESTINATION:
                m_Button = destinationBtParams;
                return;
            case TypesUI.PAGE:
                m_Button = pageBtParams;
                return;
            case TypesUI.HOLD:
                m_Button = holdBtParams;
                return;
        }
    }
    private void setFunctions()
    {
        genericClick = new Dictionary<TypesUI, clickFunction> { { TypesUI.HOLD, holdBtParams.Function },
                                                                { TypesUI.DESTINATION, destinationBtParams.Function },
                                                                { TypesUI.PAGE, pageBtParams.Function},
                                                                { TypesUI.GENERIC, clickEvent.Invoke } };
        initFunctions = new Dictionary<TypesUI, initFunction> {  { TypesUI.HOLD, holdBtParams.Init },
                                                                { TypesUI.DESTINATION, destinationBtParams.Init },
                                                                { TypesUI.PAGE, pageBtParams.Init},
                                                                { TypesUI.GENERIC, Generic2} };
    }
    #endregion


    #region Click Events
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (!interactable || typeUi == TypesUI.HOLD) return;

        numClicks++;
        Transformations();

        genericClick[typeUi]();
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Selected();

        if (typeUi == TypesUI.HOLD) holdBtParams.setRelease(false);
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Unselected();

        if (typeUi == TypesUI.HOLD) holdBtParams.setRelease(true);
    }


    protected override IEnumerator WaitStopHolding()
    {
        yield return new WaitUntil(() => !Keyboard.holding);
        LeanTween.scale(gameObject, Vector2.one, 0.15f).setOnComplete(() => LeanTween.cancel(gameObject)); ;
    }
    #endregion


    #region ContextMenu
    [ContextMenu("Change Label")]
    public override string ChangeLabelFromEditor()
    {
        if (!hasLabel) return "";

        if (labelComponent == null) labelComponent = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        labelComponent.text = labelText;

        return labelComponent.text;
    }
    public override void GetParent()
    {

    }
    private void Generic2(GameObject g = default)
    {

    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public class UIElement : MonoBehaviour
{
    protected RectTransform rect;
    protected Animator animator;
    protected CanvasGroup canvasGroup;
    protected bool m_hasAnimator = true;
    protected bool start = false;

    public TextMeshProUGUI labelComponent;
    [SerializeField] protected bool hasLabel;
    [SerializeField] protected string labelText;

    [SerializeField] protected float scaleSize;
    [SerializeField] protected bool selected, scale, scaleOnHover, scaleOnClick;

    [SerializeField] protected bool _interactable = true;
    [SerializeField] protected bool interactable
    {
        get => _interactable;
        set => _interactable = InteractableVisualization(value);
    }

    public List<GameObject> linkedObjects;
    [HideInInspector] public bool showVariables;

    void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        if (GameManager.Instance.canSetInteractable) interactable = true;
    }

    public virtual void Init()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        m_hasAnimator = animator ?? false;
        start = true;

        gameObject.tag = "UI Element";
    }

    #region Get and Set
    public bool hasAnimator() => m_hasAnimator;
    public bool isInteractable() => interactable;
    public string getLabel() => labelText;
    public Animator getAnimator() => animator;
    public CanvasGroup getCanvasGroup() => canvasGroup;
    public void setInteractable(bool value) => interactable = value;
    public void setLabel(string label)
    {
        labelText = label;
        labelComponent.text = label;
    }
    #endregion

    #region Selection
    public void EnableElement() => gameObject.SetActive(true);
    public void DisableElement() => gameObject.SetActive(false);
    public virtual void Selected()
    {
        if (!interactable) return;
        selected = true;
    }
    public virtual void Unselected()
    {
        if(!interactable) return;
        selected = false;
    }
    public bool isSelected() => selected;
    #endregion

    #region Animation
    public void ShowUi()
    {
        EnableElement();
        animator.SetBool("Hide", false);
        animator.SetBool("Show", true);
    }
    public void HideUi()
    {
        animator.SetBool("Show", false);
        animator.SetBool("Hide", true);
    }
    private bool InteractableVisualization(bool value)
    {
        if (animator == null) return true;

        if (value == true) animator.SetBool("Interactable", true);
        else animator.SetBool("Interactable", false);
        return value;
    }

    private void AnimatorEnd()
    {
        animator.SetBool("Show", false);
        animator.SetBool("Hide", false);
    }
    #endregion
}

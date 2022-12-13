using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null) instance = FindObjectOfType<GameManager>();
            if(instance == null) instance = new GameObject().AddComponent<GameManager>();

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    #region Static
    public static Dictionary<int, string> Meses = new Dictionary<int, string> { { 1, "Janeiro" }, { 2, "Fevereiro" }, { 3, "Marco" }, { 4, "Abril" }, { 5, "Maio" },
        { 6, "Junho" }, { 7, "Julho" }, { 8, "Agosto" }, { 9, "Setembro" }, { 10, "Outubro" }, { 11, "Novembro" }, { 12, "Dezembro" } };
    #endregion

    [SerializeField] GameObject leaveInput;

    public bool variablesSet;
    public bool canSetInteractable;

    void Start()
    {
        Initialize();
        variablesSet = true;
    }

    public void Initialize()
    {
        Despesa.current.Initialize();
        DespesaUI.current.Initialize();
    }

    public async void LeaveInputSection(string s=default)
    {
        canSetInteractable = true;
        List<UIElement> elements = new List<UIElement>();
        foreach(GameObject element in GameObject.FindGameObjectsWithTag("UI Element")) 
            elements.Add(element.GetComponent<UIElement>()) ;

        foreach (UIElement e in elements)
            if (e.isSelected()) e.Unselected();

        Keyboard.Instance.HideKeyboard();

        await Task.Delay(500);
        leaveInput.SetActive(false);
    }

    public GameObject getLeaveInput() => leaveInput;
}

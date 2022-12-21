using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static Enums;

public class DespesaUI : MonoBehaviour
{
    #region Aux Variables
    public int monthsAvailables;
    public bool edited;
    private bool showingLimitPopUp, isReportScene;
    private CancellationTokenSource tokenSource;

    #region Consts
    private readonly Dictionary<int, string> Months = new Dictionary<int, string> { { 1, "Janeiro" },    { 2, "Fevereiro" }, { 3, "Março" },
                                                                                    { 4, "Abril"},       { 5, "Maio"},       { 6, "Junho"},
                                                                                    { 7, "Julho"},       { 8, "Agosto"},     { 9, "Setembro"},
                                                                                    { 10, "Outubro"},    { 11, "Novembro"},  {12, "Dezembro" } };
    private readonly Dictionary<int, string> MonthsAbv = new Dictionary<int, string> { { 1, "JAN" },      { 2, "FEV" },       { 3, "MAR" },
                                                                                        { 4, "ABR"},       { 5, "MAI"},        { 6, "JUN"},
                                                                                        { 7, "JUL"},       { 8, "AGO"},        { 9, "SET"},
                                                                                        { 10, "OUT"},      { 11, "NOV"},       {12, "DEZ" } };

    public  const string SAVED = "Mudanças Salvas";
    private const string SUCESSO = "Item adicionado";
    private const string ERRO_ESTOURO = "Limite estourado";
    private const string ERRO_MISSING_INFO = "Faltando Informações";
    public  const string ERRO_NOT_EXIST = "Não existe esse item";
    public  const string ERRO_LISTNULL = "Lista sem itens";
    public  const string ERRO_MONTH_MONEY = "Dinheiro insuficiente";
    #endregion

    #region Auxs
    private float _currentLimit;
    public float currentLimit
    {
        get => _currentLimit;
        set
        {
            if (value < 0) return;

            _currentLimit = Mathf.Round(value * 100f) / 100f;
            Despesa.current.setCurrentLimit(_currentLimit);
            creation_limitText.text = _currentLimit.ToString().MoneyFormatForString();
        }
    }

    public float limit
    {
        get => inputLimit.getInputValue().floatValue;
        set
        {
            if (value <= 0) return;
            value = Mathf.Round(value * 100f) / 100f;
            Despesa.current.setDefaultLimit(value);
            inputLimit.setPlaceholder(value.ToString(), TypeInputValue.Float, true);
        }
    }
    public float income
    {
        get => inputIncome.getInputValue().floatValue;
        set 
        {
            if(value == 0) return;
            value = Mathf.Round(value * 100f) / 100f;
            Despesa.current.setIncomePerMonth(value);
            inputIncome.setPlaceholder(value.ToString(), TypeInputValue.Float, true);
        } 
    }
    public float initMoney
    {
        get => inputStored.getInputValue().floatValue;
        set
        {
            if (value == 0) return;
            value = Mathf.Round(value * 100f) / 100f;
            Despesa.current.setInitMoney(value);
            inputStored.setPlaceholder(value.ToString(), TypeInputValue.Float, true);
        }
    }
    public float fees
    {
        get => inputFees.getInputValue().floatValue;
        set
        {
            if (value == 0) return;
            value = Mathf.Round(value * 100f) / 100f;
            Despesa.current.setFees(value);
            inputFees.setPlaceholder(value.ToString().CheckCommaSituation(), TypeInputValue.Float);
        }
    }
    public string searchedItem
    {
        get
        {
            var temp = inputSearcItem.getInputString();
            if (temp == "")
            {
                parentItems.DeleteChildren();
                auxTextSearchItem.gameObject.SetActive(true);
            }
            return temp;
        }
    }
    private string nameItem
    {
        get => creation_inputName.getInputValue().stringValue;
        set => creation_inputName.setPlaceholder(value.ToString(), TypeInputValue.String);
    }
    private float priceItem
    {
        get => creation_inputPrice.getInputValue().floatValue; 

        set
        {
            value = Mathf.Round(value * 100f) / 100f;
            creation_inputPrice.setPlaceholder(value.ToString(), TypeInputValue.Float, true);
        }
    }
    private int parcelsItem
    {
        get => creation_inputParcels.getInputValue().intValue; 
        set => creation_inputParcels.setPlaceholder(value.ToString(), TypeInputValue.Int);
    }
    private int initMonthItem
    {
        get => creation_tgInitMonth.PositionOfTheOnlyActived() + 1;
        set => creation_tgInitMonth[value].isOn = true;
    }
    private bool isMonthlyItem
    {
        get => creation_tgIsMonthly.isOn;
        set => creation_tgIsMonthly.isOn = value;
    }
    private bool showMonthlyPrice
    {
        get => creation_tgMonthlyPrice.isOn;
        set => creation_tgMonthlyPrice.isOn = value;
    }
    private bool testItem
    {
        get => creation_tgTest.isOn;
        set => creation_tgTest.isOn = value;
    }
    private int typeItem
    {
        get => creation_dpTypeItem.value;
        set => creation_dpTypeItem.value = value;
    }
    private int typeExpenseItem
    {
        get => creation_dpTypeExpense.value;
        set => creation_dpTypeExpense.value = value;
    }
    #endregion

    #endregion

    #region UI and some variables
    [SerializeField] TextMeshProUGUI currentLimitPopUp;

    [Header("Part 2")]
    #region
    public UITextInput                                              inputLimit;
    public UITextInput                                              inputIncome;
    public UITextInput                                              inputStored;
    public UITextInput                                              inputFees;
    public UITextInput                                              inputSearcItem;
    public UITextInput                                              inputAmountMonths;
    [SerializeField] TextMeshProUGUI                                auxTextSearchItem, totalExpenseTxt, totalSavedTxt;
    #endregion

    [Header("Part 3")]
    #region
    public           TextMeshProUGUI                                creation_limitText;
    [SerializeField] GameObject                                     creation_warningAdded;
    [SerializeField] UITextInput                                    creation_inputName;
    [SerializeField] UITextInput                                    creation_inputPrice, creation_inputParcels;
    [SerializeField] TMP_Dropdown                                   creation_dpTypeExpense, creation_dpTypeItem;
    [SerializeField] Toggle                                         creation_tgIsMonthly, creation_tgTest, creation_tgMonthlyPrice;
    [SerializeField] List<Toggle>                                   creation_tgInitMonth;
    #endregion

    [Header("Items")]
    #region
    [SerializeField] GameObject                                     obj_extraInfo;
    [SerializeField] GameObject                                     Prefab_item, Prefab_reportGasto, Prefab_reportExtra;
    [SerializeField] Transform                                      parentItems, parentReports;
    //private GameObject                                              obj_itemToEdit;
    #endregion

    [Header("Relatorio")]
    #region
    [SerializeField] ScrollRect                                     scrollRect;
    [SerializeField] TextMeshProUGUI                                monthText, monthlyCostTotal, monthlyCost, monthlySaved;
    [SerializeField] UIType                                         setaProx, setaAnt;
    private int                                                     paginaRelatorio = 0, reportMonth, reportYear;
    #endregion

    #endregion

    public static DespesaUI current;

    private void Awake()
    {
        current = this;
        tokenSource = new CancellationTokenSource();

        reportYear = DateTime.Now.Year;
        var currentMonth = DateTime.Now.Month;
        reportMonth = currentMonth;
    }
    public void Initialize()
    { 
        paginaRelatorio = 0;
        monthsAvailables = 0;

        creation_tgInitMonth[reportMonth - 1].isOn = true;
    }

    public void ChangeTextOfCalculationScene2(float expense, float saved)
    {
        totalExpenseTxt.text = expense.MoneyFormatForNumber();
        totalSavedTxt.text = saved.MoneyFormatForNumber();
    }
    public void CurrentLimitTextPopUp()
    {
        showingLimitPopUp = !showingLimitPopUp;
        currentLimitPopUp.text = currentLimit.MoneyFormatForNumber();

        UpdateLimitPopUpOnReport();
    }
    private void UpdateLimitPopUpOnReport()
    {
        if (!isReportScene) return;
        float current = currentLimit;
        currentLimitPopUp.text = (current + float.Parse(monthlyCostTotal.text.Substring(2))).MoneyFormatForNumber();
    }


    #region Function for Input
    public void UpdateInputsFromScene2(float _limit, float currentLimit, float _income, float _initMoney, float _fees)
    {
        print($"Limite: {limit = _limit}");
        print($"Limite Atual: {this.currentLimit = currentLimit}");
        print($"Renda: {income = _income}");
        print($"Inicial: {initMoney = _initMoney}");
        print($"Juros: {fees = _fees}");
    }
    public void ReasureInputs()
    {
        limit = limit;
        income = income;
        initMoney = initMoney;
        fees = fees;
    }
    private void UpdateInputsTextFromScene3(Item dados)
    {
        nameItem = dados.getName();
        parcelsItem = dados.getParcels();

        if (dados.getShowMonthlyPrice()) priceItem = dados.getMonthlyPrice();
        else priceItem = dados.getTotalPrice();

        typeExpenseItem = dados.getUseCreditCard() ? 0 : 1;
        typeItem = ((int)dados.getType());
        isMonthlyItem = dados.getIsMonthly();
        initMonthItem = dados.getInitMonth() - 1;

        creation_limitText.CheckCommaSituationTMPro();
    }
    private void ResetInputValuesFromScene3()
    {
        nameItem = "";
        creation_inputName.setInputText("");
        parcelsItem = 0;
        creation_inputParcels.setInputText("");
        priceItem = 0;
        creation_inputPrice.setInputText("");

        typeExpenseItem = 0;
        typeItem = 0;

        showMonthlyPrice = false;
        isMonthlyItem = false;
        showMonthlyPrice = false;
        initMonthItem = Despesa.current.getCurrentMonth() - 1;
        testItem = false;
    }
    public void DropDown()
    {
        if (typeItem == 1)
        {
            typeExpenseItem = 1;
            creation_dpTypeExpense.interactable = false;
        }
        else creation_dpTypeExpense.interactable = true;
    }
    #endregion

    #region Item

    #region Creation
    public void CreationItem()
    {
        Item item = CreateItem();
        if (item == null)
            return;

        Despesa.current.AddToList(item);

        UpdateReportMonthsRange(item.getInitMonth(), item.getParcels());

        ShowWarning(SUCESSO);

        ResetInputValuesFromScene3();
    }
    private void UpdateReportMonthsRange(int month, int parcels)
    {
        if (parcels > monthsAvailables)
        {
            if (month < Despesa.current.getCurrentMonth())
                monthsAvailables = (12 + parcels - Despesa.current.getCurrentMonth());
            else
                monthsAvailables = parcels;
        }
    }


    private Item CreateItem()
    {
        if (!IsItemInfoCorrect())
        {
            ShowWarning(ERRO_MISSING_INFO, 1500);
            return null;
        }

        int id;
        if (Despesa.current.idAvaliables.Count > 0)
        {
            id = Despesa.current.idAvaliables.PickRandom();
            Despesa.current.idAvaliables.Remove(id);
        }
        else
        {
            id = Despesa.current.getLastID();
            Despesa.current.IncreaseLastID();
        }
        Item item = new(nameItem, id, typeItem, testItem, showMonthlyPrice, isMonthlyItem, typeExpenseItem, priceItem, parcelsItem, initMonthItem, false);

        if (!item.DiscountInCurrentLimit())
        {
            ShowWarning(ERRO_ESTOURO);
            Despesa.current.DeacreaseLastID();
            return null;
        }

        if (!item.getIsTest())
        {
            Despesa.current.dataToSave.AdicionarLista(item);
            Salvar.SaveItems();
        }

        return item;
    }
    internal void ShowWarning(string warn, int time = 1000)
    {
        creation_warningAdded.GetComponentInChildren<TextMeshProUGUI>().text = warn;
        var posInit = creation_warningAdded.transform.position.x;

        LeanTween.moveX(creation_warningAdded, posInit - 640, 0.3f)
            .setOnComplete(() => DisableWarning(posInit, time));
    }
    private async void DisableWarning(float posInit, int time)
    {
        await Task.Delay(time, tokenSource.Token);
        if (tokenSource.IsCancellationRequested) return;

        LeanTween.moveX(creation_warningAdded, posInit, 0.3f)
            .setOnComplete(() => LeanTween.cancel(creation_warningAdded));
    }
    private bool IsItemInfoCorrect()
    {
        //print($"{nameItem}, {priceItem}, {parcelsItem}");
        if (nameItem == "" || priceItem == 0) return false;
        if (parcelsItem == 0) parcelsItem = 1;

        return true;
    }
    #endregion

    #region Edition
    bool decreasedLimit = false;
    Item dataFromItemToEdit;
    public void EditItem()
    {
        ScenesManagment.Instance.EditScene();
        dataFromItemToEdit = Despesa.current.editar.getData();
        Despesa.current.IncreaseCurrentLimit(dataFromItemToEdit.getTotalPrice());
        UpdateInputsTextFromScene3(dataFromItemToEdit);
    }
    public void ApllyEdition()
    {
        Item item = CreateItem();
        if (item == null)
        {
            ShowWarning(ERRO_ESTOURO);
            return;
        }

        if (decreasedLimit) Despesa.current.RemoveFromList(Despesa.current.getItems().FindLast(t => t.getId() == dataFromItemToEdit.getId()));
        else Despesa.current.getItems().Remove(Despesa.current.getItems().FindLast(t => t.getId() == dataFromItemToEdit.getId()));
        Despesa.current.AddToList(item);

        ShowWarning(SUCESSO);

        Despesa.current.editar.setData(item);
        decreasedLimit = true;
        edited = true;
        if (!item.getIsTest())
        {
            Despesa.current.dataToSave.AdicionarLista(item);
            Salvar.SaveItems();
        }
    }
    public void SairEdicao()
    {
        edited = false;
        ScenesManagment.Instance.LeaveEditScene();
        ResetInputValuesFromScene3();
        if (!decreasedLimit) Despesa.current.DecreaseCurrentLimit(dataFromItemToEdit.getTotalPrice());
        decreasedLimit = false;
        Despesa.current.editar = null;
    }
    #endregion

    #region Instantiation
    public void InstantiateSavedItens(List<Item> items)
    {
        parentItems.DeleteChildren();
        auxTextSearchItem.gameObject.SetActive(false);

        foreach (Item i in items)
        {
            var itemObj = Instantiate(Prefab_item, parentItems);
            itemObj.name = i.getName();
            Despesa.current.DecreaseCurrentLimit(i.getTotalPrice());
            itemObj.GetComponent<StoreItemData>().setData(i);
        }
    }
    public void InstantiateItemsForReport(Item item)
    {
        if (item == null)
        {
            print("Does not exist Items in this month");
            return;
        }

        GameObject instance;
        if (item.getType() == Item.TipoItem.DESPESA)
            instance = Instantiate(Prefab_reportGasto, parentReports);
        else
            instance = Instantiate(Prefab_reportExtra, parentReports);

        instance.GetComponent<StoreItemData>().Initiate(item);
        //print($"Item Instanciado: {item.getName()}");
    }
    #endregion

    #endregion

    #region Report
    public void InitReportScene()
    {
        isReportScene = true;
        
        setaAnt.Init();
        setaProx.Init();

        setaAnt.getPageParams().maxPages = monthsAvailables;
        setaProx.getPageParams().maxPages = monthsAvailables;

        setaProx.getPageParams().currentPage = paginaRelatorio;
        setaAnt.getPageParams().currentPage = paginaRelatorio;

        SetMonthlyPage();
    }
    public void LeaveReportScene()
    {
        isReportScene = false;
    }
    private void SetMonthlyPage()
    {
        var results = MonthReport();

        monthText.text = $"{Months[reportMonth]} - {reportYear}";

        monthlyCostTotal.text = results[0] > 0 ? results[0].ToString("F2").MoneyFormatForString() : "R$ 000,00";
        monthlyCost.text = results[1] > 0 ? results[1].ToString("F2").MoneyFormatForString() : "R$ 000,00";
        monthlySaved.text = results[2] > 0 ? results[2].ToString("F2").MoneyFormatForString() : "R$ 000,00";
        scrollRect.verticalScrollbar.value = 1;

        if (showingLimitPopUp) UpdateLimitPopUpOnReport();
    }
    private float[] MonthReport()
    {
        parentReports.DeleteChildren();

        float[] values;
        values = Despesa.current.CalculateExpenseUntill(paginaRelatorio, reportMonth, reportYear);

        return values;
    }
    public void NextMonth()
    {
        paginaRelatorio++;
        setaProx.getPageParams().currentPage = paginaRelatorio;
        setaAnt.getPageParams().currentPage = paginaRelatorio;

        reportMonth++;
        if(reportMonth == 13) 
        { 
            reportYear++;
            reportMonth = 1;
        }

        SetMonthlyPage();
    }
    public void PreviousMonth()
    {
        paginaRelatorio--;
        setaProx.getPageParams().currentPage = paginaRelatorio;
        setaAnt.getPageParams().currentPage = paginaRelatorio;

        reportMonth--;
        if(reportMonth == 0)
        {
            reportYear--;
            reportMonth = 12;
        }

        SetMonthlyPage();
    }
    
    public void ShowExtraInfo(Vector3 position, int month, bool credit, bool isTest, bool isMonthly)
    {
        var percentX = position.x / Screen.width;
        var percentY = position.y / Screen.height;
        obj_extraInfo.GetComponent<RectTransform>().anchorMax = new Vector2(percentX, percentY);
        obj_extraInfo.GetComponent<RectTransform>().anchorMin = new Vector2(percentX, percentY);
        obj_extraInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Inicio: {MonthsAbv[month]}";

        #region Use Credit Card
        obj_extraInfo.transform.GetChild(1).GetComponent<Toggle>().isOn = !credit;
        var color = obj_extraInfo.transform.GetChild(1).GetComponent<Image>().color;
        color.a = credit ? 1 : 0.5f;
        obj_extraInfo.transform.GetChild(1).GetComponent<Image>().color = color;
        #endregion

        #region Is Test Item
        obj_extraInfo.transform.GetChild(2).GetComponent<Toggle>().isOn = !isTest;
        color = obj_extraInfo.transform.GetChild(2).GetComponent<Image>().color;
        color.a = !isTest ? 1 : 0.5f;
        obj_extraInfo.transform.GetChild(2).GetComponent<Image>().color = color;
        #endregion

        #region Is Monthly
        obj_extraInfo.transform.GetChild(3).GetComponent<Toggle>().isOn = !isMonthly;
        color = obj_extraInfo.transform.GetChild(3).GetComponent<Image>().color;
        color.a = isMonthly ? 1 : 0.5f;
        obj_extraInfo.transform.GetChild(3).GetComponent<Image>().color = color;
        #endregion

        obj_extraInfo.GetComponent<UIElement>().ShowUi();
    }
    #endregion
}

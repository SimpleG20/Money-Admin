using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DespesaUI : MonoBehaviour
{
    #region Aux Variables
    private readonly Dictionary<int, string> Months = new Dictionary<int, string> { { 1, "Janeiro" },    { 2, "Fevereiro" }, { 3, "Março" },
                                                                           { 4, "Abril"},       { 5, "Maio"},       { 6, "Junho"},
                                                                           { 7, "Julho"},       { 8, "Agosto"},     { 9, "Setembro"},
                                                                           { 10, "Outubro"},    { 11, "Novembro"},  {12, "Dezembro" } };
    private readonly Dictionary<int, string> MonthsAbv = new Dictionary<int, string> { { 1, "JAN" },      { 2, "FEV" },       { 3, "MAR" },
                                                                             { 4, "ABR"},       { 5, "MAI"},        { 6, "JUN"},
                                                                             { 7, "JUL"},       { 8, "AGO"},        { 9, "SET"},
                                                                             { 10, "OUT"},      { 11, "NOV"},       {12, "DEZ" } };

    int monthsAvailables;
    bool edited;

    private float limit
    {
        get => inputLimit.getInputValue().floatValue;
        set => inputLimit.setPlaceholder(value.ToString());
    }
    private float income
    {
        get => inputIncome.getInputValue().floatValue;
        set => inputIncome.setPlaceholder(value.ToString());
    }
    private float stored
    {
        get => inputStored.getInputValue().floatValue;
        set => inputStored.setPlaceholder(value.ToString());
    }
    private float fees
    {
        get => inputFees.getInputValue().floatValue;
        set => inputFees.setPlaceholder(value.ToString());
    }
    private string nameItem
    {
        get => creation_inputName.getInputValue().stringValue;
        set => creation_inputName.setPlaceholder(value.ToString());
    }
    private float priceItem
    {
        get => creation_inputPrice.getInputValue().floatValue; 
        set => creation_inputPrice.setPlaceholder(value.ToString());
    }
    private int parcelsItem
    {
        get => creation_inputParcels.getInputValue().intValue; 
        set => creation_inputParcels.setPlaceholder(value.ToString());
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
        set => creation_tgMonthlyPrice.isOn = true;
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

    #region UI and some variables

    [Header("Creation Part")]
    #region
    public UITextInput                                              inputLimit;
    public UITextInput                                              inputIncome;
    public UITextInput                                              inputStored;
    public UITextInput                                              inputFees;
    [SerializeField] TextMeshProUGUI                                totalExpenseTxt, totalSavedTxt;
    #endregion

    [Header("Creation Part")]
    #region
    [SerializeField] TextMeshProUGUI                                creation_limitText;
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
    private GameObject                                              obj_itemToEdit;
    #endregion

    [Header("Relatorio")]
    #region
    [SerializeField] ScrollRect                                     scrollRect;
    [SerializeField] TextMeshProUGUI                                monthText, monthlyCost, monthlySaved;
    [SerializeField] UIType                                         setaProx, setaAnt;
    private int                                                     paginaRelatorio = 0, reportMonth, reportYear;
    #endregion

    #endregion

    public static DespesaUI current;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    { 
        var currentMonth = DateTime.Now.Month;
        reportMonth = currentMonth;
        reportYear = DateTime.Now.Year;
        Despesa.current.setCurrentMonth(currentMonth);

        creation_tgInitMonth[currentMonth - 1].isOn = true;

        paginaRelatorio = 0;
        reportMonth = 0;
        reportYear = DateTime.Now.Year;
    }

    #region Function for Input
    public void InicializarInputs()
    {
        inputLimit.setPlaceholderUpdated(Despesa.current.getLimit().ToString());
        inputIncome.setPlaceholderUpdated(Despesa.current.getIncomePerMonth().ToString());
        inputStored.setPlaceholderUpdated(Despesa.current.getInitialMoney().ToString());
    }
    private void UpdateInputsText(Item dados)
    {
        nameItem = dados.getName();
        parcelsItem = dados.getParcels();

        if (dados.getShowMonthlyPrice()) priceItem = dados.getMonthlyPrice();
        else priceItem = dados.getTotalPrice();

        typeExpenseItem = dados.getUseCreditCard() ? 0 : 1;
        typeItem = ((int)dados.getType());
        isMonthlyItem = dados.getIsMonthly();
        initMonthItem = dados.getInitMonth() - 1;

        creation_limitText.text = $"R$ {Despesa.current.getCurrentLimit()}";
        creation_limitText.CheckCommaSituation();
    }
    private void ResetInputValues()
    {
        creation_inputName.Default();
        creation_inputParcels.Default();
        creation_inputPrice.Default();

        typeExpenseItem = 0;
        typeItem = 0;

        isMonthlyItem = false;
        testItem = true;
        initMonthItem = Despesa.current.getCurrentMonth() - 1;
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
        if(item == null)
        {
            print("COMPRA INDISPONIVEL! LIMITE ESTOURADO"); 
            return;
        }
        Despesa.current.AddToList(item);

        LeanTween.moveLocalY(creation_warningAdded, -50, 1.5f).setLoopPingPong(1).setOnComplete(() => LeanTween.cancel(creation_warningAdded));
        ResetInputValues();
    }
    private Item CreateItem()
    {
        Item item = new Item(nameItem, typeItem, testItem, showMonthlyPrice, isMonthlyItem, typeExpenseItem, priceItem, parcelsItem, initMonthItem);
        item.DiscountInCurrentLimit();

        if (!item.getIsTest())
        {
            Despesa.current.listaSalvos.AdicionarLista(Despesa.current.listaSalvos, item);
            Salvar.SalvarDados(Despesa.current.listaSalvos);
        }

        return item;
    }
    #endregion

    #region Edition
    public void EditarItem(bool b)
    {
        obj_itemToEdit.SetActive(b);
        if (b == false)
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "Item";

            if (!edited)
            {
                Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;
                dados.DiscountInCurrentLimit();
                //AtualizarLimite();
            }
            ResetInputValues();
        }
        else
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "";
            Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;
            
            dados.RemoveFromLimit();
            UpdateInputsText(dados);
            //AtualizarLimite();
        }
    }
    public void AplicarEdicaoItem()
    {
        Item item = CreateItem();
        if (item == null)
        {
            print("COMPRA INDISPONIVEL! LIMITE ESTOURADO");
            return;
        }
        Despesa.current.AddToList(item);

        LeanTween.moveLocalY(creation_warningAdded, -50, 1.5f).setLoopPingPong(1).setOnComplete(() => LeanTween.cancel(creation_warningAdded));

        Despesa.current.editar.GetComponent<ItemDados>().dados = item;
        Despesa.current.editar.GetComponent<ItemDados>().Setar();
        //AtualizarLimite();
        edited = true;
    }
    public void EditarSalvos(bool b)
    {
        obj_itemToEdit.SetActive(b);
        if (b == true)
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "";

            Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;
            
            UpdateInputsText(dados);
        }
        else
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "Item";
            
            ResetInputValues();
            edited = false;
        }
    }
    public void SairEdicao()
    {
        Despesa.current.editar = null;
    }
    #endregion

    #region Instantiation
    public void InstantiateSavedItens()
    {
        parentItems.DeleteChildren();

        foreach (Item i in Despesa.current.getItems())
        {
            var item = Instantiate(Prefab_item, parentItems);
            item.name = i.getName();
            Despesa.current.UpdateLimitValue(i.getTotalPrice());
            item.GetComponent<ItemDados>().dados = i;
        }
    }
    private void InstantiateItemsForReport()
    {
        var result = Despesa.current.getItems().Where(t => t.getInitMonth() == reportMonth && t.getYear() == reportYear).ToArray();


        for (int i = 0; i < result.Length; i++)
        {
            GameObject item;
            if (result[i].getType() == Item.TipoItem.DESPESA)
                item = Instantiate(Prefab_reportGasto, parentReports);
            else
                item = Instantiate(Prefab_reportExtra, parentReports);

            item.GetComponent<StoreItemData>().Initiate(result[i]);
        }
    }
    #endregion

    #endregion

    #region Report
    public void InitReportScene()
    {
        SetMonthlyPage();

        if (paginaRelatorio + 1 == monthsAvailables) setaProx.gameObject.SetActive(false);
        else if (paginaRelatorio == 0) setaAnt.gameObject.SetActive(false);
        else
        {
            setaAnt.gameObject.SetActive(true);
            setaProx.gameObject.SetActive(true);
        }
    }
    public void SetMonthlyPage()
    {
        var results = MonthReport();

        monthText.text = Months[reportMonth] + "-" + reportYear.ToString();
        monthlyCost.text = results[0].ToString().MoneyFormat();
        monthlySaved.text = results[1].ToString().MoneyFormat();
        scrollRect.verticalScrollbar.value = 0;
    }
    private float[] MonthReport()
    {
        float[] values = new float[2];

        parentReports.localPosition = new Vector3(0, 0, 0);
        parentReports.GetComponent<RectTransform>().sizeDelta = new Vector2(750, 800);
        parentReports.DeleteChildren();

        InstantiateItemsForReport();
        values = Despesa.current.CalculateExpenseUntill(paginaRelatorio);

        return values;
    }
    public void NextMonth()
    {
        paginaRelatorio++;
        reportMonth++;
        if(reportMonth == 13) 
        { 
            reportYear++;
            reportMonth = 1;
        }

        SetMonthlyPage();

        if (paginaRelatorio + 1 == monthsAvailables) setaProx.gameObject.SetActive(false);
        else if (paginaRelatorio == 0) setaAnt.gameObject.SetActive(false);
        else
        {
            setaAnt.gameObject.SetActive(true);
            setaProx.gameObject.SetActive(true);
        }
    }
    public void PreviousMonth()
    {
        paginaRelatorio--;
        reportMonth--;
        if(reportMonth == -1)
        {
            reportYear--;
            reportMonth = 12;
        }

        SetMonthlyPage();

        if (paginaRelatorio + 1 == monthsAvailables) setaProx.gameObject.SetActive(false);
        else if (paginaRelatorio == 0) setaAnt.gameObject.SetActive(false);
        else
        {
            setaAnt.gameObject.SetActive(true);
            setaProx.gameObject.SetActive(true);
        }
    }
    public void LeaveReport()
    {
        ResetInputValues();
    }
    public void ShowExtraInfo(Vector3 position, int month, bool credit, bool isTest, bool isMonthly)
    {
        obj_extraInfo.transform.position = position;
        obj_extraInfo.SetActive(true);

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
    }
    #endregion
}

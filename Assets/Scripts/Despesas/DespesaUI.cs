using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEditor.PlayerSettings;

public class DespesaUI : MonoBehaviour
{
    private static DespesaUI _current;
    public static DespesaUI current
    {
        get {
            if (_current == null) _current = FindObjectOfType<DespesaUI>();
            return _current;
        } 
    }

    private Dictionary<int, string> Months = new Dictionary<int, string> { { 1, "Janeiro" },    { 2, "Fevereiro" }, { 3, "Março" },
                                                                           { 4, "Abril"},       { 5, "Maio"},       { 6, "Junho"},
                                                                           { 7, "Julho"},       { 8, "Agosto"},     { 9, "Setembro"},
                                                                           { 10, "Outubro"},    { 11, "Novembro"},  {12, "Dezembro" } };

    int currentMonth = 1, monthsAvailables;
    bool edited;

    public UITextInput                                              inputLimit;
    public UITextInput                                              inputIncome;
    public UITextInput                                              inputStored;
    public UITextInput                                              inputFees;
    [SerializeField] TextMeshProUGUI                                totalExpenseTxt, totalSavedTxt;

    [Header("Creation Part")]
    [SerializeField] GameObject                                     creation_warningAdded;
    [SerializeField] UITextInput                                    creation_inputName;
    [SerializeField] UITextInput                                    creation_inputPrice, creation_inputParcels;
    [SerializeField] Toggle                                         creation_tgIsMonthly, creation_tgTest, creation_tgMonthlyPrice;
    [SerializeField] List<Toggle>                                   creation_tgInitMonth;
    [SerializeField] TMP_Dropdown                                   creation_dpTypeExpense, creation_dpTypeItem;
    [SerializeField] TextMeshProUGUI                                creation_limitText;

    [Header("Item")]
    [SerializeField] GameObject                                     obj_extraInfo;
    private GameObject                                              obj_itemToEdit;


    [SerializeField] GameObject                                     Prefab_item, Prefab_reportGasto, Prefab_reportExtra;
    [SerializeField] Transform                                      parentItems, parentReports;

    [Header("Relatorio")]
    [SerializeField] ScrollRect                                     scrollRect;
    [SerializeField] TextMeshProUGUI                                monthText, monthlyCost, monthlySaved;
    [SerializeField] PageButton                                     setaProx, setaAnt;
    private int                                                     paginaRelatorio = 0, reportMonth, reportYear;


    [Header("Variaveis input")]
    int mesComeca;
    int parcelas;
    float valorMensal;
    bool DespesaCartao;
    bool podeCancelar;
    string nome;
    Item.Tipo tipo;

    public void Initialize()
    {
        var currentMonth = DateTime.Now.Month;
        this.currentMonth = currentMonth;
        reportMonth = currentMonth;
        reportYear = DateTime.Now.Year;
        Despesa.current.setCurrentMonth(currentMonth);

        creation_tgInitMonth[currentMonth - 1].isOn = true;

        paginaRelatorio = 0;
    }

    public void InicializarInputs()
    {
        inputLimit.setPlaceholderUpdated(Despesa.current.getLimit().ToString());
        inputIncome.setPlaceholderUpdated(Despesa.current.getIncomePerMonth().ToString());
        inputStored.setPlaceholderUpdated(Despesa.current.getInitialMoney().ToString());
    }
    public void DropDown()
    {
        if (creation_dpTypeItem.value == 1)
        {
            creation_dpTypeExpense.value = 1;
            creation_dpTypeExpense.interactable = false;
        }
        else creation_dpTypeExpense.interactable = true;
    }


    public void CriacaoItem()
    {
        PegarInputs();

        if (!podeCancelar) valorMensal /= parcelas;

        if (parcelas * valorMensal > Despesa.current.getCurrentLimit() && podeCancelar == false && DespesaCartao && tipo == Item.Tipo.DESPESA)
        { print("COMPRA INDISPONIVEL! LIMITE ESTOURADO"); return; }

        Item item = CriarItem();
        Despesa.current.ItemsList.Add(item);

        LeanTween.moveLocalY(creation_warningAdded, -50, 1.5f).setLoopPingPong(1).setOnComplete(() => LeanTween.cancel(creation_warningAdded));
        ResetarInputs();
    }

    private Item CriarItem()
    {
        float price = creation_inputPrice.getInputValue().floatValue * creation_inputParcels.getInputValue().floatValue;
        if (price > Despesa.current.getCurrentLimit())
        {
            //show warning
            return null;
        }

        Item item = ScriptableObject.CreateInstance<Item>();
        item.name = nome;
        item.nome = nome;
        item.tipo = tipo;
        item.parcelas = parcelas;
        if (mesComeca < Despesa.current.getCurrentMonth()) item.mesComeca = Despesa.current.getCurrentMonth() + (12 - Despesa.current.getCurrentMonth()) + mesComeca;
        else item.mesComeca = mesComeca;

        item.valorTotal = price;
        item.valorMensal = valorMensal;
        item.cartao = DespesaCartao;
        item.mensal = podeCancelar;
        item.name = item.nome;

        if (podeCancelar == false && DespesaCartao && tipo == Item.Tipo.DESPESA) { Despesa.current.DecreaseLimit(valorMensal * parcelas); } //AtualizarLimite(); }

        Despesa.current.listaSalvos.AdicionarLista(Despesa.current.listaSalvos, item); 
        Salvar.SalvarDados(Despesa.current.listaSalvos);

        return item;
    }
    private void PegarInputs()
    {
        nome = creation_inputName.getInputValue().stringValue;
        valorMensal = creation_inputPrice.getInputValue().floatValue;
        parcelas = creation_inputParcels.getInputValue().intValue;
        mesComeca = currentMonth;

        tipo = creation_dpTypeItem.value == 0 ? Item.Tipo.DESPESA : Item.Tipo.EXTRA;
        DespesaCartao = creation_dpTypeExpense.value == 0 ? true : false;
        podeCancelar = creation_tgIsMonthly.isOn ? true : false;
    }

    public void EditarItem(bool b)
    {
        obj_itemToEdit.SetActive(b);
        if (b == false)
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "Item";

            if (!edited)
            {
                Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;
                if (dados.mensal == false && dados.cartao && dados.tipo == Item.Tipo.DESPESA) Despesa.current.DecreaseLimit(dados.valorMensal * dados.parcelas);
                //AtualizarLimite();
            }
            ResetarInputs();
        }
        else
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "";
            Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;
            
            UpdateInputsText(dados);

            if (dados.mensal == false && dados.cartao && dados.tipo == Item.Tipo.DESPESA) Despesa.current.IncreaseLimit(dados.valorMensal * dados.parcelas);
            //AtualizarLimite();
        }
    }

    private void UpdateInputsText(Item dados)
    {
        creation_inputName.placeholder.text = dados.nome;
        creation_inputParcels.placeholder.text = dados.parcelas.ToString();
        if (!dados.mensal && dados.cartao) creation_inputPrice.placeholder.text = (dados.valorMensal * dados.parcelas).ToString();
        else creation_inputPrice.placeholder.text = dados.valorMensal.ToString();
        creation_dpTypeExpense.value = dados.cartao ? 0 : 1;
        creation_dpTypeItem.value = ((int)dados.tipo);
        creation_tgIsMonthly.isOn = dados.mensal;
        creation_tgInitMonth[dados.mesComeca - 1].isOn = true;
    }

    public void AplicarEdicaoItem()
    {
        PegarInputs();

        if (!podeCancelar) valorMensal /= parcelas;

        if (parcelas * valorMensal > Despesa.current.getCurrentLimit() && podeCancelar == false && DespesaCartao && tipo == Item.Tipo.DESPESA)
        { print("EDICAO INDISPONIVEL! LIMITE ESTOURADO"); return; }

        /*---------------------------------------------------------------------------------*/
        Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;

        dados.nome = nome;
        dados.valorMensal = valorMensal;
        dados.parcelas = parcelas;
        if (mesComeca < Despesa.current.getCurrentMonth()) dados.mesComeca = Despesa.current.getCurrentMonth() + (12 - Despesa.current.getCurrentMonth()) + mesComeca;
        else dados.mesComeca = mesComeca;
        dados.tipo = tipo;
        dados.cartao = DespesaCartao;
        dados.mensal = podeCancelar;

        Despesa.current.listaSalvos.AdicionarLista(Despesa.current.listaSalvos, dados); Salvar.SalvarDados(Despesa.current.listaSalvos);

        if (!dados.mensal && dados.cartao && dados.tipo == Item.Tipo.DESPESA) Despesa.current.DecreaseLimit(dados.valorMensal * dados.parcelas);

        Despesa.current.editar.GetComponent<ItemDados>().dados = dados;
        Despesa.current.editar.GetComponent<ItemDados>().Setar();
        //AtualizarLimite();
        edited = true;
    }
    public void SairEdicao()
    {
        Despesa.current.editar = null;
    }


    public void InstantiateSavedItens()
    {
        parentItems.DeleteChildren();

        foreach (Item i in Despesa.current.ItemsList)
        {
            var item = Instantiate(Prefab_item, parentItems);
            item.name = i.nome;
            Despesa.current.UpdateLimitValue(i.valorMensal * i.parcelas);
            item.GetComponent<ItemDados>().dados = i;
        }
    }
    public void EditarSalvos(bool b)
    {
        obj_itemToEdit.SetActive(b);
        if (b == true)
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "";

            Item dados = Despesa.current.editar.GetComponent<ItemDados>().dados;
            
            UpdateInputsText(dados);
            /*nome_input.placeholder.text = dados.nome;
            parcelas_input.placeholder.text = dados.Parcelas.ToString();

            if (!dados.mensal && dados.cartao) valor_input.placeholder.text = (dados.valorMensal * dados.Parcelas).ToString();
            else valor_input.placeholder.text = dados.valorMensal.ToString();

            tipoDespesa_input.value = dados.cartao ? 0 : 1;
            tipoItem_input.value = ((int)dados.tipo);
            mensal_toggle.isOn = dados.mensal;
            mesesComeca_toggle[dados.mesComeca - 1].isOn = true;*/
        }
        else
        {
            GameObject.Find("Item_txt").GetComponent<TextMeshProUGUI>().text = "Item";
            
            ResetarInputs();
            edited = false;
        }
    }

    public void InstantiateReportItems(Item dados, bool test)
    {
        GameObject item;
        if(dados.tipo == Item.Tipo.DESPESA) item = Instantiate(Prefab_reportGasto, parentReports);
        else item = Instantiate(Prefab_reportExtra, parentReports);
        
        dados.test = test;
        item.GetComponent<ItemDados>().dados = dados;
    }

    private void ResetarInputs()
    {
        creation_inputName.Default();
        creation_inputParcels.Default();
        creation_inputPrice.Default();

        creation_dpTypeExpense.value = 0;
        creation_dpTypeItem.value = 0;

        creation_tgIsMonthly.isOn = false;
        creation_tgTest.isOn = true;
        creation_tgInitMonth[Despesa.current.getCurrentMonth() - 1].isOn = true;
    }


    public void MudarMesQueComeca(int i)
    {
        currentMonth = i;
    }
    public void MudarMesAtual(int i)
    {
        Despesa.current.setCurrentMonth(i);
    }


    private string[] MonthReport()
    {
        string[] result = new string[2];
        parentReports.localPosition = new Vector3(0, 0, 0);
        parentReports.GetComponent<RectTransform>().sizeDelta = new Vector2(750, 800);
        parentReports.DeleteChildren();

        return result;
    }
    public void NextMonth()
    {
        if (paginaRelatorio + 1 == monthsAvailables) return;

        paginaRelatorio++;
        reportMonth++;
        if(reportMonth == 13) 
        { 
            reportYear++;
            reportMonth = 1;
        }

        setaProx.currentPage = paginaRelatorio;
        setaAnt.currentPage = paginaRelatorio;

        SetarPaginaMes();
    }
    public void PreviousMonth()
    {
        if (paginaRelatorio - 1 < 0) return;

        paginaRelatorio--;

        setaProx.currentPage = paginaRelatorio;
        setaAnt.currentPage = paginaRelatorio;

    }
    public void SetarPaginaMes()
    {
        var results = MonthReport();

        monthText.text = Months[reportMonth] + "-" + reportYear.ToString();
        monthlyCost.text = results[0];
        monthlySaved.text = results[1];
        scrollRect.verticalScrollbar.value = 0;
    }
    public void SairRelatorio()
    {
        ResetarInputs();
    }
}

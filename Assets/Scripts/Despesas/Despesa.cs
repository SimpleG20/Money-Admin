using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class Despesa : MonoBehaviour
{
    public ListaSalvos listaSalvos;

    [Header("Inputs")]
    float limit;
    float currentLimit;
    float initialMoney;
    float incomePerMonth;
    float fees;
    int amountMonths;
    int currentMonth;

    [Header("Resultados")]
    float dinheiroPoupanca;
    float despesasCartao;
    float despesasNaoCartao;
    float despesaTotal;
    string mesFinal;

    [Header("Lista de Despesas")]
    public List<Item> ItemsList;

    [Header("Editar Item")]
    public GameObject editar;

    private static Despesa _current;
    public static Despesa current
    {
        get
        {
            if (_current == null) _current = FindObjectOfType<Despesa>();
            return _current;
        }
    }

    #region Get
    public float getLimit() => limit;
    public float getCurrentLimit() => currentLimit;
    public float getInitialMoney() => initialMoney;
    public float getIncomePerMonth() => incomePerMonth;
    public float getFees() => fees;
    public int getAmountMonths() => amountMonths;
    public int getCurrentMonth() => currentMonth;
    public float getTotalExpense() => despesaTotal;
    public float getCreditExpense() => despesasCartao;
    public float getMoneyExpense() => despesasNaoCartao;
    public float getSavedMoney() => dinheiroPoupanca;
    public string getMesFinal() => mesFinal;
    #endregion
    #region Set
    public void setInitialMoney(float value) => initialMoney = value;
    public void setIncomePerMonth(float value) => incomePerMonth = value;
    public void setCurrentMonth(int value) => currentMonth = value;
    public void setLimit(float value)
    {
        limit = value;
        currentLimit = value;
    }
    public void setCurrentLimit(float value) => currentLimit = value;
    public void DecreaseLimit(float value) => currentLimit -= value;
    public void IncreaseLimit(float value) => currentLimit += value;
    #endregion

    public void Initialize()
    {
        currentMonth = 1;
        amountMonths = 1;
        listaSalvos = new ListaSalvos();
        listaSalvos.itens = new List<Item>();
        listaSalvos.tamanhoMatriz = listaSalvos.itens.Count;

        /*Salvar.SalvarLista(listaSalvos);*/
        Salvar.CarregarDados();
        DespesaUI.current.InicializarInputs();

        print(listaSalvos.itens.Count);
        //foreach (Item item in listaSalvos.itens) DespesaUI.current.InstanciarItemPrefab(0, item, true, listaItemsSalvos_ob.transform);
    }

    public void UpdateInitialInputs()
    {
        UpdateLimitInput();
        UpdateIncomeInput();
        UpdateStoredInput();
        UpdateFeesInput();
        SaveInputs();
    }
    void UpdateLimitInput()
    {
        limit = DespesaUI.current.inputLimit.getInputValue().floatValue;
        if (limit == 0) return;
        DespesaUI.current.inputLimit.setPlaceholderUpdated(limit.ToString());
    }
    void UpdateIncomeInput()
    {
        incomePerMonth = DespesaUI.current.inputIncome.getInputValue().floatValue;
        if (incomePerMonth == 0) return;
        DespesaUI.current.inputIncome.setPlaceholderUpdated(incomePerMonth.ToString());
    }
    void UpdateFeesInput()
    {
        fees = DespesaUI.current.inputFees.getInputValue().floatValue;
        if(fees == 0) fees = 1;
        DespesaUI.current.inputFees.setPlaceholderUpdated(fees.ToString());
    }
    void UpdateStoredInput()
    {
        initialMoney = DespesaUI.current.inputStored.getInputValue().floatValue;
        if (initialMoney == 0) return;
        DespesaUI.current.inputStored.setPlaceholderUpdated(initialMoney.ToString());
    }

    public void SearchForSomething()
    {

    }
    public void ShowExpenseDescriptionsSearched()
    {

    }
    public void ShowMoreAbout()
    {

    }

    public void AddOtherExpense()
    {

    }
    public void CalculateTotalExpenses()
    {

    }
    public void RefreshTotalExpenses()
    {

    }

    public void UpdateLimitValue(float value)
    {
        limit -= value;
    }

    void SaveInputs()
    {
        listaSalvos.limite = limit;
        listaSalvos.dinheiroInicial = initialMoney;
        listaSalvos.renda = incomePerMonth;
        Salvar.SalvarDados(listaSalvos);
    }

    public void CalcularDespesa2()
    {
        if (ItemsList == null) return;

        float gastoDinheiro = 0;
        float gastoCartao = 0;
        float guardado = initialMoney;

        for (int i = 0; i <= amountMonths; i++)
        {
            float gastoMensal = 0;
            int id = 1;

            /*var novoMes = DespesaUI.current.mes_prefab;
            Instantiate(novoMes, listaRelatorio_ob.transform);
            novoMes = listaRelatorio_ob.transform.GetChild(i).gameObject;

            foreach (Item item in ItemsList)
            {
                GastoPorItem(ref guardado, ref gastoDinheiro, ref gastoCartao, i, ref gastoMensal, ref id, item);
            }
            guardado += (incomePerMonth - gastoMensal);

            int mes = Mathf.FloorToInt((currentMonth + i) % 12) == 0 ? 12 : Mathf.FloorToInt((currentMonth + i) % 12);
            novoMes.name = GameManager.Meses[mes];
            novoMes.GetComponent<MesDados>().nome = novoMes.name;
            novoMes.GetComponent<MesDados>().gasto = gastoMensal;
            novoMes.GetComponent<MesDados>().sobra = guardado;
            novoMes.transform.localPosition = Vector3.zero;
            mesesRelatorio.Add(novoMes.gameObject);

            novoMes.gameObject.GetComponent<CanvasGroup>().alpha = 0;*/
        }

        dinheiroPoupanca = guardado;
        despesasCartao = gastoCartao;
        despesasNaoCartao = gastoDinheiro;
        despesaTotal = despesasCartao + despesasNaoCartao;

        //DespesaUI.current.sobraTotal_txt.text = guardado.ToString("F2");
        //DespesaUI.current.gastoTotal_txt.text = despesaTotal.ToString("F2");

    }

    private void GastoPorItem(ref float guardado, ref float gastoDinheiro, ref float gastoCartao, int i, ref float gastoMensal, ref int id, Item item)
    {
        switch (item.tipo)
        {
            case Item.Tipo.DESPESA:
                if (currentMonth + i >= item.mesComeca && item.parcelas >= i + 1)
                {
                    if (item.cartao)
                    {
                        gastoCartao += item.valorMensal;
                        if (item.mensal == false) currentLimit += item.valorMensal;
                    }
                    else gastoDinheiro += item.valorMensal;
                    //print(id + " -> " + item.nome + " - Parcela: " + item.valorMensal);
                    id++;
                    gastoMensal += item.valorMensal;
                    /*var novo = DespesaUI.current.item_relatorio_prefab;
                    novo.GetComponent<ItemDados>().dados = item;
                    Transform t = listaRelatorio_ob.transform.GetChild(i).transform.GetChild(0).transform;
                    Instantiate(novo, t);*/
                }
                break;
            case Item.Tipo.EXTRA:
                if(currentMonth + i == item.mesComeca && !item.extraAplicado)
                {
                    //print("OPA DINHEIRO EXTRA!!!");
                    id++;
                    /*var novo = DespesaUI.current.Prefab_reportItem;
                    novo.GetComponent<ItemDados>().dados = item;
                    Transform t = listaRelatorio_ob.transform.GetChild(i).transform.GetChild(0).transform;
                    Instantiate(novo, t);*/
                    guardado += item.valorMensal;
                    item.extraAplicado = true;
                }
                break;
        }
    }

    public void ResetarResultados()
    {
        dinheiroPoupanca = initialMoney;
        despesaTotal = despesasCartao = despesasNaoCartao = 0;
        currentLimit = limit;

        foreach (Item item in ItemsList)
        {
            if (item.mensal == false && item.cartao && item.tipo == Item.Tipo.DESPESA) currentLimit -= (item.valorMensal * item.parcelas);
        }

        //listaRelatorio_ob.transform.DetachChildren();

        foreach (GameObject m in GameObject.FindGameObjectsWithTag("Meses"))
            Destroy(m);
            
        DespesaUI.current.SairRelatorio();
    }
    public void ResetarTudo()
    {
        dinheiroPoupanca = initialMoney;
        despesaTotal = despesasCartao = despesasNaoCartao = 0;
        currentLimit = limit;

        //listaItems_ob.transform.DetachChildren();
        //listaRelatorio_ob.transform.DetachChildren();

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Itens"))
            Destroy(i);

        foreach (GameObject m in GameObject.FindGameObjectsWithTag("Meses"))
            Destroy(m);

        ItemsList.Clear();

        DespesaUI.current.SairRelatorio();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

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
    List<Item> ItemsList;

    [Header("Editar Item")]
    public GameObject editar;

    public static Despesa current;

    private void Awake()
    {
        current = this;
        Initialize();
    }

    #region Get
    public int getAmountMonths()        => amountMonths;
    public int getCurrentMonth()        => currentMonth;
    public float getLimit()             => limit;
    public float getCurrentLimit()      => currentLimit;
    public float getInitialMoney()      => initialMoney;
    public float getIncomePerMonth()    => incomePerMonth;
    public float getFees()              => fees;
    public float getTotalExpense()      => despesaTotal;
    public float getCreditExpense()     => despesasCartao;
    public float getMoneyExpense()      => despesasNaoCartao;
    public float getSavedMoney()        => dinheiroPoupanca;
    public string getMesFinal()         => mesFinal;

    public List<Item> getItems()        => ItemsList;
    #endregion
    #region Set
    public void setItemList(List<Item> items) => ItemsList = items;
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
    public void AddToList(Item item) => ItemsList.Add(item);
    public void RemoveFromList(Item item) => ItemsList.Remove(item);
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

    #region Inputs Update
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
    #endregion

    #region
    public void SearchForSomething()
    {

    }
    public void ShowExpenseDescriptionsSearched()
    {

    }
    public void ShowMoreAbout()
    {

    }
    #endregion

    #region
    public void AddOtherExpense()
    {

    }
    public void CalculateTotalExpenses()
    {

    }
    public void RefreshTotalExpenses()
    {

    }
    #endregion

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

    public void CalculateMain()
    {
        if (ItemsList == null)
        {
            ItemsList = new List<Item>();
            return;
        }

        float gastoDinheiro = 0;
        float gastoCartao = 0;
        float guardado = initialMoney;



        for (int i = 0; i <= amountMonths; i++)
        {
            float gastoMensal = 0;

            foreach (Item item in ItemsList) 
                GastoPorItem(ref guardado, ref gastoDinheiro, ref gastoCartao,ref gastoMensal, i, item);
            guardado += (incomePerMonth - gastoMensal);
        }

        dinheiroPoupanca = guardado;
        despesasCartao = gastoCartao;
        despesasNaoCartao = gastoDinheiro;
        despesaTotal = despesasCartao + despesasNaoCartao;
    }
    public float[] CalculateExpenseUntill(int amountMonths)
    {
        float[] results = new float[2];
        float gastoDinheiro = 0;
        float gastoCartao = 0;
        float sobra = initialMoney;

        
        for(int i = 0; i <= amountMonths; i++)
        {
            float gastoMensal = 0;

            foreach(Item item in ItemsList)
                GastoPorItem(ref sobra, ref gastoDinheiro, ref gastoCartao, ref gastoMensal, i, item);
            sobra += (incomePerMonth - gastoMensal);
        }

        results[1] = sobra;
        results[0] = gastoCartao + gastoDinheiro;

        return results;
    }

    private void GastoPorItem(ref float guardado, ref float gastoDinheiro, ref float gastoCartao, ref float gastoMensal, int i, Item item)
    {
        switch (item.getType())
        {
            case Item.TipoItem.DESPESA:
                if (currentMonth + i >= item.getInitMonth() && item.getParcels() >= i + 1)
                {
                    if (item.getUseCreditCard())
                    {
                        gastoCartao += item.getMonthlyPrice();
                        if (!item.getIsMonthly()) currentLimit += item.getMonthlyPrice();
                    }
                    else gastoDinheiro += item.getMonthlyPrice();
                    gastoMensal += item.getMonthlyPrice();
                }
                break;
            case Item.TipoItem.EXTRA:
                if(currentMonth + i == item.getInitMonth() && !item.getExtraAdded())
                {
                    guardado += item.getMonthlyPrice();
                    item.setExtraAdded(true);
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
            item.DiscountInCurrentLimit();
    }
    public void ResetarTudo()
    {
        dinheiroPoupanca = initialMoney;
        despesaTotal = despesasCartao = despesasNaoCartao = 0;
        currentLimit = limit;


        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Itens"))
            Destroy(i);

        foreach (GameObject m in GameObject.FindGameObjectsWithTag("Meses"))
            Destroy(m);

        ItemsList.Clear();
    }
}

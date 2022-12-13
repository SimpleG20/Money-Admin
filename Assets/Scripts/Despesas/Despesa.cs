using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Despesa : MonoBehaviour
{
    public AuxDataToSave dataToSave;
    public AuxInputsToSave inputsToSave;
    public List<int> idAvaliables = new List<int>();

    static int lastUsedID;
    int amountMonths
    {
        get => DespesaUI.current.inputAmountMonths.getInputValue().intValue;
    }
    int currentMonth;

    float initMoney;
    float currentLimitCalculated;
    float dinheiroPoupanca;
    float cardExpense;
    float moneyExpense;
    float totalExpense;
    string lastMonth;

    [Header("Lista de Despesas")]
    List<Item> ItemsList;

    [Header("Editar Item")]
    public StoreItemData editar;

    public static Despesa current;

    private void Awake()
    {
        current = this;
        dataToSave = new AuxDataToSave();
        inputsToSave = new AuxInputsToSave();
    }

    #region Get
    public int getLastID()              => lastUsedID;
    public int getAmountMonths()        => amountMonths;
    public int getCurrentMonth()        => currentMonth;
    public float getDefaultLimit()      => DespesaUI.current.limit;
    public float getInitialMoney()      => DespesaUI.current.initMoney;
    public float getIncomePerMonth()    => DespesaUI.current.income;
    public float getFees()              => DespesaUI.current.fees;
    public float getTotalExpense()      => totalExpense;
    public float getCreditExpense()     => cardExpense;
    public float getMoneyExpense()      => moneyExpense;
    public string getLastMonth()        => lastMonth;

    public List<Item> getItems()        => ItemsList;
    #endregion

    #region Set
    public void setLastID(int value) => lastUsedID = value;
    public void setItemList(List<Item> items) => ItemsList = items;
    public void setCurrentMonth(int value) => currentMonth = value;

    public void DecreaseCurrentLimit(float value) => DespesaUI.current.currentLimit -= value;
    public void IncreaseCurrentLimit(float value) => DespesaUI.current.currentLimit += value;
    public void AddToList(Item item) => ItemsList.Add(item);
    public bool RemoveFromList(Item item)
    {
        ItemsList.Remove(ItemsList.Single(t => t.getId() == item.getId()));
        IncreaseCurrentLimit(item.getTotalPrice());

        idAvaliables.Add(item.getId());

        return !ItemsList.Any(t => t.getId() == item.getId());
    }
    #endregion

    public void Initialize()
    {
        currentMonth = 1;

        if (Salvar.CarregarDados())
        {
            print(dataToSave.itens.Count);
            ItemsList = dataToSave.itens;

            initMoney = inputsToSave.dinheiroInicial;

            
            DespesaUI.current.UpdateInputsFromScene2(inputsToSave.limite, inputsToSave.limiteAtual, 
                                                     inputsToSave.renda, inputsToSave.dinheiroInicial,
                                                     inputsToSave.juros);
            return;
        }
        ItemsList = new List<Item>();
    }

    #region Second Scene
    public void SaveInputsFromScene2()
    {
        var prevLimit = inputsToSave.limite;

        inputsToSave.limite =                       DespesaUI.current.limit;
        inputsToSave.renda =                        DespesaUI.current.income;
        inputsToSave.dinheiroInicial =              DespesaUI.current.initMoney;
        inputsToSave.juros =                        DespesaUI.current.fees;
        inputsToSave.lastUsedID = lastUsedID;

        if (LimitInitialization(prevLimit))
        {
            inputsToSave.limiteAtual = DespesaUI.current.currentLimit = currentLimitCalculated;
        }

        Salvar.SaveInputs();
        DespesaUI.current.ShowWarning(DespesaUI.SAVED);
    }
    public void SearchForSomething()
    {
        string toSearch = DespesaUI.current.searchedItem;
        var result = ItemsList.Where(t => t.getName() == toSearch).ToList();

        if (result.Count > 0) DespesaUI.current.InstantiateSavedItens(result);
        else
        {
            DespesaUI.current.ShowWarning(DespesaUI.ERRO_NOT_EXIST, 1500);
            print($"Couldn't found {toSearch}");
        }
    }
    #endregion

    #region Calculation
    public float[] CalculateExpenseUntill(int amountMonths, int month, int year)
    {
        if (ItemsList == null)
        {
            ItemsList = new List<Item>();
            DespesaUI.current.ShowWarning(DespesaUI.ERRO_LISTNULL);
            return null;
        }

        float gastoDinheiroAcumulado = 0, gastoDinheiroNoMes = 0;
        float gastoCartaoAcumulado = 0, gastoCartaoNoMes = 0;
        float extra = 0, gastoMensal;
        float sobra = initMoney;
        bool noMoney = false;

        //Calculo do gasto Acumulativo
        for(int i = 0; i <= amountMonths; i++)
        {
            gastoMensal = 0;

            foreach(Item item in ItemsList)
                CostPerItemForAcumulative(ref extra, ref gastoDinheiroAcumulado, ref gastoCartaoAcumulado, ref gastoMensal, i, item);
            sobra += extra + getIncomePerMonth() - gastoMensal;

            if(sobra < 0)
            {
                print(sobra);
                DespesaUI.current.ShowWarning(DespesaUI.ERRO_MONTH_MONEY);
                noMoney = true;
                break;
            }
        }
        if (noMoney) return new float[] {0,0,0};

        gastoMensal = 0;
        extra = 0;
        foreach (Item item in ItemsList)
        {
            DespesaUI.current.InstantiateItemsForReport(item);
            CostPerItemInSpecificMonth(ref extra, ref gastoDinheiroNoMes, ref gastoCartaoNoMes, ref gastoMensal, month, year, item);
        }

        float[] results = new float[3];
        results[0] = gastoCartaoAcumulado + gastoDinheiroAcumulado;             //Gasto Acumulado
        results[1] = gastoCartaoNoMes + gastoDinheiroNoMes;                     //Gasto no mes
        results[2] = sobra;                                                     //Sobra no mes
        return results;
    }
    public void CalculateTotalExpenses()
    {
        if (ItemsList == null)
        {
            ItemsList = new List<Item>();
            DespesaUI.current.ShowWarning(DespesaUI.ERRO_LISTNULL);
            return;
        }

        float gastoDinheiro = 0;
        float gastoCartao = 0;
        float guardado = initMoney;

        for (int i = 0; i <= amountMonths; i++)
        {
            float gastoMensal = 0;

            foreach (Item item in ItemsList) 
                CostPerItemForAcumulative(ref guardado, ref gastoDinheiro, ref gastoCartao,ref gastoMensal, i, item);
            guardado += (getIncomePerMonth() - gastoMensal);
        }

        dinheiroPoupanca = guardado;
        cardExpense = gastoCartao;
        moneyExpense = gastoDinheiro;
        totalExpense = cardExpense + moneyExpense;
    }
    private void CostPerItemForAcumulative(ref float extra, ref float gastoDinheiro, ref float gastoCartao, ref float gastoMensal, int i, Item item)
    {
        switch (item.getType())
        {
            case Item.TipoItem.DESPESA:
                if (currentMonth + i >= item.getInitMonth() && item.getParcels() >= i + 1)
                {
                    if (item.getUseCreditCard())
                    {
                        gastoCartao += item.getMonthlyPrice();
                        //if (!item.getIsMonthly()) currentLimit += item.getMonthlyPrice();
                    }
                    else gastoDinheiro += item.getMonthlyPrice();
                    gastoMensal += item.getMonthlyPrice();
                }
                break;
            case Item.TipoItem.EXTRA:
                if(currentMonth + i == item.getInitMonth() && !item.getExtraAdded())
                {
                    extra += item.getMonthlyPrice();
                }
                break;
        }
    }
    private void CostPerItemInSpecificMonth(ref float extra, ref float gastoDinheiro, ref float gastoCartao, ref float gastoMensal, int month, int year, Item item)
    {
        switch (item.getType())
        {
            case Item.TipoItem.DESPESA:
                if(year <= item.getYear() && month <= item.getLastMonth())
                {
                    if (item.getUseCreditCard())
                    {
                        gastoCartao += item.getMonthlyPrice();
                        //if (!item.getIsMonthly()) currentLimit += item.getMonthlyPrice();
                    }
                    else gastoDinheiro += item.getMonthlyPrice();
                    gastoMensal += item.getMonthlyPrice();
                }
                break;
            case Item.TipoItem.EXTRA:
                if (year <= item.getYear() && month <= item.getLastMonth())
                {
                    extra += item.getMonthlyPrice();
                    item.setExtraAdded(true);
                }
                break;
        }
    }
    private bool LimitInitialization(float prevLimit)
    {
        var temp = getDefaultLimit();
        foreach(Item item in ItemsList)
        {
            if (temp - item.getTotalPrice() >= 0)
            {
                temp -= item.getTotalPrice();
            }
            else
            {
                print($"{item.getName()} Excedeu o limite -> Limite: {temp} / Valor: {item.getTotalPrice()}");
                inputsToSave.limite = prevLimit;
                inputsToSave.limiteAtual = DespesaUI.current.currentLimit;
                return false;
            }
        }
        currentLimitCalculated = temp;
        return true;
    }
    public void RefreshExpensesCalculation()
    {
        dinheiroPoupanca = getInitialMoney();
        totalExpense = cardExpense = moneyExpense = 0;
        DespesaUI.current.currentLimit = getDefaultLimit();

        foreach (Item item in ItemsList)
            item.RemoveFromCurrentLimit();
    }
    public void ResetAll()
    {
        dinheiroPoupanca = getInitialMoney();
        totalExpense = cardExpense = moneyExpense = 0;
        DespesaUI.current.currentLimit = getDefaultLimit();

        ItemsList.Clear();
    }
    #endregion

}

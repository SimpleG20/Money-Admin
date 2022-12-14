using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
    float incomePerMonth;
    float defaultLimit;
    float currentLimit;
    float fees;
    
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
    public void Initialize()
    {
        currentMonth = DateTime.Now.Month;

        if (Salvar.CarregarDados())
        {
            //print(dataToSave.itens.Count);
            ItemsList = dataToSave.itens;
            foreach (Item i in ItemsList) i.PrintItem();
            defaultLimit = inputsToSave.limite;

            if (inputsToSave.limite == inputsToSave.limiteAtual && ItemsList.Count > 0) 
                inputsToSave.limiteAtual = LimitInitialization(inputsToSave.limiteAtual);
            
            DespesaUI.current.UpdateInputsFromScene2(inputsToSave.limite, inputsToSave.limiteAtual, 
                                                     inputsToSave.renda, inputsToSave.dinheiroInicial,
                                                     inputsToSave.juros);
            return;
        }
        ItemsList = new List<Item>();
    }

    #region Get
    public int getLastID()              => lastUsedID;
    public int getAmountMonths()        => amountMonths;
    public int getCurrentMonth()        => currentMonth;
    public float getCurrentLimit()      => currentLimit;
    public float getTotalExpense()      => totalExpense;
    public float getCreditExpense()     => cardExpense;
    public float getMoneyExpense()      => moneyExpense;
    public string getLastMonth()        => lastMonth;

    public List<Item> getItems()        => ItemsList;
    #endregion

    #region Set
    public void IncreaseLastID() => lastUsedID += 1;
    public void DeacreaseLastID() => lastUsedID -= 1;
    public void setItemList(List<Item> items) => ItemsList = items;
    public void setInitMoney(float value) => initMoney = value;
    public void setIncomePerMonth(float value) => incomePerMonth = value;
    public void setDefaultLimit(float value) => defaultLimit = value;
    public void setCurrentLimit(float value) => currentLimit = value;
    public void setFees(float value) => fees = value;
    public void setCurrentMonth(int value) => currentMonth = value;

    public void DecreaseCurrentLimit(float value) => DespesaUI.current.currentLimit -= value;
    public void IncreaseCurrentLimit(float value) => DespesaUI.current.currentLimit += value;
    public void AddToList(Item item) => ItemsList.Add(item);
    public bool RemoveFromList(Item item)
    {
        ItemsList.Remove(ItemsList.Where(t => t.getId() == item.getId()).ToArray()[0]);
        dataToSave.itens.Remove(dataToSave.itens.Where(t => t.getId() == item.getId()).ToArray()[0]);

        IncreaseCurrentLimit(item.getTotalPrice());

        idAvaliables.Add(item.getId());

        return !ItemsList.Any(t=> t.getId() == item.getId());
    }
    #endregion

    #region Second Scene
    public void SaveInputsFromScene2()
    {
        DespesaUI.current.ReasureInputs();
        var prevLimit = inputsToSave.limite;

        inputsToSave.limiteAtual        = LimitInitialization(prevLimit);
        inputsToSave.limite             = defaultLimit;
        inputsToSave.renda              = incomePerMonth;
        inputsToSave.dinheiroInicial    = initMoney;
        inputsToSave.juros              = fees;
        inputsToSave.lastUsedID         = lastUsedID;

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
            sobra += extra + incomePerMonth - gastoMensal;

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
        results[0] = gastoCartaoAcumulado + gastoDinheiroAcumulado;
        results[1] = gastoCartaoNoMes + gastoDinheiroNoMes;
        results[2] = sobra;
        //print($"Gasto Acumulado: {}");          //Gasto Acumulado
        //print($"Gasto Mensal: {}");             //Gasto no mes
        //print($"Sobra Mensal: {}");             //Sobra no mes
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
        float extra = 0;
        float guardado = initMoney;

        for (int i = 0; i <= amountMonths; i++)
        {
            float gastoMensal = 0;

            foreach (Item item in ItemsList) 
                CostPerItemForAcumulative(ref extra, ref gastoDinheiro, ref gastoCartao,ref gastoMensal, i, item);
            guardado += (extra + incomePerMonth - gastoMensal);
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
                if (item.getInitMonth() > month && item.getInitYear() > year || item.getLastMonth() < month && item.getLastYear() < year) break;
                
                if (item.getUseCreditCard())
                {
                    gastoCartao += item.getMonthlyPrice();
                    //if (!item.getIsMonthly()) currentLimit += item.getMonthlyPrice();
                }
                else gastoDinheiro += item.getMonthlyPrice();
                gastoMensal += item.getMonthlyPrice();
                break;
            case Item.TipoItem.EXTRA:
                if (item.getInitMonth() >= month && item.getInitYear() >= year || item.getLastMonth() <= month && item.getLastYear() <= year) break;
                
                if (!item.getExtraAdded())
                {
                    extra += item.getMonthlyPrice();
                    item.setExtraAdded(true);
                }
                break;
        }
    }

    private float LimitInitialization(float prevLimit)
    {
        var temp = defaultLimit;
        int maxMonths = 0;

        foreach(Item item in ItemsList)
        {
            if (item.getParcels() > maxMonths) maxMonths = item.getParcels();

            if (temp - item.getTotalPrice() >= 0)
            {
                temp -= item.getTotalPrice();
            }
            else
            {
                print($"{item.getName()} Excedeu o limite -> Limite: {temp} / Valor: {item.getTotalPrice()}");
                inputsToSave.limite = prevLimit;
                DespesaUI.current.monthsAvailables = maxMonths;
                return currentLimit;
            }
        }
        DespesaUI.current.currentLimit = temp;
        DespesaUI.current.monthsAvailables = maxMonths;
        return temp;
    }
    public void RefreshExpensesCalculation()
    {
        dinheiroPoupanca = initMoney;
        totalExpense = cardExpense = moneyExpense = 0;
        DespesaUI.current.currentLimit = defaultLimit;

        foreach (Item item in ItemsList)
            item.RemoveFromCurrentLimit();
    }
    public void ResetAll()
    {
        dinheiroPoupanca = initMoney;
        totalExpense = cardExpense = moneyExpense = 0;
        DespesaUI.current.currentLimit = defaultLimit;

        ItemsList.Clear();
    }
    #endregion

}

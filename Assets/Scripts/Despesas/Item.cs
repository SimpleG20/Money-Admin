using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum TipoItem { DESPESA, EXTRA }
    private TipoItem[] types = new TipoItem[] { TipoItem.DESPESA, TipoItem.EXTRA };

    private string name;
    private int id;
    private TipoItem type;

    private int parcels;
    private int initMonth;
    private int lastMonth;
    private int year;
    private int lastYear;
    private float totalPrice;
    private float monthlyPrice;

    private bool creditCard;
    private bool isMonthly;

    private bool extraAdded;
    private bool isTest;
    private bool storeMothlyPrice;


    public Item(string name, int id, int type, bool isTest, bool storeMonthlyPrice, bool isMonthly, int creditCard, float price, int parcels, int initMonth, bool loadData)
    {
        this.name = name;
        this.id = id;
        this.type = types[type];

        this.creditCard = creditCard == 0;
        this.isMonthly = isMonthly;
        if (isMonthly) this.storeMothlyPrice = true;
        else this.storeMothlyPrice = storeMonthlyPrice;
        this.isTest = isTest;

        if (!loadData)
        {
            if (storeMonthlyPrice)
            {
                totalPrice = price * parcels;
                monthlyPrice = Mathf.Round(price * 100f) / 100f;
            }
            else
            {
                totalPrice = price;
                monthlyPrice = Mathf.Round(totalPrice / parcels * 100f) / 100f;
            }
        }
        else
        {
            totalPrice = price;
            monthlyPrice = Mathf.Round(totalPrice / parcels * 100f) / 100f;
        }

        this.parcels = parcels;

        if (initMonth < Despesa.current.getCurrentMonth())
        {
            this.initMonth = Despesa.current.getCurrentMonth() + (12 - Despesa.current.getCurrentMonth()) + initMonth;
            year = DateTime.Now.Year + 1;
        }
        else
        {
            this.initMonth = initMonth;
            year = DateTime.Now.Year;
        }

        if (initMonth + parcels > 12)
        {
            lastMonth = initMonth + (parcels % 12);
            while(lastMonth > 12)
            {
                lastMonth = (lastMonth % 12);
            }
        }
        else lastMonth = initMonth + parcels;

        lastYear = (int)Mathf.Floor(parcels / 12) + year;
    }

    #region Gets
    public string getName() => name;
    public int getId() => id;
    public TipoItem getType() => type;
    public int getParcels() => parcels;
    public int getInitMonth() => initMonth;
    public int getLastMonth() => lastMonth;
    public int getInitYear() => year;
    public int getLastYear() => lastYear;
    public float getTotalPrice() => totalPrice;
    public float getMonthlyPrice() => monthlyPrice;
    public bool getUseCreditCard() => creditCard;
    public bool getIsMonthly() => isMonthly;
    public bool getExtraAdded() => extraAdded;
    public bool getIsTest() => isTest;
    public string getMonthlyPriceMoneyFormat() => monthlyPrice.ToString().MoneyFormat();
    public string getTotalPriceMoneyFormat() => totalPrice.ToString().MoneyFormat();

    public bool getShowMonthlyPrice() => storeMothlyPrice;
    public void setExtraAdded(bool value) => extraAdded = value;
    public void setIsTest(bool value) => isTest = value;
    #endregion

    public void PrintItem()
    {
        Debug.Log($"Item {name} ID: {id} Tipo: {type} " +
            $"Cartao: {creditCard} Mensal: {isMonthly}" +
            $"Preco Mensal Armazenado: {storeMothlyPrice}" +
            $"Preco Total: {totalPrice} Preco Mensal : {monthlyPrice}" +
            $"Teste: {isTest}");
    }
    public bool IsInThisMonthAndYear(int _month, int _year)
    {
        if(_year <= year && _month <= lastMonth) return true;
        return false;
    }
    public bool DiscountInCurrentLimit()
    {
        if (totalPrice > Despesa.current.getCurrentLimit()) return false;

        if (!isMonthly && creditCard && type == TipoItem.DESPESA) 
        { 
            Despesa.current.DecreaseCurrentLimit(totalPrice); 
        }
        return true;
    }
    public bool RemoveFromCurrentLimit()
    {
        if (!isMonthly && creditCard && type == TipoItem.DESPESA)
        {
            Despesa.current.IncreaseCurrentLimit(totalPrice);
            return true;
        }
        return false;
    }
}

using System;

[Serializable]
public class Item
{
    public enum TipoItem { DESPESA, EXTRA }
    private TipoItem[] types = new TipoItem[] { TipoItem.DESPESA, TipoItem.EXTRA };

    private string name;
    private TipoItem type;

    private int parcels;
    private int initMonth;
    private int lastMonth;
    private int year;
    private float totalPrice;
    private float monthlyPrice;

    private bool creditCard;
    private bool isMonthly;

    private bool extraAdded;
    private bool isTest;
    private bool showMonthlyPrice;


    public Item(string name, int type, bool isTest, bool showMonthlyPrice, bool isMonthly, int creditCard, float price, int parcels, int initMonth)
    {
        this.name = name;
        this.type = types[type];

        this.creditCard = creditCard == 0;
        this.isMonthly = isMonthly;
        if (isMonthly) this.showMonthlyPrice = true;
        else this.showMonthlyPrice = showMonthlyPrice;
        this.isTest = isTest;

        if (showMonthlyPrice)
        {
            totalPrice = price * parcels;
            monthlyPrice = price;
        }
        else
        {
            totalPrice = price;
            monthlyPrice = price / parcels;
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
        lastMonth = this.initMonth + parcels > 12 ? this.initMonth + parcels - 12 : this.initMonth + parcels;
    }

    #region Gets
    public string getName() => name;
    public TipoItem getType() => type;
    public int getParcels() => parcels;
    public int getInitMonth() => initMonth;
    public int getLastMonth() => lastMonth;
    public int getYear() => year;
    public float getTotalPrice() => totalPrice;
    public float getMonthlyPrice() => monthlyPrice;
    public bool getUseCreditCard() => creditCard;
    public bool getIsMonthly() => isMonthly;
    public bool getExtraAdded() => extraAdded;
    public bool getIsTest() => isTest;
    public string getMonthlyPriceFormated() => monthlyPrice.ToString().MoneyFormat();
    public string getTotalPriceFormated() => totalPrice.ToString().MoneyFormat();

    public bool getShowMonthlyPrice() => showMonthlyPrice;
    public void setExtraAdded(bool value) => extraAdded = value;
    public void setIsTest(bool value) => isTest = value;
    #endregion

    public bool DiscountInCurrentLimit()
    {
        if (totalPrice > Despesa.current.getCurrentLimit()) return false;

        if (!isMonthly && creditCard && type == TipoItem.DESPESA) 
        { 
            Despesa.current.DecreaseLimit(totalPrice); 
        }
        return true;
    }
    public bool RemoveFromLimit()
    {
        if (!isMonthly && creditCard && type == TipoItem.DESPESA) 
            Despesa.current.IncreaseLimit(totalPrice);

        return true;
    }
}

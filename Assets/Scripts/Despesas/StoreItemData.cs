using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class StoreItemData : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTxt, valueTxt, parcelsTxt;
    [SerializeField] Image iconType;
    [SerializeField] UIButton Bt_showPrice, Bt_edit, Bt_removeItem;
    [SerializeField] Transform extraInfoTransform;
    private Item data;

    public void Initiate(Item item)
    {
        if(data == null) data = item;

        nameTxt.text = data.getName();
        if (valueTxt != null)
            valueTxt.text = data.getShowMonthlyPrice() ? data.getMonthlyPriceMoneyFormat() : data.getTotalPriceMoneyFormat();
        if(parcelsTxt != null)
            parcelsTxt.text = $"{data.getParcels()}x";

        if(iconType != null)
        {
            Color color;
            
            if(data.getType() == Item.TipoItem.DESPESA) ColorUtility.TryParseHtmlString("#FF4C4C", out color);
            else ColorUtility.TryParseHtmlString("#6EF44B", out color);

            iconType.color = color;
        }
    }
    public void setData(Item item)
    {
        data = item;
        Initiate(data);
    }
    public Item getData() => data;

    public void ShowExtraInfoInScene4()
    {
        DespesaUI.current.ShowExtraInfo(extraInfoTransform.position, data.getInitMonth(), data.getUseCreditCard(), data.getIsTest(), data.getIsMonthly());
    }
    public void ShowItemDataToEditInScene3()
    {
        Despesa.current.editar = this;
        DespesaUI.current.EditItem();
    }
    [ContextMenu("Show Price Details")]
    public void ShowItemPriceInScene2()
    {
        nameTxt.gameObject.SetActive(!nameTxt.gameObject.activeSelf);
        parcelsTxt.gameObject.SetActive(!parcelsTxt.gameObject.activeSelf);
        valueTxt.gameObject.SetActive(!valueTxt.gameObject.activeSelf);
    }
    public void RemoveItem()
    {
        if (Despesa.current.RemoveFromList(data))
        {
            Salvar.SaveItems();
            Destroy(gameObject);
        }
    }
}

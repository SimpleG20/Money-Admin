using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDados : MonoBehaviour
{
    public enum Tipo { Lista, Relatorio, Salvos};
    public Tipo local;
    public TextMeshProUGUI nome, tipo, parcelas, valor;
    public Image salvo_img;
    public Button deletar;
    public Item dados;

    private void Start()
    {
        Setar();
    }

    public void DeletarItem()
    {
        if(local == Tipo.Salvos)
        {
            //Use Linq here
            //Despesa.current.listaSalvos
            foreach(Item i in Despesa.current.listaSalvos.itens)
            {
                if(i.getName().ToLower() == dados.getName().ToLower())
                {
                    Despesa.current.listaSalvos.itens.Remove(i);
                    Salvar.SalvarDados(Despesa.current.listaSalvos);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            foreach (Item i in Despesa.current.getItems())
            {
                if (i.getName().ToLower() == dados.getName().ToLower())
                {
                    if (!i.getIsMonthly() && i.getUseCreditCard() && i.getType() == Item.TipoItem.DESPESA) Despesa.current.setCurrentLimit(i.getTotalPrice());
                    //Despesa.current.ui.AtualizarLimite();
                    Despesa.current.RemoveFromList(i);
                    Destroy(gameObject);
                    break;
                }
            }
        }
        
    }

    public void EditarItem()
    {
        Despesa.current.editar = gameObject;
    }

    public void Setar()
    {
        Color cor;
        nome.text = dados.getName();
        if (dados.getType() == Item.TipoItem.DESPESA)
        {
            ColorUtility.TryParseHtmlString("#FF5D5D", out cor);
            if (tipo != null) { tipo.text = "-"; tipo.color = cor; }
        }
        else
        {
            ColorUtility.TryParseHtmlString("#99FF5E", out cor);
            if(tipo != null) { tipo.text = "+"; tipo.color = cor; }
        }
        nome.color = cor;

        if (parcelas != null) { parcelas.text = dados.getParcels().ToString() + "x"; parcelas.color = Color.white; }
        if (valor != null) valor.text = "R$ " + dados.getMonthlyPrice().ToString("F2");

        if (local == Tipo.Lista)
        {
            if (dados.getIsTest()) salvo_img.gameObject.SetActive(true);
            else salvo_img.gameObject.SetActive(false);
        }
        else if (local == Tipo.Salvos) salvo_img.gameObject.SetActive(true);
    }
}

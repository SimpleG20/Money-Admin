using System.Collections;
using System.Collections.Generic;
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
            foreach(Item i in Despesa.current.listaSalvos.itens)
            {
                if(i.nome.ToLower() == dados.nome.ToLower())
                {
                    Despesa.current.listaSalvos.itens.Remove(i);
                    Salvar.SalvarDados(Despesa.current.listaSalvos);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            foreach (Item i in Despesa.current.ItemsList)
            {
                if (i.nome.ToLower() == dados.nome.ToLower())
                {
                    if (!i.mensal && i.cartao && i.tipo == Item.Tipo.DESPESA) Despesa.current.setCurrentLimit(i.valorMensal * i.parcelas);
                    //Despesa.current.ui.AtualizarLimite();
                    Despesa.current.ItemsList.Remove(i);
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
        nome.text = dados.nome;
        if (dados.tipo == Item.Tipo.DESPESA)
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

        if (parcelas != null) { parcelas.text = dados.parcelas.ToString() + "x"; parcelas.color = Color.white; }
        if (valor != null) valor.text = "R$ " + dados.valorMensal.ToString("F2");

        if (local == Tipo.Lista)
        {
            if (dados.test) salvo_img.gameObject.SetActive(true);
            else salvo_img.gameObject.SetActive(false);
        }
        else if (local == Tipo.Salvos) salvo_img.gameObject.SetActive(true);
    }
}

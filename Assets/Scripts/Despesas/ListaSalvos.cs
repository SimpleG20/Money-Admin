using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListaSalvos
{
    public float dinheiroInicial, renda;
    public float limite;

    public List<Item> itens;
    public int tamanhoMatriz;

    public void AdicionarLista(ListaSalvos dados, Item item)
    {
        if (dados.itens == null) dados.itens = new List<Item>();

        foreach (Item i in dados.itens)
        {
            if (i == item) { dados.itens.Remove(i); break; }
        }
        dados.itens.Add(item);
        dados.tamanhoMatriz = dados.itens.Count;
        //if (Despesa.current.ui.instanciarItensSalvos_button.interactable == false) Despesa.current.ui.instanciarItensSalvos_button.interactable = true;
    }

    public void TransferirArraylista(ListaSalvos dados, string[] nomes, float[,] matriz, int tam)
    {
        dados.itens = new List<Item>();
        for(int i = 0; i < tam; i++)
        {
            Item item = ScriptableObject.CreateInstance<Item>();
            item.nome = nomes[i];
            item.tipo = matriz[i, 0] == 1 ? Item.Tipo.EXTRA : Item.Tipo.DESPESA;
            item.valorMensal = matriz[i, 1];
            item.parcelas = (int)matriz[i, 2];
            item.mesComeca = (int)matriz[i, 3];
            item.cartao = matriz[i, 4] == 1 ? true : false;
            item.mensal = matriz[i, 5] == 1 ? true : false;
            item.extraAplicado = false;
            dados.itens.Add(item);
        }

        if (dados.itens.Count != 0) DespesaUI.current.InstantiateSavedItens();
        //Despesa.current.listaSalvos.itens = itens;
    }

    public void TransferirListaArray(ListaSalvos dados, out float[,] matriz, out string[] nomes, out int tam)
    {
        int i = 0;
        tam = dados.tamanhoMatriz;
        nomes = new string[tam];
        matriz = new float[tam, 6];

        foreach(Item item in dados.itens)
        {
            nomes[i] = item.nome;
            matriz[i, 0] = ((int)item.tipo); //tipo
            matriz[i, 1] = item.valorMensal; //valor
            matriz[i, 2] = item.parcelas; //parcelas
            matriz[i, 3] = item.mesComeca; //mes
            matriz[i, 4] = item.cartao ? 1 : 0; //cartao
            matriz[i, 5] = item.mensal? 1 : 0; //mensal
            i++;
        }
    }

    public void AtualizarInputs(ListaSalvosData data)
    {
        Despesa.current.setInitialMoney(data.dinheiroInicial);
        Despesa.current.setIncomePerMonth(data.renda);
        Despesa.current.setLimit(data.limite);
        //Despesa.current.ui.AtualizarLimite();
    }
}

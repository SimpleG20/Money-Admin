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
            Item item = new Item(nomes[i], 
                                (int)matriz[i, 0], matriz[i, 1] == 1, 
                                matriz[i, 2] == 1, matriz[i, 3] == 1, 
                                (int)matriz[i, 4], matriz[i, 5], 
                                (int)matriz[i, 6], (int)matriz[i, 7]);
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
        matriz = new float[tam, 8];

        foreach(Item item in dados.itens)
        {
            nomes[i] = item.getName();
            matriz[i, 0] = ((int)item.getType());               //tipo
            matriz[i, 1] = item.getIsTest()? 1 : 0;             //eh soh um teste
            matriz[i, 2] = item.getShowMonthlyPrice() ? 1 : 0;  //mostrar valor mensal
            matriz[i, 3] = item.getIsMonthly() ? 1 : 0;         //mensal
            matriz[i, 4] = item.getUseCreditCard() ? 1 : 0;      //cartao
            matriz[i, 5] = item.getMonthlyPrice();              //mensal
            matriz[i, 6] = item.getParcels();                   //parcelas
            matriz[i, 7] = item.getInitMonth();                 //mes de inicio
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

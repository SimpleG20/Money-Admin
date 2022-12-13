using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AuxDataToSave
{
    public List<Item> itens;
    public int tamanhoMatriz;

    public void AdicionarLista(Item item)
    {
        var data = Despesa.current.dataToSave;
        if (data.itens == null) data.itens = new List<Item>();

        foreach (Item i in data.itens)
        {
            if (i.getId() == item.getId() || i.getName() == item.getName()) { data.itens.Remove(i); break; }
        }

        data.itens.Add(item);
        data.tamanhoMatriz = data.itens.Count;
    }

    public void ConvertArrayToList(DataToSave savedData)
    {
        Despesa.current.dataToSave.itens = new List<Item>();

        for(int i = 0; i < savedData.tamMatriz; i++)
        {
            Item item = new Item(savedData.nomes[i],         (int)savedData.matriz_itens[i, 8],
                                (int)savedData.matriz_itens[i, 0], savedData.matriz_itens[i, 1] == 1, 
                                savedData.matriz_itens[i, 2] == 1, savedData.matriz_itens[i, 3] == 1, 
                                (int)savedData.matriz_itens[i, 4], savedData.matriz_itens[i, 5], 
                                (int)savedData.matriz_itens[i, 6], (int)savedData.matriz_itens[i, 7], true);
            Despesa.current.dataToSave.itens.Add(item);
        }
    }
    public static void ConvertListToArray(out float[,] matriz, out string[] nomes, out int tam)
    {
        int i = 0;
        tam = Despesa.current.dataToSave.tamanhoMatriz;
        nomes = new string[tam];
        matriz = new float[tam, 9];

        foreach(Item item in Despesa.current.dataToSave.itens)
        {
            nomes[i] = item.getName();
            matriz[i, 0] = ((int)item.getType());               //tipo
            matriz[i, 1] = item.getIsTest()? 1 : 0;             //eh soh um teste
            matriz[i, 2] = item.getShowMonthlyPrice() ? 1 : 0;  //mostrar valor mensal
            matriz[i, 3] = item.getIsMonthly() ? 1 : 0;         //mensal
            matriz[i, 4] = item.getUseCreditCard() ? 0 : 1;     //cartao
            matriz[i, 5] = item.getTotalPrice();                //mensal
            matriz[i, 6] = item.getParcels();                   //parcelas
            matriz[i, 7] = item.getInitMonth();                 //mes de inicio
            matriz[i, 8] = item.getId();                        //id 
            i++;
        }
    }
}

public class AuxInputsToSave
{
    public int lastUsedID;
    public float dinheiroInicial, renda;
    public float limite, limiteAtual, juros;

    public AuxInputsToSave()
    {
        lastUsedID = 0;
        dinheiroInicial = 0;
        renda = 0;
        limite = 0;
        limiteAtual = 0;
        juros = 0;
    }

    public void ConvertInputs(InputsToSave inputsSaved)
    {
        Despesa.current.inputsToSave.limite = inputsSaved.limite;
        Despesa.current.inputsToSave.limiteAtual = DespesaUI.current.currentLimit = inputsSaved.limiteAtual;
        Despesa.current.inputsToSave.renda = inputsSaved.renda;
        Despesa.current.inputsToSave.dinheiroInicial = inputsSaved.dinheiroInicial;
        Despesa.current.inputsToSave.juros = inputsSaved.juros;
        Despesa.current.inputsToSave.lastUsedID = inputsSaved.lastUsedID;
        Despesa.current.idAvaliables = inputsSaved.ids.ToList() == null ? new List<int>() : inputsSaved.ids.ToList();
    }
}

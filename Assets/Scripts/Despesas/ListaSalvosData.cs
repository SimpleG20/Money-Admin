using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListaSalvosData
{
    public float dinheiroInicial;
    public float renda;
    public float limite;

    public string[] nomes;
    public float[,] matriz_itens;
    public int tamMatriz;

    public ListaSalvosData(ListaSalvos lista)
    { 
        lista.TransferirListaArray(lista, out matriz_itens, out nomes, out tamMatriz);
        renda = lista.renda;
        dinheiroInicial = lista.dinheiroInicial;
        limite = lista.limite;
    }
}

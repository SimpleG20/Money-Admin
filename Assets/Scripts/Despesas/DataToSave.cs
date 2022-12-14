using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataToSave
{
    public string[] nomes;
    public float[,] matriz_itens;
    
    public int tamMatriz;

    public DataToSave()
    {
        AuxDataToSave.ConvertListToArray(out matriz_itens, out nomes, out tamMatriz);
    }
}

[Serializable]
public class InputsToSave
{
    public int[] ids;
    public int lastUsedID;
    public float dinheiroInicial;
    public float renda;
    public float limite;
    public float limiteAtual;
    public float juros;

    public InputsToSave()
    {
        ids             = Despesa.current.idAvaliables.ToArray();
        lastUsedID      = Despesa.current.inputsToSave.lastUsedID;
        renda           = Despesa.current.inputsToSave.renda;
        dinheiroInicial = Despesa.current.inputsToSave.dinheiroInicial;
        limite          = Despesa.current.inputsToSave.limite;
        limiteAtual     = Despesa.current.inputsToSave.limiteAtual;
        juros           = Despesa.current.inputsToSave.juros;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Despesas")]
public class Item: ScriptableObject
{
    public enum Tipo { DESPESA, EXTRA }
    public string nome;
    public Tipo tipo;

    public float valorTotal;
    public float valorMensal;
    public int parcelas;
    public int mesComeca;
    public int mesTermina;
    
    public bool cartao;
    public bool mensal;

    public bool extraAplicado;
    public bool test;
}

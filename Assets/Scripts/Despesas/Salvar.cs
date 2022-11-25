using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Salvar
{
    public static void SalvarDados(ListaSalvos lista)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saves.AGSD";

        FileStream stream = new FileStream(path, FileMode.Create);

        ListaSalvosData data = new ListaSalvosData(lista);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ListaSalvosData LoadDados()
    {
        string path = Application.persistentDataPath + "/saves.AGSD";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            ListaSalvosData data = formatter.Deserialize(stream) as ListaSalvosData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);

            SalvarDados(null);
            return null;
        }
    }

    public static void CarregarDados()
    {
        ListaSalvosData data = LoadDados();

        if (data == null) 
        {
            Debug.Log("Novo Salvamento");
            ListaSalvos lista = new ListaSalvos();
            Despesa.current.listaSalvos = lista;
            //Despesa.current.ui.instanciarItensSalvos_button.gameObject.SetActive(false);
            SalvarDados(Despesa.current.listaSalvos);
        }
        else
        {
            Debug.Log("Carregado Com Sucesso");
            Despesa.current.listaSalvos.AtualizarInputs(data);
            Despesa.current.listaSalvos.TransferirArraylista(Despesa.current.listaSalvos, data.nomes, data.matriz_itens, data.tamMatriz);
        }
    }

}

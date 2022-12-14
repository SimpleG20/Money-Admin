using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

public static class Salvar
{
    public static void SaveInputs()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/MAdm.AGSD";

        FileStream stream = new FileStream(path, FileMode.Create);

        InputsToSave data = new InputsToSave();

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static InputsToSave LoadInputs()
    {
        string path = Application.persistentDataPath + "/MAdm.AGSD";
        Debug.Log(path);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            InputsToSave data = formatter.Deserialize(stream) as InputsToSave;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);

            return null;
        }
    }

    public static void SaveItems()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Items_MAdm.AGSD";

        FileStream stream = new FileStream(path, FileMode.Create);

        DataToSave data = new DataToSave();

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static DataToSave LoadItems()
    {
        string path = Application.persistentDataPath + "/Items_MAdm.AGSD";
        Debug.Log(path);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            DataToSave data = formatter.Deserialize(stream) as DataToSave;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);

            return null;
        }
    }


    public static bool CarregarDados()
    {
        DataToSave itemsData = LoadItems();
        InputsToSave inputsData = LoadInputs();

        if (itemsData == null && inputsData == null) 
        {
            Debug.Log("Novo Salvamento");

            Despesa.current.dataToSave = new AuxDataToSave();
            Despesa.current.dataToSave.itens = new List<Item>();
            Despesa.current.inputsToSave = new AuxInputsToSave();

            SaveItems();
            SaveInputs();
            return false;
        }
        else if(itemsData == null)
        {
            Debug.Log("Items Novo Salvamento");

            Despesa.current.dataToSave = new AuxDataToSave();
            Despesa.current.dataToSave.itens = new List<Item>();
            Despesa.current.inputsToSave.ConvertInputs(inputsData);

            SaveItems();
            return false;
        }
        else if(inputsData == null)
        {
            Debug.Log("Inputs Novo Salvamento");

            Despesa.current.inputsToSave = new AuxInputsToSave();
            Despesa.current.dataToSave.ConvertToItems(itemsData);

            SaveInputs();
            return false;
        }
        else
        {
            Debug.Log("Carregado Com Sucesso");
            Despesa.current.dataToSave.ConvertToItems(itemsData);
            Despesa.current.inputsToSave.ConvertInputs(inputsData);
            return true;
        }
    }

}

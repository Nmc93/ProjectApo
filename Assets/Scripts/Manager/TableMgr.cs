using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class TableMgr : MgrBase
{
    public static TableMgr instance;

    /// <summary> csv������ ���� ���� </summary>
    private const string csvPath = "Assets\\Resources\\TableCSV\\{0}.csv";
    /// <summary> ���̳ʸ� ������ ���� ���� </summary>
    private const string binaryPath = "TableBytes\\{0}";

    /// <summary> ���̺� ������ </summary>
    private Dictionary<string, TableData> dicTable = new Dictionary<string, TableData>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        //���̺� ����
        SetTableDatas();
    }

    #region ���̺� �ε�
    /// <summary> ���̺� ���� </summary>
    private void SetTableDatas()
    {
        //Assets\\Resources\\TableBytes\\StringTableData.bytes
        //���̺� �����͸� ����

        //StringTableData ����
        dicTable.Add("StringTableData", LoadTable<StringTableData>());
        
        //OptionTableData ����
        dicTable.Add("OptionTableData", LoadTable<OptionTableData>());

        //OptionTableData ����
        dicTable.Add("SoundTableData", LoadTable<SoundTableData>());

        //InputKeyTableData ����
        dicTable.Add("InputKeyTableData", LoadTable<InputKeyTableData>());
    }

    /// <summary> ������ ���̺��� �ε� </summary>
    /// <typeparam name="T"> ���̺� Ŭ������ ���� </typeparam>
    private TableData LoadTable<T>() where T : TableBase
    {
        //�ش� Ÿ���� ���̺� ������
        TableData tableData = null;
        //��� ����
        string path = string.Format(binaryPath, typeof(T).ToString());

        //��ο� �ִ� ������ �ε�
        TextAsset textAsset = Resources.Load<TextAsset>(path);

        if (textAsset != null)
        {
            //����ȭ�� Ŭ������ ������ȭ
            using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                tableData = bf.Deserialize(stream) as TableData;
            }
        }

        //���̺��� ã�� �� ���� ���
        if (tableData == null)
        {
            Debug.LogError(textAsset == null ? $"{typeof(T)}�� ����ȭ�� �����͸� ã�� �� �����ϴ�." : $"{typeof(T)}�� ������ȭ�� �����߽��ϴ�.");
        }

        return tableData;
    }

    #endregion ���̺� �ε�

    #region public ��ƿ
    /// <summary> ������ ���̺��� ���� ��ȯ </summary>
    /// <typeparam name="T">������ ���̺�</typeparam>
    /// <param name="key"> ���ϴ� ���̺� �������� ID </param>
    /// <returns> ã�� ���ߴٸ� null ��ȯ </returns>
    public static T Get<T>(object key) where T : TableBase
    {
        Type type = typeof(T);
        if (!instance.dicTable.TryGetValue(type.ToString(), out TableData tbleData))
        {
            Debug.LogError($"{type}�� ���̺� ������ ã�� �� �����ϴ�.");
        }

        TableBase tb = tbleData[key];
        if(tb == null)
        {
            Debug.LogError($"{type}���̺��� {key}�� ID�� ���� ������ ã�� �� �����ϴ�.");
        }

        return tb as T;
    }

    /// <summary> ������ ���̺��� ���� �����ϰ� ���� ���θ� ��ȯ </summary>
    /// <typeparam name="T">������ ���̺�</typeparam>
    /// <param name="key">���ϴ� ���̺� �������� ID</param>
    /// <param name="table">���̺� �����͸� ������ �����</param>
    /// <returns>ã�� ���ߴٸ� false ��ȯ</returns>
    public static bool Get<T>(object key, out T table) where T : TableBase
    {
        Type type = typeof(T);
        if (!instance.dicTable.TryGetValue(type.ToString(), out TableData tbleData))
        {
            Debug.LogError($"{type}�� ���̺� ������ ã�� �� �����ϴ�.");
            table = null;
            return false;
        }

        table = tbleData[key] as T;
        return table != null;
    }
    #endregion public ��ƿ 

    #region private ��ƿ
    /// <summary> ������ Ÿ������ ���� ����ȯ�ؼ� ��ȯ </summary>
    /// <param name="type"> ���� Ÿ�� </param>
    /// <param name="value"> ���̺� </param>
    /// <returns> ���� ����� �������� ������ string���� ��ȯ </returns>
    private object GetValue(string type,string value)
    {
        switch(type)
        {
            case "int":
                return int.Parse(value);
            case "long":
                return long.Parse(value);
            case "string":
                return value;
            case "bool":
                return bool.Parse(value);
            default:
                return value;
        }
    }
    #endregion private ��ƿ
}

#region ���̺� ������
/// <summary> ���̺� ������ </summary>
[Serializable]
public class TableData
{
    public TableData(Dictionary<object, TableBase> dicTable)
    {
        this.dicTable = dicTable;
    }

    public TableBase this[object key]
    {
        get
        {
            if(!dicTable.TryGetValue(key,out TableBase table))
            {
                UnityEngine.Debug.LogError($"{key}�� ���� ���� �����Ͱ� �����ϴ�.");
            }

            return table;
        }
    }

    /// <summary> ���̺� ��ųʸ� </summary>
    public Dictionary<object, TableBase> dicTable = new Dictionary<object, TableBase>();
}
#endregion ���̺� ������
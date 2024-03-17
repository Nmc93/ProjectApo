using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;

    [Header("[����Ʈ ID]")]
    /// <summary> �⺻���� ���õ� �ΰ� ID </summary>
    public static int DefaultHumanID;
    /// <summary> �⺻���� ���õ� ���� ID </summary>
    public static int DefaultZombieID;

    [Header("[Ȱ��ȭ�� ���� ���]"), Tooltip("Ȱ��ȭ�� ���� ���")]
    /// <summary> ������ ���� ��� </summary>
    public static List<Unit> unitList = new List<Unit>();

    [Header("[��Ȱ��ȭ�� ���� ���]"),Tooltip("��Ȱ��ȭ�� ���� ���")]
    public static Queue<Unit> unitPool = new Queue<Unit>();

    /// <summary> �ѹ� ���� ���� ����� ĳ�� </summary>
    private static Dictionary<int, UnitRandomData> dicRandomData = new Dictionary<int, UnitRandomData>();

    /// <summary> ���� ������� ĳ������ UID </summary>
    private static int NextCharUID;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #region ���� ������Ʈ

    /// <summary> ĳ������ ������Ʈ�� �̺�Ʈ </summary>
    public static List<System.Action> charUpdateList = new List<System.Action>();
    private void Update()
    {
        //Ŀ���� ������Ʈ
        foreach(var item in charUpdateList)
        {
            item();
        }
    }

    /// <summary> ������ ������Ʈ �̺�Ʈ�� ��� </summary>
    /// <param name="updateAction"> ������Ʈ �Լ� </param>
    public static void AddUpdateEvent(System.Action updateAction)
    {
        if(!charUpdateList.Contains(updateAction))
        {
            charUpdateList.Add(updateAction);
        }
    }

    /// <summary> ������ ������Ʈ �̺�Ʈ�� ���� </summary>
    /// <param name="updateAction"> ������Ʈ �Լ� </param>
    public static void RemoveUpdateEvent(System.Action updateAction)
    {
        if(charUpdateList.Contains(updateAction))
        {
            charUpdateList.Remove(updateAction);
        }
    }

    #endregion ���� ������Ʈ

    #region ���� ����

    /// <summary> ���̺��� ID�� �´� ������ ���� </summary>
    /// <param name="id"> UnitRandomTableData ���̺� ID </param>
    /// <param name="weaponID"> ���� ID ���� ��� �Ǽ� </param>
    public static void CreateUnit(Vector3 pos, int id,int weaponID = 0)
    {
        UnitData unitData = CreateUnitData(id, weaponID);
        eUnitType unitType = unitData.unitType;

        //��� �ִ� ������ ã�Ƽ� ����, ���ٸ� ����
        if (!GetDeActiveUnit(out Unit unit))
        {
            // ���� ����
            GameObject unitObj = AssetsMgr.LoadResourcesPrefab("Char/Human");
            unitObj.transform.SetParent(instance.transform);
            unit = unitObj.GetComponent<Unit>();

            // ĳ���� ������Ʈ�� �̸��� ������ TID�� ����(������ �����ϴµ� ����� ����)
            unitObj.name = NextCharUID.ToString();
            unit.UID = NextCharUID;
            NextCharUID++;
        }

        //���� ���� ����
        unit.Init(unitData);

        //���� ���� ����
        unit.transform.position = pos;
        //������ ������ Ȱ��ȭ�� ���� ����Ʈ�� ����
        unitList.Add(unit);
    }

    /// <summary> ���ֵ����͸� �����ؼ� ��ȯ </summary>
    public static UnitData CreateUnitDate(int id, int weaponID = 0)
    {
       return CreateUnitData(id, weaponID);
    }

    #endregion ���� ����

    #region Ǯ�� ����� ���� ��ȯ

    /// <summary> Ǯ �ȿ� ��Ȱ��ȭ �� ������ �ִٸ� �����ϰ� ���� ���θ� ��ȯ </summary>
    /// <param name="unit"> ã�� ������ ������ ���� ���� </param>
    /// <returns> ��Ȱ��ȭ�� ������ ���忡 �����Ѵٸ� True ��ȯ </returns>
    private static bool GetDeActiveUnit(out Unit unit)
    {
        unit = null;

        if (unitPool.Count > 0)
        {
            unit = unitPool.Dequeue();
        }

        return unit != null;
    }
    #endregion Ǯ�� ����� ���� ��ȯ

    #region ���� ������ ����

    /// <summary> �������̺��� ID�� ������� ������ ����, ��ȯ </summary>
    /// <param name="unitRanID"> UnitRandomTable�� ID </param>
    /// <returns> �ش� ID�� ������ ���� ��� null ��ȯ </returns>
    private static UnitData CreateUnitData(int unitRanID, int weaponID = 0)
    {
        UnitData unitData = null;

        //�ش� ID�� ���������� �˻� - ���� ��� ���� �� ĳ��
        if(!dicRandomData.TryGetValue(unitRanID, out UnitRandomData ranData))
        {
            if (TableMgr.Get(unitRanID, out UnitRandomTableData tbl))
            {
                ranData = new UnitRandomData(tbl);
                dicRandomData.Add(unitRanID, ranData);
            }
            else
            {
                Debug.LogError($"[{unitRanID}]�� ID�� ���� ���������͸� ���̺��� ã�� �� �����ϴ�.");
                return null;
            }
        }

        int[] stats = ranData.GetRanStats;
        //�����͸� �������� ����
        unitData = new UnitData(
            ranData.unitType,
            ranData.GetRanHeads,
            ranData.GetRanHat,
            ranData.GetRanHair,
            ranData.GetRanBackHair,
            ranData.GetRanHeadAnim,
            ranData.GetRanFaceDeco,
            ranData.GetRanBodyAnim,
            stats[0],               //��
            stats[1],               //��
            stats[2],               //��
            (float)stats[3] / 100,  //����
            (float)stats[4] / 100,  //�̼�
            stats[5],               //Ž������
            weaponID);              //����

        return unitData;
    }

    #endregion ���� ������ ����

    #region Get

    /// <summary> �ش� TID�� ���� ������ Ÿ���� ��ȯ </summary>
    /// <param name="tid"> ������ TID </param>
    /// <returns> TID�� ���� ��� eUnitType.None ��ȯ </returns>
    public static eUnitType GetUnitType(int tid)
    {
        //�ε����� �˻�
        for(int i = 0; i < unitList.Count; ++i)
        {
            if(unitList[i].UID == tid)
            {
                return unitList[i].data.unitType;
            }
        }

        return eUnitType.None;
    }

    #endregion Get
}

#region ������ Ŭ����

/// <summary> ���� ���� ������ </summary>
public class UnitRandomData
{
    public UnitRandomData(UnitRandomTableData tbl)
    {
        if (tbl == null)
        {
            Debug.LogError("���ַ������̺��� null�Դϴ�.");
            return;
        }

        id = tbl.ID;
        unitType = tbl.UnitType;

        heads = Convert(tbl.Head);
        hairs = Convert(tbl.Hair);
        backHairs = Convert(tbl.BackHair);
        faceDecos = Convert(tbl.FaceDeco);
        hats = Convert(tbl.Hat);
        headAnims = Convert(tbl.HeadAnim);
        bodyAnims = Convert(tbl.BodyAnim);
        stats = Convert(tbl.Stat);
    }

    public int[] Convert(string str)
    {
        string[] strs = str.Split("/");
        int[] result = new int[strs.Length];

        int value;
        for(int i = 0; i < strs.Length; ++i)
        {
            if(!int.TryParse(strs[i],out value))
            {
                Debug.LogError($"{strs[i]}�� int�� ��ȯ�� �� ��� ���̺��� ������ �ʿ��մϴ�.");
                value = 0;
            }
            result[i] = value;
        }
        return result;
    }

    public int id;
    public int unitType;

    public int[] heads;
    public int[] hairs;
    public int[] backHairs;
    public int[] faceDecos;
    public int[] hats;
    public int[] headAnims;
    public int[] bodyAnims;
    public int[] stats;

    public int GetRanHeads => heads[Random.Range(0, heads.Length)];
    public int GetRanHair => hairs[Random.Range(0, hairs.Length)];
    public int GetRanBackHair => backHairs[Random.Range(0, backHairs.Length)];
    public int GetRanFaceDeco => faceDecos[Random.Range(0, faceDecos.Length)];
    public int GetRanHat => hats[Random.Range(0, hats.Length)];
    public int GetRanHeadAnim => headAnims[Random.Range(0, headAnims.Length)];
    public int GetRanBodyAnim => bodyAnims[Random.Range(0, bodyAnims.Length)];

    /// <summary> [0 : ��]<br/>[1 : ��]<br/>[2 : ��]<br/>[3 : ����]<br/>[4 : �̼�] </summary>
    public int[] GetRanStats
    {
        get
        {
            if (!TableMgr.Get(Random.Range(0, stats.Length), out UnitStatTableData tbl))
            {
                return null;
            }

            int[] statArray = new int[6];

            statArray[0] = Random.Range(tbl.MinHp, tbl.MaxHp);                  //ü��
            statArray[1] = Random.Range(tbl.MinDmg, tbl.MaxDmg);                //���ݷ�
            statArray[2] = Random.Range(tbl.MinDef, tbl.MaxDef);                //����
            statArray[3] = Random.Range(tbl.MinAttSpeed, tbl.MaxAttSpeed);      //���ݼӵ�
            statArray[4] = Random.Range(tbl.MinMoveSpeed, tbl.MaxMoveSpeed);    //�̵��ӵ�
            statArray[5] = Random.Range(tbl.MinSearchSize, tbl.MaxSearchSize);  //Ž������

            return statArray;
        }
    }
}

#endregion ������ Ŭ����
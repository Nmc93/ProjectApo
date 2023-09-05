using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;
using System;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;


    [Header("[����Ʈ ID]")]
    /// <summary> �⺻���� ���õ� �ΰ� ID </summary>
    public static int DefaultHumanID;
    /// <summary> �⺻���� ���õ� ���� ID </summary>
    public static int DefaultZombieID;

    /// <summary> ������ ���� ��� </summary>
    public static List<Unit> unitList = new List<Unit>();

    /// <summary> ��Ȱ�� ���� ��� </summary>
    private static Queue<Unit> unitPool = new Queue<Unit>();
    /// <summary> ��Ȱ�� ���� ��� </summary>
    private static Queue<Unit> zombiePool = new Queue<Unit>();

    /// <summary> �ѹ� ���� ���� ����� ĳ�� </summary>
    private static Dictionary<int, UnitRandomData> dicRandomData = new Dictionary<int, UnitRandomData>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #region ���� ����

    ///// <summary> �⺻���� ������ ���� ���� </summary>
    //public static void CreateDefaultUnit(eUnitType unitType)
    //{
    //    //�⺻ �ΰ�, ���� Ÿ�� ID ����
    //    int unitID = unitType == eUnitType.Human ? 0 : 1;
    //
    //    CreateUnit(unitID);
    //}

    /// <summary> ���̺��� ID�� �´� ������ ���� </summary>
    public static void CreateUnit(int id)
    {
        UnitData unitData = CreateUnitData(id);
        eUnitType unitType = unitData.unitType;

        //��� �ִ� ������ ã�Ƽ� ����, ���ٸ� ����
        if (!GetDeActiveUnit(unitType,out Unit unit))
        {
            string path = unitType == eUnitType.Human ? "Char/Human" : "Char/Zombie";
            GameObject unitObj = Instantiate(AssetsMgr.LoadResourcesPrefab(path));
            unitObj.transform.SetParent(instance.transform);
        }

        //unit.Init(unitData,);

        unitList.Add(unit);
    }

    #endregion ���� ����

    #region Ǯ�� ����� ���� ��ȯ

    /// <summary> Ǯ �ȿ� ��Ȱ��ȭ �� ������ �ִٸ� �����ϰ� ���� ���θ� ��ȯ </summary>
    /// <param name="unitType"> ��ȯ�� ������ Ÿ�� </param>
    /// <param name="unit"> ã�� ������ ������ ���� ���� </param>
    /// <returns> ��Ȱ��ȭ�� ������ ���忡 �����Ѵٸ� True ��ȯ </returns>
    private static bool GetDeActiveUnit(eUnitType unitType, out Unit unit)
    {
        unit = null;

        switch (unitType)
        {
            case eUnitType.Human:
                {
                    if (unitPool.Count > 0)
                    {
                        unit = unitPool.Dequeue();
                    }
                }
                break;
            case eUnitType.Zombie:
                {
                    if (zombiePool.Count > 0)
                    {
                        unit = zombiePool.Dequeue();
                    }
                }
                break;
        }

        return unit != null;
    }
    #endregion Ǯ�� ����� ���� ��ȯ

    /// <summary> �������̺��� ID�� ������� ������ ����, ��ȯ </summary>
    /// <param name="unitRanID"> UnitRandomTable�� ID </param>
    /// <returns> �ش� ID�� ������ ���� ��� null ��ȯ </returns>
    private static UnitData CreateUnitData(int unitRanID)
    {
        UnitData unitData = null;

        Func<string,List<int>> CreateIntList = item =>
        {
            string[] strs = item.Split("/");
            List<int> list = new List<int>();

            if (strs.Length != 0 && strs[0] != "0")
            {
                for(int i = 0; i < strs.Length; ++i)
                {
                    if(int.TryParse(strs[i],out int result))
                    {
                        list.Add(result);
                    }
                    else
                    {
                        Debug.LogError($"{strs[i]}�� int�� Ÿ�Ժ����� �� �����ϴ�.");
                    }
                }

                return list;
            }
            else
            {
                return null;
            }
        };

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
            }
        }

        //�����͸� �������� ����

        return unitData;
    }

    /// <summary> ������ ���� ���� ���� �� ��ȯ </summary>
    /// <param name="unitType"> ������ ������ ���� </param>
    /// <returns> ���� �����͸� ��ȯ </returns>
    private static UnitData CreateRandomUnitData(eUnitType unitType)
    {
        UnitData unitData = new UnitData();

        switch(unitType)
        {
            case eUnitType.Human:
                {
                }
                break;
            case eUnitType.Zombie:
                {
                }
                break;
        }

        return null;
    }
}

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

        hairs = Convert(tbl.Hair);
        backHairs = Convert(tbl.BackHair);
        faces = Convert(tbl.Face);
        faceDecos = Convert(tbl.FaceDeco);
        hats = Convert(tbl.Hat);
        bodys = Convert(tbl.Body);
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
                Debug.LogError("{strs[i]}�� int�� ��ȯ�� �� ��� ���̺��� ������ �ʿ��մϴ�.");
                value = 0;
            }
            result[i] = value;
        }
        return result;
    }

    public int id;
    public int unitType;
    public int hairCount => hairs.Length;
    public int backHairCount => backHairs.Length;
    public int faceCount => faces.Length;
    public int faceDecoCount => faceDecos.Length;
    public int hatCount => hats.Length;
    public int bodyCount => bodys.Length;
    public int statCount => stats.Length;

    public int[] hairs;
    public int[] backHairs;
    public int[] faces;
    public int[] faceDecos;
    public int[] hats;
    public int[] bodys;
    public int[] stats;
}
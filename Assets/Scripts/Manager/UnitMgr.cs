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

    /// <summary> ���̺��� ID�� �´� ������ ���� </summary>
    /// <param name="id"> UnitRandomTableData ���̺� ID </param>
    /// <param name="weaponID"> ���� ID ���� ��� �Ǽ� </param>
    public static void CreateUnit(Vector3 pos, int id,int weaponID = 0)
    {
        UnitData unitData = CreateUnitData(id);
        eUnitType unitType = unitData.unitType;

        //��� �ִ� ������ ã�Ƽ� ����, ���ٸ� ����
        if (!GetDeActiveUnit(unitType,out Unit unit))
        {
            //���� ����
            string path = unitType == eUnitType.Human ? "Char/Human" : "Char/Zombie";
            GameObject unitObj = AssetsMgr.LoadResourcesPrefab(path);
            unitObj.transform.SetParent(instance.transform);

            unit = unitObj.GetComponent<Unit>();
        }

        //���� ���� ����
        unit.Init(unitData);

        //���� ���� ����
        unit.transform.position = pos;
        //������ ������ Ȱ��ȭ�� ���� ����Ʈ�� ����
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
            ranData.GetRanFace,
            ranData.GetRanFaceDeco,
            ranData.GetRanBody,
            stats[0],
            stats[1],
            stats[2],
            (float)stats[3] / 100,
            (float)stats[4] / 100);

        return unitData;
    }
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

    public int[] heads;
    public int[] hairs;
    public int[] backHairs;
    public int[] faces;
    public int[] faceDecos;
    public int[] hats;
    public int[] bodys;
    public int[] stats;

    public int GetRanHeads => heads[Random.Range(0, heads.Length)];
    public int GetRanHair => hairs[Random.Range(0, hairs.Length)];
    public int GetRanBackHair => backHairs[Random.Range(0, backHairs.Length)];
    public int GetRanFace => faces[Random.Range(0, faces.Length)];
    public int GetRanFaceDeco => faceDecos[Random.Range(0, faceDecos.Length)];
    public int GetRanHat => hats[Random.Range(0, hats.Length)];
    public int GetRanBody => bodys[Random.Range(0, bodys.Length)];

    /// <summary> [0 : ��]<br/>[1 : ��]<br/>[2 : ��]<br/>[3 : ����]<br/>[4 : �̼�] </summary>
    public int[] GetRanStats
    {
        get
        {
            if (!TableMgr.Get(Random.Range(0, stats.Length), out UnitStatTableData tbl))
            {
                return null;
            }

            int[] statArray = new int[5];

            statArray[0] = Random.Range(tbl.MinHp, tbl.MaxHp);              //ü��
            statArray[1] = Random.Range(tbl.MinDmg, tbl.MaxDmg);            //���ݷ�
            statArray[2] = Random.Range(tbl.MinDef, tbl.MaxDef);            //����
            statArray[3] = Random.Range(tbl.MinAttSpeed, tbl.MaxAttSpeed);  //���ݼӵ�
            statArray[4] = Random.Range(tbl.MinMoveSpeed, tbl.MaxMoveSpeed);//�̵��ӵ�

            return statArray;
        }
    }
}

#endregion ������ Ŭ����
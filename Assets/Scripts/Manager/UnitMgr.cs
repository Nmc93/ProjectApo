using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;

    #region �ν�����

    [Header("[����Ʈ ID]")]
    /// <summary> �⺻���� ���õ� �ΰ� ID </summary>
    public static int DefaultHumanID;
    /// <summary> �⺻���� ���õ� ���� ID </summary>
    public static int DefaultZombieID;

    [Header("[Ȱ��ȭ�� ���� ���]"), Tooltip("Ȱ��ȭ�� ���� ���")]
    /// <summary> ������ ���� ��� </summary>
    public static Dictionary<int, Unit> activeUnits = new Dictionary<int, Unit>();

    [Header("[��Ȱ��ȭ�� ���� ���]"),Tooltip("��Ȱ��ȭ�� ���� ���")]
    public static Queue<Unit> unitPool = new Queue<Unit>();

    /// <summary> Ȱ��ȭ�� ������ �θ� TF </summary>
    private static Transform activeUnitParent;
    /// <summary> ��Ȱ��ȭ�� ������ �θ� TF </summary>
    private static Transform deactiveUnitParent;

    #endregion �ν�����

    #region ����

    /// <summary> �ѹ� ���� ���� ����� ĳ�� </summary>
    private static Dictionary<int, UnitRandomData> dicRandomData = new Dictionary<int, UnitRandomData>();

    /// <summary> ���� ������� ĳ������ UID </summary>
    private static int NextCharUID;

    /// <summary> ��� ������� ���� �̺�Ʈ Ǯ </summary>
    private static Queue<UnitEventData> unitEventPool = new Queue<UnitEventData>(50);

    #endregion ����

    #region �������̵�, �⺻ ����

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
        activeUnitParent = new GameObject("ActiveUnit").transform;
        activeUnitParent.SetParent(transform);
        deactiveUnitParent = new GameObject("DeactiveUnit").transform;
        deactiveUnitParent.SetParent(transform);
    }

    #endregion �������̵�, �⺻ ����

    #region ���� ������Ʈ

    /// <summary> ĳ������ ������Ʈ�� �̺�Ʈ </summary>
    public static List<System.Action> charUpdateList = new List<System.Action>();
    /// <summary> ������ �̺�Ʈ ��� </summary>
    private static Queue<System.Action> delActions = new Queue<System.Action>();
    private void Update()
    {
        //Ŀ���� ������Ʈ
        foreach(var item in charUpdateList)
        {
            item();
        }

        //�̺�Ʈ ����
        while (delActions.Count > 0)
        {
            charUpdateList.Remove(delActions.Dequeue());
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
        delActions.Enqueue(updateAction);
    }

    #endregion ���� ������Ʈ

    #region ���� ����

    /// <summary> ���̺��� ID�� �´� ������ ���� </summary>
    /// <param name="id"> UnitRandomTableData ���̺� ID </param>
    /// <param name="weaponID"> ���� ID ���� ��� �Ǽ� </param>
    public static void CreateUnit(Vector3 pos, int id, int weaponID = 0)
    {
        UnitData unitData = CreateUnitData(id, weaponID);
        eUnitType unitType = unitData.unitType;

        //���� ȣ��, ���� - ��� �ִ� ������ ã�Ƽ� ����, ���ٸ� ����
        if (GetUnitFromPool(out Unit unit) == false)
        {
            // ���� ����
            GameObject unitObj = AssetsMgr.LoadResourcesPrefab("Char/Human");
            unitObj.transform.SetParent(activeUnitParent);
            unit = unitObj.GetComponent<Unit>();
        }

        //���� UID ���� - �ߺ� �˻� �� ����
        while (true)
        {
            //���� ��ȣ�� ��쿡 ���
            if (activeUnits.ContainsKey(NextCharUID) == false)
            {
                unit.gameObject.name = NextCharUID.ToString();
                unit.UID = NextCharUID++;
                break;
            }
            else
            {
                ++NextCharUID;
            }
        }

        //���� ���� ����
        unit.Init(unitData);

        //���� ���� ����
        unit.transform.position = pos;

        //������ ������ Ȱ��ȭ�� ���� ����Ʈ�� ����
        activeUnits.Add(unit.UID, unit);
    }

    #endregion ���� ����

    #region ���� Ǯ Dequeue, Enqueue

    /// <summary> Ǯ�� ������ �ִٸ� ������ Ǯ���� ������ ��ȯ </summary>
    /// <param name="unit"> ã�� ������ ������ ���� ���� </param>
    /// <returns> ��Ȱ��ȭ�� ������ ���忡 �����Ѵٸ� True ��ȯ </returns>
    private static bool GetUnitFromPool(out Unit unit)
    {
        unit = null;

        if (unitPool.Count > 0)
        {
            unit = unitPool.Dequeue();
        }

        return unit != null;
    }

    /// <summary> ������ Ǯ�� ��ȯ </summary>
    public static void ReturnUnitToPool(Unit unit)
    {
        //������ ���°� �������� ��쿡�� ���
        if(unit != null && activeUnits.ContainsKey(unit.UID))
        {
            //������ ������� ���� ��Ͽ��� ����
            activeUnits.Remove(unit.UID);

            //��Ȱ��ȭ�� ������ Ư�� �������� �ű�� ��Ȱ��ȭ
            unit.transform.localPosition = new Vector3(20f,20f,0f);
            unit.gameObject.SetActive(false);
            //��Ȱ��ȭ �������� �̵�
            unit.transform.SetParent(deactiveUnitParent);

            //��Ȱ��ȭ ���� Ǯ�� �̵�
            unitPool.Enqueue(unit);
        }
    }

    #endregion ���� Ǯ Dequeue, Enqueue

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

        //�����͸� �������� ����
        unitData = new UnitData(ranData, weaponID);

        return unitData;
    }

    #endregion ���� ������ ����

    #region ���� �̺�Ʈ Ǯ

    /// <summary> ��� �Ϸ��� �̺�Ʈ �����͸� ��ȯ </summary>
    /// <param name="eventData"> ��� �Ϸ��� �̺�Ʈ ������ </param>
    public static void UnitEventReturn(UnitEventData eventData)
    {
        eventData.DataReset();
        unitEventPool.Enqueue(eventData);
    }

    /// <summary> �� ���� �̺�Ʈ�� ȹ�� </summary>
    /// <returns> Ǯ�� ���� ��� ���� ���� ��ȯ </returns>
    public static UnitEventData GetUnitEvent()
    {
        if(unitEventPool.Count <= 0)
        {
            return new UnitEventData();
        }

        return unitEventPool.Dequeue();
    }

    #endregion ���� �̺�Ʈ Ǯ

    #region Get, Is

    /// <summary> �ش� TID�� ���� ������ Ÿ���� ��ȯ </summary>
    /// <param name="uID"> ������ TID </param>
    /// <returns> TID�� ���� ��� eUnitType.None ��ȯ </returns>
    public static eUnitType GetUnitType(int uID)
    {
        if(activeUnits.TryGetValue(uID, out var unit))
        {
            return unit.data.unitType;
        }

        return eUnitType.None;
    }

    /// <summary> �ش� UID�� ���� ������ ����ִ� ��� </summary>
    /// <returns> �׾��ų� ���� ��� false </returns>
    public static bool IsUnitAlive(int uID)
    {
        //�ش� ������ �����ϰ� ���� ����ִ� ���
        if (activeUnits.TryGetValue(uID, out var unit) &&
            unit.uState != eUnitActionEvent.Die)
        {
            return true;
        }

        return false;
    }

    #endregion Get, Is

    #region ���� ��ȣ�ۿ�

    /// <summary> A ������ B������ ���� </summary>
    public void AttackUnit(int targetUID, int damage)
    {
        //TODO: ���� ���� ó�� �۾� �ʿ�

        //������ �����Ѵٸ� ������ ����
        if (activeUnits.TryGetValue(targetUID, out var unit))
        {
            unit.CurHP -= damage;
        }
    }

    #endregion ���� ��ȣ�ۿ�
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

    /// <summary> [0 : ü��]<br/>[1 : ���ݷ�]<br/>[2 : ����]
    /// <br/>[3 : ���ݼӵ�]<br/>[4 : �̵��ӵ�]<br/>[5 : �����ӵ�]
    /// <br/>[6 : Ž������] </summary>
    public int[] GetRanStats
    {
        get
        {
            if (!TableMgr.Get(Random.Range(0, stats.Length), out UnitStatTableData tbl))
            {
                return null;
            }

            int[] statArray = new int[7];

            statArray[0] = Random.Range(tbl.MinHp, tbl.MaxHp);                          // ü��
            statArray[1] = Random.Range(tbl.MinDmg, tbl.MaxDmg);                        // ���ݷ�
            statArray[2] = Random.Range(tbl.MinDef, tbl.MaxDef);                        // ����
            statArray[3] = Random.Range(tbl.MinAttSpeed, tbl.MaxAttSpeed);              // ���ݼӵ�
            statArray[4] = Random.Range(tbl.MinMoveSpeed, tbl.MaxMoveSpeed);            // �̵��ӵ�
            statArray[5] = Random.Range(tbl.MinxReactionSpeed, tbl.MaxReactionSpeed);   // �����ӵ�
            statArray[6] = Random.Range(tbl.MinDetectionRange, tbl.MaxDetectionRange);  // Ž������

            return statArray;
        }
    }
}

#endregion ������ Ŭ����
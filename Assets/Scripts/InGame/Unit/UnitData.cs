using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

/// <summary> ������ ������ </summary>
[Serializable]
public class UnitData
{
    public UnitData(
        int unitType,
        int headID,
        int hatID,
        int hairID,
        int backHairID,
        int headAnimID,
        int faceDecoID,
        int bodyAnimID,
        int maxHp,
        int attack,
        int defence,
        float attackSpeed,
        float moveSpeed,
        float reActionSpeed,
        int detectionRange,
        int weaponID = 0)
    {
        //������ Ÿ�� ����
        switch(unitType)
        {
            case 0: this.unitType = eUnitType.None; break;
            case 1: this.unitType = eUnitType.Human; break;
            case 2: this.unitType = eUnitType.Zombie; break;
        }

        //���� ����
        this.headID = headID;
        this.hatID = hatID;
        this.hairID = hairID;
        this.backHairID = backHairID;
        this.headAnimID = headAnimID;
        this.faceDecoID = faceDecoID;
        this.bodyAnimID = bodyAnimID;
        
        //���̽� ���� ����
        tbl_MaxHp = maxHp;
        tbl_Attack = attack;
        tbl_Defence = defence;
        tbl_ASpeed = attackSpeed;
        tbl_MSpeed = moveSpeed;
        tbl_RSpeed = reActionSpeed;
        tbl_DetectionRange = detectionRange;

        //���� ����
        weaponTbl = TableMgr.Get<UnitWeaponTableData>(weaponID);

        //���� ����
        RefreshStat(true);
    }

    /// <summary> ���� ������ ���� </summary>
    public void SetWeaponData(int weaponID)
    {
        //���� ������ ����ְų� ��ü ���Ⱑ ����� �ٸ� ��� ����
        if (weaponTbl != null || weaponTbl.ID != weaponID)
        {
            //���� ���� �� ���� �������� ���� ���� ����
            weaponTbl = TableMgr.Get<UnitWeaponTableData>(weaponID);
            RefreshStat();
        }
    }

    /// <summary> ���� Ÿ�� </summary>
    public eUnitType unitType;

    /// <summary> ������ ���̺� ���� </summary>
    public UnitWeaponTableData weaponTbl;
    
    /// <summary> ���� �̸� </summary>
    public string name;

    #region ���� ����
    /// <summary> �Ӹ� Ÿ�� </summary>
    public int headID;
    /// <summary> ���� Ÿ�� </summary>
    public int hatID;
    /// <summary> �Ӹ�ī�� Ÿ�� </summary>
    public int hairID;
    /// <summary> �޸Ӹ� Ÿ�� </summary>
    public int backHairID;
    /// <summary> �� Ÿ�� </summary>
    public int headAnimID;
    /// <summary> �� ��� Ÿ��(����� ��) </summary>
    public int faceDecoID;
    /// <summary> �� Ÿ�� </summary>
    public int bodyAnimID;
    #endregion ���� ���� 

    #region ���� ����

    #region �⺻ ����
    /// <summary> �⺻ - �ִ� ü�� </summary>
    public int tbl_MaxHp;

    /// <summary> �⺻ - ���ݷ� </summary>
    public int tbl_Attack;
    /// <summary> �⺻ - ���� </summary>
    public int tbl_Defence;

    /// <summary> �⺻ - ���ݼӵ� </summary>
    public float tbl_ASpeed;
    /// <summary> �⺻ - �̵��ӵ� </summary>
    public float tbl_MSpeed;
    /// <summary> �⺻ - �����ӵ�(��) </summary>
    public float tbl_RSpeed;

    /// <summary> ������ Ž������ </summary>
    public int tbl_DetectionRange;

    #endregion �⺻ ����

    #region ���� ����

    /// <summary> ���� �ִ� ü�� </summary>
    public int f_MaxHp;
    /// <summary> ���� ü�� </summary>
    public int f_CurHp;
    /// <summary> ���� ���ݷ� </summary>
    public int f_Damage;
    /// <summary> ���� ���� </summary>
    public int f_Defence;

    /// <summary> ���� ���ݼӵ� </summary>
    public float f_ASpeed;
    /// <summary> ���� �̵��ӵ� </summary>
    public float f_MSpeed;
    /// <summary> ���� ���� �ӵ� </summary>
    public float f_RSpeed;

    /// <summary> Ž�� ���� </summary>
    public int f_DetectionRange;

    #endregion ���� ����

    #endregion ���� ����

    /// <summary> ���� �⺻ ������ ������� ���� ��� </summary>
    public void RefreshStat(bool isCreate = false)
    {
        f_MaxHp = tbl_MaxHp;          // �ִ� ü��

        f_Damage = tbl_Attack;                  // + �ѱ� ������
        f_Defence = tbl_Defence;                // + �߰����� ����

        f_ASpeed = tbl_ASpeed;                  // ��� Ư���� ���� ���� ���� �߰�
        f_MSpeed = tbl_MSpeed;                  // ��� Ư���� ���� ���� ���� �߰�
        f_RSpeed = tbl_RSpeed;                  // ��� Ư���� ���� ���� ���� �߰�

        f_DetectionRange = tbl_DetectionRange; // ��� Ư���� ���� ���� ���� �߰�

        // ù ������ ��� ���� ü�� == �ִ� ü��
        if(isCreate)
        {
            f_CurHp = f_MaxHp;
        }
    }
}
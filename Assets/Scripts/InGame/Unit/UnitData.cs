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
        int searchSize,
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
        this.maxHp = maxHp;
        this.attack = attack;
        this.defence = defence;
        this.attackSpeed = attackSpeed;
        this.moveSpeed = moveSpeed;
        this.searchSize = searchSize;

        //���� ����
        weaponTbl = TableMgr.Get<UnitWeaponTableData>(weaponID);

        //���� ����
        RefreshStat();
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
    /// <summary> ���� ü�� </summary>
    public int maxHp;

    /// <summary> ���� ���ݷ� </summary>
    public int attack;
    /// <summary> ���� ���� </summary>
    public int defence;

    /// <summary> ������ ���ݼӵ� </summary>
    public float attackSpeed;
    /// <summary> ������ �̵��ӵ� </summary>
    public float moveSpeed;
    /// <summary> ������ Ž������ </summary>
    public int searchSize;

    /// <summary> �����ӵ� - �� </summary>
    public float reactionSpeed;

    #endregion �⺻ ����

    #region ���� ����

    /// <summary> ���� ���� ü�� </summary>
    public int curMaxHp;
    /// <summary> ���� ���� ü�� </summary>
    public int curHp;
    /// <summary> ���� ���ݷ� </summary>
    public int dmg;
    /// <summary> ���� ���� </summary>
    public int def;

    /// <summary> ���� ���ݼӵ� </summary>
    public float aSpeed;
    /// <summary> ���� �̵��ӵ� </summary>
    public float mSpeed;

    /// <summary> Ž�� ���� </summary>
    public int sSize;

    #endregion ���� ����

    #endregion ���� ����

    /// <summary> ���� �⺻ ������ ������� ���� ��� </summary>
    public void RefreshStat()
    {
        //ü�� ����
        curHp = curMaxHp = maxHp;

        dmg = attack;// + �ѱ� ������
        def = defence;// + �߰����� ����

        aSpeed = attackSpeed; // ��� Ư���� ���� ���� ���� �߰�
        mSpeed = moveSpeed; // ��� Ư���� ���� ���� ���� �߰�

        sSize = searchSize; // ��� Ư���� ���� ���� ���� �߰�
    }
}
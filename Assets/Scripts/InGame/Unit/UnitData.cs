using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

/// <summary> ������ ������ </summary>
[Serializable]
public class UnitData
{
    public UnitData(UnitRandomData ranData, int weaponID = 0)
    {
        //������ Ÿ�� ����
        switch (ranData.unitType)
        {
            case 0: unitType = eUnitType.None; break;
            case 1: unitType = eUnitType.Human; break;
            case 2: unitType = eUnitType.Zombie; break;
        }

        //���� ����
        headID = ranData.GetRanHeads;
        hatID = ranData.GetRanHat;
        hairID = ranData.GetRanHair;
        backHairID = ranData.GetRanBackHair;
        headAnimID = ranData.GetRanHeadAnim;
        faceDecoID = ranData.GetRanFaceDeco;
        bodyAnimID = ranData.GetRanBodyAnim;

        //���̽� ���� ����
        int[] stats = ranData.GetRanStats;
        tbl_MaxHp = stats[0];               //��
        tbl_Attack = stats[1];              //��
        tbl_Defence = stats[2];             //��
        tbl_ASpeed = (float)stats[3] / 100; //����
        tbl_MSpeed = (float)stats[4] / 100; //�̼�
        tbl_RSpeed = (float)stats[5] / 100; //�����ӵ�
        tbl_DetectionRange = stats[6];      //Ž������

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
        // �ִ� ü��
        f_MaxHp = tbl_MaxHp;

        // ���ݷ�
        f_Damage = GetF_Damage();
        // ����
        f_Defence = tbl_Defence;

        // �����ӵ�
        f_RSpeed = tbl_RSpeed;
        // ���ݼӵ�
        f_ASpeed = GetF_ASpeed();
        // �̵��ӵ�
        f_MSpeed = tbl_MSpeed;

        // ��� Ư���� ���� ���� ���� �߰�
        f_DetectionRange = tbl_DetectionRange;

        // ù �����̰ų� ���� ü���� �ִ� ü�º��� ���� ���
        if(isCreate || f_CurHp > f_MaxHp)
        {
            //���� ü�� == �ִ� ü��
            f_CurHp = f_MaxHp;
        }
    }

    #region ����

    /// <summary> ���� ���ݷ��� ��� �� ��ȯ </summary>
    private int GetF_Damage()
    {
        int fDamage = tbl_Attack;

        if (weaponTbl.WeaponType != 0)
        {
            int limit = weaponTbl.Damage / 20;
            int plusDamage = tbl_Attack > limit ? limit : tbl_Attack;
            f_Damage = weaponTbl.Damage + plusDamage;
        }

        return fDamage < 0 ? 0 : fDamage;
    }

    /// <summary> ���� ���ݼӵ��� ��� �� ��ȯ </summary>
    private float GetF_ASpeed()
    {
        float speed = 0;

        //���⸦ ����� ���
        if (weaponTbl.WeaponType != 0)
        {
            speed = f_RSpeed + (weaponTbl.Speed / 100);
            speed -= (speed * tbl_ASpeed);
        }
        //���� �ʾ��� ���
        else
        {
            speed = f_RSpeed - (f_RSpeed * tbl_ASpeed);
        }

        return speed < 0 ? 0 : speed;
    }

    #endregion ����
}

/// <summary> ���,��.���ο��� ���� �̺�Ʈ ������ </summary>
public class UnitEventData
{
    /// <summary> �켱���� </summary>
    public eUnitEventPriority priority;
    /// <summary> ���� ���ֿ��� ���� ��Ȳ Ÿ�� </summary>
    public eUnitSituation eventType;
    /// <summary> ��� �ð� </summary>
    public float waitTime;

    /// <summary> �̺�Ʈ ������ ���� </summary>
    /// <param name="priority"> �켱���� </param>
    /// <param name="eventType"> �̺�Ʈ Ÿ�� </param>
    /// <param name="waitTime"> ���� �� ��� �ð� </param>
    public void SetData(eUnitEventPriority priority, eUnitSituation eventType, float waitTime)
    {
        this.priority = priority;
        this.eventType = eventType;
        this.waitTime = waitTime;
    }

    /// <summary> ������ ���� </summary>
    public void DataReset()
    {
        priority = eUnitEventPriority.WaitState;
        eventType = eUnitSituation.None;
        waitTime = 0;
    }
}
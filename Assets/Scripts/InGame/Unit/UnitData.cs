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
        int hatType,
        int hairType,
        int backHairType,
        int faceType,
        int faceDecoType,
        int bodyType,
        int maxHp,
        int attack,
        int defence,
        float attackSpeed,
        float moveSpeed)
    {
        switch(unitType)
        {
            case 0: this.unitType = eUnitType.None; break;
            case 1: this.unitType = eUnitType.Human; break;
            case 2: this.unitType = eUnitType.Zombie; break;
        }

        this.hatType = hatType;
        this.hairType = hairType;
        this.hairType = hairType;
        this.backHairType = backHairType;
        this.faceType = faceType;
        this.faceDecoType = faceDecoType;
        this.bodyType = bodyType;
        this.maxHp = maxHp;
        this.attack = attack;
        this.defence = defence;
        this.attackSpeed = attackSpeed;
        this.moveSpeed = moveSpeed;
    }

    /// <summary> ���� Ÿ�� </summary>
    public eUnitType unitType;

    /// <summary> ���� Ÿ�� </summary>
    public eWeaponType weaponType;
    /// <summary> ���� ID </summary>
    public int weaponID;
    
    /// <summary> ���� �̸� </summary>
    public string name;
    
    #region ���� ����
    /// <summary> ���� Ÿ�� </summary>
    public int hatType;
    /// <summary> �Ӹ�ī�� Ÿ�� </summary>
    public int hairType;
    /// <summary> �޸Ӹ� Ÿ�� </summary>
    public int backHairType;
    /// <summary> �� Ÿ�� </summary>
    public int faceType;
    /// <summary> �� ��� Ÿ��(����� ��) </summary>
    public int faceDecoType;
    /// <summary> �� Ÿ�� </summary>
    public int bodyType;
    #endregion ���� ���� 

    #region ���� ����
    /// <summary> ���� �ִ� ü�� </summary>
    public int maxHp;
    /// <summary> ���� ���� ü�� </summary>
    public int curHp;

    /// <summary> ���� ���ݷ� </summary>
    public int attack;
    /// <summary> ���� ���� </summary>
    public int defence;

    /// <summary> ������ ���� �ӵ� </summary>
    public float attackSpeed;
    /// <summary> ������ �̵� �ӵ� </summary>
    public float moveSpeed;
    #endregion ���� ����
}
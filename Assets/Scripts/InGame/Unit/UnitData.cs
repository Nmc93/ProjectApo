using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

/// <summary> ������ ������ </summary>
[Serializable]
public class UnitData
{
    /// <summary> ���� Ÿ�� </summary>
    public eUnitType unitType;

    /// <summary> ���� Ÿ�� </summary>
    public eWeaponType weaponType;
    /// <summary> ���� ID </summary>
    public int weaponID;
    
    #region ���� ����
    /// <summary> ���� �̸� </summary>
    public string name;
    /// <summary> ���� Ÿ�� </summary>
    public int hatType;
    /// <summary> �Ӹ�ī�� Ÿ�� </summary>
    public int hairType;
    /// <summary> �޸Ӹ� Ÿ�� </summary>
    public int backHairType;
    /// <summary> ����,������ Ÿ�� </summary>
    public int maskType;
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

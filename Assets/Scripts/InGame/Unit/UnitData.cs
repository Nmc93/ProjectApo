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

    /// <summary> ���� �̸� </summary>
    public string name;

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

}

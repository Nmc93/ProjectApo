using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

/// <summary> 유닛의 데이터 </summary>
[Serializable]
public class UnitData
{
    /// <summary> 유닛 타입 </summary>
    public eUnitType unitType;
    /// <summary> 무기 타입 </summary>
    public eWeaponType weaponType;

    /// <summary> 유닛 이름 </summary>
    public string name;

    /// <summary> 유닛 최대 체력 </summary>
    public int maxHp;
    /// <summary> 유닛 현재 체력 </summary>
    public int curHp;

    /// <summary> 유닛 공격력 </summary>
    public int attack;
    /// <summary> 유닛 방어력 </summary>
    public int defence;

    /// <summary> 유닛의 공격 속도 </summary>
    public float attackSpeed;
    /// <summary> 유닛의 이동 속도 </summary>
    public float moveSpeed;

}

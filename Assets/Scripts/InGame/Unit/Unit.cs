using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    /// <summary> �ش� ������ ���� </summary>
    public UnitData data;

    /// <summary> ���� ������ �ൿ </summary>
    public eUintState state;

    /// <summary> �����Ϳ� �°� ĳ���� ���� ���� </summary>
    public void Init(UnitData data)
    {
        this.data = data;



    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    /// <summary> 해당 유닛의 정보 </summary>
    public UnitData data;

    /// <summary> 현재 유닛의 행동 </summary>
    public eUintState state;

    /// <summary> 데이터에 맞게 캐릭터 최초 세팅 </summary>
    public void Init(UnitData data)
    {
        this.data = data;



    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMgr : MgrBase
{
    public static SaveMgr instance;

    //[Header("�ִ� ����� ����")]
    //[SerializeField] private int saveMaxCount = 3;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}

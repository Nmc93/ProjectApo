using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;
using UnityEngine.UI;
using System;

[Serializable]
public abstract class UIBase : MonoBehaviour
{

    /// <summary> ĵ���� Ÿ�� </summary>
    public abstract eCanvas canvasType { get; }

    /// <summary> �ش� UI�� Ÿ�� </summary>
    public abstract eUI uiType { get; }

    /// <summary> �� ��ȯ�� ����Ǵ��� ���� </summary>
    public bool IsSceneChangeClose { get => isSceneChangeClose; }

    /// <summary> UI ���� ���� </summary>
    public bool IsOpen { get; private set; }

    [Header("---------------- Base ----------------")]
    [Header("[�� ��ȯ�� ����Ǵ��� ����]"),Tooltip("true : �� ����� UI ����\nfalse: �� ����� UI ������")]
    [SerializeField] private bool isSceneChangeClose = true;

    public virtual void Init()
    {

    }

    public virtual void Open()
    {
        IsOpen = true;
        DataSetting();
    }

    /// <summary> ������ ���� �Լ� </summary>
    public virtual void DataSetting()
    {
        Refresh();
    }

    /// <summary> ����� �����Ϳ� �°� UI�� �����ϴ� �Լ� </summary>
    public virtual void Refresh()
    {

    }

    /// <summary> UIMgr�� �ش� UI�� ���Ḧ ��û�ϴ� �Լ� </summary>
    public void Close()
    {
        UIMgr.instance.CloseUI(uiType);
    }

    /// <summary> ������ UI ���Ḧ �����ϴ� �Լ� </summary>
    public virtual void DataClear()
    {
        BackThePool();
    }

    protected virtual void BackThePool()
    {
        IsOpen = false;
        UIMgr.instance.ReturnToUIPool(this);
    }

}

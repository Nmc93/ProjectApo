using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

// 1. Awake�ܿ��� �Ŵ����� �����ϰ� �����
// 2. ������ ������� �Ŵ����� �ʱ�ȭ ��Ŵ(������ ���� ���� ����)

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    /// <summary> �Ŵ��� Ŭ���� ���� </summary>
    public static Dictionary<eMgr, MgrBase> mgrDic = new Dictionary<eMgr, MgrBase>();

    /// <summary> ���۽� ���� �Ŵ��� ���� </summary>
    private void Awake()
    {
        //�̹� �ִٸ� ���� ������Ʈ �ı�
        if(instance != null)
            Destroy(this);

        //�ı��Ұ� ������Ʈ ����
        DontDestroyOnLoad(gameObject);
        instance = this;

        //���̺� �Ŵ��� ����
        GameObject tableMgr = new GameObject();
        tableMgr.name = "TableMgr";
        mgrDic.Add(eMgr.TableMgr, tableMgr.AddComponent<TableMgr>());
        
        //�ɼ� �Ŵ���
        GameObject optionMgrObj = new GameObject();
        optionMgrObj.name = "OptionMgr";
        mgrDic.Add(eMgr.OptionMgr, optionMgrObj.AddComponent<OptionMgr>());

        //���� �Ŵ���
        GameObject soundMgr = new GameObject();
        soundMgr.name = "SoundMgr";
        mgrDic.Add(eMgr.SoundMgr, soundMgr.AddComponent<SoundMgr>());

        //Ű �Է� �Ŵ���
        GameObject inputMgr = new GameObject();
        inputMgr.name = "InputMgr";
        mgrDic.Add(eMgr.InputMgr, inputMgr.AddComponent<InputMgr>());

        //UI �Ŵ��� ����
        GameObject uiMgrObj = new GameObject();
        uiMgrObj.name = "UIMgr";
        mgrDic.Add(eMgr.UIMgr, uiMgrObj.AddComponent<UIMgr>());

        //�� �Ŵ���
        GameObject sceneMgrObj = new GameObject();
        sceneMgrObj.name = "SceneMgr";
        mgrDic.Add(eMgr.SceneMgr, sceneMgrObj.AddComponent<SceneMgr>());

        #region ���� �߰�

        ////���̺� �Ŵ���
        //GameObject saveMgrObj = new GameObject();
        //saveMgrObj.name = "SaveMgr";
        //mgrDic.Add(eMgr.SaveMgr, saveMgrObj.AddComponent<SaveMgr>());

        #endregion ���� �߰�

        //���� ����
        GameStart();
    }

    /// <summary> ���� ���� </summary>
    public void GameStart()
    {
        SceneMgr.instance.OpenCurScene();
    }

    #region Get

    /// <summary> �Ŵ��� Ŭ������ ��ȯ�ϴ� �Լ� </summary>
    /// <param name="type"> �Ŵ����� enum Ÿ�� </param>
    public static MgrBase GetMgr(eMgr type)
    {
        if (!mgrDic.TryGetValue(type, out MgrBase mgr))
        {
            Debug.LogError($"{type}�� �ش��ϴ� �Ŵ����� �����ϴ�.");
            return null;
        }

        return mgr;
    }

    #endregion Get
}
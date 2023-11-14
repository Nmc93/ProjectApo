using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitTestSceneMgr : MonoBehaviour
{
    public static UnitTestSceneMgr instance;

    /// <summary> �Ŵ��� Ŭ���� ���� </summary>
    public static Dictionary<eMgr, MgrBase> mgrDic = new Dictionary<eMgr, MgrBase>();

    /// <summary> ���۽� ���� �Ŵ��� ���� </summary>
    private void Awake()
    {
        //�̹� �ִٸ� ���� ������Ʈ �ı�
        if (instance != null)
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

        //�� �Ŵ���
        GameObject MapMgrObj = new GameObject();
        MapMgrObj.name = "MapMgr";
        mgrDic.Add(eMgr.MapMgr, MapMgrObj.AddComponent<MapMgr>());

        //���� �Ŵ���
        GameObject UnitMgrObj = new GameObject();
        UnitMgrObj.name = "UnitMgr";
        mgrDic.Add(eMgr.UnitMgr, UnitMgrObj.AddComponent<UnitMgr>());
    }

    #region �׽�Ʈ

    [Header("[���� ���� ��ȯ ����]")]
    public bool isCreateUnit = false;
    float count = 2;
    bool btnActive;

    private void Update()
    {
        //���� ���� �׽�Ʈ���� ��쿡�� ���
        if (isCreateUnit)
        {
            if (!btnActive && count >= 2)
            {
                btnActive = true;
                count = 0;
            }
            else if (!btnActive && count < 2)
            {
                count += Time.deltaTime;
            }

            if (btnActive && Input.GetMouseButtonDown(0))
            {
                TestSummonUnit();
            }
        }
    }

    //���� ���� �׽�Ʈ
    public void TestSummonUnit()
    {
        Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v3.z = 0;

        UnitMgr.CreateUnit(v3, 0, 1);
        btnActive = false;
    }

    #endregion �׽�Ʈ
}
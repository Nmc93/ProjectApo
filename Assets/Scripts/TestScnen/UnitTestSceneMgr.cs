using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitTestSceneMgr : MonoBehaviour
{
    public static UnitTestSceneMgr instance;

    /// <summary> 매니저 클래스 저장 </summary>
    public static Dictionary<eMgr, MgrBase> mgrDic = new Dictionary<eMgr, MgrBase>();

    /// <summary> 시작시 최초 매니저 세팅 </summary>
    private void Awake()
    {
        //이미 있다면 현재 오브젝트 파괴
        if (instance != null)
            Destroy(this);

        //파괴불가 오브젝트 지정
        DontDestroyOnLoad(gameObject);
        instance = this;

        //테이블 매니저 세팅
        GameObject tableMgr = new GameObject();
        tableMgr.name = "TableMgr";
        mgrDic.Add(eMgr.TableMgr, tableMgr.AddComponent<TableMgr>());

        //옵션 매니저
        GameObject optionMgrObj = new GameObject();
        optionMgrObj.name = "OptionMgr";
        mgrDic.Add(eMgr.OptionMgr, optionMgrObj.AddComponent<OptionMgr>());

        //사운드 매니저
        GameObject soundMgr = new GameObject();
        soundMgr.name = "SoundMgr";
        mgrDic.Add(eMgr.SoundMgr, soundMgr.AddComponent<SoundMgr>());

        //키 입력 매니저
        GameObject inputMgr = new GameObject();
        inputMgr.name = "InputMgr";
        mgrDic.Add(eMgr.InputMgr, inputMgr.AddComponent<InputMgr>());

        //맵 매니저
        GameObject MapMgrObj = new GameObject();
        MapMgrObj.name = "MapMgr";
        mgrDic.Add(eMgr.MapMgr, MapMgrObj.AddComponent<MapMgr>());

        //유닛 매니저
        GameObject UnitMgrObj = new GameObject();
        UnitMgrObj.name = "UnitMgr";
        mgrDic.Add(eMgr.UnitMgr, UnitMgrObj.AddComponent<UnitMgr>());
    }

    #region 테스트

    [Header("[랜덤 유닛 소환 여부]")]
    public bool isCreateUnit = false;
    float count = 2;
    bool btnActive;

    private void Update()
    {
        //유닛 생성 테스트중일 경우에만 사용
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

    //유닛 생성 테스트
    public void TestSummonUnit()
    {
        Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v3.z = 0;

        UnitMgr.CreateUnit(v3, 0, 1);
        btnActive = false;
    }

    #endregion 테스트
}

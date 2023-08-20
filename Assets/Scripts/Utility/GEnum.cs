using System;

namespace GEnum
{
    #region 게임 매니저
    /// <summary> 매니저에 지정된 enum <br/> 
    /// [매니저 클래스와 이름이 동일함] </summary>
    public enum eMgr
    {
        SceneMgr = 0,
        UIMgr,
        SaveMgr,
        TableMgr,
        OptionMgr,
        InputMgr,
        SoundMgr,
        MapMgr,
    }
    #endregion 게임 매니저

    #region UI 관련
    /// <summary> 씬 타입, 씬 이름과 동일 </summary>
    public enum eScene
    {
        LobbyScene = 0,
        GameScene,
    }

    /// <summary> 캔버스 타입 </summary>
    public enum eCanvas : byte
    {
        Scene = 0,
        Page,
        Popup
    }

    /// <summary> UI를 쉽게 찾기 위해 지정된 enum <br/> 
    /// [UI 프리팹의 대표 컴포넌트와 이름이 동일함] </summary>
    public enum eUI : short
    {
        UILoading = 0,
        UILobby,
    }

    public enum ePage
    {
        
    }
    #endregion UI 관련

    #region 로딩
    /// <summary> 로딩 상태 </summary>
    public enum eLoadingState
    {
        /// <summary> 씬 변경 진행중이 아님 </summary>
        None,
        /// <summary> UI 종료 시작 </summary>
        CloseCurScene,
        /// <summary> 씬 변경 시작 </summary>
        SceneChange,
        /// <summary> 씬 변경 대기 </summary>
        WaitChangeScene
    }
    #endregion 로딩

    #region 입력 타입

    public enum eInputType
    {
        /// <summary> 다음 씬 이동 </summary>
        MoveNextScene = 0,
        /// <summary> 키의 마지막, 사용하지 않음 </summary>
        Count
    }

    #endregion 입력 타입

    #region 사운드
    public enum eSoundType
    {
        /// <summary> 타입없음, 재생불가 </summary>
        None = 0,
        /// <summary> 배경음(전체) </summary>
        BGM,
        /// <summary> 게임 시스템 관련 사운드(전체) </summary>
        System,
        /// <summary> 게임 관련 이펙트 사운드(위치 지정, 3D) </summary>
        Effect,
    }
    #endregion 사운드

    #region 유닛

    /// <summary> 유닛의 타입 </summary>
    public enum eUnitType
    {
        /// <summary> 내게 속해있는 유닛 </summary>
        MyUnit,
        /// <summary> 적 유닛 </summary>
        Enemy,
        /// <summary> 중립, 우호 유닛 </summary>
        Npc,
    }

    /// <summary> 유닛의 무기 타입 </summary>
    public enum eWeaponType
    {
        /// <summary> 맨손 </summary>
        Hand,
        /// <summary> 한손총 </summary>
        Gun,
        /// <summary> 소총 </summary>
        LongGun,
    }

    #endregion 유닛
}
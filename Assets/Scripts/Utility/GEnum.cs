using System;

namespace GEnum
{
    #region ���� �Ŵ���
    /// <summary> �Ŵ����� ������ enum <br/> 
    /// [�Ŵ��� Ŭ������ �̸��� ������] </summary>
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
        UnitMgr,
    }
    #endregion ���� �Ŵ���

    #region UI ����
    /// <summary> �� Ÿ��, �� �̸��� ���� </summary>
    public enum eScene
    {
        LobbyScene = 0,
        GameScene,
    }

    /// <summary> ĵ���� Ÿ�� </summary>
    public enum eCanvas : byte
    {
        Scene = 0,
        Page,
        Popup
    }

    /// <summary> UI�� ���� ã�� ���� ������ enum <br/> 
    /// [UI �������� ��ǥ ������Ʈ�� �̸��� ������] </summary>
    public enum eUI : short
    {
        UILoading = 0,
        UILobby,
    }

    public enum ePage
    {
        
    }
    #endregion UI ����

    #region �ε�
    /// <summary> �ε� ���� </summary>
    public enum eLoadingState
    {
        /// <summary> �� ���� �������� �ƴ� </summary>
        None,
        /// <summary> UI ���� ���� </summary>
        CloseCurScene,
        /// <summary> �� ���� ���� </summary>
        SceneChange,
        /// <summary> �� ���� ��� </summary>
        WaitChangeScene
    }
    #endregion �ε�

    #region �Է� Ÿ��

    public enum eInputType
    {
        /// <summary> ���� �� �̵� </summary>
        MoveNextScene = 0,
        /// <summary> Ű�� ������, ������� ���� </summary>
        Count
    }

    #endregion �Է� Ÿ��

    #region ����
    public enum eSoundType
    {
        /// <summary> Ÿ�Ծ���, ����Ұ� </summary>
        None = 0,
        /// <summary> �����(��ü) </summary>
        BGM,
        /// <summary> ���� �ý��� ���� ����(��ü) </summary>
        System,
        /// <summary> ���� ���� ����Ʈ ����(��ġ ����, 3D) </summary>
        Effect,
    }
    #endregion ����

    #region ����

    /// <summary> ������ ���� Ÿ�� </summary>
    public enum eUnitType
    {
        /// <summary> ���� Ÿ�� </summary>
        None,
        /// <summary> �ΰ� </summary>
        Human,
        /// <summary> ���� </summary>
        Zombie,
    }

    /// <summary> ������ ���� Ÿ�� </summary>
    public enum eWeaponType
    {
        /// <summary> �Ǽ� </summary>
        Hand,
        /// <summary> �Ѽ��� </summary>
        Gun,
        /// <summary> ���� </summary>
        LongGun,
    }

    /// <summary>������ �ൿ Ÿ�� </summary>
    public enum eUintState
    {
        /// <summary> ��� </summary>
        Idle,
        /// <summary> �̵� </summary>
        Move,
        /// <summary> ��� </summary>
        Alert,
        /// <summary> ���� </summary>
        Attack,
    }

    /// <summary> ������ ��ȣ�ۿ� �̺�Ʈ </summary>
    public enum eUnitActionEvent : byte
    {
        NoEvent = 0,
        EnemySearch,
    }

    #endregion ����

    #region ��Ʋ��, ��������Ʈ
    public enum eAtlasType
    {
        /// <summary> �ΰ� ĳ���� ���� �̹��� ��Ʋ�� </summary>
        Unit_Human = 0,
    }
    #endregion ��Ʋ��, ��������Ʈ
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GEnum;

public class SoundMgr : MgrBase
{
    public static SoundMgr instance;

    /// <summary> Ŭ�� ���� <br/> [Key : ID(SoundTable)] <br/> [Value : AudioClip] </summary>
    public static Dictionary<int, List<SoundCell>> dicSoundClip = new Dictionary<int, List<SoundCell>>();

    /// <summary> ��� ���� �ҽ� ����� </summary>
    public static SoundCell curBGMCell;

    /// <summary> ���� ���� ��� </summary>
    private const string path = "Sound\\";

    #region ���� �ɼ�

    /// <summary> BGM ���� ���Ұ� </summary>
    private bool isBGMMute;
    /// <summary> BGM ���� ���� </summary>
    private float bGMVol;

    /// <summary> �ý��� ���� ���Ұ� </summary>
    private bool isSystemMute;
    /// <summary> �ý��� ���� ���� </summary>
    private float systemVol;

    /// <summary> ���� �� ���� ���Ұ� </summary>
    private bool isEffectMute;
    /// <summary> ���� �� ���� ���� </summary>
    private float effectVol;

    #endregion ���� �ɼ�

    #region ���� �� ����

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

        //���� Ŭ�� ����
        SetSound();
    }

    /// <summary> ���� �� ������ �ε�Ǵ� ���� �̸� �ε� </summary>
    public void SetSound()
    {
        //��� ����
        isBGMMute = OptionMgr.GetBoolOption("Sound_Mute_BGM");
        bGMVol = OptionMgr.GetfloatOption("Sound_Vol_BGM");

        //�ý��� ���� ����
        isSystemMute = OptionMgr.GetBoolOption("Sound_Mute_System");
        systemVol = OptionMgr.GetfloatOption("Sound_Vol_System");

        //����Ʈ ���� ����
        isEffectMute = OptionMgr.GetBoolOption("Sound_Mute_Effect");
        effectVol = OptionMgr.GetfloatOption("Sound_Vol_Effect");
    }

    /// <summary> �ش� ID�� ���带 �ε� �� ��ȯ </summary>
    /// <param name="id"> ������ ID <br/> [SoundTable ����] </param>
    /// <param name="tbl"> ��ȯ�� ���� ���̺� ������ </param>
    /// <returns> ID�� ã�� �� ���� ��쿣 null ��ȯ </returns>
    public static AudioSource CreateAudioSource(int id, out SoundTableData tbl)
    {
        AudioSource source = new GameObject().AddComponent<AudioSource>();

        if (TableMgr.Get(id, out tbl))
        {
            AudioClip clip = Resources.Load($"{path}{tbl.Path}", typeof(AudioClip)) as AudioClip;

            if (clip != null)
            {
                //����Ŭ�� �� ������Ʈ ��ġ ����
                source.clip = clip;
                source.transform.SetParent(instance.transform);
                source.transform.localPosition = Vector3.zero;
                source.transform.localRotation = Quaternion.identity;

                //�ش� ������ Ÿ��
                eSoundType sType = ConvertIntToSoundType(tbl.SoundType);

                #region ���� ������Ʈ �̸� ���� - ������ ����
#if UNITY_EDITOR
                //�����Ϳ����� ���п����� �̸� ����
                source.transform.name = $"{id}_{sType}Sound";
#endif
                #endregion ���� ������Ʈ �̸� ���� - ������ ����
                switch (sType)
                {
                    case eSoundType.BGM:    //���
                        source.loop = tbl.IsLoop;
                        break;
                    case eSoundType.System: //�ý��� ����
                        source.loop = tbl.IsLoop;
                        //source.volume
                        break;
                    case eSoundType.Effect: //���� �� ����
                        source.loop = tbl.IsLoop;
                        break;
                    case eSoundType.None:   //����
                        Debug.LogError($"{id}�� ���� Ÿ���� ���������Դϴ�. SoundTable Ȯ���� �ʿ��մϴ�.");
                        break;
                }

                return source;
            }
            else
            {
                Debug.LogError($"{path}{tbl.Path}�� ��ο��� �ش� Clip�� ã�� �� �����ϴ�.");
                return null;
            }
        }
        else
        {
            Debug.LogError($"{id}�� ID�� ���� ���̺��� ã�� �� �����ϴ�.");
            return null;
        }
    }

    #endregion ���� �� ����

    #region ����

    /// <summary> ���� ���� </summary>
    /// <param name="soundID"> ������ ������ ID </param>
    /// <param name="tf"> ���� ��ġ ���� </param>
    public static void Play(int soundID, Transform tf = null)
    {
        SoundCell playCell = null;

        //1. ID�� ���� ���� �˻�
        //�ȿ� �ش� ID�� ���� ���弿 ����� ���� ���
        if(dicSoundClip.TryGetValue(soundID,out List<SoundCell> list))
        {
            //������ �ʴ� ���� ã��
            for (int i = 0; i < list.Count; ++i)
            {
                if(!list[i].IsPlaying)
                {
                    playCell = list[i];
                    break;
                }
            }

            //��� ���� ���ϰ� ���� ���
            if(playCell == null)
            {
                playCell = new SoundCell(list[0], list.Count);
                list.Add(playCell);
            }
        }
        //�ش� ID�� ���� ���� ���� ���� ���
        else
        {
            //���� �߰�
            playCell = new SoundCell(soundID, 0);
            dicSoundClip.Add(soundID, new List<SoundCell>() { playCell });
        }

        //�̷��µ��� ������ �־� ���� ���� ��� ����
        if (playCell == null)
        {
            Debug.LogError($"[ID : {soundID}] ���� ��¿� �����߽��ϴ�.");
            return;
        }

        //2. ã�� ���带 ���� 
        //��� Ÿ���� ��� ������ �ִ� ����� �����ϰ� �����
        if (playCell.SoundType == eSoundType.BGM)
        {
            //���� �������� ����� ���� ��� ����� �����ϰ� ���� ������� ���
            if (curBGMCell != null)
            {
                Stop(curBGMCell);
                curBGMCell = playCell;
            }
        }

        // ��ġ�� �����Ǿ� ���� ��� ��ġ�� ���� ������ �ʿ�
        if(tf != null)
        {
            // ���� �ڷ�ƾ ����
            instance.StartCoroutine(instance.PlayCoroutine(playCell,tf));
        }
        // �����Ǿ� ���� �ʾ� ������ �ʿ����
        else
        {
            // �׳� ���
            playCell.Play();
        }
    }

    /// <summary> ��ġ ������ ���Ǵ� ���� ���� �ڷ�ƾ </summary>
    /// <param name="cell"> ���Ǵ� ���� </param>
    /// <param name="tf"> ������ ��ġ tf </param>
    IEnumerator PlayCoroutine(SoundCell cell, Transform tf)
    {
        //���� ��ġ ����
        cell.SetPos(tf);
        cell.Play();

        yield return GUtility.GetWaitForSeconds(cell.Time);

        //�� ��ġ �ʱ�ȭ
        cell.Stop();
        cell.SetPos(transform);
    }

    #endregion ����

    #region ����

    /// <summary> ���� ���� </summary>
    /// <param name="cell"> ������ ������ cell </param>
    public static void Stop(SoundCell cell)
    {
        cell.Stop();

        //�ش� ���尡 ���� ������� BGM���� üũ
        if (cell.ID == curBGMCell.ID &&
            cell.idx == curBGMCell.idx)
        {
            curBGMCell = null;
        }
    }

    /// <summary> ���� ���� </summary>
    /// <param name="id"> ������ ID </param>
    /// <param name="idx"> �ش� ���� ����� �ε��� </param>
    public static void Stop(int id, int idx)
    {
        //���� �˻�
        if(dicSoundClip.TryGetValue(id,out List<SoundCell> cells))
        {
            //idx ��ȿ �˻�
            if(cells.Count < idx)
            {
                //����
                Stop(cells[idx]);
            }
            else
            {
                Debug.LogError($"{id}ID�� ���� ��Ͽ��� {idx}��° ����ҽ��� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError($"{id}�� ID�� ���� ���带 ã�� �� �����ϴ�.");
        }
    }

    /// <summary> �ش� ID�� �ɸ� ���� ���� ���� </summary>
    /// <param name="id"> ������ ������ ID </param>
    public static void IDStop(int id)
    {
        //�˻�
        if(dicSoundClip.TryGetValue(id, out List<SoundCell> list))
        {
            //�÷������� ��� ���� ����
            foreach (var cell in list)
            {
                if (cell.IsPlaying)
                {
                    cell.Stop();

                    //���� ������� bgm�� ��� curBGMCell ����
                    if (cell.SoundType == eSoundType.BGM &&
                        cell.ID == curBGMCell.ID && cell.idx == curBGMCell.idx)
                    {
                        curBGMCell = null;
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"{id}�� ID�� ���� ���� �׷��� �����ϴ�.");
        }
    }

    /// <summary> ������ Ÿ���� ���带 ��� ���� </summary>
    /// <param name="type"> ������ Ÿ��, �������� ���� ��� ��� ���尡 ������ </param>
    public static void TypeAllStop(eSoundType type = eSoundType.None)
    {
        //�÷������� ��� ���� ����
        foreach (var group in dicSoundClip)
        {
            //None�̰ų� Ÿ���� ������ ��� Ÿ�԰� ���� Ÿ���� ���� ���
            if (type == eSoundType.None || type == group.Value[0].SoundType)
            {
                foreach (var cell in group.Value)
                {
                    if (cell.IsPlaying)
                    {
                        cell.Stop();

                        //���� ������� bgm�� ��� curBGMCell ����
                        if (cell.SoundType == eSoundType.BGM &&
                            cell.ID == curBGMCell.ID && cell.idx == curBGMCell.idx)
                        {
                            curBGMCell = null;
                        }
                    }
                }
            }
        }
    }

    #endregion ����

    #region ����Ʈ

    /// <summary> Ÿ�Թ�ȣ�� ����Ÿ������ ��ȯ </summary>
    /// <param name="typeNum"> GEnum.eSoundType ���� </param>
    /// <returns> 0 or �������� ���� ��ȣ ���� �� eSoundType.None ��ȯ </returns>
    public static eSoundType ConvertIntToSoundType(int typeNum)
    {
        return typeNum switch
        {
            1 => eSoundType.BGM,
            2 => eSoundType.System,
            3 => eSoundType.Effect,
            _ => eSoundType.None
        };
    }

#endregion ����Ʈ

    #region ������ Ŭ����

    /// <summary> ����� �ҽ� </summary>
    [Serializable]
    public class SoundCell
    {
        /// <summary> ������ ID</summary>
        public int ID { get => tbl.ID; }
        /// <summary> ������ Ÿ�� </summary>
        public eSoundType SoundType { get => (eSoundType)tbl.SoundType; }
        /// <summary> ������ ��� ���� </summary>
        public bool IsPlaying { get => source.isPlaying; }
        /// <summary> ������ ��� �ð� </summary>
        public float Time { get => source.clip.length; }

        /// <summary> ���� ���� �ε��� </summary>
        public int idx;

        /// <summary> �ش� ������ ���̺� ������ </summary>
        private SoundTableData tbl;
        /// <summary> ����� �ҽ� </summary>
        private AudioSource source;

        /// <summary> ID�� Ÿ���� �����ϰ� �׿� �´� ���� �ҽ� ���� </summary>
        /// <param name="id"> ������ ID <br/> [SoundTable ����] </param>
        /// <param name="idx"> ���� ���� �ε��� <br/> SoundMgr.dicSoundClip[i]�� idx </param>
        public SoundCell(int id, int idx)
        {
            source = CreateAudioSource(id, out tbl);
            this.idx = idx;
        }

        /// <summary> �Ű������� ���� ���� ���� ������ Ÿ���� �� ���� </summary>
        /// <param name="cell"> ���� ��� <br/> [SoundTable ����] </param>
        public SoundCell(SoundCell cell, int idx)
        {
            tbl = cell.tbl;
            source = Instantiate(cell.source);
            source.transform.SetParent(instance.transform);
            source.transform.localPosition = Vector3.zero;
            this.idx = idx;
        }

        /// <summary> ��ġ ���� </summary>
        public void SetPos(Transform tf)
        {
            source.transform.SetParent(tf);
            source.transform.position = Vector3.zero;
        }

        /// <summary> ���� </summary>
        public void Play()
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
        }

        /// <summary> ���� </summary>
        public void Stop()
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    #endregion ������ Ŭ����
}
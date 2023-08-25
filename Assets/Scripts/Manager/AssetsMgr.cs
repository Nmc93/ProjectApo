using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class AssetsMgr : MgrBase
{
    public static AssetsMgr instance;

    private void Awake()
    {
        instance = this;
    }

    #region �⺻ ���
    /// <summary> ������ �⺻ ��� </summary>
    private const string UIPrefabPath = "Prefab/";
    /// <summary> ��Ʋ�� �⺻ ��� </summary>
    private const string AtlasPath = "Image/";
    /// <summary> �ִϸ����� ��Ʈ�ѷ� �⺻ ��� </summary>
    private const string AniCtlrPath = "Ani/";
    #endregion �⺻ ���

    #region ������

    /// <summary> ��ο� �ִ� �������� �����ؼ� ��ȯ </summary>
    /// <param name="path"> ������Ʈ ��� </param>
    public static GameObject LoadResourcesPrefab(string path)
    {
        path = $"{UIPrefabPath}{path}";
        GameObject obj = Instantiate(Resources.Load<GameObject>(path));

        if (obj == null)
        {
            Debug.LogError($"�߸��� ����Դϴ�. [{path}]");
        }

        return obj;
    }

    /// <summary> ��ο� �ִ� �������� �����ؼ� obj�� �����ϰ� ���� ���θ� ��ȯ </summary>
    /// <param name="path"> ������Ʈ ��� </param>
    public static bool LoadResourcesPrefab(string path, out GameObject obj)
    {
        path = $"{UIPrefabPath}{path}";
        
        obj = Instantiate(Resources.Load<GameObject>(path));

        if (obj == null)
        {
            Debug.LogError($"�߸��� ����Դϴ�. [{path}]");
        }

        return obj != null;
    }

    #endregion ������

    #region ��Ʋ��

    /// <summary> ��Ʋ�� ���� ��ųʸ� <br/>[Key : ��Ʋ�� �� ��������Ʈ ���] </summary>
    private static Dictionary<string, SpriteAtlas> dicAtlas = new Dictionary<string, SpriteAtlas>();

    /// <summary> �ش� ��Ʋ�� </summary>
    /// <param name="atlasPath"> SpriteAtlas ��� </param>
    /// <param name="spritePath"> SpriteAtlas�� ĳ�̵� Sprite ��� </param>
    public static Sprite GetSprite(string atlasPath, string spritePath)
    {
        //���, ��Ʋ�� ��ųʸ� Ű
        string key = $"{AtlasPath}{atlasPath}";

        if (!dicAtlas.TryGetValue(key, out SpriteAtlas atlas))
        {
            atlas = Resources.Load<SpriteAtlas>(key);

            //�ش� ��ο� ��Ʋ�󽺰� ���� ���
            if(atlas == null)
            {
                Debug.LogError($"{atlasPath}�� ��ο� �ش� ��������Ʈ ��Ʋ�󽺰� �����ϴ�.");
                return null;
            }

            //��Ʋ�� ĳ��
            dicAtlas.Add(key, atlas);
        }

        Sprite sprite = atlas.GetSprite(spritePath);

        if (sprite == null)
        {
            Debug.LogError($"{atlasPath}�� ��Ʋ�󽺿� {spritePath}�� ��ο� �ش� ��������Ʈ�� �����ϴ�.");
            return null;
        }

        return sprite;
    }

    #endregion ��Ʋ��

    #region �ִϸ����� ��Ʈ�ѷ�

    /// <summary> Animator�� ��Ʈ�ѷ��� ��ȯ </summary>
    /// <param name="path"> ��� </param>
    public static RuntimeAnimatorController GetRuntimeAnimatorController(string path)
    {
        path = $"{AniCtlrPath}{path}";

        RuntimeAnimatorController ctlr = Resources.Load<RuntimeAnimatorController>(path); ;

        return ctlr;
    }

    #endregion �ִϸ����� ��Ʈ�ѷ� 
}
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

    #region 기본 경로
    /// <summary> 프리팹 기본 경로 </summary>
    private const string UIPrefabPath = "Prefab/";
    /// <summary> 아틀라스 기본 경로 </summary>
    private const string AtlasPath = "Image/";
    /// <summary> 애니메이터 컨트롤러 기본 경로 </summary>
    private const string AniCtlrPath = "Ani/";
    #endregion 기본 경로

    #region 프리팹

    /// <summary> 경로에 있는 프리팹을 복사해서 반환 </summary>
    /// <param name="path"> 오브젝트 경로 </param>
    public static GameObject LoadResourcesPrefab(string path)
    {
        path = $"{UIPrefabPath}{path}";
        GameObject obj = Instantiate(Resources.Load<GameObject>(path));

        if (obj == null)
        {
            Debug.LogError($"잘못된 경로입니다. [{path}]");
        }

        return obj;
    }

    /// <summary> 경로에 있는 프리팹을 복사해서 obj에 저장하고 성공 여부를 반환 </summary>
    /// <param name="path"> 오브젝트 경로 </param>
    public static bool LoadResourcesPrefab(string path, out GameObject obj)
    {
        path = $"{UIPrefabPath}{path}";
        
        obj = Instantiate(Resources.Load<GameObject>(path));

        if (obj == null)
        {
            Debug.LogError($"잘못된 경로입니다. [{path}]");
        }

        return obj != null;
    }

    #endregion 프리팹

    #region 아틀라스

    /// <summary> 아틀라스 저장 딕셔너리 <br/>[Key : 아틀라스 내 스프라이트 경로] </summary>
    private static Dictionary<string, SpriteAtlas> dicAtlas = new Dictionary<string, SpriteAtlas>();

    /// <summary> 해당 아틀라스 </summary>
    /// <param name="atlasPath"> SpriteAtlas 경로 </param>
    /// <param name="spritePath"> SpriteAtlas에 캐싱된 Sprite 경로 </param>
    public static Sprite GetSprite(string atlasPath, string spritePath)
    {
        //경로, 아틀라스 딕셔너리 키
        string key = $"{AtlasPath}{atlasPath}";

        if (!dicAtlas.TryGetValue(key, out SpriteAtlas atlas))
        {
            atlas = Resources.Load<SpriteAtlas>(key);

            //해당 경로에 아틀라스가 없을 경우
            if(atlas == null)
            {
                Debug.LogError($"{atlasPath}의 경로에 해당 스프라이트 아틀라스가 없습니다.");
                return null;
            }

            //아틀라스 캐싱
            dicAtlas.Add(key, atlas);
        }

        Sprite sprite = atlas.GetSprite(spritePath);

        if (sprite == null)
        {
            Debug.LogError($"{atlasPath}의 아틀라스에 {spritePath}의 경로에 해당 스프라이트가 없습니다.");
            return null;
        }

        return sprite;
    }

    #endregion 아틀라스

    #region 애니메이터 컨트롤러

    /// <summary> Animator의 컨트롤러를 반환 </summary>
    /// <param name="path"> 경로 </param>
    public static RuntimeAnimatorController GetRuntimeAnimatorController(string path)
    {
        path = $"{AniCtlrPath}{path}";

        RuntimeAnimatorController ctlr = Resources.Load<RuntimeAnimatorController>(path); ;

        return ctlr;
    }

    #endregion 애니메이터 컨트롤러 
}
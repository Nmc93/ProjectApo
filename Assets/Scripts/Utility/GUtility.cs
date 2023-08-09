using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUtility
{
    #region �ڷ�ƾ
    /// <summary> �ð��� Ű�� WaitForSeconds�� �����ϴ� ��ųʸ� </summary>
    private static Dictionary<float, WaitForSeconds> dicWaitForSeconds = new Dictionary<float, WaitForSeconds>();
    
    /// <summary> �ð��� �´� WaitForSeconds�� ��ȯ, ������ ���� �� </summary>
    /// <param name="time"> �����ִ� �ð� </param>
    public static WaitForSeconds GetWaitForSeconds(float time)
    {
        if(!dicWaitForSeconds.TryGetValue(time,out WaitForSeconds waitForSeconds))
        {
            waitForSeconds = new WaitForSeconds(time);
            dicWaitForSeconds.Add(time, waitForSeconds);
        }

        return waitForSeconds;
    }
    #endregion �ڷ�ƾ

}

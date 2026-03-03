using UnityEngine;

public class SunView : MonoBehaviour
{
    /// <summary>
    /// Place le Soleil à l'origine du système solaire.
    /// </summary>
    public void Init()
    {
        transform.localPosition = Vector3.zero;
    }
}

using UnityEngine;

public class SunView : MonoBehaviour
{
    // Place le Soleil à l'origine du système solaire.
    public void Init(float sizeScale = 1f)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * sizeScale;
    }
}

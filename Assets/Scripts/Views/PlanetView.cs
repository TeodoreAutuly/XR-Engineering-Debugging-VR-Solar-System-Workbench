using UnityEngine;

public class PlanetView : MonoBehaviour
{
    public PlanetData.Planet planet;

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void SetSize(float sizeScale)
    {
        transform.localScale = Vector3.one * sizeScale;
    }
}

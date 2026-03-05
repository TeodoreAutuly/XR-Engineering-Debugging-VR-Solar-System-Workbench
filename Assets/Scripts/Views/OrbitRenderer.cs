using UnityEngine;
using UnityEngine.Rendering;
using System;

[RequireComponent(typeof(LineRenderer))]
public class OrbitRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;

    public PlanetData.Planet planet;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;

        // Nouveau matériau pour éviter les trous dans les orbites
        var mat = new Material(lineRenderer.sharedMaterial);
        mat.SetInt("_Cull", (int)CullMode.Off);
        lineRenderer.material = mat;
    }

    public void DrawOrbit(
        PlanetData.Planet planet,
        IPlanetEphemerisService ephemeris,
        DateTime start,
        int samples = 360,
        float distanceScale = 1f,
        bool show = true)
    {
        lineRenderer.enabled = show;

        if (!show) return;

        float periodDays = PlanetData.GetOrbitalPeriodDays(planet);
        float stepDays = periodDays / samples;

        var points = new Vector3[samples];

        for (int i = 0; i < samples; i++)
        {
            var t = start.AddDays(i * stepDays);
            points[i] = ephemeris.GetPlanetPosition(planet, t) * distanceScale;
        }

        lineRenderer.positionCount = samples;
        lineRenderer.SetPositions(points);
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class OrbitRenderer : MonoBehaviour
{
    // Périodes orbitales en jours (source : NASA)
    static readonly Dictionary<PlanetData.Planet, float> OrbitalPeriods =
        new Dictionary<PlanetData.Planet, float>
        {
            { PlanetData.Planet.Mercury,    87.97f   },
            { PlanetData.Planet.Venus,     224.70f   },
            { PlanetData.Planet.Earth,     365.25f   },
            { PlanetData.Planet.Mars,      686.97f   },
            { PlanetData.Planet.Jupiter,  4332.59f   },
            { PlanetData.Planet.Saturn,  10759.22f   },
            { PlanetData.Planet.Uranus,  30688.50f   },
            { PlanetData.Planet.Neptune, 60182.00f   },
        };

    LineRenderer lineRenderer;

    public PlanetData.Planet planet;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        // Local space : les positions bougent avec le parent (SolarSystemRoot) lors du grab
        lineRenderer.useWorldSpace = false;
    }

    /// <summary>
    /// Calcule et affiche la trajectoire orbitale complète d'une planète.
    /// Les positions sont converties dans le même espace que PlanetView.SetPosition (localPosition)
    /// pour éviter tout décalage dû au transform du parent.
    /// À appeler une seule fois (pas dans Update).
    /// </summary>
    public void DrawOrbit(
        PlanetData.Planet planet,
        IPlanetEphemerisService ephemeris,
        DateTime start,
        int samples = 360)
    {
        float periodDays = OrbitalPeriods[planet];
        float stepDays = periodDays / samples;

        var points = new Vector3[samples];

        for (int i = 0; i < samples; i++)
        {
            var t = start.AddDays(i * stepDays);
            // Positions en espace local du parent (root) : pas de TransformPoint
            points[i] = ephemeris.GetPlanetPosition(planet, t);
        }

        lineRenderer.positionCount = samples;
        lineRenderer.SetPositions(points);
    }
}

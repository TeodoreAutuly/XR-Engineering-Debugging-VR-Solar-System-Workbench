using System;
using UnityEngine;

public class PlanetSystemController
{
    TimeModel timeModel;
    IPlanetEphemerisService ephemeris;
    PlanetView[] planets;
    float distanceScale;
    float planetSizeScale;

    public PlanetSystemController(
        TimeModel timeModel,
        IPlanetEphemerisService ephemeris,
        PlanetView[] planets,
        float distanceScale = 1f,
        float planetSizeScale = 1f)
    {
        this.timeModel = timeModel;
        this.ephemeris = ephemeris;
        this.planets = planets;
        this.distanceScale = distanceScale;
        this.planetSizeScale = planetSizeScale;

        foreach (var planet in planets)
        {
            if (planet == null) { Debug.LogWarning("[BOOT] PlanetSystemController : une PlanetView est null dans le tableau planets."); continue; }
            planet.SetSize(planetSizeScale);
        }

        timeModel.OnTimeChanged += UpdatePlanets;
    }

    void UpdatePlanets(DateTime time)
    {
        //Debug.Log("[TIME] Updating planets " + time);

        foreach (var planet in planets)
        {
            if (planet == null) continue;
            Vector3 pos =
                ephemeris.GetPlanetPosition(planet.planet, time) * distanceScale;
            planet.SetPosition(pos);
        }
    }
}

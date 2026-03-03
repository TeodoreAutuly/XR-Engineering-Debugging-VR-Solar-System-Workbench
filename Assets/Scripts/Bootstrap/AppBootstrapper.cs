using UnityEngine;
using System;

public class AppBootstrapper : MonoBehaviour
{
    public SolarSystemConfig config;

    public PlanetView[] planets;
    TimeModel timeModel;
    TimeController timeController;
    PlanetSystemController controller;

    void Start()
    {
        Debug.Log("[BOOT] Initializing application");

        timeModel = new TimeModel();

        timeController = gameObject.AddComponent<TimeController>();

        var ephemeris = new PlanetEphemerisService();
        var orbitStartDate = DateTime.Now;

        controller = new PlanetSystemController(
            timeModel,
            ephemeris,
            planets
        );

        timeController.Init(timeModel);

        foreach (var planet in planets)
        {
            var orbitRenderer = planet.gameObject.GetComponent<OrbitRenderer>();
            if (orbitRenderer == null)
            {
                Debug.LogWarning($"[BOOT] {planet.gameObject.name} n'a pas de composant OrbitRenderer.");
                continue;
            }
            orbitRenderer.DrawOrbit(planet.planet, ephemeris, orbitStartDate);
        }
    }
}

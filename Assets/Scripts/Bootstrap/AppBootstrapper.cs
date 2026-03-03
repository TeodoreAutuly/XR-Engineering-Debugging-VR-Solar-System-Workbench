using UnityEngine;
using System;

public class AppBootstrapper : MonoBehaviour
{
    public SolarSystemConfig config;

    public Transform solarSystemRoot;
    public SunView sun;
    public PlanetView[] planets;
    public OrbitRenderer[] orbits;
    TimeModel timeModel;
    TimeController timeController;
    PlanetSystemController controller;

    void Start()
    {
        Debug.Log("[BOOT] Initializing application");

        if (sun != null)
            sun.Init();
        else
            Debug.LogWarning("[BOOT] Aucun SunView assigné.");

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

        foreach (var orbit in orbits)
        {
            if (orbit == null)
            {
                Debug.LogWarning("[BOOT] Un OrbitRenderer est null dans le tableau orbits.");
                continue;
            }
            orbit.DrawOrbit(orbit.planet, ephemeris, orbitStartDate);
        }
    }
}

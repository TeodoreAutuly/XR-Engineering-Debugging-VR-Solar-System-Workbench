using UnityEngine;
using System;

public class AppBootstrapper : MonoBehaviour
{
    public SolarSystemConfig config;

    public Transform solarSystemRoot;
    public SunView sun;
    public PlanetView[] planets;
    public OrbitRenderer[] orbits;
    public FocusController focusController;
    TimeModel timeModel;
    TimeController timeController;
    PlanetSystemController controller;

    void Start()
    {
        Debug.Log("[BOOT] Initializing application");

        float planetSizeScale = config != null ? config.planetSizeScale : 1f;

        if (sun != null)
            sun.Init(planetSizeScale);
        else
            Debug.LogWarning("[BOOT] Aucun SunView assigné.");

        timeModel = new TimeModel();

        if (focusController != null)
            focusController.Init(timeModel);

        timeController = gameObject.AddComponent<TimeController>();

        var ephemeris = new PlanetEphemerisService();
        var orbitStartDate = DateTime.Now;

        float distanceScale = config != null ? config.distanceScale : 1f;

        controller = new PlanetSystemController(
            timeModel,
            ephemeris,
            planets,
            distanceScale,
            planetSizeScale
        );

        timeController.Init(timeModel);

        bool showOrbits = config != null ? config.showOrbits : true;

        foreach (var orbit in orbits)
        {
            if (orbit == null)
            {
                Debug.LogWarning("[BOOT] Un OrbitRenderer est null dans le tableau orbits.");
                continue;
            }
            orbit.DrawOrbit(orbit.planet, ephemeris, orbitStartDate, distanceScale: distanceScale, show: showOrbits);
        }
    }
}

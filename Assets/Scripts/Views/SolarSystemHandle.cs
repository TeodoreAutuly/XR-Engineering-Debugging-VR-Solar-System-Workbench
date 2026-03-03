using UnityEngine;


/// <summary>
/// À placer sur le GameObject racine du système solaire (SolarSystemRoot).
/// Ce même GameObject doit aussi avoir un composant XRGrabInteractable.
///
/// Hiérarchie attendue dans Unity :
///   SolarSystemRoot  [SolarSystemHandle] [XRGrabInteractable] [Rigidbody isKinematic=true]
///   ├── Handle        [MeshFilter] [MeshRenderer] [Collider]  ← la poignée visible
///   ├── Sun           [SunView]
///   ├── Mercury       [PlanetView]
///   ├── Mercury_Orbit [OrbitRenderer] [LineRenderer]
///   └── ...
///
/// L'XRGrabInteractable déplace SolarSystemRoot, entraînant tout le système avec lui
/// car toutes les planètes et orbites sont enfants de ce root.
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class SolarSystemHandle : MonoBehaviour
{
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        // Rigidbody kinématique : XRI déplace l'objet sans physique
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrabbed(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        Debug.Log($"[HANDLE] Solar system grabbed by {args.interactorObject.transform.name}");
    }

    void OnRelease(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
    {
        Debug.Log($"[HANDLE] Solar system released by {args.interactorObject.transform.name}");
    }
}

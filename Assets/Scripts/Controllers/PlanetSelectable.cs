using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class PlanetSelectable : MonoBehaviour
{
    public static event Action<PlanetSelectable> OnPlanetSelected;
    public PlanetView PlanetView { get; private set; }

    XRSimpleInteractable interactable;

    void Awake()
    {
        PlanetView   = GetComponent<PlanetView>();
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"[SELECT] Planet selected: {gameObject.name}");
        OnPlanetSelected?.Invoke(this);
    }
}

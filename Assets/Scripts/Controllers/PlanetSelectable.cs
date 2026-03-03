using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;

/// <summary>
/// À placer sur un GameObject planète avec un Collider (non trigger).
/// Utilise XRSimpleInteractable pour être compatible avec XRRayInteractor.
/// La sélection se déclenche quand l'utilisateur appuie sur la gâchette
/// en pointant la planète avec le rayon.
///
/// Setup Unity :
///   - Ajouter un SphereCollider sur la planète (isTrigger = false)
///   - Ajouter XRSimpleInteractable (ajouté automatiquement via RequireComponent)
///   - Ajouter ce composant PlanetSelectable
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class PlanetSelectable : MonoBehaviour
{
    /// <summary>
    /// Déclenché quand une planète est sélectionnée (gâchette pressée sur la planète).
    /// Paramètre : l'instance PlanetSelectable sélectionnée.
    /// </summary>
    public static event Action<PlanetSelectable> OnPlanetSelected;

    /// <summary>
    /// Référence à la PlanetView associée pour accéder aux données de la planète.
    /// </summary>
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

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// À placer sur SolarSystemRoot.
/// Utilise l'axe vertical du joystick droit pour scaler le système solaire.
/// Assigner "rightHandScaleAction" dans l'Inspector (ex : XRI Right/Primary2DAxis).
/// </summary>
public class ScaleController : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference rightHandScaleAction;

    [Header("Scale")]
    [Tooltip("Vitesse de changement de scale par seconde")]
    public float scaleSpeed = 0.5f;

    [Tooltip("Scale minimale du système")]
    public float minScale = 0.01f;

    [Tooltip("Scale maximale du système")]
    public float maxScale = 5f;

    Coroutine scaleCoroutine;

    void OnEnable()
    {
        if (rightHandScaleAction == null) return;

        rightHandScaleAction.action.Enable();
        rightHandScaleAction.action.performed += OnAxisChanged;
        rightHandScaleAction.action.canceled  += OnAxisCanceled;
    }

    void OnDisable()
    {
        if (rightHandScaleAction == null) return;

        rightHandScaleAction.action.performed -= OnAxisChanged;
        rightHandScaleAction.action.canceled  -= OnAxisCanceled;
        rightHandScaleAction.action.Disable();

        StopScaleCoroutine();
    }

    void OnAxisChanged(InputAction.CallbackContext ctx)
    {
        Debug.Log($"[SCALE] Axis input received: {ctx.ReadValue<Vector2>()}");
        float y = ctx.ReadValue<Vector2>().y;

        if (Mathf.Abs(y) < 0.1f) // deadzone
        {
            StopScaleCoroutine();
            return;
        }

        // Redémarre si l'axe change de valeur en cours de route
        StopScaleCoroutine();
        scaleCoroutine = StartCoroutine(ScaleRoutine(y));
        Debug.Log($"[SCALE] Axis changed: {y:F2}, starting scale coroutine.");
    }

    void OnAxisCanceled(InputAction.CallbackContext ctx)
    {
        StopScaleCoroutine();
        Debug.Log("[SCALE] Axis input canceled, stopping scale coroutine.");
    }

    void StopScaleCoroutine()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }
    }

    IEnumerator ScaleRoutine(float axisY)
    {
        while (true)
        {
            float current = transform.localScale.x;
            float next = Mathf.Clamp(current + axisY * scaleSpeed * Time.deltaTime * current, minScale, maxScale);
            transform.localScale = Vector3.one * next;
            yield return null;
        }
    }
}

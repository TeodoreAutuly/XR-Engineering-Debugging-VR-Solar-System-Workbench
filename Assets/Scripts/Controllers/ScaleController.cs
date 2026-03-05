using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ScaleController : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference rightHandScaleAction;

    [Header("Scale")]
    [Tooltip("Transform du Soleil : le système solaire sera agrandi/réduit autour de sa position monde")]
    public Transform sunTransform;

    [Tooltip("Vitesse de changement de scale par seconde")]
    public float scaleSpeed = 0.5f;

    [Tooltip("Scale minimale du système")]
    public float minScale = 1f;

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

            if (sunTransform != null)
            {
                Vector3 sunWorldPosBefore = sunTransform.position;

                transform.localScale = Vector3.one * next;

                transform.position += sunWorldPosBefore - sunTransform.position;
            }
            else
            {
                transform.localScale = Vector3.one * next;
            }

            yield return null;
        }
    }
}

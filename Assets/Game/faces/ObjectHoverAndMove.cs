using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectHoverAndMove : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isClicked = false;

    public float hoverScaleFactor = 1.1f;
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;
    public Transform waypoint; // Assign a GameObject in Unity as the waypoint

    public AudioSource pressAudioSource;
    public AudioSource returnAudioSource;
    public AudioClip pressAudio;
    public AudioClip returnAudio;

    private Camera mainCamera;

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        mainCamera = Camera.main;
    }

    void OnMouseEnter()
    {
        if (!isClicked)
            transform.localScale = originalScale * hoverScaleFactor;
    }

    void OnMouseExit()
    {
        if (!isClicked)
            transform.localScale = originalScale;
    }

    void OnMouseDown()
    {
        if (!isClicked && waypoint != null)
        {
            isClicked = true;
            if (pressAudioSource != null && pressAudio != null)
                pressAudioSource.PlayOneShot(pressAudio);
            StartCoroutine(MoveAndRotateToPosition(waypoint.position, waypoint.rotation, originalScale));
        }
    }

    void Update()
    {
        if (isClicked && Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !IsMouseOverObject())
        {
            isClicked = false;
            if (returnAudioSource != null && returnAudio != null)
                returnAudioSource.PlayOneShot(returnAudio);
            StartCoroutine(MoveAndRotateToPosition(originalPosition, originalRotation, originalScale));
        }
    }

    private IEnumerator MoveAndRotateToPosition(Vector3 targetPos, Quaternion targetRot, Vector3 targetScale)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startScale = transform.localScale;

        while (time < 1)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPos, time);
            transform.rotation = Quaternion.Slerp(startRotation, targetRot, time * rotateSpeed);
            transform.localScale = Vector3.Lerp(startScale, targetScale, time);
            yield return null;
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsMouseOverObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }
}

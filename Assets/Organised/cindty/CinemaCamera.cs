using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaCamera : MonoBehaviour
{
    public enum CameraMode
    {
        Static,
        MoveBetweenWaypoints,
        OrbitAroundWaypoint
    }

    [System.Serializable]
    public class CameraData
    {
        public Camera camera;
        public float duration = 5f;

        public CameraMode mode = CameraMode.Static;

        // For Move mode
        public List<Transform> waypoints = new List<Transform>();
        public float moveSpeed = 2f;

        // For Orbit mode
        public Transform orbitCenter;
        public float orbitSpeed = 30f;
        public float orbitDistance = 5f;
        public float heightOffset = 2f;
    }

    public List<CameraData> Cameras = new List<CameraData>();
    public bool autoCycle = false;

    private int currentIndex = 0;
    private Coroutine cameraRoutine;
    private Coroutine movementRoutine;

    void Start()
    {
        ActivateCamera(currentIndex);

        if (autoCycle)
        {
            cameraRoutine = StartCoroutine(CycleCameras());
        }
    }

    void ActivateCamera(int index)
    {
        StopAllCoroutines(); // Stop any previous movement

        for (int i = 0; i < Cameras.Count; i++)
        {
            Cameras[i].camera.gameObject.SetActive(i == index);
        }

        CameraData camData = Cameras[index];

        switch (camData.mode)
        {
            case CameraMode.MoveBetweenWaypoints:
                if (camData.waypoints.Count > 0)
                    movementRoutine = StartCoroutine(MoveCameraAlongWaypoints(camData));
                break;

            case CameraMode.OrbitAroundWaypoint:
                if (camData.orbitCenter != null)
                    movementRoutine = StartCoroutine(OrbitCamera(camData));
                break;
        }
    }

    IEnumerator MoveCameraAlongWaypoints(CameraData camData)
    {
        Camera cam = camData.camera;
        foreach (Transform point in camData.waypoints)
        {
            while (Vector3.Distance(cam.transform.position, point.position) > 0.1f)
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, point.position, camData.moveSpeed * Time.deltaTime);
                // cam.transform.LookAt(point); // Removed: this prevents camera auto-rotating toward its path
                yield return null;
            }
        }
    }
    IEnumerator OrbitCamera(CameraData camData)
    {
        Camera cam = camData.camera;
        Transform center = camData.orbitCenter;
        float angle = 0f;

        while (true)
        {
            angle += camData.orbitSpeed * Time.deltaTime;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * camData.orbitDistance;
            Vector3 targetPos = center.position + offset + Vector3.up * camData.heightOffset;

            cam.transform.position = targetPos;
            cam.transform.LookAt(center);

            yield return null;
        }
    }

    IEnumerator CycleCameras()
    {
        while (true)
        {
            yield return new WaitForSeconds(Cameras[currentIndex].duration);
            NextCamera();
        }
    }

    public void NextCamera()
    {
        currentIndex = (currentIndex + 1) % Cameras.Count;
        ActivateCamera(currentIndex);
    }

    public void SwitchToCamera(int index)
    {
        if (index >= 0 && index < Cameras.Count)
        {
            currentIndex = index;
            ActivateCamera(index);
        }
    }
}


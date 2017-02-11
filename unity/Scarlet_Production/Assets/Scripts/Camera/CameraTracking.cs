using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public TrackingBehaviour m_TrackingBehaviour;

    private void Start()
    {
        float[] distances = new float[32];
        distances[0] = 100;
        GetComponent<Camera>().layerCullDistances = distances;
    }

    void Update()
    {
        Vector3 pos = m_TrackingBehaviour.CalculateCameraPosition();
        Quaternion rot = m_TrackingBehaviour.CalculateCameraRotation();
        transform.position = Vector3.Lerp(transform.position, pos, m_TrackingBehaviour.m_LerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, m_TrackingBehaviour.m_LerpSpeed);
    }
}

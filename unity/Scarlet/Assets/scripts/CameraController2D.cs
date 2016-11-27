using UnityEngine;
using System.Collections;

public class CameraController2D : MonoBehaviour
{
    public Transform player;
    public float yOffset;
    public float zOffset;
    public float trackAhead;
    public float viewAngle;
    public float smoothingFactor;

    private float zeroY;
    private float trackAheadCurrent;

    void Start()
    {
        zeroY = player.position.y;
        transform.position = new Vector3(player.position.x, yOffset, zOffset);
        transform.rotation = Quaternion.Euler(viewAngle, 0, 0);
    }

    void Update()
    {
        Track();
    }

    private void Track()
    {
        float hInput = Input.GetAxis("Horizontal");
        trackAheadCurrent = trackAhead * hInput;
        Vector3 targetPosition = transform.position;
        targetPosition.x = player.position.x + trackAheadCurrent;
        targetPosition.y = player.position.y - zeroY + yOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingFactor * Time.deltaTime);
    }
}

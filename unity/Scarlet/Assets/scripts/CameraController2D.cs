using UnityEngine;
using System.Collections;

public class CameraController2D : MonoBehaviour
{
    public GameObject player;
    public float yOffset;
    public float zOffset;
    public float trackAhead;
    public float viewAngle;
    public float smoothingFactor;

    private float zeroY;
    private float trackAheadCurrent;
    private float playerRunSpeed;
    private Rigidbody playerRb;
    private Transform playerT;

    void Start()
    {
        playerRunSpeed = player.GetComponent<PlayerControlsCharController>().m_SpeedRun;
        playerRb = player.GetComponent<Rigidbody>();
        playerT = player.transform;
        zeroY = playerT.position.y;
        transform.position = new Vector3(playerT.position.x, yOffset, zOffset);
        transform.rotation = Quaternion.Euler(viewAngle, 0, 0);
    }

    void Update()
    {
        Track();
    }

    private void Track()
    {
        trackAheadCurrent = trackAhead * (playerRb.velocity.x / playerRunSpeed);
        Vector3 targetPosition = transform.position;
        targetPosition.x = playerT.position.x + trackAheadCurrent;
        targetPosition.y = playerT.position.y - zeroY + yOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingFactor * Time.deltaTime);
    }
}

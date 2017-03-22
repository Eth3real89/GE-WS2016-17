using UnityEngine;
using System.Collections;

public class FairyCircling : MonoBehaviour
{
    public Transform target;

    public float minDistance = 0.5f, maxDistance = 2f;
    public float minRotSpeed = 1.5f, maxRotSpeed = 2.5f;
    public float minMovSpeed = 3.5f, maxMovSpeed = 4.5f;

    private float distance, rotSpeed, movSpeed;

    private void Start()
    {
        RandomizeDistance();
    }

    void Update()
    {
        Vector3 relativePos = (target.position + new Vector3(0, distance, 0)) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, rotSpeed * Time.deltaTime);
        transform.Translate(0, 0, movSpeed * Time.deltaTime);
    }

    private void RandomizeDistance()
    {
        distance = Random.Range(minDistance, maxDistance);
        rotSpeed = Random.Range(minRotSpeed, maxRotSpeed);
        movSpeed = Random.Range(minMovSpeed, maxMovSpeed);

        Invoke("RandomizeDistance", Random.Range(1f, 3f));
    }
}
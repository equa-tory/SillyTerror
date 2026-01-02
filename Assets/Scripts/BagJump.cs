using UnityEngine;
using System.Linq;

public class BagJumpWithNearestLight : MonoBehaviour
{
    public Camera playerCamera;
    public Transform player;
    public float jumpDistance = 5f;
    public float lookTimeThreshold = 3f;
    public float blinkDuration = 0.2f;

    private float lookTimer = 0f;
    private bool isLooking = false;
    private Light nearestLight;
    [SerializeField] private Light exceptionalLight;

    void Update()
    {
        // Найти ближайший источник света один раз
        if (nearestLight == null)
            nearestLight = FindNearestLight();

        Vector3 dirToBag = (transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToBag);

        isLooking = dot > 0.5f; // игрок почти прямо смотрит на сумку

        if (isLooking)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookTimeThreshold && nearestLight != null)
            {
                Jump();
                StartCoroutine(BlinkLight(nearestLight));
                lookTimer = 0f;
            }
        }
        else
        {
            lookTimer = 0f;
        }
        nearestLight = null;
    }

    void Jump()
    {
        Vector3 jumpDir = (transform.position - player.position).normalized;
        jumpDir.y = 0; // движение только по полу
        // transform.position += jumpDir * jumpDistance;
        transform.position += jumpDir * -(Vector3.Distance(transform.position, player.position) / 2f);
    }

    System.Collections.IEnumerator BlinkLight(Light light)
    {
        light.enabled = false;
        yield return new WaitForSeconds(blinkDuration);
        light.enabled = true;
    }

    Light FindNearestLight()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        if (allLights.Length == 0) return null;

        Light closest = allLights[0];
        float minDist = Vector3.Distance(transform.position, closest.transform.position);

        foreach (var l in allLights)
        {
            float dist = Vector3.Distance(transform.position, l.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                if (l == exceptionalLight) continue;
                closest = l;
            }
        }

        return closest;
    }
}

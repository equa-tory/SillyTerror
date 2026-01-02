using System.Collections.Generic;
using UnityEngine;

public class BagMover : MonoBehaviour
{
    public Transform player;
    public Camera playerCamera;
    public float speed = 2f;
    public float stopDistance = 2f;
    public float viewDot = 0.7f; // чем больше — тем уже угол зрения

    public float lookTimeThreshold = 3f;
    public float blinkDuration = 0.2f;

    private float lookTimer = 0f;
    private bool isLooking = false;
    private Light nearestLight;
    private List<Light> nearestLights = new List<Light>();
    [SerializeField] private Light exceptionalLight;

    //--------------------------------------------------------------------------------------------

    void Update()
    {
        Vector3 dirToBag = (transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToBag);

        float distance = Vector3.Distance(transform.position, player.position);

        // игрок НЕ смотрит на сумку
        if (dot < viewDot && distance > stopDistance)
        {
            Vector3 target = player.position;
            target.y = transform.position.y; // движение только по одной координате (по полу)

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
        }




        // Найти ближайший источник света один раз
        if (nearestLights.Count == 0)
            nearestLights = FindTwoNearestLights();

        isLooking = dot > viewDot; // игрок почти прямо смотрит на сумку

        // if (isLooking && Vector3.Distance(transform.position, player.position) > stopDistance)
        // {
        //     lookTimer += Time.deltaTime;

        //     if (lookTimer >= lookTimeThreshold && nearestLight != null)
        //     {
        //         Jump();
        //         StartCoroutine(BlinkLight(nearestLight));
        //         lookTimer = 0f;
        //     }
        // }
        // else
        // {
        //     lookTimer = 0f;
        // }
        // nearestLight = null;

        if (isLooking && Vector3.Distance(transform.position, player.position) > stopDistance)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookTimeThreshold && nearestLights.Count > 0)
            {
                Jump();
                StartCoroutine(BlinkLight(nearestLights[0]));
                StartCoroutine(BlinkLight(nearestLights[1]));
                lookTimer = 0f;
            }
        }
        else
        {
            lookTimer = 0f;
        }
        nearestLights.Clear();
    }

    //--------------------------------------------------------------------------------------------

    void Jump()
    {
        Vector3 jumpDir = (transform.position - player.position).normalized;
        jumpDir.y = 0; // движение только по полу
        // transform.position += jumpDir * jumpDistance;
        transform.position += jumpDir * -(Vector3.Distance(transform.position, player.position) / 1.5f);
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

    List<Light> FindTwoNearestLights()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        if (allLights.Length == 0) return null;

        Light bagLight = null;
        Light playerLight = null;

        float bagDist = float.MaxValue;
        float playerDist = float.MaxValue;

        foreach (var l in allLights)
        {
            if (l == exceptionalLight) continue;

            float dBag = Vector3.Distance(transform.position, l.transform.position);
            float dPlayer = Vector3.Distance(player.position, l.transform.position);

            if (dBag < bagDist)
            {
                bagDist = dBag;
                bagLight = l;
            }

            if (dPlayer < playerDist)
            {
                playerDist = dPlayer;
                playerLight = l;
            }
        }

        return new List<Light> { bagLight, playerLight };
    }

}

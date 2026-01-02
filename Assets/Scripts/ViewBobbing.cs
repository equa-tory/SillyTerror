using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(ObjTransform))]
public class ViewBobbing : MonoBehaviour
{
    public PlayerController pc;

    public float walkingSpeed = 5;
    public float runningSpeed = 10;

    public float EffectIntensity;
    public float EffectIntensityX;
    public float EffectSpeed;

    private ObjTransform FollowerInstance;
    private Vector3 OriginalOffset;
    private float SinTime;

    private void Start()
    {
        FollowerInstance = GetComponent<ObjTransform>();
        OriginalOffset = FollowerInstance.posOffset;
    }

    private void Update()
    {
        EffectSpeed = pc.state == PlayerController.MovementState.sprinting
            ? runningSpeed
            : walkingSpeed;

        if (pc.rb.velocity.magnitude > 0.1f)
        {
            SinTime += Time.deltaTime * EffectSpeed;

            float sinY = -Mathf.Abs(EffectIntensity * Mathf.Sin(SinTime));
            float sinX = Mathf.Cos(SinTime) * EffectIntensity * EffectIntensityX;

            Vector3 targetOffset =
                OriginalOffset +
                Vector3.up * sinY +
                FollowerInstance.transform.right * sinX;

            FollowerInstance.posOffset = targetOffset;
        }
        else
        {
            SinTime = 0f;
            FollowerInstance.posOffset = Vector3.Lerp(
                FollowerInstance.posOffset,
                OriginalOffset,
                Time.deltaTime * EffectSpeed
            );
        }
    }



}

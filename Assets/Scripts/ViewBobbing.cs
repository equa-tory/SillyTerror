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

        // if(pc.isSprinting) EffectSpeed = runningSpeed;
        // else EffectSpeed = walkingSpeed;

        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
        if (inputVector.magnitude > 0f)
        {
            SinTime += Time.deltaTime * EffectSpeed;
        }
        else
        {
            SinTime = 0f;
            FollowerInstance.posOffset = Vector3.Lerp(FollowerInstance.posOffset, Vector3.zero, Time.deltaTime * EffectSpeed);
            // FollowerInstance.posOffset = Vector3.Lerp(FollowerInstance.posOffset, OriginalOffset, Time.deltaTime * EffectSpeed);
        }

        float sinAmountY = -Mathf.Abs(EffectIntensity * Mathf.Sin(SinTime));
        Vector3 sinAmountX = FollowerInstance.transform.right * EffectIntensity * Mathf.Cos(SinTime) * EffectIntensityX;

        FollowerInstance.posOffset = new Vector3
        {
            x = OriginalOffset.x,
            y = OriginalOffset.y + sinAmountY,
            z = OriginalOffset.z
        };

        FollowerInstance.posOffset += sinAmountX;
    }


}

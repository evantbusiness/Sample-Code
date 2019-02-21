using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    private Vector3 defaultPos;
    private Vector3 targetPos;
    private float adjustSpeed;

    private bool canAdjust;
    private bool dashTargetSet;

    private float shakeTime;
    private float shakeIntensity;
    private bool isShaking;

    private bool isObstructed;

    //Initialize
    private void Start ()
    {
        defaultPos = transform.localPosition;
        targetPos = defaultPos;
        adjustSpeed = 8.0f;

        canAdjust = true;
        dashTargetSet = false;

        shakeTime = 0;
        shakeIntensity = 0;
        isShaking = false;

        isObstructed = false;
	}
	
	//Update
	private void LateUpdate ()
    {
        if (Input.GetButtonDown("ShoulderSwap") && canAdjust && !GetComponentInParent<PlayerController>().IsDashing)
        {
            targetPos.Set(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            defaultPos = targetPos;
            canAdjust = false;
        }

        if (GetComponentInParent<PlayerController>().IsDashing && !dashTargetSet)
        {
            targetPos.Set(0, transform.localPosition.y + 0.2f, transform.localPosition.z - 2.0f);
            canAdjust = false;
            dashTargetSet = true;
        }
        else if (!GetComponentInParent<PlayerController>().IsDashing && dashTargetSet)
        {
            targetPos = defaultPos;
            canAdjust = false;
            dashTargetSet = false;
        }

        if (Vector3.Distance(transform.localPosition, targetPos) > 0.0001f && !isShaking && !isObstructed)
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * adjustSpeed);
        else
            canAdjust = true;

        if (shakeTime > 0)
        {
            transform.localPosition = targetPos + Random.insideUnitSphere * shakeIntensity;
            shakeTime -= Time.deltaTime;
        }
        else
            isShaking = false;

        RaycastHit hit;
        Ray cameraBack = new Ray(transform.position, -transform.forward);

        if (Physics.SphereCast(cameraBack, 0.4f, out hit, 0.55f))
        {
            isObstructed = true;
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.forward, Time.deltaTime * 5);
        }
        else
            isObstructed = false;
    }

    public void ShakeCam(float time, float intensity)
    {
        isShaking = true;
        shakeTime = time;
        shakeIntensity = intensity;
    }
}

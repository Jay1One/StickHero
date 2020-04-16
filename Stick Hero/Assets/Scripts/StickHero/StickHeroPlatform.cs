using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickHeroPlatform : MonoBehaviour
{
    [SerializeField] private Transform m_stickPoint;

    public Vector3 GetStickPosition()
    {
        return m_stickPoint.position;
    }

    public float GetPlatformSize()
    {
        return transform.localScale.x;
    }
}

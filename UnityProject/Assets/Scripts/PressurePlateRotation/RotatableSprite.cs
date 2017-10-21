﻿using UnityEngine;

public class RotatableSprite : MonoBehaviour
{
    public int rotationSpeed;
    public int lowerAngle;
    public int upperAngle;

    private bool activated;
    private float translation;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        translation = Time.deltaTime * rotationSpeed;
        if (activated && getRotation() <= upperAngle)
        {
            transform.Rotate(0, 0, translation);
        }
        else if (!activated && getRotation() >= lowerAngle)
        {
            transform.Rotate(0, 0, -1 * translation);
        }
    }

    public void setActivation(bool active)
    {
        activated = active;
    }

    private int getRotation()
    {
        return (int)transform.rotation.eulerAngles.z;
    }
}
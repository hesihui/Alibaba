using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public float minSpeed = 1f; // Minimum speed of movement
    public float maxSpeed = 3f; // Maximum speed of movement
    public float maxDistance = 3f; // Maximum distance to move forward and backward
    public float colorIntensity = 2f; // Multiplier for emission intensity
    public float minAlpha = 0.3f; // Minimum alpha value when the object is at the back
    public float maxAlpha = 1f; // Maximum alpha value when the object is at the front

    private Vector3 localStartPosition;
    private Material[] objectMaterials;
    private float speed;

    void Start()
    {
        // Store the local starting position relative to the parent
        localStartPosition = transform.localPosition;

        // Get all materials from the renderer
        objectMaterials = GetComponent<Renderer>().materials;

        // Ensure all materials have an emission property
        foreach (var material in objectMaterials)
        {
            material.EnableKeyword("_EMISSION");
        }

        // Assign a random speed between minSpeed and maxSpeed
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        // Calculate the sine wave value based on the speed and time
        float sinValue = Mathf.Sin(Time.time * speed) * maxDistance;

        // Update the local position, only modifying the local Z value
        transform.localPosition = new Vector3(localStartPosition.x, localStartPosition.y, localStartPosition.z + sinValue);

        // Calculate the intensity of the color based on the sine value
        float intensity = (sinValue / maxDistance + 1) / 2; // Normalize intensity to range from 0 to 1
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, intensity); // Interpolate alpha based on intensity

        // Create the new color with varying alpha
        Color newColor = new Color(1f, 1f, 1f, alpha);

        // Set the color and emission for all materials
        foreach (var material in objectMaterials)
        {
            material.SetColor("_Color", newColor);
            material.SetColor("_EmissionColor", newColor * Mathf.LinearToGammaSpace(colorIntensity * intensity));
        }
    }
}

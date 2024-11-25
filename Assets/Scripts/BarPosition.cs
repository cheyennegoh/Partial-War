using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPosition : MonoBehaviour
{
    public float heightOffset = 2f;  // Height above the unit where the canvas will be placed
    private Canvas canvas;           // Reference to the Canvas
    private GameObject unit;         // Reference to the unit GameObject

    void Start()
    {
        // Ensure the canvas is in world space
        canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }

        // Get the unit's GameObject by finding it using the parent name (this will be the unit the canvas is attached to)
        string parentName = gameObject.name; // This gets the name of the GameObject this script is attached to
        unit = GameObject.Find(parentName);  // Find the unit by its name

        if (unit == null)
        {
            Debug.LogError("Unit not found! Ensure the unit's name matches the Canvas name.");
        }
    }

    void Update()
    {
        if (unit != null && canvas != null)
        {
            // Position the canvas just above the unit, with the height offset
            Vector3 aboveUnitPosition = unit.transform.position + new Vector3(0, heightOffset, 0);
            canvas.transform.position = aboveUnitPosition;

            // Ensure the canvas always faces the camera (billboarding effect)
            Vector3 directionToFace = Camera.main.transform.position - canvas.transform.position;
            directionToFace.y = 0; // Keep the canvas aligned horizontally
            canvas.transform.rotation = Quaternion.LookRotation(directionToFace);

            // Optional: You can add a slight smoothing effect to make the movement smoother
            canvas.transform.position = Vector3.Lerp(canvas.transform.position, aboveUnitPosition, Time.deltaTime * 10f);
        }
    }
}

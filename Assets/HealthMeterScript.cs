using ECS;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMeterScript : MonoBehaviour
{
    public const float MaxLen = 0.15f;
    public const float MinLen = 0.02f;
    public Vector3 MaxColor = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 MinColor = new Vector3(1.0f, 0.0f, 0.0f);

    private SpriteRenderer spriteRenderer;
    private HealthProvider healthProv;
    private float currentHealthState = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        healthProv = transform.parent.GetComponent<HealthProvider>();
        
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var health = healthProv.GetData();
        currentHealthState = Mathf.InverseLerp(0, health.maxHealth, health.currentHealth);
        Vector3 color = Vector3.Lerp(MinColor, MaxColor, currentHealthState);
        float width = Mathf.Lerp(MinLen, MaxLen, currentHealthState);
        transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z);
        spriteRenderer.color = new Color(color.x, color.y, color.z, 1);
    }
}

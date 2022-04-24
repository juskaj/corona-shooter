using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ExplosionScript : MonoBehaviour
{
    private Light2D pointLight;

    // Start is called before the first frame update
    void Start()
    {
        pointLight = transform.GetChild(1).GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        pointLight.intensity -= Time.deltaTime;

        if (pointLight.intensity <= 0)
        {
            Destroy(gameObject);
        }
    }
}

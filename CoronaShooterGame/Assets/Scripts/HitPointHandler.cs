using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPointHandler : MonoBehaviour
{
    private TMP_Text hitPointsText;
    private float speed;
    private float time;
    private float totalTime;
    private Color color;
    private void Awake()
    {
        time = 0;
        hitPointsText = this.transform.GetChild(0).GetComponent<TMP_Text>();        
    }
    public void HitPointsSetup(int hitAmount, float speed, float time, Color color)
    {
        hitPointsText.text = hitAmount.ToString();

        this.speed = speed;
        this.time = time;
        totalTime = time;
        this.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (time < 0)
        {
            Destroy(gameObject);
        }

        color.a = time / totalTime;
        hitPointsText.color = color;
        time -= Time.deltaTime;
        transform.Translate(new Vector3(0, 1 * speed * Time.deltaTime, 0));    
    }
}

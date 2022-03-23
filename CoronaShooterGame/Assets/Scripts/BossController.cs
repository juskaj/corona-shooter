using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Boss currentBoss;

    void Start()
    {
    }

    public void SetBoss(Boss boss)
    {
        currentBoss = boss;
    }

    void Update()
    {
        switch (currentBoss.Type)
        {
            case BossType.shooting:
                Debug.Log("Shooting boss");
                break;
            case BossType.spawning:
                Debug.Log("Spawning boss");
                break;
        }   
    }
}

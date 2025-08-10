using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDifficulty : MonoBehaviour
{
    public bool isActivated = false;
    public GameObject Player;
    public GameObject DungeonPortal;
    public TextMeshProUGUI DistanceText;
    public float DistanceToPortal;

    void Update()
    {
        if (isActivated)
        {
            DistanceToPortal = Vector2.Distance(Player.transform.position, DungeonPortal.transform.position);
            DistanceText.text = DistanceToPortal.ToString("0") + " M";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PotalDungeon : MonoBehaviour
{
    public Transform DungeonDestination;
    public Transform TownDestination;

    public GameObject uiWindow;
    public GameObject uiWindow_retrunExpendition;
    public NpcInteraction npcInteraction_potal;


    private bool isPlayerInRange = false;
    public GameObject[] objectsToDisable;

    public GameObject[] objectsToEnable_grass;
    public GameObject[] objectsToEnable_rock;
    public GameObject[] objectsToEnable_deeper_rock;

    public GameObject Canvas;
    public GameObject LoadingScreen;

    private void Start()
    {
        uiWindow.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void ShowUI()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Movement>().canMove = false;
        player.GetComponent<Movement>().animator.SetBool("IsMoving", false);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        uiWindow.SetActive(true);
    }

    public void Teleport(int ToWhere)
    {
        CloseUI();
        GameObject player = GameObject.FindWithTag("Player");

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }

        switch (ToWhere)
        {
            case 1:
                foreach (GameObject obj in objectsToEnable_grass)
                {
                    obj.SetActive(true);
                }
                break;
            case 2:
                foreach (GameObject obj in objectsToEnable_rock)
                {
                    obj.SetActive(true);
                }
                break;
            case 3:
                foreach (GameObject obj in objectsToEnable_deeper_rock)
                {
                    obj.SetActive(true);
                }
                break;
        }


        DungeonEntrance();
    }

    public void CloseUI()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Movement>().canMove = true;
        uiWindow.SetActive(false);
    }

    public void DungeonEntrance()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = DungeonDestination.position;
        }

        if (Camera.main.GetComponent<CameraFollow>() != null)
        {
            Camera.main.GetComponent<CameraFollow>().limitFollow = false;
        }

        if (LoadingScreen != null)
        {
            GameObject loadingscreen = Instantiate(LoadingScreen, Canvas.transform);
        }

        player.GetComponent<PlayerCombat>().Medic_health = player.GetComponent<PlayerCombat>().Medic_health_max;

    }

    public void DungeonLeave(bool RetrunExpendition)
    {
        npcInteraction_potal.CloseWindow();

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToEnable_grass)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in objectsToEnable_rock)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in objectsToEnable_deeper_rock)
        {
            obj.SetActive(false);
        }

        if (Camera.main.GetComponent<CameraFollow>() != null)
        {
            Camera.main.GetComponent<CameraFollow>().limitFollow = true;
        }


        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = TownDestination.position;
        }

        if (RetrunExpendition)
        {
            player.GetComponent<Player>().TransferDungeonItemsToPlayer();
        }

        if (LoadingScreen != null)
        {
            GameObject loadingscreen = Instantiate(LoadingScreen, Canvas.transform);
        }

        player.GetComponent<PlayerCombat>().Medic_health = player.GetComponent<PlayerCombat>().Medic_health_max;

    }
}

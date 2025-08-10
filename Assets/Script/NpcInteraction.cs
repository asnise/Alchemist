using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NpcInteraction : MonoBehaviour
{
    public GameObject uiElement; // The UI element to show
    public Button CloseButton;
    public GameObject[] objectsToDeactivate; // Array of objects to deactivate when the UI is shown
    public UnityEvent onToggle;

    private bool isInRange = false;

    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            AtiveUI();
        }

        if(isInRange && Input.GetKeyDown(KeyCode.Escape))
        {
            FindAnyObjectByType<UIManager>().enabled = true;
            uiElement.SetActive(false);
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            player.GetComponent<Movement>().enabled = true;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            DepthOfFieldControl.Instance.FocusOut();
        }
    }

    public void CloseWindow()
    {
        FindAnyObjectByType<UIManager>().enabled = true;
        uiElement.SetActive(false);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        player.GetComponent<Movement>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        DepthOfFieldControl.Instance.FocusOut();
    }

    GameObject player;


    private void AtiveUI()
    {
        FindAnyObjectByType<UIManager>().CloseAllUIWithOutLocked();
        FindAnyObjectByType<UIManager>().enabled = false;

        CloseButton.onClick.AddListener(() =>
        {
            CloseWindow();
        });

        uiElement.SetActive(true);

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        player.GetComponent<Movement>().enabled = false;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        DepthOfFieldControl.Instance.FocusIn();
        player.GetComponent<Movement>().animator.SetBool("IsMoving", false);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        if (onToggle != null)
        {
            onToggle?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}

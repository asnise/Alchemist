using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemDrop : MonoBehaviour
{
    public Item item;
    public ElementalStructure elementalStructure;

    public GameObject VFX;
    public Sprite item_sprite;
    public Sprite gas_sprite;
    public Sprite liquid_sprite;
    public Sprite solid_sprite;

    GameObject player;

    public AudioClip pickup_sfx;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;

        if (item != null)
        {
            item = Instantiate(item);
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = item_sprite;
        }
        else if (elementalStructure != null)
        {
            elementalStructure = Instantiate(elementalStructure);

            switch(elementalStructure.elementState_)
            {
                case ElementState.Solid:
                    gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = solid_sprite;
                    break;
                case ElementState.Liquid:
                    gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = liquid_sprite;
                    break;
                case ElementState.Gas:
                    gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = gas_sprite;
                    break;
            }
        }
    }

    public void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < 1)
        {
            if (item != null)
            {
                AddItem();
            }
            else if (elementalStructure != null)
            {
                AddElement();
            }
        }
    }

    public void AddItem()
    {
        item.AddToInventory(player.GetComponent<Player>());
        Destroy(gameObject);
        GameObject obj = Instantiate(VFX, transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySoundEffect(pickup_sfx);
        Destroy(obj, 2f);

    }

    public void AddElement()
    {
        elementalStructure.AddToInventory(player.GetComponent<Player>());
        Destroy(gameObject);
        GameObject obj = Instantiate(VFX, transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySoundEffect(pickup_sfx);
        Destroy(obj, 2f);

    }
}

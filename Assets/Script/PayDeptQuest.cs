using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayDeptQuest : MonoBehaviour
{

    private PlayerStatus playerStatus;

    public GameObject FineshPay_Alert;
    public AudioClip RewardSfx;
    public AudioClip PayDeptSfx;

    private void Start()
    {
        playerStatus = GameObject.Find("Player").GetComponent<Player>().playerStatus_;

    }

    public void PayDeptALL()
    {
        int goldToPay = playerStatus.gold_;

        if (goldToPay != 0)
        {
            if (playerStatus.dept_ <= goldToPay)
            {
                playerStatus.gold_ -= playerStatus.dept_;
                playerStatus.dept_ = 0;
            }
            else
            {
                playerStatus.dept_ -= goldToPay;
                playerStatus.gold_ = 0;
            }

            SoundManager.Instance.PlaySoundEffect(PayDeptSfx);
        }

        if (playerStatus.dept_ <= 0)
        {
            playerStatus.dept_ = 0;
            Reward();
        }
    }

    public void PayDept(int gold)
    {
        if (playerStatus.gold_ >= gold && playerStatus.dept_ != 0)
        {
            playerStatus.gold_ -= gold;
            playerStatus.dept_ -= gold;

            SoundManager.Instance.PlaySoundEffect(PayDeptSfx);
        }

        if(playerStatus.dept_ <= 0)
        {
            playerStatus.dept_ = 0;
            Reward();
        }
    }


    public void Reward()
    {
        playerStatus.gold_ += 10000;
        GetComponent<NpcInteraction>().CloseButton.onClick.Invoke();
        GetComponent<NpcInteraction>().enabled = false;
        SoundManager.Instance.PlaySoundEffect(RewardSfx);
        GameObject alert = Instantiate(FineshPay_Alert, FindObjectOfType<Canvas>().transform);
        Destroy(alert, 2f);
    }
}

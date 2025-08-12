using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCombat : MonoBehaviour
{
    public GameObject hitbox_pos;
    public int damage = 1;
    public float radius_range_attack;
    public float range_attack = 1f;
    public float attack_delay = 1f;
    private float attack_delay_interval;
    public GameObject Canvas_;
    public GameObject DamagePopUp;
    public GameObject SlashVfx;
    public Animator animator; // Add reference to Animator

    public AudioClip Heal_sfx;
    public TextMeshProUGUI Medic_health_txt;
    public int Medic_health = 5;
    public int Medic_health_max = 5;

    public AudioClip Slash_Sfx;
    public AudioClip Hit_Sfx;
    public bool canAttack = true;

    private void Start()
    {
        //UnityEngine.Cursor.visible = false;
        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        Medic_health = Medic_health_max;
    }
    void FixedUpdate()
    {
        MoveHitboxToMouse();

        if (Input.GetMouseButton(0) && canAttack)
        {
            PlayerAttack();
        }
        else
        {
            // Set blend layer animation opacity to 0
            animator.SetLayerWeight(1, 0f);
        }

        if(Input.GetMouseButtonDown(1))
        {
            if (Medic_health != 0 && GetComponent<Player>().health != GetComponent<Player>().maxHealth)
            {
                Medic_health -= 1;
                GetComponent<Player>().health = GetComponent<Player>().maxHealth;
                SoundManager.Instance.PlaySoundEffectOnPlayer(Heal_sfx);
            }
        }

        Medic_health_txt.text = Medic_health.ToString() + "/" + Medic_health_max.ToString();

    }

    void PlayerAttack()
    {
        attack_delay_interval -= Time.deltaTime;

        if (attack_delay_interval <= 0)
        {
            attack_delay_interval = attack_delay;
            TakeDamageTarget();
            animator.SetLayerWeight(1, 1f);

            GameObject obj = Instantiate(SlashVfx, hitbox_pos.transform.GetChild(0).position, Quaternion.identity);
            Destroy(obj, 0.2f);
        }
    }

    public void TakeDamageTarget()
    {
        Collider2D collider2D = Physics2D.OverlapCircle(hitbox_pos.transform.GetChild(0).position, range_attack);
        if (collider2D != null && collider2D.CompareTag("Enemy"))
        {
            collider2D.GetComponent<Enemy>().TakeDamage(damage);

            GameObject dmg_txt = Instantiate(DamagePopUp, Canvas_.transform);
            dmg_txt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = damage.ToString();
            dmg_txt.GetComponent<UIFollowTarget>().target = collider2D.gameObject;
            FindObjectOfType<CameraFollow>().ShakeCamera(0.1f, 0.05f);
            SoundManager.Instance.PlaySoundEffect(Hit_Sfx);
            Destroy(dmg_txt, 1f);
        }

        SoundManager.Instance.PlaySoundEffectOnPlayer(Slash_Sfx);
    }

    private Vector3 localOffset = Vector3.zero; // Persisting offset
    public float moveSpeed = 10f;

    void MoveHitboxToMouse()
    {
        // แปลงตำแหน่งเมาส์บนหน้าจอ (Screen Space) เป็นตำแหน่งในโลก (World Space)
        // Camera.main คือกล้องหลักของเกม
        // Z-coordinate ถูกกำหนดให้เป็นระยะห่างจากกล้อง
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        // คำนวณ Vector ที่ชี้จากผู้เล่นไปยังตำแหน่งเมาส์
        Vector3 directionToMouse = mouseWorldPosition - transform.position;

        // ตั้งค่า Z ให้เป็น 0 เพราะเกมของคุณเป็น 2D หรือต้องการควบคุมในแกน XY เท่านั้น
        directionToMouse.z = 0;

        // คำนวณ localOffset ใหม่โดยใช้ vector ที่ชี้ไปยังเมาส์
        // เราจะใช้ .normalized เพื่อให้ได้ vector ที่มีขนาดเท่ากับ 1
        // แล้วคูณด้วย radius_range_attack เพื่อจำกัดระยะห่าง
        localOffset = directionToMouse.normalized * Mathf.Min(directionToMouse.magnitude, radius_range_attack);

        // อัปเดตตำแหน่ง hitbox
        hitbox_pos.transform.position = transform.position + localOffset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitbox_pos.transform.GetChild(0).position, range_attack);
    }
}

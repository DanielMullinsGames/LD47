using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeaponVolume : MonoBehaviour
{
    public PlayerController.Weapon weapon;

    [SerializeField]
    private SpriteRenderer weaponSpriteOnBody;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            if (PlayerController.Instance.Moving)
            {
                PlayerController.Instance.GainWeapon(weapon, immediate: false);
                weaponSpriteOnBody.enabled = false;
            }
        }
    }
}
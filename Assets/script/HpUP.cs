using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUP : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.Heal(1);

                Destroy(gameObject);
            }
        }
    }
}

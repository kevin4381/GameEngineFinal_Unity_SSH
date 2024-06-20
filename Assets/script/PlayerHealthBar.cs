using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartsContainer;

    private List<Image> hearts = new List<Image>();
    private Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        InitializeHealthBar(player.maxHealth);
    }

    void InitializeHealthBar(int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            hearts.Add(heart.GetComponent<Image>());
        }
        UpdateHealthBar(player.currentHealth);
    }

    public void UpdateHealthBar(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].gameObject.SetActive(true);
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // N�cessaire pour manipuler l'UI

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;  // Points de vie maximum
    private int currentHealth;

    public GameObject heartPrefab;  // Le prefab du c�ur � dupliquer dans la HealthBar
    public Transform healthBar;  // Le conteneur de la HealthBar (l'endroit o� les c�urs seront ajout�s)

    private List<GameObject> hearts = new List<GameObject>();  // Liste pour stocker les c�urs actifs

    void Start()
    {
        currentHealth = maxHealth;  // Commencer avec la vie maximale
        UpdateHealthBar();  // Mettre � jour la barre de sant� pour afficher les c�urs
    }

    // M�thode pour infliger des d�g�ts au joueur
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;  // Eviter les points de vie n�gatifs
        }
        UpdateHealthBar();  // Mettre � jour l'affichage des c�urs
    }

    // M�thode pour soigner le joueur
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;  // Eviter de d�passer la vie maximale
        }
        UpdateHealthBar();  // Mettre � jour l'affichage des c�urs
    }

    // Mettre � jour la barre de sant� (ajouter ou supprimer des c�urs)
    void UpdateHealthBar()
    {
        // Supprimer tous les c�urs actuels
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        // Ajouter un c�ur pour chaque point de vie actuel
        for (int i = 0; i < currentHealth; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, healthBar);
            hearts.Add(newHeart);  // Ajouter le c�ur � la liste pour le traquer
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Infliger des d�g�ts au joueur lorsqu'il entre en collision avec un ennemi
            GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }

}

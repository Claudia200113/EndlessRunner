using A2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace A2
{
    //Sets and controls the UI of the game. Includes score counter, score banner, health banner, laser instructions and player died banner.
    public class UIScoreHealth : MonoBehaviour
    {
        //Textbox for health to be shown
        [SerializeField] private TextMeshProUGUI healthText;
        //Textbox for score to be shown
        [SerializeField] private TextMeshProUGUI scoreText;
        //GO with death message
        [SerializeField] private GameObject playerDead;
        //GO with instructions message
        [SerializeField] private GameObject instructions;
        [SerializeField] private Health health;
        private float currentScore = 1;

        public static UIScoreHealth Instance
        {
            get;
            private set;
        }

        //Sets the script to be a singleton.
        private void Awake()
        {
            //If script is instanced it will destroy it 
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

        }
        private void Update()
        {
            GetHealth();
            GetScore();
            DisappearInstructions();
            PlayerDied();
        }


        //Gets the current health through the HEALTH script
        private void GetHealth()
        {
            healthText.text = "Health: " + health.currentHP.ToString();
        }

        //Sets the score and sets it in the UI
        private void GetScore()
        {
            //If player is valid, the score continuously goes up
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                currentScore += 1 * Time.deltaTime;
            }

            //Prints the score in the UI
            scoreText.text = "Score: " + (int)currentScore;
        }

        //Sets unactive the instructons to activate the laser
        private void DisappearInstructions()
        {
            //When laser is activated for first time the instrcution disappear (set unactive)
            if (Input.GetMouseButtonDown(2))
            {
                instructions.SetActive(false);
            }
        }

        //If the health of the player goes lower than 0, the "Player died" banner sets active
        private void PlayerDied()
        {
            if (health.currentHP <= 0)
            {
                playerDead.SetActive(true);
            }
        }

    }

}

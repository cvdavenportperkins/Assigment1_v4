using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionManager : MonoBehaviour
{
    public PlayerMovement playerMovement;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            playerMovement.AddTailSegment();
            Debug.Log("Food Collected");
            Destroy(collision.gameObject);
            GameManager.GameMan.AddScore(20);
        }
        else if (collision.CompareTag("Goal"))
        {
            int segmentsDeposited = playerMovement.tailSegments.Count;
            int points = 0;

            for (int i = 0; i < segmentsDeposited; i++)
            {
                if (i % 2 == 0)
                {
                    points += 100;
                    SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.ScoreSFX);
                    Debug.Log("Even segment score");
                }
                else
                {
                    points += (segmentsDeposited * 100);
                    Debug.Log("Odd segment score");
                }
            }

            Debug.Log("Total Points: " + points);
            GameManager.GameMan.AddScore(points);

            foreach (GameObject segment in playerMovement.tailSegments)
            {
                if (segment != null)
                {
                    Destroy(segment);
                    SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.ComboSFX);
                    Debug.Log("Combo");
                }
            }
            playerMovement.tailSegments.Clear();
            playerMovement.hasTail = false;
        }
    }

     // Start is called before the first frame update
     void Start()
     {
        playerMovement = GetComponent<PlayerMovement>();
     }

     // Update is called once per frame
     void Update()
     {

     }
}

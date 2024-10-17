using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement PlayMove;                         //assign PlayerMovement singleton variable

    public float moveSpeed = 6f;                                   //assign default move speed
    public Rigidbody2D rb;
    private Vector2 moveInput;
    public List<GameObject> tailSegments = new List<GameObject>();
    public GameObject tailPrefab;
    public GameObject playerPrefab;
    public bool hasTail = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");                       //definer player control parameters
        moveInput.y = Input.GetAxis("Vertical");
        if (hasTail == true)                                              //set condition for speed boost if the player has a tail
        {
            moveSpeed = 9f;
        }
    }

    public void AddTailSegment()                                        //define method of adding player tail segments
    {
        Debug.Log("Adding Tail Segment");
        Vector2 newTailPosition;
        if (tailSegments.Count > 0)
        {
            newTailPosition = (Vector2)(tailSegments[tailSegments.Count - 1].transform.position) + new Vector2(0, -0.2f);  //set behavior if the player has no tail
        }
        else
        {
            newTailPosition = (Vector2)transform.position - (Vector2)transform.up * 1f;  //adjust positioning for new tail segments added to existing tail
        }

        GameObject newTailSegment = Instantiate(tailPrefab, newTailPosition, Quaternion.identity);  //instantiate tail segments
        tailSegments.Add(newTailSegment);                                                           //add tail segments to list
        SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.PickUpSFX);                           //play PickUpSFX sound effect     
        StartCoroutine(DespawnTailSegment(newTailSegment, 8f));                                     //Inititate tail despawn coroutine (not working)
        Debug.Log("Tail segment added at position: ");
        hasTail = true;                                                                              //set hasTail condition to true
    }

    IEnumerator DespawnTailSegment(GameObject tailSegment, float delay)                              //assign tail segment despawn behavior
    {
        float timePassed = 0f;                                                                       //assign tail timer variable
        SpriteRenderer sprite = tailSegment != null ? tailSegment.GetComponent<SpriteRenderer>() : null;           //assign SpriteRenderer variable
        Color originalColor = sprite != null ? sprite.color : Color.white;                               //assign fallback safety color value

        while (timePassed < delay)                                                                   //start while loop condition for the timer delay
        {
            if (sprite != null && timePassed >= 4f && timePassed < 7f)                               //set timeer activation parameters for color flash if sprite is valid
            {
                sprite.color = Color.red;
                yield return new WaitForSeconds(0.1f);                                                //set colorflash interval
                if (sprite != null) sprite.color = originalColor;
                yield return new WaitForSeconds(0.4f);
            }
            else
            {
                yield return null;                                                                   //if conditions are not met, return no action
            }
            timePassed += Time.deltaTime;                                                            //maintain track of elapsed game time 
        }

        if (tailSegment != null)                                                                     //if tailsegment is valid
        {
            tailSegment.SetActive(false);                                                           //set tail segments to inactive and remove from list (not working)
            tailSegments.Remove(tailSegment);
            SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.DecaySFX);                        //play DecaySFX (not working)
            GameManager.GameMan.DecreaseHealth();                                                   //decrease health on timer expiration (not working)
            Debug.Log("Play Sound Decay");
        }
    }


    void OnTriggerExit2D(Collider2D other)                            //set condition for health damage on boundary collision using boundary tag
    {
        if (other.CompareTag("Boundary"))
        {
            Debug.Log("Hit Boundary");
            SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.BoundarySFX);
            GameManager.GameMan.DecreaseHealth();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;

        if (tailSegments.Count > 0)
        {
            Vector2 previousPosition = transform.position;

            for (int i = 0; i < tailSegments.Count; i++)
            {
                Vector2 tempPosition = tailSegments[i].transform.position;
                tailSegments[i].transform.position = Vector2.Lerp(tailSegments[i].transform.position, previousPosition, 0.2f);
                previousPosition = tempPosition;
            }
        }
    }
}

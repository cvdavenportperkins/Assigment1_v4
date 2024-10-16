using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement PlayMove;

    public float moveSpeed = 6f;
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
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        if (hasTail == true)
        {
            moveSpeed = 9f;
        }
    }

    public void AddTailSegment()
    {
        Debug.Log("Adding Tail Segment");
        Vector2 newTailPosition;
        if (tailSegments.Count > 0)
        {
            newTailPosition = (Vector2)(tailSegments[tailSegments.Count - 1].transform.position) + new Vector2(0, -0.2f);
        }
        else
        {
            newTailPosition = (Vector2)transform.position - (Vector2)transform.up * 1f;
        }

        GameObject newTailSegment = Instantiate(tailPrefab, newTailPosition, Quaternion.identity);
        tailSegments.Add(newTailSegment);
        SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.PickUpSFX);
        StartCoroutine(DespawnTailSegment(newTailSegment, 8f));
        Debug.Log("Tail segment added at position: ");
        hasTail = true;
    }

    IEnumerator DespawnTailSegment(GameObject tailSegment, float delay)
    {
        float timePassed = 0f;
        SpriteRenderer sprite = tailSegment != null ? tailSegment.GetComponent<SpriteRenderer>() : null;
        Color originalColor = sprite != null ? sprite.color : Color.white;

        while (timePassed < delay)
        {
            if (sprite != null && timePassed >= 4f && timePassed < 7f)
            {
                sprite.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                if (sprite != null) sprite.color = originalColor;
                yield return new WaitForSeconds(0.4f);
            }
            else
            {
                yield return null;
            }
            timePassed += Time.deltaTime;
        }

        if (tailSegment != null)
        {
            tailSegment.SetActive(false);
            tailSegments.Remove(tailSegment);
            SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.DecaySFX);
            GameManager.GameMan.DecreaseHealth();
            Debug.Log("Play Sound Decay");
        }
    }


    void OnTriggerExit2D(Collider2D other)
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

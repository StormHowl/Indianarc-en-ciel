using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using Random = UnityEngine.Random;
using UnityEngine.UI;

//RED : gun
//YELLOW : Hand to hand
//PINK : Pacific (can't kill)
//GREEN : 
//ORANGE : 
public enum Theme
{
    RED,
    YELLOW,
    PINK,
    GREEN,
    ORANGE
};

public class PlayerControl : MonoBehaviour
{
    [HideInInspector] public bool facingRight = true; // For determining which way the player is currently facing.
    [HideInInspector] public bool jump = false; // Condition for whether the player should jump.

    public float moveForce = 365f; // Amount of force added to move the player left and right.
    public float maxSpeed = 5f; // The fastest the player can travel in the x axis.
    public AudioClip[] jumpClips; // Array of clips for when the player jumps.
    public float jumpForce = 1000f; // Amount of force added when the player jumps.
    public AudioClip[] taunts; // Array of clips for when the player taunts.
    public float tauntProbability = 50f; // Chance of a taunt happening.
    public float tauntDelay = 1f; // Delay for when the taunt should happen.

    private int tauntIndex; // The index of the taunts array indicating the most recent taunt.
    private Transform groundCheck; // A position marking where to check if the player is grounded.
    private bool grounded = false; // Whether or not the player is grounded.
    private Animator anim; // Reference to the player's animator component.

    static public Theme currentTheme;

    static public int score;
    static public int life;
    static public float timeStamp;
    static public int invulnerabilityFrame = 2; //invulnerability 2 secondes
	float jumpTimeStamp;
    private Vector3 spawnPosition;

    public Color Red = new Color(283, 89, 85);
    public Color Yellow = new Color(183, 167, 93);
    public Color Green = new Color(171, 121, 183);
    public Color Pink = new Color(112, 183, 107);
    public Color Orange = new Color(183, 118, 84);
    public Color previous = new Color(50, 50, 50);

    static public Color _scoreColor;

    public GameObject explosion;

    void Awake()
    {
        score = 0;
        _scoreColor = Color.yellow;
        life = 3;
        spawnPosition = new Vector3(0.0f, 0.0f, 0.0f);
        timeStamp = Time.time + invulnerabilityFrame;
        currentTheme = Theme.YELLOW;
        previous = Yellow;
        // Setting up references.
        groundCheck = transform.Find("groundCheck");
        anim = GetComponent<Animator>();
        gameObject.GetComponentInChildren<Camera>().clearFlags = CameraClearFlags.SolidColor;
        gameObject.GetComponentInChildren<Camera>().orthographicSize = 10;
    }


    void Update()
    {
        // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        // If the jump button is pressed and the player is grounded then the player should jump.
        if (Input.GetButtonDown("Jump"))
        {
            if (GetComponent<CircleCollider2D>().IsTouchingLayers())
            {
                // Set the Jump animator trigger parameter.
                anim.SetTrigger("Jump");

                // Play a random jump audio clip.
                int i = Random.Range(0, jumpClips.Length);
                AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

                // Add a vertical force to the player.
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
            }
        }

        if (Input.GetKeyDown(KeyCode.S) && PlayerControl.currentTheme == Theme.GREEN)
        {
            // Create a quaternion with a random rotation in the z-axis.
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            // Instantiate the explosion where the rocket is with the random rotation.
            Instantiate(explosion, Input.mousePosition, randomRotation);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //ContactPoint2D[] contacts;
        //other.GetContacts(contacts);
        var hit = other.gameObject;
        //Debug.Log(other.gameObject.tag);

        if (hit.GetComponent<PlatformEffect>())
        {
            Oscillator.instance.PlaySound(hit.GetComponent<PlatformEffect>().Tag.Substring(2));
        }

        var tag = hit.tag;
        if (tag == "Enemy")
        {
            float y = 0.0f;
            int nb = 0;
            foreach (ContactPoint2D cp in other.contacts)
            {
                //Debug.Log("CONTACT POINT : " + cp.point);
                y += cp.point.y;
                nb++;
            }
            //Debug.Log ("avg y = " + y / nb);
			if (y / nb < gameObject.transform.position.y && y / nb > hit.transform.position.y && currentTheme == Theme.ORANGE) {
				Destroy (hit);
				score += 25;
				jumpTimeStamp = Time.time + 0.5f;
			} else if (timeStamp <= Time.time && jumpTimeStamp <= Time.time) { //invulnerability frame and prevent bug when jumping
				life--;
				timeStamp = Time.time+invulnerabilityFrame;
			}
        }

        switch (tag)
        {
            case "Red":
                currentTheme = Theme.RED;
                score += 100;
                _scoreColor = Color.red;
                GetComponentInChildren<MeshRenderer>().material.color = Red;
                //gameObject.GetComponentInChildren<Camera>().backgroundColor = Red;
                break;
            case "Yellow":
                currentTheme = Theme.YELLOW;
                score += 100;
                GetComponentInChildren<MeshRenderer>().material.color = Red;
                //gameObject.GetComponentInChildren<Camera>().backgroundColor = Yellow;
                _scoreColor = Color.yellow;
                break;
            case "Pink":
                currentTheme = Theme.PINK;
                score += 100;
                GetComponentInChildren<MeshRenderer>().material.color = Red;
                //gameObject.GetComponentInChildren<Camera>().backgroundColor = Pink;
                _scoreColor = Color.magenta;
                break;
            case "Green":
                currentTheme = Theme.GREEN;
                score += 100;
                GetComponentInChildren<MeshRenderer>().material.color = Red;
                //gameObject.GetComponentInChildren<Camera>().backgroundColor = Green;
                _scoreColor = Color.green;
                break;
            case "Orange":
                currentTheme = Theme.ORANGE;
                score += 100;
                GetComponentInChildren<MeshRenderer>().material.color = Red;
                //gameObject.GetComponentInChildren<Camera>().backgroundColor = Orange;
                _scoreColor = Color.white;
                break;
            default:
                return;
        }

        Oscillator.instance.PlayBonus();

        //Debug.Log ("NEW THEME = " + currentTheme);
        Destroy(hit);
    }


    void FixedUpdate()
    {
        // Cache the horizontal input.
        float h = Input.GetAxis("Horizontal");

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat("Speed", Mathf.Abs(h));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
            // ... add a force to the player.
            GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

        // If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
            // ... set the player's velocity to the maxSpeed in the x axis.
            GetComponent<Rigidbody2D>().velocity = new Vector2(
                Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

        // If the input is moving the player right and the player is facing left...
        if (h > 0 && !facingRight)
            // ... flip the player.
            Flip();
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (h < 0 && facingRight)
            // ... flip the player.
            Flip();

        // If the player should jump...
        if (jump)
        {
            // Make sure the player can't jump again until the jump conditions from Update are satisfied.
            jump = false;
        }
    }


    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    public IEnumerator Taunt()
    {
        // Check the random chance of taunting.
        float tauntChance = Random.Range(0f, 100f);
        if (tauntChance > tauntProbability)
        {
            // Wait for tauntDelay number of seconds.
            yield return new WaitForSeconds(tauntDelay);

            // If there is no clip currently playing.
            if (!GetComponent<AudioSource>().isPlaying)
            {
                // Choose a random, but different taunt.
                tauntIndex = TauntRandom();

                // Play the new taunt.
                GetComponent<AudioSource>().clip = taunts[tauntIndex];
                GetComponent<AudioSource>().Play();
            }
        }
    }

    int TauntRandom()
    {
        // Choose a random index of the taunts array.
        int i = Random.Range(0, taunts.Length);

        // If it's the same as the previous taunt...
        if (i == tauntIndex)
            // ... try another random taunt.
            return TauntRandom();
        else
            // Otherwise return this index.
            return i;
    }
}
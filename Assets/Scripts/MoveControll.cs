using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MoveControll : NetworkBehaviour
{
    public Animator anim;
    public float moveSpeed = 4f;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    private Vector2 dir;
    private bool isControllActive = false;
    public AudioSource footStapsSource;
    public AudioClip[] footstapsClips;
    private float timerFootStaps = 0.3f;
    public bool isDead;
    private void Start()
    {
        if (isClient)
        {
            footStapsSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        }
    }

    private void Update()
    {
        if (isClient && !isDead) FootStaps();

        if (!isLocalPlayer || !isControllActive) return;

        dir.x = Input.GetAxis("Horizontal");
        dir.y = Input.GetAxis("Vertical");
        
        anim.SetFloat("Hor", dir.x);
        anim.SetFloat("Ver", dir.y);

    }
    private void FootStaps()
    {
        Vector2 dir = new Vector2(anim.GetFloat("Hor"), anim.GetFloat("Ver"));
        if (isDead || !isControllActive) dir = Vector2.zero;
        if (dir.magnitude > 0.1f)
        {
            if (timerFootStaps >= 0) timerFootStaps -= Time.deltaTime;
            else
            {
                timerFootStaps = 0.3f;
                footStapsSource.PlayOneShot(footstapsClips[Random.Range(0, footstapsClips.Length)]);
            }
        }
        if (dir.x < -0.1f && sprite.flipX == false) sprite.flipX = true;
        else if (dir.x > 0.1f && sprite.flipX) sprite.flipX = false;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || !isControllActive) return;
        rb.MovePosition(rb.position + dir.normalized * moveSpeed * Time.fixedDeltaTime);
    }
    public bool SetControlActive
    {
        get { return isControllActive; }
        set
        {
            isControllActive = value;
            if (value == false)
            {
                anim.SetFloat("Hor", 0);
                anim.SetFloat("Ver", 0);
            }
        }
    }




    public void StopMove()
    {
        dir = Vector2.zero;
        anim.SetFloat("Hor", dir.x);
        anim.SetFloat("Ver", dir.y);
        isControllActive = false;
    }
}

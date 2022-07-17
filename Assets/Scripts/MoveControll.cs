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
    [SyncVar(hook =(nameof(OnFlip)))]
    private bool isFlip = false;
    
    private void Update()
    {
        if (!isLocalPlayer || !isControllActive)return;
       
        dir.x = Input.GetAxis("Horizontal");
        dir.y = Input.GetAxis("Vertical");
        if (dir.x > 0.1f && isFlip) CmdFlip(false);
        if (dir.x < -0.1f && isFlip == false) CmdFlip(true);
        anim.SetFloat("Hor", dir.x);
        anim.SetFloat("Ver", dir.y);
        
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer || !isControllActive) return;
        rb.MovePosition(rb.position + dir.normalized * moveSpeed * Time.fixedDeltaTime);
    }
    public bool SetControlActive
    {
        get { return isControllActive; }
        set { isControllActive = value; }
    }
    [Command]
    private void CmdFlip(bool value)
    {
        isFlip = value;
    }

    private void OnFlip(bool _, bool value)
    {
        sprite.flipX = value;
    }
}

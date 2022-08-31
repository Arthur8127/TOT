using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class Killer : NetworkBehaviour
{

    [HideInInspector] public Image imgKD;
    [HideInInspector] public Button btnKill;
    private float timer;
    private bool isReloading;
    public List<Collider2D> targets = new List<Collider2D>();
    public CircleCollider2D targetTrigger;
    public Animator anim;
    public AudioClip killClip;
    public Player player;
    private void OnEnable()
    {
        if (isServer)
        {
            AddTargetTrigger();
        }

    }


    private void Update()
    {
        if (isLocalPlayer)
        {
            if (isReloading)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    imgKD.fillAmount = NormalizationValue(timer, 0f, NetManager.instance.game.timerAttack);
                }
                else
                {
                    timer = 0;
                    imgKD.fillAmount = 0f;
                    isReloading = false;
                    btnKill.interactable = targets.Count > 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                TryAttack();
            }
        }


    }


    public float NormalizationValue(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }




    public void TryAttack()
    {
        if (targets.Count < 1) return;
        uint id = GetTargetConn();
        MatchController.instance.PlaySound(killClip);
        StartReload();
        anim.SetTrigger("Kill");
        StartCoroutine(StoppedMove());
        player.currentMatch.CmdAttack(id);


    }
    public uint GetTargetConn()
    {
        return targets[0].GetComponent<Player>().netId;
    }

    public void StartReload()
    {
        isReloading = true;
        timer = NetManager.instance.game.timerAttack;
        imgKD.fillAmount = 1f;
        btnKill.interactable = false;
    }
    public IEnumerator StoppedMove()
    {
        MoveControll mControl = GetComponent<MoveControll>();
        mControl.SetControlActive = false;
        yield return new WaitForSeconds(1f);
        mControl.SetControlActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;
        if (collision.CompareTag("Player") && collision.gameObject != gameObject)
        {
            if (isLocalPlayer || isServer)
                AddTarget(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;
        if (collision.CompareTag("Player") && collision.gameObject != gameObject)
        {
            if (isLocalPlayer || isServer)
                RemoveTarget(collision);
        }
    }

    private void AddTarget(Collider2D col)
    {
        targets.Add(col);
        CheckTargetCount();
    }
    private void RemoveTarget(Collider2D col)
    {
        targets.Remove(col);
        CheckTargetCount();
    }
    private void CheckTargetCount()
    {
        if (!isLocalPlayer) return;
        if (targets.Count > 0 && !isReloading) btnKill.interactable = true;
        else btnKill.interactable = false;
    }

    public void AddTargetTrigger()
    {
        targetTrigger = gameObject.AddComponent<CircleCollider2D>();
        targetTrigger.isTrigger = true;
        targetTrigger.radius = 1f;
    }
}

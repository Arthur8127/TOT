using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TochControll : MonoBehaviour
{
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Check();
        }
    }

    private void Check()
    {
        RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
           
            MiniGamePoint miniGamePoint = hit.collider.GetComponent<MiniGamePoint>();
            VotingButton voting = hit.collider.GetComponent<VotingButton>();
            if (miniGamePoint)
            {
                Player localPlayer = MatchController.instance.localPlayer;
                if (Vector2.Distance(localPlayer.transform.position, miniGamePoint.transform.position) < 2f && localPlayer.isDead == false)
                {
                    if(miniGamePoint.isComplite == false)
                    {
                        if((int)localPlayer.playerType == (int)miniGamePoint.playerType)
                        {
                            miniGamePoint.windows.Show();
                        }
                       
                    }
                }
            }
            if(voting)
            {
                voting.UseButton();
            }
        }
    }
}

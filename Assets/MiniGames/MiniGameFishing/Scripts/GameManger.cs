using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniGameFishing
{
    public class GameManger : MonoBehaviour
    {
        public Text text;
        public RectTransform fishPos, indicator;
        public Image currentFish;
        public float minX, maxX;
        public List<Sprite> allFish;
        private int currentStep;
        private bool moveRight = true;
        public float moveSpeed;
        public enum GameMode { Inicializ = 0, WaitingClick = 1, Analis = 2 }
        public GameMode gameMode;

        public UnityEvent OnComplite, OnExit;
        public bool isActive;

        private void Awake()
        {
            text.text = currentStep + "/" + allFish.Count;
        }
        private void SetFish()
        {
            currentFish.sprite = allFish[currentStep];
            fishPos.localPosition = new Vector2(Random.Range(minX, maxX), 0);
            gameMode = GameMode.WaitingClick;
        }
        private void Update()
        {
            if (gameMode == GameMode.WaitingClick)
            {
                if (moveRight)
                {
                    indicator.localPosition += Vector3.right * Time.deltaTime * moveSpeed;
                    if (indicator.localPosition.x > maxX) moveRight = false;
                }
                else
                {
                    indicator.localPosition -= Vector3.right * Time.deltaTime * moveSpeed;
                    if (indicator.localPosition.x < minX) moveRight = true;
                }
            }
            if (isActive && Input.GetKeyDown(KeyCode.Escape))
            {
                isActive = false;
                OnExit.Invoke();
            }
        }
        private void Analis()
        {
            gameMode = GameMode.Analis;
            Vector2 x = fishPos.localPosition - indicator.localPosition;
            x.x = Mathf.Abs(x.x);
            if (x.x < 80)
            {
                currentStep++;
                text.text = currentStep + "/" + allFish.Count;
            }
            if (currentStep < allFish.Count)
            {
                SetFish();
            }
            else
            {
                OnComplite.Invoke();
                if (MatchController.instance)
                    MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
            }
        }
        public void Click()
        {
            switch (gameMode)
            {
                case GameMode.Inicializ:
                    SetFish();

                    break;
                case GameMode.WaitingClick:
                    Analis();
                    break;
                case GameMode.Analis:

                    break;

            }
        }

        public void Show()
        {
            isActive = true;
        }
    }

}

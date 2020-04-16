using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class StickHeroController : MonoBehaviour
{
    [SerializeField] private StickHeroStick m_Stick;
    [SerializeField] private StickHeroPlayer m_Player;
    [SerializeField] private List<StickHeroPlatform> m_Platforms;

    private int counter; //это счетчик платформ

    private enum EGameState
    {
        Wait,
        Scaling,
        Rotate,
        Movement,
        Defeate
    }

    private EGameState currentGameState;

    // Use this for initialization
    private void Start()
    {
        currentGameState = EGameState.Wait;
        counter = 0;

        m_Stick.ResetStick(m_Platforms[0].GetStickPosition());
    }


    // Update is called once per frame
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        //нужна ли реакция на нажитие кнопки мыши
        switch (currentGameState)
        {
            //если не нажата кнопка старт
            case EGameState.Wait:
                currentGameState = EGameState.Scaling;
                m_Stick.StartScaling(m_Platforms[2].transform.position.x - m_Platforms[0].GetStickPosition().x - m_Platforms[2].transform.localScale.x*0.5f -m_Player.transform.localScale.x);
                break;

            //стик увеличивается - прерываем увеличением и запускаем поворот
            case EGameState.Scaling:
                currentGameState = EGameState.Rotate;
                m_Stick.StopScaling();
                break;

            //ничего не делать
            case EGameState.Rotate:
                break;

            //ничего не делать
            case EGameState.Movement:
                break;

            //перезапускаем игру
            case EGameState.Defeate:
                print("Game restarted");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public void StopStickScale()
    {
        currentGameState = EGameState.Rotate;
        m_Stick.StartRotate();
    }

    public void StopStickRotate()
    {
        currentGameState = EGameState.Movement;
    }

    public void StartPlayerMovement(float lenght)
    {
        currentGameState = EGameState.Movement;
        StickHeroPlatform nextPlatform = m_Platforms[1];
        //находим минимальную длину стика для успешного перехода
        float targetLenght = nextPlatform.transform.position.x - m_Stick.transform.position.x;
        float platformSize = nextPlatform.GetPlatformSize();
        float min = targetLenght - platformSize * 0.5f;
        min -= m_Player.transform.localScale.x * 0.9f;

        //находим максимальную длину стика для успешного перехода
        float max = targetLenght + platformSize * 0.5f;

        //при успехе переходим в центр платформы, иначе падаем
        if (lenght < min || lenght > max)
        {
            float targetPosition = m_Stick.transform.position.x + lenght + m_Player.transform.localScale.x;
            m_Player.StartMovement(targetPosition, true);
        }
        else
        {
            float targetPosition = nextPlatform.transform.position.x;
            m_Player.StartMovement(targetPosition, false);
        }
    }

    public void StopPlayerMovement()
    {
        currentGameState = EGameState.Wait;
        counter++;
        ShiftPlatforms();
        m_Stick.ResetStick(m_Platforms[0].GetStickPosition());

    }

    public void ShowScores()
    {
        currentGameState = EGameState.Defeate;
        print($"$Game Over. Your Score: {m_Player.transform.position.x* 4} m");
    }

    //Сдвигаем первую платформу в конец поля и массива платформ
    private void ShiftPlatforms()
    {
        StickHeroPlatform firstPlatform = m_Platforms[0];
        m_Platforms.RemoveAt(0);
        
        //меняем ширину платформы;
        float minPlatformXScale = 0.05f +0.35f/counter;
        float maxPlatformXScale = 0.5f; 
        firstPlatform.transform.localScale=new Vector3(Random.Range(minPlatformXScale, maxPlatformXScale), 1,1);
        
        
        
        // Выставляем случайнайный сдвиг от предыдущей платформы
        float minXOffset = m_Player.transform.localScale.x +firstPlatform.transform.localScale.x*0.5f;
        float maxXOffset = 1.9f+firstPlatform.transform.localScale.x;
        float randomOffset = Random.Range(minXOffset, maxXOffset);
        firstPlatform.transform.position= new Vector3(randomOffset + m_Platforms[m_Platforms.Count-1].GetStickPosition().x, -1,0);
        
        
        m_Platforms.Add(firstPlatform);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public Player player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject Restart;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        // �������� ü����
        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerRepos();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        else
        {
            //Game Clar
            //�ð����߱�
            Time.timeScale = 0;
            // UI�ֱ�
            Debug.Log("���� Ŭ����");
            Restart.SetActive(true);
            Text btnText = Restart.GetComponentInChildren<Text>();
        }

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            // �÷��̾� ����
            player.OnDie();
            // �״� UI
            Debug.Log("�׾����ϴ�!");
            // ����
            Restart.SetActive(true);
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(health > 1)
                PlayerRepos();
            HealthDown();
            
        }
    }

    void PlayerRepos()
    {
        player.transform.position = new Vector3(0, 0, 0);
        player.velocityZero();
    }

    public void BrtRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}

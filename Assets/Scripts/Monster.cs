using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public int nextMove;
    BoxCollider2D boxcollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxcollider = GetComponent<BoxCollider2D>();
        Invoke("MonsterAi", 5);
    }

    void FixedUpdate()
    {
        //�̵�
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        //����üũ
        Vector2 frontVec = new Vector2( rigid.position.x + nextMove * 0.2f , rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Floor"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    //����Լ�
    void MonsterAi()
    {
        //���� �̵�
        nextMove = Random.Range(-1, 2);


        // ������ȯ
        if(nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //�����ൿ
        float nextTime = Random.Range(2f, 5f);
        Invoke("MonsterAi", nextTime);

        // �ִϸ��̼�
        anim.SetInteger("Walk", nextMove);
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("MonsterAi", 5);
    }

    public void OnDamged()
    {
        // ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // ������
        spriteRenderer.flipY = true;
        // �ݶ��̴�����
        boxcollider.enabled = false;
        // ����
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // ����
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
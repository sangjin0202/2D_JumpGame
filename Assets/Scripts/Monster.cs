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
        //이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        //지형체크
        Vector2 frontVec = new Vector2( rigid.position.x + nextMove * 0.2f , rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Floor"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    //재귀함수
    void MonsterAi()
    {
        //다음 이동
        nextMove = Random.Range(-1, 2);


        // 방향전환
        if(nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //다음행동
        float nextTime = Random.Range(2f, 5f);
        Invoke("MonsterAi", nextTime);

        // 애니메이션
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
        // 투명도
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 뒤집음
        spriteRenderer.flipY = true;
        // 콜라이더삭제
        boxcollider.enabled = false;
        // 점프
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // 삭제
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
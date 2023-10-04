using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer; 
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        // 점프
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
            PlayerSound("JUMP");
        }

        // 멈췄을때 스피드
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        // 방향전환
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // 뛰는 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.5)
            anim.SetBool("isWalk", false);
        else
            anim.SetBool("isWalk", true);




    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)                // 오른쪽 맥스속도
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1))      // 왼쪽 맥스속도
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //랜더링 플랫폼 (점프 착지이용)
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1, LayerMask.GetMask("Floor"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJump", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Monster")
            //공격
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else // 데미지
            OnDmaged(collision.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item" )
        {
            // 점수
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if(isGold)
                gameManager.stagePoint += 300;

            collision.gameObject.SetActive(false);

            PlayerSound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            // 다음스테이지
            gameManager.NextStage();

            PlayerSound("FINISH");
        }
    }

    void OnAttack(Transform _monster)
    {
        // 포인트
        gameManager.stagePoint += 100;

        // 몬스터 밟을시 반발력
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        // 몬스터 죽음
        Monster monster = _monster.GetComponent<Monster>();
        monster.OnDamged();

        PlayerSound("ATTACK");
    }

    void OnDmaged(Vector2 targetPos)
    {
        if (gameManager.health > 1)
            PlayerSound("DAMAGED");
        // HP감소
        gameManager.HealthDown();

        gameObject.layer = 11;
        // 투명도
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //튕겨나가기
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc,1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("damged");
        Invoke("OffDamged",2);

        

    }

    void OffDamged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // 투명도
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 뒤집음
        spriteRenderer.flipY = true;
        // 콜라이더삭제
        capsuleCollider.enabled = false;
        // 점프
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlayerSound("DIE");

    }

    public void velocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlayerSound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

}

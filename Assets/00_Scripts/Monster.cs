using System.Collections;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character
{
    public int HP, MaxHP;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private HitText hitText;
    [SerializeField]
    private Image m_Fill, m_Fill_Deco;


    private int target_Value = 0;
    private bool isDead = false;

    public override void Start()
    {
        HP = MaxHP;
        base.Start();
    }

    private void Update()
    {
        m_Fill_Deco.fillAmount = Mathf.Lerp(m_Fill_Deco.fillAmount, m_Fill.fillAmount, Time.deltaTime * 2.0f);

        if (isDead == true)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, Character_Spawner.move_list[target_Value], Time.deltaTime * moveSpeed);

        if(Vector2.Distance(transform.position, Character_Spawner.move_list[target_Value]) <= 0.0f)
        {
            target_Value++;
            spriteRenderer.flipX = target_Value >= 3 ? true : false;

            if(target_Value >= 4)
            {
                target_Value = 0;
            }
        }
    }

    public void GetDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        HP -= damage;
        m_Fill.fillAmount = (float)HP / (float)MaxHP;
        Instantiate(hitText, transform.position, Quaternion.identity).Init(damage);
        if(HP <= 0)
        {
            isDead = true;
            GameManager.Instance.GetMoney(1);
            GameManager.Instance.RemoveMonster(this);
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(Dead_Coroutine());
            AnimatorChange("Dead", true);
        }
    }

    private IEnumerator Dead_Coroutine()
    {
        float Alpha = 1.0f;

        while(spriteRenderer.color.a > 0.0f)
        {
            Alpha -= Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Alpha);

            yield return null;
        }

        Destroy(this.gameObject);
    }
}

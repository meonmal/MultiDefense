using System.Collections;
using Unity.Android.Types;
using UnityEngine;

public class Monster : Character
{
    public int HP;

    [SerializeField]
    private float moveSpeed;

    private int target_Value = 0;
    private bool isDead = false;

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if(isDead == true)
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
        if(HP < 0)
        {
            isDead = true;
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

using System.Collections;
using Unity.Android.Types;
using UnityEngine;

public class Monster : Character
{
    [SerializeField]
    private float moveSpeed;

    private int target_Value = 0;

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
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
}

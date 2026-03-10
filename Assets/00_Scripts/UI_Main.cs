using TMPro;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI MonsterCount_T;
    [SerializeField]
    private TextMeshProUGUI Money_T;
    [SerializeField]
    private TextMeshProUGUI Summon_T;
    [SerializeField]
    private Animator MoneyAnimator;

    private void Start()
    {
        GameManager.Instance.OnMoneyUp += Money_Anim;
    }

    private void Update()
    {
        MonsterCount_T.text = GameManager.Instance.monsters.Count.ToString() + " / 100";
        Money_T.text = GameManager.Instance.Money.ToString();
        Summon_T.text = GameManager.Instance.SummonCount.ToString();
        Summon_T.color = GameManager.Instance.Money >= GameManager.Instance.SummonCount ? Color.white : Color.red;
    }

    private void Money_Anim()
    {
        MoneyAnimator.SetTrigger("Get");
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class HitText : MonoBehaviour
{
    /// <summary>
    /// 텍스트가 올라가는 속도
    /// </summary>
    [SerializeField]
    private float floatSpeed;
    /// <summary>
    /// 텍스트가 올라가는데 걸리는 시간
    /// </summary>
    [SerializeField]
    private float riseDuration = 1.0f;
    [SerializeField]
    private float fadeDuration = 1.0f;

    public Vector3 offset = new Vector3(0, 2, 0);
    public TextMeshPro damageText;

    private Color textColor;

    public void Init(int damage)
    {
        damageText.text = damage.ToString();
        textColor = damageText.color;
        StartCoroutine(MoveAndFade());
    }

    private IEnumerator MoveAndFade()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + offset;

        float elapsedTime = 0;

        while(elapsedTime < riseDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / riseDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;

        while(elapsedTime < fadeDuration)
        {
            textColor.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            damageText.color = textColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}

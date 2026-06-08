using UnityEngine;

public class Ranger_Bullet : MonoBehaviour
{
    private Vector3 dir;
    private Vector3 endPos;

    [SerializeField] private float speed = 5f;

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, endPos) < 0.1f)
        {
            int rand = Random.Range(0, 100);
            if (rand < 70)
            {
                Debug.Log("Ranger attack hit the player!");
                PlayerStatsManager.instance.TakeDamage();
            }
            else
            {
                Debug.Log("Ranger attack missed the player!");
            }
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 direction, Vector3 targetPosition)
    {
        dir = direction.normalized;
        Vector3 targetPos = targetPosition;

        Vector3 delta = endPos - transform.position;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            endPos = new Vector3(targetPos.x, transform.position.y, transform.position.z);
        }
        else
        {
            endPos = new Vector3(transform.position.x, transform.position.y, targetPos.z);
        }
    }
}

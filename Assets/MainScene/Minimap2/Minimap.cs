using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Transform player;  // Reference to the player's Transform
    public Image allyPointPrefab;
    public Image enemyPointPrefab;

    private void Start()
    {
        // Create points for allies and enemies (adjust positions accordingly)
        CreateAllyPoint(new Vector3(10f, 0f, 10f));
        CreateEnemyPoint(new Vector3(-5f, 0f, -5f));
    }

    private void Update()
    {
        // Optionally, update minimap orientation based on player's rotation
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }

    private void CreateAllyPoint(Vector3 position)
    {
        Image allyPoint = Instantiate(allyPointPrefab, transform);
        SetPointPosition(allyPoint, position);
    }

    private void CreateEnemyPoint(Vector3 position)
    {
        Image enemyPoint = Instantiate(enemyPointPrefab, transform);
        SetPointPosition(enemyPoint, position);
    }

    private void SetPointPosition(Image point, Vector3 worldPosition)
    {
        // Convert world position to minimap position
        Vector3 localPosition = new Vector3(worldPosition.x, worldPosition.z, 0f);
        point.rectTransform.anchoredPosition = localPosition;
    }
}

using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Camera mainCamera;
    private SpawnedIngredient spawnedIngredient;
    private Joint2D cursorAnchor;
    private Vector2 lastCursorPosition;
    private Vector2 cursorDelta;

    private const float forceMultiplier = 1000;
    private const float maxForceMagnitude = 2000;

    private void Awake()
    {
        mainCamera = Camera.main;
        cursorAnchor = GetComponentInChildren<Joint2D>();
    }

    Vector2 MousePointInWorld
    {
        get
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return new Vector2(ray.origin.x, ray.origin.y);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.transform == null)
                return;

            Debug.Log($"Hit {hit.transform.name}");

            switch (hit.transform.tag)
            {
                case "spawn":
                    var spawn = hit.transform.GetComponent<Spawn>();
                    var go = SpawnIngredient(spawn.type);
                    TakeIngredient(go.GetComponent<SpawnedIngredient>());
                    spawnedIngredient.ingredientData = GameData.ingredientDict[spawn.type];
                    break;

                case "spawnedIngredient":
                    TakeIngredient(hit.transform.GetComponent<SpawnedIngredient>());
                    break;

                default:
                    break;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Preparing picked ingredient for throwing if it's not null
            if (spawnedIngredient == null)
                return;

            // Calculating the force vector and restricting if it's magnitude is higher than allowed
            var forceToAdd = cursorDelta * forceMultiplier;
            if (forceToAdd.magnitude > maxForceMagnitude)
                forceToAdd = forceToAdd.normalized * maxForceMagnitude;

            spawnedIngredient._rigidbody2D.velocity = Vector2.zero;
            spawnedIngredient._rigidbody2D.AddForce(forceToAdd, ForceMode2D.Force);
            spawnedIngredient._rigidbody2D.AddTorque(Random.Range(-2F, 2F), ForceMode2D.Impulse);
            ReleaseIngredient();
        }
    }

    void FixedUpdate()
    {
        cursorAnchor.transform.position = MousePointInWorld;
        var newCursorPosition = MousePointInWorld;
        cursorDelta = newCursorPosition - lastCursorPosition;

        if (spawnedIngredient != null)
        {
            var rb = spawnedIngredient._rigidbody2D;
            rb.MovePosition(newCursorPosition);
        }

        lastCursorPosition = newCursorPosition;
    }

    private void TakeIngredient(SpawnedIngredient ingredient)
    {
        spawnedIngredient = ingredient;
        spawnedIngredient._rigidbody2D.angularVelocity = 0;
        spawnedIngredient.onColliderEnter += ReleaseIngredient;
    }

    private void ReleaseIngredient()
    {
        spawnedIngredient.onColliderEnter -= ReleaseIngredient;
        spawnedIngredient = null;
    }

    // Creating new ingredient in mouse position
    private GameObject SpawnIngredient(IngredientType type)
    {
        return Instantiate(
            Resources.Load<GameObject>($"Ingredients/Ingredient{type.ToString()}") as GameObject,
            MousePointInWorld,
            Quaternion.Euler(0, 0, Random.Range(-15, 15)),
            null);
    }

}
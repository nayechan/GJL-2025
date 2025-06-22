using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] private TMP_Text floorIndicatorTextField;
    [SerializeField] private GameObject backdrop, mainCamera, playerPrefab;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private Vector2 playerPos;
    
    private Window floorClearWindow;
    private Window statDistributeWindow;
    private Player player;

    public int CurrentFloor { get; private set; } = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        backdrop.SetActive(true);

        player = Player.Instance;
        if (player == null)
        {
            player = Instantiate(playerPrefab).GetComponent<Player>();
        }
        
        player.enabled = false;
        player.transform.position = playerPos;

        CurrentFloor = GameManager.Instance.CurrentFloor;
        floorIndicatorTextField.text = $"{CurrentFloor}F";
        mapGenerator.GenerateMap(CurrentFloor);
        player.enabled = true;
        
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        Vector3 cameraPos = new Vector3(playerPos.x, playerPos.y + cameraFollow.offsetY, cameraFollow.fixedZ);
        cameraFollow.transform.position = cameraPos;
        
        player.PlayerStatus.OnLevelUp.AddListener(OnLevelUp);
        
        backdrop.SetActive(false);
        
        floorClearWindow = WindowManager.Instance.GetWindow("FloorClearWindow");
        statDistributeWindow = WindowManager.Instance.GetWindow("StatDistributeWindow");
    }

    private void Update()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.KarmaGauge -= KarmaDecreaseAmount(CurrentFloor) * Time.deltaTime;
    }

    private float KarmaDecreaseAmount(int floor)
    {
        if (floor <= 10)
        {
            return 2.5f - 0.1f * (floor-1);
        }
        if (floor <= 20)
        {
            return 1.5f - 0.05f * (floor - 11);
        }

        return 1.0f;
    }

    public void OnMobKilled(IEnemy enemy)
    {
        GameManager gameManager = GameManager.Instance;
        //gameManager.KarmaGauge += (2 + Mathf.Log(enemy.CombatStatus.level) / Mathf.Log(1.5f));
        gameManager.KarmaGauge += 4;
    }

    public void ClearFloor()
    {
        StartCoroutine(ProgressClear());
    }

    public void OnLevelUp()
    { 
        statDistributeWindow.Open();
    }

    public IEnumerator ProgressClear()
    {
        KillAllEnemies();
        yield return new WaitUntil(() => player.isGrounded);

        floorClearWindow.Open();
        yield return new WaitUntil(() => !floorClearWindow.IsOpen);
        
        player.PlayerStatus.OnLevelUp.RemoveListener(OnLevelUp);
        GameManager.Instance.ClearFloor();
    }

    public void KillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}
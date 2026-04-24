using UnityEngine;

public enum GameState { Idle, Playing, Win, Lose }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Config")]
    public float gameDuration = 90f;

    public GameState State    { get; private set; } = GameState.Idle;
    public float     TimeLeft { get; private set; }

    void Awake() => Instance = this;

    void Start() => StartGame();

    void Update()
    {
        if (State != GameState.Playing) return;

        TimeLeft -= Time.deltaTime;
        UIManager.Instance?.UpdateTimer(TimeLeft);

        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            Lose();
        }
    }

    public void StartGame()
    {
        TimeLeft = gameDuration;
        State    = GameState.Playing;
        BoardManager.Instance?.GenerateBoard();
        FitCameraToBoard();
        UIManager.Instance?.UpdateTimer(TimeLeft);
        UIManager.Instance?.HideMessage();
    }

    private void FitCameraToBoard()
    {
        var cam   = Camera.main;
        var board = BoardManager.Instance;
        if (cam == null || board == null) return;

        float aspect      = (float)Screen.width / Screen.height;
        float boardW      = board.cols  * board.cellSize;
        float boardH      = board.rows  * board.cellSize;
        const float pad   = 1.20f;   // 20 % margin

        float byHeight = boardH * pad * 0.5f;
        float byWidth  = boardW * pad * 0.5f / aspect;

        cam.orthographicSize  = Mathf.Max(byHeight, byWidth);
        cam.transform.position = new Vector3(0f, 0f, -10f);
    }

    public void CheckWinCondition()
    {
        if (BoardManager.Instance == null) return;
        if (BoardManager.Instance.Holes.Count == 0)
            Win();
    }

    private void Win()
    {
        State = GameState.Win;
        UIManager.Instance?.ShowMessage("YOU WIN!", true);
    }

    private void Lose()
    {
        State = GameState.Lose;
        UIManager.Instance?.ShowMessage("TIME'S UP!", false);
    }

    public void RestartGame() => StartGame();
}

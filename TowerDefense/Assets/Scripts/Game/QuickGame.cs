using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class QuickGame : MonoBehaviour
{
    [field: SerializeField] private Vector2Int _boardSize;
    [field: SerializeField] private GameBoard _board;
    [field: SerializeField] private GameResultWindow _gameResultWindow;
    [field: SerializeField] private DefenderHud _defenderHud;
    [field: SerializeField] private TileBuilder _tileBuilder;
    [field: SerializeField] private PrepareGamePanel _prepareGamePanel;
    [field: SerializeField] private Camera _camera;
    [field: SerializeField] private GameTileContentFactory _contentFactory;
    [field: SerializeField] private EnemyFactory _enemyFactory;
    [field: SerializeField] private WarFactory _warFactory;
    [field: SerializeField] private GameScenario _scenario;
    [field: SerializeField, Range(10, 100)] private int _startingPlayerHealth = 100;
    [field: SerializeField, Range(5f, 30f)] private float _prepareTime = 10f;

    private bool _isPaused = false;
    private bool _scenarioInProcess = false;
    private GameScenario.State _activeScenario;
    private GameBehaviourCollection _enemies = new GameBehaviourCollection();
    private GameBehaviourCollection _nonEnemies = new GameBehaviourCollection();
    private CancellationTokenSource _prepareCancelation;

    private int _playerHealth;
    private int PlayerHealth
    {
        get => _playerHealth;
        set
        {
            _playerHealth = Mathf.Max(0, value);
            _defenderHud.UpdatePlayerHealth(_playerHealth, _startingPlayerHealth);
        }
    }

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
    private static QuickGame _instance;

    public string SceneName => Constants.Scenes.QUICK_GAME;
    public IEnumerable<GameObjectFactory> Factories => new GameObjectFactory[]{_contentFactory,
        _warFactory, _enemyFactory};

    private void OnEnable()
    {
        _instance = this;
    }

    private void Start()
    {
        _defenderHud.PauseClicked += OnPauseClicked;
        _defenderHud.QuitGame += OnQuitGame;
        _board.Initialze(_boardSize, _contentFactory);
        _tileBuilder.Initialize(_contentFactory, _camera, _board);
        BeginNewGame();
    }

    private void OnPauseClicked(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void OnQuitGame()
    {
        GoToMainMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            BeginNewGame();
        }        

        if (_scenarioInProcess)
        {
            var waves = _activeScenario.GetWaves();
            _defenderHud.UpdateScenarioWaves(waves.currentWave, waves.wavesCount);
            if (PlayerHealth <= 0)
            {
                _scenarioInProcess = false;
                _gameResultWindow.Show(GameResultType.Defeat, BeginNewGame, GoToMainMenu);
            }
            if (_activeScenario.Progress() == false && _enemies.IsEmpty)
            {
                _scenarioInProcess = false;
                _gameResultWindow.Show(GameResultType.Victory, BeginNewGame, GoToMainMenu);
            }
        }

        _enemies.GameUpdate();
        Physics.SyncTransforms();

        _board.GameUpdate();
        _nonEnemies.GameUpdate();
    }

    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        GameTile spawnpoint = _instance._board.GetSpawnpoint(UnityEngine.Random.Range(0, _instance._board.SpawnpointsCount));
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnpoint);
        _instance._enemies.Add(enemy);
    }

    public static Shell SpawnShell()
    {
        Shell shell = _instance._warFactory.Shell;
        _instance._nonEnemies.Add(shell);
        return shell;
    }

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = _instance._warFactory.Explosion;
        _instance._nonEnemies.Add(explosion);
        return explosion;
    }

    private async void BeginNewGame()
    {
        Cleanup();
        _tileBuilder.Enable();
        PlayerHealth = _startingPlayerHealth;

        try
        {
            _prepareCancelation?.Dispose();
            _prepareCancelation = new CancellationTokenSource();

            if (await _prepareGamePanel.Prepare(_prepareTime, _prepareCancelation.Token))
            {
                _activeScenario = _scenario.Begin();
                _scenarioInProcess = true;
            }
        }
        catch (TaskCanceledException e) 
        {
            #if UNITY_EDITOR
            Debug.LogError(e);
            #endif
        }
    }

    public void Cleanup()
    {
        _defenderHud.UpdatePlayerHealth(0, 0);
        _defenderHud.UpdateScenarioWaves(0, 0);
        _tileBuilder.Disable();
        _scenarioInProcess = false;
        _prepareCancelation?.Cancel();
        _prepareCancelation?.Dispose();
        _enemies.Clear();
        _nonEnemies.Clear();
        _board.Clear();
    }

    public static void EnemyReachedDestination()
    {
        _instance.PlayerHealth -= 1;
    }

    private void GoToMainMenu()
    {
        var operations = new Queue<ILoadingOperation>();
        operations.Enqueue(new ClearGameOperation(this));
        LoadingScreen.Instance.Load(operations);
    }
}

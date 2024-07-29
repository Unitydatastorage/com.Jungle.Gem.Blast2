using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace MatchThreeEngine
{
    public sealed class LevelGrid : MonoBehaviour
    {
      
        private readonly List<Tile> _selection = new List<Tile>();
        private bool _isSwapping;
        public GameObject EventGame;
        private bool _isMatching;
        private bool _isShuffling;
        private bool _isGameRunning;
        public TMP_Text scoreLose;
        public int score;
        public GameObject winPanel;
        public GameObject lostPanel;
        private float _timeRemaining;
        private Coroutine _GameTimer;
        public int currentLevel = 1;
        public TMP_Text currentLevelText;
        public Button[] levelButtons;
        public int levels = 1;
        public GameObject levelMenu;
        public Slider winTime; //
        public Slider winScoreSlider; //
        public Slider LoseTime; //
        public Slider LoseScoreSlider; //

        [SerializeField] private TileType[] tileTypes;
        [SerializeField] private RowController[] rows;
        [SerializeField] private AudioClip SoundEffect;
        [SerializeField] private AudioSource GameMusic;
        [SerializeField] private float AnimaTime;
        [SerializeField] private Transform Overlay;
        [SerializeField] private bool ensureNoStartingMatches;
        [SerializeField] private Slider TimeGameSlider;
        [SerializeField] private Slider GaolSlider; // Updated to Slider for score
        [SerializeField] private float TimeGame = 120f;

        public event Action<TileType, int> OnMatch;

        
        
        void DoNothing()
        {
            // Этот метод ничего не делает
        }


        private TileDataContainer[,] GameMake
        {
            get
            {
                var width = rows.Max(row => row.tiles.Length);
                var height = rows.Length;

                var data = new TileDataContainer[width, height];

                for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                        data[x, y] = GetTile(x, y).Data;

                return data;
            }
        }

        public void ButtonLevelSelect(int HandleLevelSelectButton)
        {
            currentLevel = HandleLevelSelectButton;
            levelMenu.SetActive(false);
            BeginGame();
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        void SaveLevelProgress()
        {
            PlayerPrefs.SetInt("LevelsCompleted", levels);
            PlayerPrefs.Save();
        }

        void Initialize()
        {
            LoadLevelProgress();
            HandleLevelSelectButton();
            GaolSlider.maxValue = 350; // 
        }

        public void AdvanceToNextLevel()
        {
            if (currentLevel < levels)
            {
                currentLevel++;
            }
            currentLevelText.text = $"Level {currentLevel}";

            PrepareBoardForNextLevel();
            BeginGame();
        }

        void PrepareBoardForNextLevel()
        {
            score = 0;
            GaolSlider.value = score; // Обновление слайдера очков
        }

        void LoadLevelProgress()
        {
            levels = PlayerPrefs.GetInt("LevelsCompleted", 1);
        }

        void HandleLevelSelectButton()
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = i < levels;
            }
        }

        public void BeginGame()
        {
            for (var y = 0; y < rows.Length; y++)
            {
                for (var x = 0; x < rows.Max(row => row.tiles.Length); x++)
                {
                    var tile = GetTile(x, y);

                    tile.x = x;
                    tile.y = y;

                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];

                    tile.button.onClick.AddListener(() => SelectTile(tile));
                }
            }
            score = 0;
            GaolSlider.value = score; // 

            if (score >= 350)
            {
                OnGameWon();
                return;
            }

            if (ensureNoStartingMatches)
            {
                StartCoroutine(EnsureNoInitialMatches());
            }
            HaltTimer();
            _timeRemaining = TimeGame;
            TimeGameSlider.maxValue = TimeGame;
            _isGameRunning = true;
            _GameTimer = StartCoroutine(GameTimer());
        }

        private IEnumerator GameTimer()
        {
            while (_timeRemaining > 0 && _isGameRunning)
            {
                _timeRemaining -= Time.deltaTime;
                TimeGameSlider.value = _timeRemaining;

                if (_timeRemaining <= 0)
                {
                    OnGameLost();
                }

                yield return null;
            }
        }

        public void HaltTimer()
        {
            _isGameRunning = false;
            if (_GameTimer != null)
            {
                StopCoroutine(_GameTimer);
            }
        }

        public void StartTimer()
        {
            _isGameRunning = true;
            _GameTimer = StartCoroutine(GameTimer());
        }

        public void RefreshScoreDisplay()
        {
            GaolSlider.value = score; // Обновление слайдера очков
            if (score >= 350)
                OnGameWon();
        }

        public void PostWinActions()
        {
            if (levels < 30)
            {
                levels++;
            }
            SaveLevelProgress();
            HandleLevelSelectButton();
        }

        void OnGameWon()
        {
            Debug.Log("Левел пройден!");

            if (winPanel != null)
            {
                // 
                if (winTime != null)
                {
                    winTime.value = _timeRemaining;
                }
                else
                {
                    Debug.LogError("winTimeSlider ошибка инспектора!");
                }

                // 
                if (winScoreSlider != null)
                {
                    winScoreSlider.value = score; //
                }
                else
                {
                    Debug.LogError("winScoreSlider ошибка инспектора!");
                }

                winPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("winPanel is not set in the inspector!");
            }

            HaltTimer(); 
        }


        void OnGameLost()
        {
            
            // 
            if (LoseTime != null)
            {
                LoseTime.value = _timeRemaining;
            } 
            // 
            if (LoseScoreSlider != null)
            {
                LoseScoreSlider.value = score; //
            }
           
            lostPanel.SetActive(true);
            
            HaltTimer();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var bestMove = LevelDataUtility.FindBestMove(GameMake);

                if (bestMove != null)
                {
                    SelectTile(GetTile(bestMove.X1, bestMove.Y1));
                    SelectTile(GetTile(bestMove.X2, bestMove.Y2));
                }
            }
            RefreshScoreDisplay();
        }

        private IEnumerator EnsureNoInitialMatches()
        {
            var wait = new WaitForEndOfFrame();

            while (LevelDataUtility.FindBestMatch(GameMake) != null)
            {
                ShuffleTiles();
                yield return wait;
            }
        }

        private Tile GetTile(int x, int y) => rows[y].tiles[x];

        private Tile[] GetTilesFromData(IList<TileDataContainer> TileDataContainer)
        {
            var length = TileDataContainer.Count;

            var tiles = new Tile[length];

            for (var i = 0; i < length; i++) tiles[i] = GetTile(TileDataContainer[i].X, TileDataContainer[i].Y);

            return tiles;
        }

        private async void SelectTile(Tile tile)
        {
            if (_isSwapping || _isMatching || _isShuffling)
            {
                Debug.Log("Action in progress, selection ignored.");
                return;
            }

            if (!_selection.Contains(tile))
            {
                if (_selection.Count > 0)
                {
                    if (Math.Abs(tile.x - _selection[0].x) == 1 && Math.Abs(tile.y - _selection[0].y) == 0
                        || Math.Abs(tile.y - _selection[0].y) == 1 && Math.Abs(tile.x - _selection[0].x) == 0)
                    {
                        _selection.Add(tile);
                    }
                }
                else
                {
                    _selection.Add(tile);
                }
            }

            if (_selection.Count < 2) return;

            _isSwapping = true;
            bool success = await SwapAndMatchTilesAsync(_selection[0], _selection[1]);
            if (!success)
            {
                await SwapTilesAsync (_selection[0], _selection[1]);
            }
            _isSwapping = false;

            _selection.Clear();
            EnsurePlayableGrid();
        }

        private async Task<bool> SwapAndMatchTilesAsync(Tile tile1, Tile tile2)
        {
            await SwapTilesAsync (tile1, tile2);
            if (await TryToMatchTilesAsync())
            {
                return true;
            }
            return false;
        }

        private async Task SwapTilesAsync (Tile tile1, Tile tile2)
        {
            var icon1 = tile1.icon;
            var icon2 = tile2.icon;

            var icon1Transform = icon1.transform;
            var icon2Transform = icon2.transform;

            icon1Transform.SetParent(Overlay);
            icon2Transform.SetParent(Overlay);

            icon1Transform.SetAsLastSibling();
            icon2Transform.SetAsLastSibling();

            icon1Transform.SetParent(tile2.transform);
            icon2Transform.SetParent(tile1.transform);

            tile1.icon = icon2;
            tile2.icon = icon1;

            var tile1Item = tile1.Type;
            tile1.Type = tile2.Type;
            tile2.Type = tile1Item;
        }

        private void EnsurePlayableGrid()
        {
            var matrix = GameMake;

            while (LevelDataUtility.FindBestMove(matrix) == null || LevelDataUtility.FindBestMatch(matrix) != null)
            {
                ShuffleTiles();
                matrix = GameMake;
            }
        }

        private async Task<bool> TryToMatchTilesAsync()
        {
            var didMatch = false;

            _isMatching = true;

            var match = LevelDataUtility.FindBestMatch(GameMake);

            while (match != null)
            {
                didMatch = true;

                var tiles = GetTilesFromData(match.Tiles);

                var deflateSequence = DOTween.Sequence();

                foreach (var tile in tiles)
                    deflateSequence.Join(tile.icon.transform.DOScale(Vector3.zero, AnimaTime).SetEase(Ease.InBack));

                GameMusic.PlayOneShot(SoundEffect);

                await deflateSequence.Play().AsyncWaitForCompletion();

                foreach (var tile in tiles)
                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];

                var inflateSequence = DOTween.Sequence();

                foreach (var tile in tiles)
                    inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, AnimaTime).SetEase(Ease.OutBack));

                await inflateSequence.Play().AsyncWaitForCompletion();

                OnMatch?.Invoke(tiles[0].Type, tiles.Length);

                int matchPoints = 10;
                if (tiles.Length >= 5)
                {
                    matchPoints += 10;
                }

                score += matchPoints;
                GaolSlider.value = score; // Обновление слайдера очков

                match = LevelDataUtility.FindBestMatch(GameMake);
            }

            _isMatching = false;

            return didMatch;
        }

        private void ShuffleTiles()
        {
            _isShuffling = true;

            foreach (var row in rows)
                foreach (var tile in row.tiles)
                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];

            _isShuffling = false;
        }
        
        public class EmptyClass
        {
            // Этот класс ничего не делает
        }


        public void RefreshGameBoard ()
        {
            float remainingTime = _timeRemaining;

            PrepareBoardForNextLevel();
            BeginGame();

            _timeRemaining = remainingTime;
            TimeGameSlider.value = _timeRemaining;
            StartCoroutine(GameTimer());
        }

        public void OnButtonActivated()
        {
            EventGame.SetActive(false);
            StartCoroutine("ResetGameEvent", 0.1f);
        }

        public void ResetGameEvent()
        {
            EventGame.SetActive(true);
        }
    }
}

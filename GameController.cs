using SplashKitSDK;

namespace FlappyBird
{
    public class GameController
    {
        private Window _GameWindow;
        private Bitmap _ThemeButtonBitmap;
        private Bitmap _PauseButtonBitmap;
        private Bitmap _BackgroundBitmap;
        private Bitmap _BackgroundBitmapDay;
        private Bitmap _BackgroundBitmapNight;
        private Bitmap _GroundBitmapDay;
        private Bitmap _GroundBitmapNight;
        private Bitmap _GroundBitmap;
        private float _groundX = 0; // Set initial ground position and scrolling speed
        private float _groundSpeed = 2;
        private Bitmap _PlayerBitmapDay;
        private Bitmap _PlayerBitmapNight;
        private bool _isDayTheme;
        private Music _backgroundMusic;

        public bool Quit { get; private set; } // property to tell the program to stop running the game

        private Player _Player;

        // Create a list to store game objects
        private List<GameObject> _GameObjects = new List<GameObject>();
        private List<GameObject> _GameObjectsToRemove = new List<GameObject>();

        // The gap that exists between pipe pairs for players to fly through
        private float _GAP;

        // Create a random number generator
        private Random _rnd = new Random();

        // Player score variable
        private double _playerScore = 0;
        private double _highScore = 0;

        // Boolean to start the game session in a paused state
        private bool _isStartMenu;
        private bool _isPaused;
        private bool _isGameOver;
        private bool _isGameOverBitmapDrawn;
        private Bitmap _GameOverBitmap;

        // Constructor to create the game controller
        // We will perform all initializations here
        public GameController()
        {
            SplashKit.LoadFont("Score", "Press_Start_2P/PressStart2P-Regular.ttf");
            // Load the background music
            _backgroundMusic = SplashKit.LoadMusic("BackgroundMusic", "background_music.mp3");

            // Play the background music
            SplashKit.PlayMusic(_backgroundMusic, -1); // -1 indicates the music should loop indefinitely
            _ThemeButtonBitmap = new Bitmap("ThemeButton", "images/theme_button.png");
            _PauseButtonBitmap = new Bitmap("PauseButton", "images/pause_button4.png");
            _BackgroundBitmapDay = new Bitmap("BackgroundDay", "images/background2.png");
            _BackgroundBitmapNight = new Bitmap("BackgroundNight", "images/background_night.png");
            _GroundBitmapDay = new Bitmap("GroundDay", "images/stoneground.png");
            _GroundBitmapNight = new Bitmap("GroundNight", "images/stoneground_night.png");

            _GroundBitmap = new Bitmap("Ground", "stoneground.png");
            _PlayerBitmapDay = new Bitmap("PlayerDay", "images/dove.png");
            _PlayerBitmapNight = new Bitmap("PlayerNight", "images/crow.png");
            _GameOverBitmap = new Bitmap("GameOver", "gameover.png");

            // Set the initial theme
            _BackgroundBitmap = _BackgroundBitmapDay;
            _GroundBitmap = _GroundBitmapDay;

            _GameWindow = new Window("Twilight Wings", _BackgroundBitmap.Width, _BackgroundBitmap.Height);
            _Player = new Player(_GameWindow);

            // Set the initial player
            _Player.SetPlayerBitmap(_PlayerBitmapDay); // Call a method to set the player bitmap

            _isDayTheme = true;

            _GAP = _Player.Height * 4.0f; // Wide enough to fit thrice the height of the player
            Quit = false;
            _isPaused = true;
            _isGameOver = false;
            _isGameOverBitmapDrawn = false;
            _isStartMenu = true;
        }


        public void HandleInput()
        {
            // Quit the game if the escape key is pressed or the window is closed
            if (SplashKit.KeyDown(KeyCode.EscapeKey) || _GameWindow.CloseRequested)
            {
                Quit = true;
                SplashKit.StopMusic();
                return;
            }

            // Handle input for start menu
            if (_isStartMenu)
            {
                HandleStartMenuInput();
                return;
            }

            // Start the game if it is paused, or reset if the game is over
            if (SplashKit.KeyDown(KeyCode.SpaceKey))
            {
                if (_isGameOver)
                {
                    _isStartMenu = true; // Go to the start menu when space is pressed after game over
                }
                else
                {
                    _isPaused = false;
                }

                if (_isGameOver)
                {
                    ResetGame();
                }
            }

            if (_isStartMenu && SplashKit.KeyDown(KeyCode.SpaceKey))
            {
                _isStartMenu = false;
                _isPaused = false;
            }

            // Handle pause menu input
            if (_isPaused)
            {
                float mouseX = SplashKit.MouseX();
                float mouseY = SplashKit.MouseY();

                if (SplashKit.MouseClicked(MouseButton.LeftButton))
                {
                    if (mouseX >= _GameWindow.Width / 2 - 60 && mouseX <= _GameWindow.Width / 2 + 60)
                    {
                        if (mouseY >= _GameWindow.Height / 2 - 20 && mouseY <= _GameWindow.Height / 2 + 20)
                        {
                            _isPaused = false;
                        }
                        else if (mouseY >= _GameWindow.Height / 2 + 30 && mouseY <= _GameWindow.Height / 2 + 70)
                        {
                            Quit = true;
                        }
                    }
                }
            }
        }

        private void IncreaseDifficulty()
        {
            // Increase difficulty based on score
            int scoreThreshold = (int)(_playerScore / 10); // Calculate how many thresholds have been crossed
            _GAP = _Player.Height * 4.0f - (scoreThreshold * 0.5f * _Player.Height); // Decrease the gap by 0.25 for each threshold crossed

            // Ensure the gap doesn't get too small
            if (_GAP < _Player.Height * 2.0f)
            {
                _GAP = _Player.Height * 2.0f; // Minimum gap size
            }

            Console.WriteLine($"Difficulty increased. New gap: {_GAP}");
        }

        // Draws all active game objects on screen
        public void Draw()
        {
            // When the game is over, we draw the GameOver bitmap and prevent any GameObject interaction
            if (_isGameOver)
            {
                if (!_isGameOverBitmapDrawn)
                {
                    _GameOverBitmap.Draw((_GameWindow.Width - _GameOverBitmap.Width) / 2, (_GameWindow.Height - _GameOverBitmap.Height) / 2 - 40);
                    _GameWindow.Refresh(60);
                    return;
                }

                // Draw the start menu after game over
                _isStartMenu = true;
                _isGameOverBitmapDrawn = false; // Reset flag for next game over
                return;
            }

            _GameWindow.Clear(Color.White);
            _BackgroundBitmap.Draw(0, 0);

            // Start Menu
            if (_isStartMenu)
            {
                DrawStartMenu();
                return;
            }

            // When the game is paused, we know the game session just began. We will only draw the background, the floor, and the player and
            // wait for the player to start the game
            if (_isPaused)
            {
                _Player.Draw();
                _Player.MaintainGround(_GameWindow.Height - _GroundBitmap.Height);

                // Draw the ground multiple times to create the illusion of infinite scrolling
                for (float x = _groundX; x < _GameWindow.Width; x += _GroundBitmap.Width)
                {
                    _GameWindow.DrawBitmap(_GroundBitmap, x, _GameWindow.Height - _GroundBitmap.Height);
                }

                SplashKit.DrawText("" + _playerScore, Color.White, "Score", 32, _GameWindow.Width / 2, 20);

                // Draw the theme button and pause button
                _PauseButtonBitmap.Draw(_GameWindow.Width - _PauseButtonBitmap.Width - 10, 10);
                
                DrawPauseMenu();

                // Handle button clicks in the pause menu
                if (SplashKit.MouseClicked(MouseButton.LeftButton))
                {
                    float mouseX = SplashKit.MouseX();
                    float mouseY = SplashKit.MouseY();
                    float themeButtonX = (_GameWindow.Width - _ThemeButtonBitmap.Width) / 2;
                    float themeButtonY = _GameWindow.Height / 2 + 100;

                    if (mouseX >= themeButtonX && mouseX <= themeButtonX + _ThemeButtonBitmap.Width &&
                        mouseY >= themeButtonY && mouseY <= themeButtonY + _ThemeButtonBitmap.Height)
                    {
                        ChangeTheme();
                    }
                }

                _GameWindow.Refresh(60);
                return;
            }

            IncreaseDifficulty();

            // Spawn pipes
            // Here we're checking if the space between the last added pipe and the end of the screen is above a certain amount (half the game window height)
            // If it is, then we add a new pair of pipes.
            if (_GameWindow.Width - (_GameObjects.Count > 0 ? (_GameObjects[_GameObjects.Count - 1] is Coin ? _GameObjects[_GameObjects.Count - 2].X : _GameObjects[_GameObjects.Count - 1].X) : 0) > _GameWindow.Height / 2)
            {
                // We've already set the _GAP (between pipe pairs) to be thrice the height of the player. Here we are trying to calculate the point
                // on the y-axis where we will begin to draw this gap. We have bounds from the top of the window, to the ground level.
                // We use the Random() object to generate a random value from this bound, with a padding of 100 on either side so the gap isn't
                // overly close to the edges to give our players a fair fight.
                float gapY = (float)_rnd.NextDouble() * (_GameWindow.Height - _GAP - _GroundBitmap.Height - 200) + 100; // Generate a random gapY

                // Once we get this gap origin, we place the bottom of the topPipe (read that again...bottom of the topPipe) at this origin point,
                // and the top of the bottomPipe (need I say it again?), the _GAP point away from this origin point.
                GameObject topPipe = new TopPipe(_GameWindow.Width, gapY, _isDayTheme);
                GameObject bottomPipe = new BottomPipe(_GameWindow.Width, gapY + _GAP, _isDayTheme);

                _GameObjects.Add(topPipe);
                _GameObjects.Add(bottomPipe);
            }
            else
            {
                // Spawn a coin
                // To spawn a coin, we just check if there is no coin on scene, and that enough space (1/4 of the game window height)
                // exists between our current position and the last added pipe pair
                if (!_GameObjects.Any(gameObject => gameObject is Coin) && _GameWindow.Width - (_GameObjects.Count > 0 ? _GameObjects[_GameObjects.Count - 1].X : 0) > _GameWindow.Height / 4)
                {
                    bool isCoinSpawned = false;
                    int maxAttempts = 100; // Maximum number of attempts to find a valid position
                    int attempts = 0;

                    while (!isCoinSpawned && attempts < maxAttempts)
                    {
                        // The padding here is reduced to make it tempting but not too easy for players to get the coins
                        int coinY = _rnd.Next(80, _GameWindow.Height - _GroundBitmap.Height - 80); // Generate a random coinY
                        bool overlapsWithPipe = false;

                        // Check if the generated coin position overlaps with any pipes
                        foreach (var gameObject in _GameObjects)
                        {
                            if (gameObject is TopPipe || gameObject is BottomPipe)
                            {
                                if (Math.Abs(gameObject.X - _GameWindow.Width) < gameObject._ObjectBitmap.Width &&
                                    Math.Abs(gameObject.Y - coinY) < gameObject._ObjectBitmap.Height)
                                {
                                    overlapsWithPipe = true;
                                    break;
                                }
                            }
                        }

                        // If no overlap is found, spawn the coin
                        if (!overlapsWithPipe)
                        {
                            GameObject coin = new Coin(_GameWindow.Width, coinY);
                            _GameObjects.Add(coin);
                            isCoinSpawned = true;
                        }

                        attempts++;
                    }
                }
            }

            _GameObjects.ForEach(gameObject =>
            {
                gameObject.Draw();

                // Thanks to Polymorphism, we can distinguish between GameObjects in our list
                if (gameObject is Coin)
                {
                    // We want to only check for collisions with coins, however that happens
                    // When there is a collision, we want to remove the coin from the scene, and reward the player
                    if (new Collision(_Player.CollidedWith)(gameObject))
                    {
                        _GameObjectsToRemove.Add(gameObject);
                        _playerScore += 3;
                    }
                }
                else
                {
                    // Here, we check for collisions with pipes; the only other type of GameObject we have
                    // First, we want to check if the player is close to a pair of pipes
                    // If the player is within range of a pair of pipes, one of two things will happen; either they collide with the pipe,
                    // or they pass through the gap successfully.
                    if (new Collision(_Player.IsWithinGameObjectBounds)(gameObject))
                    {
                        // We check if the player passes through successfully
                        if (new Collision(_Player.PassesGap)(gameObject))
                        {
                            // Since this will count for the pair of pipes, we can increment score by 0.5
                            _playerScore += 0.5;
                        }
                        else if (new Collision(_Player.CollidedWith)(gameObject))
                        {
                            // Game over
                            _isGameOver = true;
                            return;
                        }
                    }
                }

                // Remove game objects that go off-screen
                if (gameObject.IsOffScreen())
                {
                    _GameObjectsToRemove.Add(gameObject);
                }
            });

            _Player.Draw();
            _Player.HandleInput();
            _Player.MaintainGround(_GameWindow.Height - _GroundBitmap.Height);

            _GameObjectsToRemove.ForEach(gameObject =>
            {
                _GameObjects.Remove(gameObject);
            });

            // Update ground position
            _groundX -= _groundSpeed;

            // Reset ground position if it goes off-screen
            if (_groundX <= -_GroundBitmap.Width)
            {
                _groundX = 0;
            }

            // Draw the ground multiple times to create the illusion of infinite scrolling
            for (float x = _groundX; x < _GameWindow.Width; x += _GroundBitmap.Width)
            {
                _GameWindow.DrawBitmap(_GroundBitmap, x, _GameWindow.Height - _GroundBitmap.Height);
            }

            // Draw the player's score on the screen with the Score font
            SplashKit.DrawText("" + _playerScore, Color.White, "Score", 32, _GameWindow.Width / 2, 20);
            if (_highScore > 0)
            {
                SplashKit.DrawText("High Score: " + _highScore, Color.White, "Score", 12, 10, 20);
            }

            // Draw the pause button
            _PauseButtonBitmap.Draw(_GameWindow.Width - _PauseButtonBitmap.Width - 10, 10);

            _GameWindow.Refresh(60);

            // Handle pause button click
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                float mouseX = SplashKit.MouseX();
                float mouseY = SplashKit.MouseY();
                if (mouseX >= _GameWindow.Width - _PauseButtonBitmap.Width - 10 &&
                    mouseX <= _GameWindow.Width - 10 &&
                    mouseY >= 10 && mouseY <= 10 + _PauseButtonBitmap.Height)
                {
                    TogglePause();
                }
            }
        }

        private void ResetGame()
        {
            _Player = new Player(_GameWindow);
            _Player.SetPlayerBitmap(_isDayTheme ? _PlayerBitmapDay : _PlayerBitmapNight);

            // Update pipes to current theme
            foreach (var gameObject in _GameObjects.OfType<BottomPipe>())
            {
                gameObject.SetTheme(_isDayTheme);
            }

            foreach (var gameObject in _GameObjects.OfType<TopPipe>())
            {
                gameObject.SetTheme(_isDayTheme);
            }

            // Keep a record of the highest score for the game session
            if (_playerScore > _highScore)
            {
                _highScore = _playerScore;
            }
            _playerScore = 0;
            _GameObjects = new List<GameObject>();
            _GameObjectsToRemove = new List<GameObject>();
            _isGameOver = false;

            // Set to start menu
            _isStartMenu = true;
            _isPaused = false; // Ensure the game is not paused when displaying the start menu

            // Set background and ground bitmaps according to the current theme
            _BackgroundBitmap = _isDayTheme ? _BackgroundBitmapDay : _BackgroundBitmapNight;
            _GroundBitmap = _isDayTheme ? _GroundBitmapDay : _GroundBitmapNight;

            SplashKit.PlayMusic(_backgroundMusic, -1); // Restart the music if it was stopped
        }




        private void ChangeTheme()
        {
            // Toggle the theme
            _isDayTheme = !_isDayTheme;

            // Update background, ground, and player based on the new theme
            if (_isDayTheme)
            {
                _BackgroundBitmap = _BackgroundBitmapDay;
                _GroundBitmap = _GroundBitmapDay;
                _Player.SetPlayerBitmap(_PlayerBitmapDay);
            }
            else
            {
                _BackgroundBitmap = _BackgroundBitmapNight;
                _GroundBitmap = _GroundBitmapNight;
                _Player.SetPlayerBitmap(_PlayerBitmapNight);
            }

            // Update all pipes to the new theme
            foreach (var gameObject in _GameObjects.OfType<BottomPipe>())
            {
                gameObject.SetTheme(_isDayTheme);
            }

            foreach (var gameObject in _GameObjects.OfType<TopPipe>())
            {
                gameObject.SetTheme(_isDayTheme);
            }
        }

        private void TogglePause()
        {
            if (_isGameOver)
                return;

            _isPaused = !_isPaused;

            // If the game was just unpaused, reset the game over flag
            if (!_isPaused)
            {
                _isGameOver = false;
            }
        }

        private void DrawPauseMenu()
        {
            Color semiTransparentBlack = Color.RGBAColor(0, 0, 0, 128); // 50% transparent black

            // Draw the semi-transparent background
            SplashKit.FillRectangle(semiTransparentBlack, 0, 0, _GameWindow.Width, _GameWindow.Height);

            // Calculate text widths and positions for centering
            string pauseText = "Game Paused";
            string resumeText = "Resume";
            string exitText = "Exit";

            int pauseTextWidth = SplashKit.TextWidth(pauseText, "Score", 48);
            int resumeTextWidth = SplashKit.TextWidth(resumeText, "Score", 32);
            int exitTextWidth = SplashKit.TextWidth(exitText, "Score", 32);

            float pauseTextX = (_GameWindow.Width - pauseTextWidth) / 2;
            float resumeTextX = (_GameWindow.Width - resumeTextWidth) / 2;
            float exitTextX = (_GameWindow.Width - exitTextWidth) / 2;
            float themeButtonX = (_GameWindow.Width - _ThemeButtonBitmap.Width) / 2;

            float pauseTextY = _GameWindow.Height / 2 - 100;
            float resumeTextY = _GameWindow.Height / 2;
            float exitTextY = _GameWindow.Height / 2 + 50;
            float themeButtonY = _GameWindow.Height / 2 + 100; // Position the theme button below exit text

            // Draw the text and the theme button
            SplashKit.DrawText(pauseText, Color.White, "Score", 48, pauseTextX, pauseTextY);
            SplashKit.DrawText(resumeText, Color.White, "Score", 32, resumeTextX, resumeTextY);
            SplashKit.DrawText(exitText, Color.White, "Score", 32, exitTextX, exitTextY);
            _ThemeButtonBitmap.Draw(themeButtonX, themeButtonY);

            _GameWindow.Refresh(60);
        }

        private void DrawStartMenu()
        {
            Color semiTransparentBlack = Color.RGBAColor(0, 0, 0, 128); // 50% transparent black

            // Draw the semi-transparent background
            SplashKit.FillRectangle(semiTransparentBlack, 0, 0, _GameWindow.Width, _GameWindow.Height);

            // Calculate text widths and positions for centering
            string gameTitleText = "Twilight Wings";
            string startText = "Start";
            string exitText = "Exit";

            int gameTitleTextWidth = SplashKit.TextWidth(gameTitleText, "Score", 48);
            int startTextWidth = SplashKit.TextWidth(startText, "Score", 32);
            int exitTextWidth = SplashKit.TextWidth(exitText, "Score", 32);

            float gameTitleTextX = (_GameWindow.Width - gameTitleTextWidth) / 2;
            float startTextX = (_GameWindow.Width - startTextWidth) / 2;
            float exitTextX = (_GameWindow.Width - exitTextWidth) / 2;

            float gameTitleTextY = _GameWindow.Height / 2 - 100;
            float startTextY = _GameWindow.Height / 2;
            float exitTextY = _GameWindow.Height / 2 + 50;

            // Draw the text
            SplashKit.DrawText(gameTitleText, Color.White, "Score", 48, gameTitleTextX, gameTitleTextY);
            SplashKit.DrawText(startText, Color.White, "Score", 32, startTextX, startTextY);
            SplashKit.DrawText(exitText, Color.White, "Score", 32, exitTextX, exitTextY);

            _GameWindow.Refresh(60);
        }

        private void HandleStartMenuInput()
        {
            float mouseX = SplashKit.MouseX();
            float mouseY = SplashKit.MouseY();

            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                if (mouseX >= _GameWindow.Width / 2 - 60 && mouseX <= _GameWindow.Width / 2 + 60)
                {
                    if (mouseY >= _GameWindow.Height / 2 - 20 && mouseY <= _GameWindow.Height / 2 + 20)
                    {
                        _isStartMenu = false;
                        _isPaused = false;
                    }
                    else if (mouseY >= _GameWindow.Height / 2 + 30 && mouseY <= _GameWindow.Height / 2 + 70)
                    {
                        Quit = true;
                    }
                }
            }
        }
    }
}

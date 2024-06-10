using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Direction, int> dirToRotations = new()
        {
            {Direction.Up, 0},
            {Direction.Down, 180},
            {Direction.Right, 90},
            {Direction.Left, 270},
        };

        private readonly int rows = 25;
        private readonly int columns = 25;
        private readonly Image[,] gridImages;
        private GameState gamestate;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = InitializeGameGrid();
            gamestate = new GameState(rows, columns);
        }

        private Image[,] InitializeGameGrid()
        {
            Image[,] images = new Image[rows, columns];
            GameGrid.Rows = rows;
            GameGrid.Columns = columns;

            InitializeGridImages(images);

            return images;
        }

        private void InitializeGridImages(Image[,] images)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Image image = new Image()
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
        }

        private void RenderGame()
        {
            RenderGrid();
            RenderSnakeHead();
            ScoreText.Text = $"SCORE {gamestate.Score}";
        }

        private void RenderGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    GridValue gridVal = gamestate.grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void RenderSnakeHead()
        {
            Position headPos = gamestate.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Column];
            image.Source = Images.Head;

            int rotation = dirToRotations[gamestate.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task RenderDeadSnake()
        {
            List<Position> positions = new (gamestate.SnakePositions());

            for (int i = 0; i < positions.Count; i++) 
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task RunGame()
        {
            RenderGame();
            await DisplayCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await DisplayGameOverScreen();
            gamestate = new GameState(rows, columns);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (gamestate.GameOver)
            {
                return;
            }

            switch (e.Key) 
            {
                case Key.Left:
                    gamestate.ChangeDirection(Direction.Left); 
                    break;

                case Key.Right:
                    gamestate.ChangeDirection(Direction.Right); 
                    break;
                case Key.Up:
                    gamestate.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gamestate.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gamestate.GameOver)
            {
                await Task.Delay(100);
                gamestate.Move();
                RenderGame();
            }
        }

        private async Task DisplayCountDown()
        {
            for (int i = 3; i >= 0; i--) 
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task DisplayGameOverScreen()
        {
            await RenderDeadSnake();
            await Task.Delay(100);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "\tGAME OVER\nPRESS ANY KEY TO START";
        }
    }
}
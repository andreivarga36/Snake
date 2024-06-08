using System.Runtime.InteropServices.Marshalling;
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

        private readonly int rows = 15;
        private readonly int columns = 15;
        private readonly Image[,] gridImages;
        private readonly GameState gamestate;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gamestate = new GameState(rows, columns);
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, columns];
            GameGrid.Rows = rows;
            GameGrid.Columns = columns;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Image image = new Image()
                    {
                        Source = Images.Empty
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"{gamestate.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    GridValue gridVal = gamestate.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            await GameLoop();
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
                Draw();
            }
        }
    }
}
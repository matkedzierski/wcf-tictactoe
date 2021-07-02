using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TicTacToe.WCFGame;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private readonly GameServiceClient client = new GameServiceClient();
        private readonly Button[,] buttons;

        public MainWindow()
        {
            InitializeComponent();
            buttons = new[,] { { b0, b1, b2 }, { b3, b4, b5 }, { b6, b7, b8 } };
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            client.Start();
            status.Content = "Rozpoczęto grę! Twoja kolej!";
            
            // clear the panels
            var all = board.Children.Cast<UIElement>().ToList();
            foreach (var el in all.OfType<StackPanel>())
            {
                board.Children.Remove(el);
            }

            foreach(var b in buttons)
            {
                b.Content = "";
                b.IsEnabled = true;
            }
            startb.IsEnabled = false;
        }

        private void UserMove(object sender, RoutedEventArgs e)
        {
            var bt = (Button)sender;
            var r = Grid.GetRow(bt);
            var c = Grid.GetColumn(bt);

            var moved = client.Move(r, c, out var serverRow, out var serverCol);
            if (moved) bt.Content = "O";
            else {
                status.Content = "Nie możesz wykonać takiego ruchu!";
                return;
            }

            var userResult = client.CheckWin(r, c);
            if(userResult != 0)
            {
                status.Content = "Wygrałeś!";
                Stop(r, c, userResult, true);
                return;
            }
            

            if (serverCol == -1 && serverRow == -1)
            {
                status.Content = "Remis!";
                Stop(r, c, 0, false);
                return;
            }
                

            var srv = (Button)board.Children.Cast<UIElement>()
                .First(b => Grid.GetRow(b) == serverRow && Grid.GetColumn(b) == serverCol);

            srv.Content = "X";

            var serverResult = client.CheckWin(serverRow, serverCol);
            if (serverResult != 0)
            {
                status.Content = "Serwer wygrał!";
                Stop(serverRow, serverCol, serverResult, false);
                return;
            }

            status.Content = "Gra w toku...";
        }

        private void Stop(int w, int k, int result, bool userWon)
        {
            var brush = userWon ? Brushes.LawnGreen : Brushes.Red;
            switch (result)
            {
                case 0:
                    //draw
                    break;
                // vertical
                case 1:
                {
                    for (var i = 0; i < 3; i++){
                        //buttons[i, k]
                        Panel p = new StackPanel { Background = brush, Opacity = 0.3 };
                        board.Children.Add(p);
                        Grid.SetRow(p, i);
                        Grid.SetColumn(p, k);
                    }

                    break;
                }
                // horizontal
                case 2:
                {
                    for (var i = 0; i < 3; i++){
                        //buttons[w, i].Style = (Style)Resources["winning"];
                        Panel p = new StackPanel { Background = brush, Opacity = 0.3 };
                        board.Children.Add(p);
                        Grid.SetRow(p, w);
                        Grid.SetColumn(p, i);
                    }

                    break;
                }
                // diagonall \
                case 4:
                {
                    for (var i = 0; i < 3; i++)
                    {
                        //buttons[i, i].Style = (Style)Resources["winning"];
                        Panel p = new StackPanel { Background = brush, Opacity = 0.3 };
                        board.Children.Add(p);
                        Grid.SetRow(p, i);
                        Grid.SetColumn(p, i);
                    }

                    break;
                }
                // diagonal /
                case 8:
                {
                    for (var i = 0; i < 3; i++)
                    {
                        //buttons[i, 2-i].Style = (Style)Resources["winning"];
                        Panel p = new StackPanel { Background = brush, Opacity = 0.3 };
                        board.Children.Add(p);
                        Grid.SetRow(p, i);
                        Grid.SetColumn(p, 2-i);
                    }
                    break;
                }
            }

            foreach (var b in buttons)
            {

                b.IsEnabled = false;
            }
            startb.IsEnabled = true;
        }

    }
}

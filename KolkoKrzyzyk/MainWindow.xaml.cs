using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KolkoKrzyzyk.ServiceKolkoKrzyzyk;

namespace KolkoKrzyzyk
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GraKolkoKrzyzykClient client = new GraKolkoKrzyzykClient();
        private Button[,] buttons;

        public MainWindow()
        {
            InitializeComponent();
            buttons = new Button[,] { { b0, b1, b2 }, { b3, b4, b5 }, { b6, b7, b8 } };
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            client.Start();
            status.Content = "Rozpoczęto grę! Twoja kolej!";
            
            // wyczyszczenie kolorowych paneli
            List<UIElement> all = new List<UIElement>();
            foreach (UIElement el in plansza.Children) all.Add(el);
            foreach(var el in all)
            {
                if (el is StackPanel) plansza.Children.Remove(el as StackPanel);
            }



            foreach(var b in buttons)
            {
                b.Content = "";
                b.IsEnabled = true;
            }
            startb.IsEnabled = false;
        }

        private void Ruch(object sender, RoutedEventArgs e)
        {
            status.Content = "Kolej serwera!";
            Button kl = (Button)sender;
            int w = Grid.GetRow(kl);
            int k = Grid.GetColumn(kl);

            bool wykonano = client.WykonajRuch(w, k, out int wierszServer, out int kolumnaServer);
            if (wykonano) kl.Content = "O";
            else {
                status.Content = "Nie możesz wykonać takiego ruchu!";
                return;
            }

            int wynik = client.SparwdzWygrana(w, k);
            if(wynik!=0)
            {
                status.Content = "Wygrałeś!";
                Stop(w, k, wynik);
                return;
            }
            

            if (kolumnaServer == -1 && kolumnaServer == -1)
            {
                status.Content = "Remis!";
                Stop(w, k, 0);
                return;
            }
                

            Button srv = (Button)plansza.Children.Cast<UIElement>()
                .First(b => Grid.GetRow(b) == wierszServer && Grid.GetColumn(b) == kolumnaServer);

            srv.Content = "X";

            int wynikServer = client.SparwdzWygrana(wierszServer, kolumnaServer);
            if (wynikServer != 0)
            {
                status.Content = "Serwer wygrał!";
                Stop(wierszServer, kolumnaServer, wynikServer);
                return;
            }

            status.Content = "Twoja kolej!";
        }

        private void Stop(int w, int k, int result)
        {
            
            if (result == 0)
            {
                //remis
            }
            else if (result == 1) // pionowo
            {
                for (var i = 0; i < 3; i++){
                    //buttons[i, k]
                    Panel p = new StackPanel { Background = Brushes.Aqua, Opacity = 0.3 };
                    plansza.Children.Add(p);
                    Grid.SetRow(p, i);
                    Grid.SetColumn(p, k);
                }
            }
            else if (result == 2) // poziomo
            {
                for (var i = 0; i < 3; i++){
                    //buttons[w, i].Style = (Style)Resources["winning"];
                    Panel p = new StackPanel { Background = Brushes.Aqua, Opacity = 0.3 };
                    plansza.Children.Add(p);
                    Grid.SetRow(p, w);
                    Grid.SetColumn(p, i);
                }
            }
            else if (result == 4) // skos \
            {
                for (var i = 0; i < 3; i++)
                {
                    //buttons[i, i].Style = (Style)Resources["winning"];
                    Panel p = new StackPanel { Background = Brushes.Aqua, Opacity = 0.3 };
                    plansza.Children.Add(p);
                    Grid.SetRow(p, i);
                    Grid.SetColumn(p, i);
                }
            }
            else if (result == 8) // skos /
            {
                for (var i = 0; i < 3; i++)
                {
                    //buttons[i, 2-i].Style = (Style)Resources["winning"];
                    Panel p = new StackPanel { Background = Brushes.Aqua, Opacity = 0.3 };
                    plansza.Children.Add(p);
                    Grid.SetRow(p, i);
                    Grid.SetColumn(p, 2-i);
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

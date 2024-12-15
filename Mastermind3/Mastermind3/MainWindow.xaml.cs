using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mastermind3
{
    public partial class MainWindow : Window
    {
        private string[] beschikbareKleuren = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
        private string color1, color2, color3, color4;
        private int pogingen = 0;
        private int score = 0;
        private List<string> spelers = new List<string>(); 
        private int huidigeSpelerIndex = 0; 

        public MainWindow()
        {
            InitializeComponent();
            GenereerGeheimeCode(out color1, out color2, out color3, out color4);
            Title = "MijnMastermind";
            VraagSpelers(); 
        }

        
        private void VraagSpelers()
        {
            string spelerNaam;
            do
            {
                spelerNaam = Microsoft.VisualBasic.Interaction.InputBox("Voer de naam van de speler in:", "Speler toevoegen", "");
                if (!string.IsNullOrEmpty(spelerNaam))
                {
                    spelers.Add(spelerNaam);
                }
            } while (!string.IsNullOrEmpty(spelerNaam) && MessageBox.Show("Wil je nog een speler toevoegen?", "Nieuwe Speler?", MessageBoxButton.YesNo) == MessageBoxResult.Yes);

            
            UpdateHuidigeSpeler();
        }

        
        private void UpdateHuidigeSpeler()
        {
            string spelerNaam = spelers[huidigeSpelerIndex];
            spelerNaamLabel.Content = $"Huidige speler: {spelerNaam}";
            scoreLabel.Content = $"Score: {score}";
            pogingenLabel.Content = $"Pogingen: {10 - pogingen}";
        }

        
        private void EindeSpel(bool gewonnen)
        {
            string spelerNaam = spelers[huidigeSpelerIndex];
            if (gewonnen)
            {
                MessageBox.Show($"Gefeliciteerd {spelerNaam}, je hebt de code gekraakt!");
            }
            else
            {
                MessageBox.Show($"Helaas {spelerNaam}, je hebt verloren. De code was: {color1}, {color2}, {color3}, {color4}.");
            }

           
            huidigeSpelerIndex = (huidigeSpelerIndex + 1) % spelers.Count;
            UpdateHuidigeSpeler();

            
            var result = MessageBox.Show($"Wil je opnieuw spelen, {spelerNaam}?", "Mastermind", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ResetSpel();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        
        private void ResetSpel()
        {
            pogingen = 0;
            score = 0;
            GenereerGeheimeCode(out color1, out color2, out color3, out color4);
            historiekListBox.Items.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            UpdateHuidigeSpeler();
        }

        
        private void GenereerGeheimeCode(out string color1, out string color2, out string color3, out string color4)
        {
            Random random = new Random();
            List<string> gekozenKleuren = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                string kleur;
                do
                {
                    kleur = beschikbareKleuren[random.Next(beschikbareKleuren.Length)];
                } while (gekozenKleuren.Contains(kleur));

                gekozenKleuren.Add(kleur);
            }
            color1 = gekozenKleuren[0];
            color2 = gekozenKleuren[1];
            color3 = gekozenKleuren[2];
            color4 = gekozenKleuren[3];
        }

        
        private void codeKnop_Click(object sender, RoutedEventArgs e)
        {
            if (pogingen >= 10)
            {
                MessageBox.Show("Je kansen zijn op!");
                EindeSpel(false);
                return;
            }

            pogingen++;
            pogingenLabel.Content = (10 - pogingen).ToString();

            string gekozen1 = comboBox1.Text;
            string gekozen2 = comboBox2.Text;
            string gekozen3 = comboBox3.Text;
            string gekozen4 = comboBox4.Text;

            if (string.IsNullOrEmpty(gekozen1) || string.IsNullOrEmpty(gekozen2) ||
                string.IsNullOrEmpty(gekozen3) || string.IsNullOrEmpty(gekozen4))
            {
                MessageBox.Show("Vul alle velden in voordat je op 'Controleer' klikt.");
                return;
            }

            List<string> gekozenKleuren = new List<string> { gekozen1, gekozen2, gekozen3, gekozen4 };
            List<string> geheimeKleuren = new List<string> { color1, color2, color3, color4 };

            int exactGoed = 0, kleurGoed = 0;
            for (int i = 0; i < 4; i++)
            {
                if (gekozenKleuren[i] == geheimeKleuren[i])
                {
                    exactGoed++;
                }
                else if (geheimeKleuren.Contains(gekozenKleuren[i]))
                {
                    kleurGoed++;
                }
            }

            string pogingInfo = $"Poging {pogingen}: {gekozen1}, {gekozen2}, {gekozen3}, {gekozen4} - Rood: {exactGoed}, Wit: {kleurGoed}";
            historiekListBox.Items.Add(pogingInfo);

            if (exactGoed == 4)
            {
                EindeSpel(true);
                return;
            }

            if (pogingen >= 10)
            {
                EindeSpel(false);
            }
        }
    }
}

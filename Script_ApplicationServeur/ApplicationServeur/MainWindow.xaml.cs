using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace ApplicationServeur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public string nvJeton = "";
        public string jetonServeur = "";
        public string ancienJetonServer = "";
        public int nombreEssaies = 5;
        private readonly long seed = 285516254816548;
        public MainWindow()
        {
            InitializeComponent();

            //Coller 
            Code1.PreviewKeyDown += Code_PreviewKeyDown;
            Code2.PreviewKeyDown += Code_PreviewKeyDown;
            Code3.PreviewKeyDown += Code_PreviewKeyDown;
            Code4.PreviewKeyDown += Code_PreviewKeyDown;
            Code5.PreviewKeyDown += Code_PreviewKeyDown;
            Code6.PreviewKeyDown += Code_PreviewKeyDown;
            Code7.PreviewKeyDown += Code_PreviewKeyDown;
            Code8.PreviewKeyDown += Code_PreviewKeyDown;


            // Avec un delai de 1 seconde, ca génère un jeton 
            Task.Delay(1000).ContinueWith(_ =>
            {
                jetonServeur = GenerateHashedOTP();
            });


            StartTimer();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Code_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //  (Ctrl + V)
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true; // On bloque l'action normale du collage

                
                string texteColle = Clipboard.GetText().Trim();

                if (texteColle.Length == 8 && texteColle.All(char.IsDigit))
                {
                    //  Remplit chaque champ avec le chiffre correspondant
                    Code1.Text = texteColle[0].ToString();
                    Code2.Text = texteColle[1].ToString();
                    Code3.Text = texteColle[2].ToString();
                    Code4.Text = texteColle[3].ToString();
                    Code5.Text = texteColle[4].ToString();
                    Code6.Text = texteColle[5].ToString();
                    Code7.Text = texteColle[6].ToString();
                    Code8.Text = texteColle[7].ToString();

                   
                    Code8.Focus();
                }
            }
        }

        private void Code_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox currentBox = sender as TextBox;
            if (currentBox != null && currentBox.Text.Length == 1)
            {
                // Trouver la prochaine case et lui donner le focus
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                currentBox.MoveFocus(request);
            }
        }

        private void VerifierCode_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les 8 chiffres et les concaténer en un string
            string codeSaisi = Code1.Text + Code2.Text + Code3.Text + Code4.Text +
                               Code5.Text + Code6.Text + Code7.Text + Code8.Text;

            // Vérifier que l'utilisateur a bien entré 8 chiffres
            if (codeSaisi.Length != 8)
            {
                return;
            }

            // Comparer le code entré avec le jeton généré
            if (codeSaisi == jetonServeur || codeSaisi == ancienJetonServer)
            {
                messageValidation.Text = "Accès confirmé !";
                messageValidation.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                messageValidation.Text = "Accès refusé !";
                messageValidation.Foreground = new SolidColorBrush(Colors.Red);
                ViderChamps();
                nombreEssaies--;
                nombreEssaie.Text = nombreEssaies.ToString();
                if (nombreEssaies == 0)
                    this.Close();
            }
        }
        private void Nettoyer_Click(object sender, RoutedEventArgs e)
        {
            ViderChamps();
            return;
        }
        private void ViderChamps()
        {
            Code1.Text = "";
            Code2.Text = "";
            Code3.Text = "";
            Code4.Text = "";
            Code5.Text = "";
            Code6.Text = "";
            Code7.Text = "";
            Code8.Text = "";
            return;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int secondsLeft = 30 - (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 30);
            if (secondsLeft ==30)
            {
                nvJeton = GenerateHashedOTP();
                ancienJetonServer = jetonServeur;
                AncienJetonText.Visibility = Visibility.Visible;
                AncienJeton.Text = ancienJetonServer;
                AncienJeton.Visibility = Visibility.Visible;
                jetonServeur = nvJeton;
            }

        }


        //hash du OTP generer
     

        private string GenerateHashedOTP()
        {
            double BrutOtp = Algorithme();
            return HashOTP(BrutOtp.ToString(), seed.ToString());
        }


        // Hachage HMC SHA256

        private string HashOTP(string otp, string seed)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(seed);
            byte[] otpBytes = Encoding.UTF8.GetBytes(otp);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(otpBytes);

                // On extrait 4 octets pseudo aleatoires pour generer un otp final
                int offset = hash[hash.Length - 1] & 0xf;
                int hashedOTP = (hash[offset] & 0x7F) << 24 |
                    (hash[offset + 1] & 0xFF) << 16
                    | (hash[offset + 2] & 0xFF) << 8
                    | (hash[offset + 3] & 0xFF);

                return (hashedOTP % 100000000).ToString("D8");
            }



        }


        public double Algorithme()
        {
            long temps = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            temps = temps - (temps % 30); //modulo 30 pour garder toujours l'intervalle de 30s
                                         
            long jeton = temps * this.seed * 6364136223846793005L;   //Ici 6364136223846793005 est un très grand nombre premier pour augmenter énomrément l'aléatoire
            jeton %= 982451653;     //On fait le modulo d'un grand nombre premier pour réduire le nombre de chiffre et augmenter l'effet aléatoire

            jeton = Math.Abs(jeton);

            jeton %= 100000000;      //Le modulo de 100 000 000 renvoit la parti entiere de la division de 100 000 000 ce qui assure un résultat entre 0 et 99 999 999 donc un nombre de 8 chiffres

            return jeton;
        }
       
    }
}

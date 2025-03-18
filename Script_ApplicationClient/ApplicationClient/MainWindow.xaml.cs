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
using System.Security.Cryptography;

namespace ApplicationClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private readonly long seed = 285516254816548; //notre clef secrete
        public MainWindow()
        {
            InitializeComponent();
            StartTimer();
            jeton.Text = GenerateHashedOTP();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

       private long lastUpdate = -1;

 private void Timer_Tick(object sender, EventArgs e)
 {
     long currentPeriod = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
     int secondsLeft = 30 - (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 30);
     tictac.Text = $"temps restant : {secondsLeft} ";

     if (currentPeriod != lastUpdate)
     {
         jeton.Text = GenerateHashedOTP();
         lastUpdate = currentPeriod;
     }
 }
        private void CopyOTP(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(jeton.Text);

            // Affiche l'InfoBar (notification) et la cache après 2 secondes
            notifBar.Visibility = Visibility.Visible;

            Task.Delay(2000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => notifBar.Visibility = Visibility.Collapsed);
            });
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
            temps = temps - (temps % 30); 
                                          //Nombre de ticks depuis le 1er janvier de l'an 1 avec le temps UTC qui est la norme
            long jeton = temps * this.seed * 6364136223846793005L;   //Ici 6364136223846793005 est un très grand nombre premier pour augmenter énomrément l'aléatoire
            jeton %= 982451653;     //On fait le modulo d'un grand nombre premier pour réduire le nombre de chiffre et augmenter l'effet aléatoire

            jeton = Math.Abs(jeton);

            jeton %= 100000000;      //Le modulo de 100 000 000 renvoit la parti entiere de la division de 100 000 000 ce qui assure un résultat entre 0 et 99 999 999 donc un nombre de 8 chiffres

            return jeton;
        }
    }
}

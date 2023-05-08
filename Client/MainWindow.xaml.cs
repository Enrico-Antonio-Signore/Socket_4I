using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Dichiarazione del Socket e del DispatcherTimer
        Socket socket = null;
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            InitializeComponent(); //Inizializza interfaccia

            //Creazione di un socket UDP associato alla porta 11000 su tutte le interfacce di rete
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //Associata a tutte le interfaccie di rete
            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 11000); //Porta Mittente

            //Associa il socket a tutte le interfaccie
            socket.Bind(local_endpoint);

            //Il Socket in modalità non bloccante e eabilita dei messaggi in broadcast
            socket.Blocking = false;
            socket.EnableBroadcast = true;

            //Creazione di un timer che avvia il metodo (aggiornamento_dTimer) ogni 250 millisecondi
            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            //Intervallo di aggiornamento
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dTimer.Start();

        }
        //Metodo che viene eseguito ogni tot tempo del timer
        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int nBytes = 0;
            //Verifica se ci sono byte disponibili per la lettura
            if ((nBytes = socket.Available) > 0)
            {
                //Ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];

                //Crea un EndPoint per contenere il mittente del messaggio
                EndPoint remoreEndPoint = new IPEndPoint(IPAddress.Any, 0);

                //Riceve il messaggio dal socket e lo memorizza in un buffer
                nBytes = socket.ReceiveFrom(buffer, ref remoreEndPoint);

                string from = ((IPEndPoint)remoreEndPoint).Address.ToString();

                //Converte il buffer in stringa
                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);


                lstMessaggi.Items.Add(from + ": " + messaggio);

            }
        }
        //Metodo avviato al premere del pulsante invia nel WPF
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //Richiede l'IP all'utente dalla WPF
            IPAddress remote_address = IPAddress.Parse(txtTo.Text);

            //Crea un oggetto IPEndPoint che rappresenta il destinatario del messaggio
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 10000); //Porta Destinatario

            //Converte il messaggio in un array di byte e lo invia al destinatario
            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

            //Invio del messaggio al remote_endpoint del destinatario
            socket.SendTo(messaggio, remote_endpoint);
        }
    }
}

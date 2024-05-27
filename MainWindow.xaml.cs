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
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Xml;

namespace Socket4F
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int sourcePort = 50000; //dichiarazione del sourceport per facile modifica del port sorgente

        Socket s = null;
        DispatcherTimer dTimer = null;

        public MainWindow()
        {
            InitializeComponent();

            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //creazione del socket

            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address, sourcePort);
            
            s.Bind(local_endpoint); //si aggiunge un endpoint al socket: è pronto all'uso

            dTimer = new DispatcherTimer();
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); 
            dTimer.Tick += new EventHandler(ricezioneSocket); //ogni 250 secondi, controlla ricezione socket
        }

        private void ricezioneSocket(object sender, EventArgs e)
        {
            int nBytes = 0;
            if ((nBytes = s.Available) > 0)
            {
                byte[] buffer = new byte[nBytes];
                EndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0); //creazione dell'endpoint remoto per la ricezione
                s.ReceiveFrom(buffer, ref remote_endpoint); //si riceve il messaggio

                string from = ((IPEndPoint)remote_endpoint).Address.ToString(); //creazione stringa del indirizzo mittente

                string msg = Encoding.UTF8.GetString(buffer, 0, nBytes); //

                lbxMsg.Items.Add(from + ": " + msg); //aggiunta del messaggio alla lista
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] data = txtDst.Text.Split(':'); //si separa txtdst in due
                IPAddress remote_address = IPAddress.Parse(data[0]); //la prima parte è l'indirizzo
                int remote_port = int.Parse(data[1]); //la seconda parte è il port

                IPEndPoint remote_endpoint = new IPEndPoint(remote_address, remote_port); //con essi si crea un endpoint da usare come destinatario

                byte[] messaggio = Encoding.UTF8.GetBytes(txtMsg.Text); //si trasforma il messaggio in byte

                s.SendTo(messaggio, remote_endpoint); //si manda
            }
            catch (FormatException) //gestione input sbagliato
            {
                MessageBox.Show("Il socket inserito non è valido.", "Errore", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtDst.Focus();
            }
            catch (Exception) //la migliore misura di backup è quella che implementi ma non usi mai. -qualcuno
            {
                MessageBox.Show("Errore sconosciuto. Cos'hai fatto???", "Questa è una misura di fallback", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            
        }
    }
}
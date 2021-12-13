using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace ConsumirAPISteam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Variáveis globais
        int posicaoGlobal = 0;
        int totalRegistros = 0;
        string xmlStr = "";

        //Chaves de acesso do Steam
        string steamAPIKey = "02FF648481E5AFD95EE9BCAC2A995A18";
        string steamID = "76561198014127469";

        public MainWindow()
        {
            InitializeComponent();

            //Consome a API
            ConsumirAPI(steamAPIKey, steamID, posicaoGlobal);
        }

        private void ConsumirAPI(string steamAPIKey, string steamID, int posicao)
        {
            //Endereço de consulta da API, necessário fornecer os dados do perfil a ser consultado, assim como o perfil deverá ser público.
            var steamAPIRequest = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={steamAPIKey}&steamid={steamID}&format=xml&include_appinfo=true";

            //Caso o XML não tenha sido baixado da API do Steam, faz o download.
            if (string.IsNullOrEmpty(xmlStr)) using (var wc = new WebClient()) xmlStr = wc.DownloadString(steamAPIRequest);

            //Leitura do XML
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            XmlNodeList games = xmlDoc.SelectNodes("/response/games/message");
            //Seta a posição no XML para consulta.
            XmlNode index = games[posicao];
            totalRegistros = games.Count;

            //Seta os labels
            TextTotalEntradas.Text = $"Total de Registros: {totalRegistros}";
            labelRegistro.Content = $"Registro: {posicaoGlobal + 1}";

            #region Preenchimento dos campos
            //
            if (index != null && posicao != games.Count)
            {
                //Dados do XML
                var appID = index.SelectSingleNode("appid").InnerText;
                var appName = index.SelectSingleNode("name").InnerText;
                var playTime = index.SelectSingleNode("playtime_forever").InnerText;
                var icon = $"http://media.steampowered.com/steamcommunity/public/images/apps/{appID}/{index.SelectSingleNode("img_icon_url").InnerText}.jpg";
                var logo = $"http://media.steampowered.com/steamcommunity/public/images/apps/{appID}/{index.SelectSingleNode("img_logo_url").InnerText}.jpg";

                //Seta variáveis
                labelAppID.Content = $"AppID: {appID}";
                textJogo.Text = $"Jogo: {appName}";
                LogoImage.Source = new BitmapImage(new Uri(logo));
                IconImage.Source = new BitmapImage(new Uri(icon));

                #region Validações de Tempo Jogado
                //Caso o tempo jogado passe de 1 horas, muda a descrição do campo.
                if (Convert.ToInt32(playTime) > 60)
                {
                    int playTimeConvertido = Convert.ToInt32(playTime) / 60;
                    labelTempoJogado.Content = playTimeConvertido.Equals(1) ? labelTempoJogado.Content = $"Tempo Jogado: {playTimeConvertido} Hora" : labelTempoJogado.Content = $"Tempo Jogado: {playTimeConvertido} Horas";
                }
                else
                {
                    // Caso a não tenha jogado, não inclui o valor
                    if (Convert.ToInt32(playTime).Equals(0))
                    {
                        labelTempoJogado.Content = $"Tempo Jogado: Não jogado";
                    }
                    else if (Convert.ToInt32(playTime).Equals(1))
                    {
                        labelTempoJogado.Content = $"Tempo Jogado: {playTime} minuto";
                    }
                    else
                    {
                        labelTempoJogado.Content = $"Tempo Jogado: {playTime} minutos";
                    }
                }
                #endregion
            }
            #endregion
        }

        private void ExecutaMudanca(string tecla)
        {
            /*Trecho que trata o primeiro e último valor da lista, visto que 
             * não existem valores negativos na lista e a seleção não pode ser 
             * maior que o total de itens na lista.
             */
            if (tecla == "Left")
            {
                if (posicaoGlobal == 0) posicaoGlobal = totalRegistros - 1; else posicaoGlobal--;
                ConsumirAPI(steamAPIKey, steamID, posicaoGlobal);
            }
            else if (tecla == "Right")
            {
                if (posicaoGlobal == totalRegistros - 1) posicaoGlobal = 0; else posicaoGlobal++;
                ConsumirAPI(steamAPIKey, steamID, posicaoGlobal);
            }
        }

        private void btn_left_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExecutaMudanca("Left");
        }

        private void btn_right_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExecutaMudanca("Right");
        }
                
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Tratamento das teclas pressionadas no teclado.
            switch (e.Key)
            {
                case Key.Left:
                    ExecutaMudanca(e.Key.ToString());
                    break;
                case Key.Right:
                    ExecutaMudanca(e.Key.ToString());
                    break;
                case Key.Escape:
                    //Fecha a aplicação em caso do ESC
                    Application.Current.Shutdown();
                    break;
            }
        }        

        private void btn_right_MouseEnter(object sender, MouseEventArgs e)
        {            
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btn_right_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btn_left_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btn_left_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}

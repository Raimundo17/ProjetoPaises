namespace ProjetoPaises
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Net;
    using Microsoft.Maps.MapControl.WPF;
    using Modelos;
    using Servicos;
    using Svg;

    public partial class MainWindow : Window
    {
        #region Atributes

        private List<Country> Countries; // Referência para a lista de Países

        private List<Weather> Weathers; // Referência para a lista de Weathers

        private NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        private DataService dataService;

        private DataServiceWeather dataServiceWeather;

        public object GetWeather { get; private set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService(); // verifica a pasta "Data" atraves do construtor da classe
            dataServiceWeather = new DataServiceWeather();
            LoadCountriesandWeather();
        }

        private async void LoadCountriesandWeather()
        {
            bool load; // para saber se o load ja foi feito ou não

            Status.Text = "Updating countries...";

            var connection = networkService.CheckConnection(); //vou testar a conexão á internet

            if (!connection.IsSuccess) // se nao existe ligação á internet
            {
                LoadLocalCountries(); // Carrega os países atraves da base dados
                LoadLocalWeathers(); // Carrega a meteorologia atraves da base dados
                load = false;

            }
            else
            {
                // se tiver conexao chamo a api
                await LoadApiCountries();
                await LoadApiWeathers();
                load = true;
            }

            if (Countries.Count == 0 && Weathers.Count == 0) // As listas de dados não foram carregadas nem localmente nem pela API
            {
                Result.Visibility = Visibility.Visible;
                Result.Text = "There is no internet connection" + Environment.NewLine + "The list of countries and weather were not loaded previously.";
                return;
            }

            // Carrega a combox com o nome dos países
            cb_ListaPaises.ItemsSource = Countries;
            cb_ListaPaises.DisplayMemberPath = "DisplayName";
            
            // Carrega a combox com a meteorologia vinda da minha API
            cb_weather.ItemsSource = Weathers;
            cb_weather.DisplayMemberPath = "DisplayName";

            Status.Text = "Countries and Weather Info loaded successfully!";

            if (load) // Se o load for true a Api carregou os dados correctamente
            {
                Status.Text = string.Format("Countries and Weather Information loaded from the Internet at {0:F}", DateTime.Now); // data e hora do carregamento da API
            }
            else
            {
                Status.Text = string.Format("Countries and Weather Info loaded from DataBase.");
            }

            ProgressBar.Value = 100; // Carrega o progress bar
        }

        private void LoadLocalCountries() // carrega a informacao através da base de dados local
        {
            Countries = dataService.GetData();
        }

        private void LoadLocalWeathers() // carrega a informacao através da base de dados local
        {
            Weathers = dataServiceWeather.GetData();
        }

        private async Task LoadApiCountries()
        {
            ProgressBar.Value = 0;

            var response = await apiService.GetCountries("http://restcountries.eu", "/rest/v2/all"); // endereço base da API + vai buscar o controlador de forma assíncrona e guarda na variável response

            Countries = (List<Country>)response.Result; //convert do objeto para a lista

            dataService.DeleteData();

            dataService.SaveData(Countries); // guardo na base de dados
        }

        private async Task LoadApiWeathers()
        {
          var response = await apiService.GetWeather("http://weatherapi.somee.com", "/api/Countries"); // MINHA API WEATHER

            Weathers = (List<Weather>)response.Result; //convert do objeto para a lista

            dataServiceWeather.DeleteData();

            dataServiceWeather.SaveData(Weathers); // guardo na base de dados
        }

        private void cb_ListaPaises_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Country countryselected = (Country)cb_ListaPaises.SelectedItem;

            //Mostra a informação nos Textblocks - Capital
            if (countryselected.Capital is "")
            {
                tb_capital.Text = "N/A"; // Caso a informação não esteja disponível
                label_capital.Visibility = Visibility.Visible;
            }
            else
            {
                tb_capital.Text = countryselected.Capital.ToString();
                label_capital.Visibility = Visibility.Visible;
            }

            //Mostra a informação nos Textblocks - Region
            if (countryselected.Region is "")
            {
                tb_region.Text = "N/A";// Caso a informação não esteja disponível
                label_region.Visibility = Visibility.Visible;
            }
            else
            {
                tb_region.Text = countryselected.Region.ToString();
                label_region.Visibility = Visibility.Visible;
            }

            //Mostra a informação nos Textblocks - SubRegion
            if (countryselected.Subregion is "")
            {
                tb_subregion.Text = "N/A";// Caso a informação não esteja disponível
                label_subregion.Visibility = Visibility.Visible;
            }
            else
            {
                tb_subregion.Text = countryselected.Subregion.ToString();
                label_subregion.Visibility = Visibility.Visible;
            }

            //Mostra a informação nos Textblocks - Population
            tb_population.Text = countryselected.Population.ToString();
            label_population.Visibility = Visibility.Visible;

            //Mostra a informação nos Textblocks - Gini
            if (countryselected.Gini is null)
            {
                tb_gini.Text = "N/A";
                label_gini.Visibility = Visibility.Visible;
            }
            else
            {
                tb_gini.Text = countryselected.Gini.ToString();
                label_gini.Visibility = Visibility.Visible;
            }

            //Mostra a informação nos Textblocks - Demonym
            if (countryselected.Demonym is "")
            {
                tb_demonym.Text = "N/A";// Caso a informação não esteja disponível
                label_demonym.Visibility = Visibility.Visible;
            }
            else
            {
                tb_demonym.Text = countryselected.Demonym.ToString();
                label_demonym.Visibility = Visibility.Visible;
            }

            //Mostra a informação nos Textblocks - Native
            tb_native.Text = countryselected.NativeName.ToString();
            label_native.Visibility = Visibility.Visible;

            //Mostra a informação nos Textblocks - Native
            if (countryselected.Area is null)
            {
                tb_area.Text = "N/A";// Caso a informação não esteja disponível
                label_area.Visibility = Visibility.Visible;
            }
            else
            {
                tb_area.Text = countryselected.Area.ToString();
                label_area.Visibility = Visibility.Visible;
            }

            GetFlag(); // Método para mostrar a bandeira
            GetMap(); // Método para mostrar o mapa
        }

        private void GetFlag()
        {
            Country countryselected = (Country)cb_ListaPaises.SelectedItem;

            if (!Directory.Exists("Flags"))
            {
                Directory.CreateDirectory("Flags");
            }

            var connection = networkService.CheckConnection(); //vou testar a conexão á internet

            if (connection.IsSuccess) // se nao existe ligação á internet
            {

                foreach (var country in Countries)
                {
                    var fileNameSVG = Environment.CurrentDirectory + "/Flags" + $"/{countryselected.Alpha3Code.ToLower()}.svg";
                    var fileNameJPG = Environment.CurrentDirectory + "/Flags" + $"/{countryselected.Alpha3Code.ToLower()}.jpg";
                    var pathBackup = Environment.CurrentDirectory + "/FlagsBackup" + $"/{countryselected.Alpha3Code.ToLower()}.jpg";
                    FileInfo imageFile = new FileInfo(fileNameSVG);

                    if (!File.Exists(fileNameJPG))
                    {
                        try
                        {
                            // Guarda a imagem do URL
                            string svgFileName = "https://restcountries.eu" + $"/data/{countryselected.Alpha3Code.ToLower()}.svg";

                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFileTaskAsync(svgFileName, fileNameSVG);
                            }

                            // Le o svg document do file system
                            var svgDocument = SvgDocument.Open(fileNameSVG);

                            try
                            {
                                var bitmap = svgDocument.Draw();

                                bitmap.Save(fileNameJPG, ImageFormat.Jpeg);

                                File.Delete(fileNameSVG);
                            }
                            catch
                            {
                                if (File.Exists(pathBackup))
                                {
                                    imageFile = new FileInfo(pathBackup);
                                    File.Delete(fileNameSVG);
                                    imageFile.CopyTo(fileNameJPG);
                                }
                            }
                        }
                        catch
                        {
                            if (File.Exists(pathBackup))
                            {
                                imageFile = new FileInfo(pathBackup);
                                File.Delete(fileNameSVG);
                                imageFile.CopyTo(fileNameJPG);
                            }
                            continue;
                        }
                    }
                }
                // vai buscar a imagem guardada na pasta "Flags"
                string fileNameFlags = Environment.CurrentDirectory + "/Flags" + $"/{countryselected.Alpha3Code.ToLower()}.jpg";

                //Converte a imagem
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                if (File.Exists(fileNameFlags))
                {
                    img.UriSource = new Uri(fileNameFlags);
                }
                else
                {
                    img.UriSource = new Uri(Environment.CurrentDirectory + "/ImageUnavailable.jpg");
                }
                img.EndInit();
                imageflag.Source = img;
            }
        }
        private void GetMap()
        {
            Country countryselected = (Country)cb_ListaPaises.SelectedItem;

            // com a latitude e longitude vinda da API, uso como coordenadas para mostrar o país no mapa
            myMap.Mode = new AerialMode(true);
            if (countryselected.Latlng.Count == 0) 
            {
                dialogService.ShowMessage("Error", "LOCATION NOT FOUND");
                return;
            }
            else
            {
                myMap.ZoomLevel = 4;
                myMap.Center = new Microsoft.Maps.MapControl.WPF.Location(countryselected.Latlng[0], countryselected.Latlng[1]);
                Point.Location = new Microsoft.Maps.MapControl.WPF.Location(countryselected.Latlng[0], countryselected.Latlng[1]);
            }
        }

        private void cb_weather_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Weather weatherselected = (Weather)cb_weather.SelectedItem;

            //Mostra a informação nos Textblocks - Temperature
            tb_temperature.Text = weatherselected.Temperature.ToString() + " ºC";
            label_temperature.Visibility = Visibility.Visible;

            //Mostra a informação nos Textblocks - Wind Speed
            tb_wind.Text = weatherselected.Wind.ToString() + " Km/h";
            label_wind.Visibility = Visibility.Visible;

            //Mostra a informação nos Textblocks - Precipitation
            tb_precipitation.Text = weatherselected.Precipitation.ToString() + " %";
            label_precipitation.Visibility = Visibility.Visible;

            //Mostra a informação nos Textblocks - Humidity
            tb_humidity.Text = weatherselected.Humidity.ToString() + " %";
            label_humidity.Visibility = Visibility.Visible;
        }
    }
}





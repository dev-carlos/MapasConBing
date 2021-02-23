using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

namespace MapasConBing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static HttpClient ComunidadHttp = new HttpClient();
        static HttpClient ProvinciaHttp = new HttpClient();
        static HttpClient MunicipioHttp = new HttpClient();
        static List<Comunidad> lsComunidades;
        static List<Provincia> lsProvincia;
        static List<Municipio> lsMunicipios;
        List<Gasolinera> lsGasolineras = new List<Gasolinera>();
        static Boolean verTodas = true;
        static Boolean verUna = false;

        public MainWindow()
        {
            InitializeComponent();
            cargarComunidades();
            cargarProvincias();
            cargarMunicipios();
        }

        public async void cargarComunidades()
        {
            try
            {
                lsComunidades = await GetComunidades("https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/Listados/ComunidadesAutonomas/");
                
                foreach(Comunidad com in lsComunidades)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = com.CCAA;
                    item.Tag = com.IDCCAA;
                    cbComunidaddes.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        static async Task<List<Comunidad>> GetComunidades(String path)
        {
            List<Comunidad> comunidades = null;
            
            HttpResponseMessage msg = await ComunidadHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                
                comunidades = System.Text.Json.JsonSerializer.Deserialize<List<Comunidad>>(salida);
                
            }
            return comunidades;
        }

        public async void cargarProvincias(string idComunidad = null)
        {
            try
            {
                string url = "https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/Listados/Provincias/";
                if(idComunidad != null)
                {
                    url = "https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/Listados/ProvinciasPorComunidad/"+ idComunidad;
                }
               
                lsProvincia = await GetProvincias(url);
                cbProvincias.Items.Clear();

                foreach (Provincia pro in lsProvincia)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = pro.provincia;
                    item.Tag = pro.IDPovincia;
                    cbProvincias.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        static async Task<List<Provincia>> GetProvincias(String path)
        {
            List<Provincia> provincias = null;

            HttpResponseMessage msg = await ProvinciaHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                provincias = System.Text.Json.JsonSerializer.Deserialize<List<Provincia>>(salida, options);

            }
            return provincias;
        }

        public async void cargarMunicipios(string idPro = null)
        {
            try
            {
                string url = "https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/Listados/Municipios/";
                if(idPro != null)
                {
                    url = "https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/Listados/MunicipiosPorProvincia/"+ idPro;
                }
                lsMunicipios = await GetMunicipios(url);
                cbMunicipios.Items.Clear();

                foreach (Municipio mu in lsMunicipios)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = mu.municipio;
                    item.Tag = mu.IDMunicipio;
                    cbMunicipios.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        static async Task<List<Municipio>> GetMunicipios(String path)
        {
            List<Municipio> municipios = null;

            HttpResponseMessage msg = await MunicipioHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                municipios = System.Text.Json.JsonSerializer.Deserialize<List<Municipio>>(salida, options);

            }
            return municipios;
        }

        private void cbComunidaddes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            ComboBoxItem itemSelect = cb.SelectedItem as ComboBoxItem;
            string idCom = itemSelect.Tag as string;

            cargarProvincias(idCom);
        }

        private void cbProvincias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string idPro = null;
            ComboBox cb = sender as ComboBox;
            ComboBoxItem itemSelect = cb.SelectedItem as ComboBoxItem;
            if(itemSelect != null)
            {
                idPro = itemSelect.Tag as string;
            }
            

            cargarMunicipios(idPro);
        }

        private void cbMunicipios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string idMu = null;
            ComboBox cb = sender as ComboBox;
            ComboBoxItem itemSelect = cb.SelectedItem as ComboBoxItem;
            if(itemSelect != null)
            {
                idMu = itemSelect.Tag as string;
            }
            

            cargarGasolineras(idMu);
        }

        public async void cargarGasolineras(string idMu = null)
        {
            try
            {
                string url = "https://sedeaplicaciones.minetur.gob.es/ServiciosRESTCarburantes/PreciosCarburantes/EstacionesTerrestres/FiltroMunicipio/" + idMu;                
                lsGasolineras = await GetGasolinera(url);
                crearPuspin(lsGasolineras);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        

        static async Task<List<Gasolinera>> GetGasolinera(String path)
        {
            List<Gasolinera> gasolineras = new List<Gasolinera>();
            HttpResponseMessage msg = await MunicipioHttp.GetAsync(path);

            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(salida);

                foreach(dynamic item in data["ListaEESSPrecio"])
                {
                    Gasolinera g = new Gasolinera();
                    g.CP = item["C.P."];
                    g.direccion = item["Dirección"];
                    g.horario = item["Horario"];
                    g.latitud = item["Latitud"];
                    g.localidad = item["Localidad"];
                    g.provincia = item["Provincia"];
                    g.longitud = item["Longitud (WGS84)"];
                    g.municipio = item["Municipio"];

                    try
                    {
                        string p = item["Precio Gasoleo A"];
                        p.Replace(",", ".");
                        g.precioGasoleo = double.Parse(p);

                        p = item["Precio Gasolina 95 E5"];
                        p.Replace(",", ".");
                        g.precioGasolina = double.Parse(p);
                    }catch(Exception ex)
                    {

                    }
                   
                        
                    

                    gasolineras.Add(g);
                }

            }
                
            return gasolineras;
        }

        private void crearPuspin(List<Gasolinera> lsgasolineras)
        {
            MapLayer miPushpinLayer = new MapLayer();
            if (gasMap != null)
            {
                gasMap.Children.Clear();
                gasMap.Children.Add(miPushpinLayer);
            }
            
           

            foreach (Gasolinera gas in lsgasolineras)
            {

                var pin = new Pushpin();
                pin.Location = new Location(Convert.ToDouble(gas.latitud), Convert.ToDouble(gas.longitud));

                ToolTipService.SetToolTip(pin, new ToolTip()
                {
                    Content = gas.obtenerDatos()

                });
               
                gasMap.Children.Add(pin);
            }
        }

       

        private void cbx_verTodo_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            if (ch.IsChecked == true)
            {
                if(rb_verBarato != null && rb_verCaro != null)
                {
                    rb_verBarato.IsEnabled = false;
                    rb_verCaro.IsEnabled = false;
                }
                crearPuspin(lsGasolineras);

            }
            else
            {
                if (rb_verBarato != null && rb_verCaro != null)
                {
                    rb_verBarato.IsEnabled = true;
                    rb_verCaro.IsEnabled = true;
                    rb_verBarato.IsChecked = true;
                }
               
                verTodas = false;
                verUna = true;
                masBarata();
            }
        }

        private void masBarata()
        {
            Gasolinera barataGasoil = new Gasolinera();
            Gasolinera barataGasolina = new Gasolinera();
            List<Gasolinera> lsNueva = new List<Gasolinera>();
            if(lsGasolineras.Count > 0)
            {
                barataGasoil = lsGasolineras.Where(p => p.precioGasoleo > 0).OrderBy(p => p.precioGasoleo).First();
                barataGasolina = lsGasolineras.Where(p => p.precioGasolina > 0).OrderBy(p => p.precioGasolina).First();

                if (barataGasoil.Equals(barataGasolina))
                {
                    lsNueva.Add(barataGasoil);
                }
                else
                {
                    lsNueva.Add(barataGasoil);
                    lsNueva.Add(barataGasolina);
                }
                crearPuspin(lsNueva);
            }

            
        }

        private void masCara()
        {
            Gasolinera caraGasoil = new Gasolinera();
            Gasolinera caraGasolina = new Gasolinera();
            List<Gasolinera> lsNueva = new List<Gasolinera>();
            if (lsGasolineras.Count > 0)
            {
                caraGasoil = lsGasolineras.Where(p => p.precioGasoleo > 0).OrderByDescending(p => p.precioGasoleo).First();
                caraGasolina = lsGasolineras.Where(p => p.precioGasolina > 0).OrderByDescending(p => p.precioGasolina).First();

                if (caraGasoil.Equals(caraGasolina))
                {
                    lsNueva.Add(caraGasoil);
                }
                else
                {
                    lsNueva.Add(caraGasoil);
                    lsNueva.Add(caraGasolina);
                }
                crearPuspin(lsNueva);
            }


        }

        private void rb_verBarato_Checked(object sender, RoutedEventArgs e)
        {
            masBarata();
        }

        private void rb_verCaro_Checked(object sender, RoutedEventArgs e)
        {
            masCara();
        }
    }
}

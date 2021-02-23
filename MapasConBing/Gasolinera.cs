using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MapasConBing
{
    public class Gasolinera
    {
        
        public string CP { get; set; }
       
        public string direccion { get; set; }
        
        public string horario { get; set; }
       
        public string latitud { get; set; }
       
        public string longitud { get; set; }
       
        public string localidad { get; set; }
       
        public string municipio { get; set; }
       
        public string provincia { get; set; }
       
        public double precioGasoleo { get; set; } = -1;
      
        public double precioGasolina { get; set; } = -1; 

        public String obtenerDatos()
        {
            String datos = "";
            datos = this.direccion + "\n " + this.CP + " Municipio: " + this.municipio + " Provincia: " + this.provincia + "\n Horario: " + this.horario + "\n Precio Gasoil: " + precioGasoleo + " Precio Gasolina: " + precioGasolina;

            return datos;
        }

        

    }
}

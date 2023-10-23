using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaDNI_Premios_CargaDeTabla.Model
{
    public class DatosCliente
    {
        public string CUIT { get; set; }
        public string CBU { get; set; }

        public string fechaUltAcreditacionBIP { get; set; }

        public string EjecutivoAsociado { get; set; }

        public string BranchEjecutivo { get; set; }
        public DatosCliente() { }
    }
}

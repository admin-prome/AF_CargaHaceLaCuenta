using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaDNI_Premios_CargaDeTabla.Model
{
    public class GFFormModel
    {
            public int total_count { get; set; }
            public List<Entry> entries { get; set; }

        public GFFormModel() 
        {
            //entries= new Entry[totalRecord];
            entries= new List<Entry>();
        }
        
        

    }
}

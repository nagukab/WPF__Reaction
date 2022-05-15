using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Reaction
{
    internal class вДанные
    {
        public вДанные()
        {
            ЛистДаблов = new List<double>();
        }

        public List<double> ЛистДаблов;//= new List<double>();

        public bool новыйЦикл { get; set; }=true;
        public bool наБазе { get; set; }
        public int фалшстарт { get; set; }  
        public int вовремя { get; set; }  
        public int опаздание { get; set; }  
        public int промах { get; set; }  
        public int вНе { get; set; }  
        public int попаданий { get; set; }

        public double среднее { get; set; }
        public double минимальное { get; set; }
        public double максимальное { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Reaction
{
    internal class Данные
    {
        public List<double> листРеакций;//= new List<double>();
        public Данные()
        {
            листРеакций = new List<double>();            
        }      
          

        /// <summary>
        /// попадания вовремя
        /// </summary>
        public int вовремя { get; set; }  

        /// <summary>
        /// попаданий с опозданием
        /// </summary>
        public int опаздание { get; set; }  

        /// <summary>
        /// попаданий  вовремя + с опазданями
        /// </summary>
        public int попаданийВсего { get; set; }        

        /// <summary>
        /// клик в поле рядом
        /// </summary>
        public int промах { get; set; }

        /// <summary>
        /// клик на не ту кнопку
        /// </summary>
        public int ошибок { get; set; }       

        /// <summary>
        /// среднее время реакции
        /// </summary>
        public double среднее { get; set; }

        /// <summary>
        /// минимальное время реакции
        /// </summary>
        public double минимальное { get; set; }

        /// <summary>
        /// максимальное время реакции
        /// </summary>
        public double максимальное { get; set; }
    }
}

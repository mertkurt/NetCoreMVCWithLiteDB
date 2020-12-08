using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCWithLiteDB.Models
{
    public class Makale
    {
        public int ID { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public DateTime? YazilmaTarihi { get; set; }
    }
}

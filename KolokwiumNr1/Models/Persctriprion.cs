using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumNr1.Models
{
    public class Persctriprion
    {
        public int id { get; set; }
        public string date { get; set; }
        public string dueDtate { get; set; }
        public int idPat { get; set; }
        public int idDoc { get; set; }
        public int dose { get; set; }
        public string details { get; set; }
    }
}

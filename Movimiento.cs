using System;

namespace CajeroAutomatico
{
    internal class Movimiento
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public decimal SaldoResultante { get; set; }
    }
} 
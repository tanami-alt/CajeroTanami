using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CajeroAutomatico
{
    internal class ArchivoHelper
    {
        private const string UsuariosCsv = "usuarios.csv";
        private const string MovimientosCsv = "movimientos.csv";

        //Método para cargar los usuarios desde un archivo CSV
        public static List<Usuario> CargarUsuarios()
        {
            var lista = new List<Usuario>();
            if (!File.Exists(UsuariosCsv)) return lista;

            using var reader = new StreamReader(UsuariosCsv);
            string? header = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = ParseCsvLine(line);
                if (cols.Count < 4) continue; // Asegura que haya suficientes columnas

                lista.Add(new Usuario
                {
                    Id = cols[0],
                    Nombre = cols[1],
                    Pin = cols[2],
                    Saldo = decimal.TryParse(cols[3], out var saldo) ? saldo : 0m;

                });
            }
            return lista;
        }

        //Método para guardar los usuarios en un archivo CSV
        public static void GuardarUsuarios(IEnumerable<Usuario> usuarios)
        {
            using var writer = new StreamWriter(UsuariosCsv, false);
            writer.WriteLine("Id,Nombre,Pin,Saldo");

            foreach (var u in usuarios)
            {
                writer.WriteLine($"{u.Id},{u.Nombre},{u.Pin},{u.Saldo.ToString(CultureInfo.InvariantCulture)}")
            }
        }

        //Método para agregar un movimiento al archivo CSV
        public static void AgregarMovimiento(string usuarioId, Movimiento mov)
        {
            bool existe = File.Exists(MovimientosCsv);
            using var writer = new StreamWriter(MovimientosCsv, true);
            if (!existe)
            {
                writer.WriteLine("UsuarioId,Fecha,Tipo,Monto,SaldoResultante");
            }
            writer.WriteLine($"{usuarioId},{mov.Fecha:o},{mov.Tipo},{mov.Monto.ToString(CultureInfo.InvariantCulture)}," +
                $"{mov.SaldoResultante.ToString(CultureInfo.InvariantCulture)}");

        }

        //Método para obtener los últimos movimientos de un usuario
        public static List<Movimiento> ObtenerUltimosMovimientos(string usuarioId, int cantidad)
        {
            var lista = new List<Movimiento>();
            if (!File.Exists(MovimientosCsv)) return lista;

            var temp = new List<Movimiento>(); // Lista temporal para almacenar todos los movimientos del usuario
            using var reader = new StreamReader(MovimientosCsv);
            string? header = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = ParseCsvLine(line);
                if (cols.Count < 5) continue; // Asegura que haya suficientes columnas

                //Filtra solo los movimientos del usuario especificado
                if (cols[0] != usuarioId) continue;

                //Crea el objeto Movimiento y lo agrega a la lista temporal
                temp.Add(new Movimiento
                {
                    Fecha = DateTime.TryParse(cols[1], out var f) ? f : DateTime.MinValue,
                    Tipo = cols[2],
                    Monto = decimal.TryParse(cols[3], out var m) ? m : 0m,
                    SaldoResultante = decimal.TryParse(cols[4], out var s) ? s : 0m
                });

            }
            //Ordena la lista temporal por fecha descendente y toma los últimos 'cantidad' movimientos
            return temp.OrderByDescending(t => t.Fecha).Take(cantidad).ToList();
        }

        // Recorre cada carácter de la línea
            foreach (var c in line)
            {
                if (c == '"')  // Alterna estado quoted al encontrar comilla
                {
                    quoted = !quoted;
                    continue;  // No agrega la comilla al campo
                }
                if (c == ',' && !quoted)  // Fin de campo si coma fuera de comillas
                {
                    res.Add(cur.ToString());  // Agrega campo actual
                    cur.Clear();  // Reinicia builder
                }
                else
{
    cur.Append(c);  // Agrega carácter al campo actual
}
            }
            res.Add(cur.ToString());  // Agrega el último campo
         return res;  // Retorna lista de columnas
        }
    }
} 
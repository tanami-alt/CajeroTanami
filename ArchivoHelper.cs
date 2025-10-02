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
                writer.WriteLine(string.Join(',',
                    Escape(u.Id),
                    Escape(u.Nombre),
                    Escape(u.Pin),
                    u.Saldo.ToString()));
            }
        }

        public static void AgregarMovimiento(string usuarioId, Movimiento mov)
        {
            bool existe = File.Exists(MovimientosCsv);
            using var writer = new StreamWriter(MovimientosCsv, true);
            if (!existe)
            {
                writer.WriteLine("UsuarioId,Fecha,Tipo,Monto,SaldoResultante");
            }
            writer.WriteLine(string.Join(',',
                Escape(usuarioId),
                mov.Fecha.ToString("o", CultureInfo.InvariantCulture),
                Escape(mov.Tipo),
                mov.Monto.ToString(CultureInfo.InvariantCulture),
                mov.SaldoResultante.ToString(CultureInfo.InvariantCulture)));
        }

        public static List<Movimiento> ObtenerUltimosMovimientos(string usuarioId, int cantidad)
        {
            var lista = new List<Movimiento>();
            if (!File.Exists(MovimientosCsv)) return lista;

            var temp = new List<Movimiento>();
            using var reader = new StreamReader(MovimientosCsv);
            string? header = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var cols = ParseCsvLine(line);
                if (cols.Count < 5) continue;
                if (cols[0] != usuarioId) continue;
                temp.Add(new Movimiento
                {
                    Fecha = DateTime.TryParse(cols[1], null, DateTimeStyles.RoundtripKind, out var fecha) ? fecha : DateTime.MinValue,
                    Tipo = cols[2],
                    Monto = decimal.TryParse(cols[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var monto) ? monto : 0m,
                    SaldoResultante = decimal.TryParse(cols[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var saldo) ? saldo : 0m
                });
            }
            return temp.OrderByDescending(t => t.Fecha).Take(cantidad).ToList();
        }

        private static string Escape(string? valor)
        {
            valor ??= string.Empty;
            if (valor.Contains(',') || valor.Contains('"'))
            {
                return "\"" + valor.Replace("\"", "\"\"") + "\"";
            }
            return valor;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var res = new List<string>();
            bool quoted = false;
            var cur = new System.Text.StringBuilder();
            foreach (var c in line)
            {
                if (c == '"') { quoted = !quoted; continue; }
                if (c == ',' && !quoted)
                {
                    res.Add(cur.ToString());
                    cur.Clear();
                }
                else
                {
                    cur.Append(c);
                }
            }
            res.Add(cur.ToString());
            return res;
        }
    }
} 
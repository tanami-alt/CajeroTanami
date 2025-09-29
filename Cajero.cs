using System;
using System.Collections.Generic;
using System.Linq;

namespace CajeroAutomatico
{
    internal class Cajero
    {
        private List<Usuario> _usuarios;
        private Usuario? _usuarioActual;

        public Cajero()
        {
            _usuarios = ArchivoHelper.CargarUsuarios();
        }

        public bool IniciarSesion(string identificador, string pin)
        {
            var usuario = _usuarios.FirstOrDefault(u =>
                (string.Equals(u.Id, identificador, StringComparison.OrdinalIgnoreCase)
                 || string.Equals(u.Nombre, identificador, StringComparison.OrdinalIgnoreCase))
                && u.Pin == pin);
            if (usuario == null) return false;
            _usuarioActual = usuario;
            return true;
        }

        public Usuario? ObtenerUsuarioActual()
        {
            return _usuarioActual;
        }

        public void AsegurarUsuarioDemo()
        {
            if (_usuarios.Count == 0)
            {
                var demo = new Usuario { Id = "u1", Nombre = "Demo", Pin = "1234", Saldo = 0m };
                _usuarios.Add(demo);
                ArchivoHelper.GuardarUsuarios(_usuarios);
            }
        }

        // Realiza un depósito en la cuenta del usuario actual
        public bool Depositar(decimal monto)
        {
            if (_usuarioActual == null) return false;
            if (monto <= 0) return false;

            _usuarioActual.Saldo += monto;
            
            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                Tipo = "Deposito",
                Monto = monto,
                SaldoResultante = _usuarioActual.Saldo
            };
            ArchivoHelper.AgregarMovimiento(_usuarioActual.Id, movimiento);
            ArchivoHelper.GuardarUsuarios(_usuarios);
            return true;
        }

        // Realiza un retiro validando fondos suficientes
        public bool Retirar(decimal monto)
        {
            if (_usuarioActual == null) return false;
            if (monto <= 0) return false;
            if (_usuarioActual.Saldo < monto) return false;

            _usuarioActual.Saldo -= monto;

            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                Tipo = "Retiro",
                Monto = monto,
                SaldoResultante = _usuarioActual.Saldo
            };
            ArchivoHelper.AgregarMovimiento(_usuarioActual.Id, movimiento);
            ArchivoHelper.GuardarUsuarios(_usuarios);
            return true;
        }

        public decimal? ConsultarSaldo()
        {
            if (_usuarioActual == null) return null;
            return _usuarioActual.Saldo;
        }
    }
}

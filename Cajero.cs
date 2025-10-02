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


        //Inicio de sesión donde se valida que las credenciales sea correctas
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

        //Obtener el usuario que tiene la sesión activa actualmente
        public Usuario? ObtenerUsuarioActual()
        {
            return _usuarioActual;
        }

        //Asegura que exista un usuario demo para pruebas       
        public void AsegurarUsuarioDemo()
        {
            if (_usuarios.Count == 0)
            {
                var demo = new Usuario { Id = "u1", Nombre = "Demo", Pin = "1234", Saldo = 0m };
                _usuarios.Add(demo);
                ArchivoHelper.GuardarUsuarios(_usuarios);
            }
        }

        //Metodo para deporsitar dinero en la cuenta del usuario actual
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

        //Metodo para retirar dinero de la cuenta del usuario actual
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

        //Metodo para consultar el saldo de la cuenta del usuario actual
        public decimal? ConsultarSaldo()
        {
            if (_usuarioActual == null) return null;
            return _usuarioActual.Saldo;
        }

        //Metodo para obtener los ultimos movimientos del usuario actual
        //Por defecto obtiene los ultimos 5 movimientos, se hace la lista de 
        //movimientos en orden descendente (del mas reciente al mas antiguo) 
        //lista de objetos vacia si no hay movimientos o no hay usuario activo
        public List<Movimiento> ObtenerUltimosMovimientos(int cantidad = 5)
        {
            if (_usuarioActual == null) return new List<Movimiento>();
            return ArchivoHelper.ObtenerUltimosMovimientos(_usuarioActual.Id, cantidad);
        }

        //Metodo para cambiar el pin del usuario actual
        public bool CambiarPin(string pinActual, string pinNuevo)
        {
            if (_usuarioActual == null) return false;
            //verifica que el pin actual coincida y que el nuevo pin no sea nulo o vacio
            if (_usuarioActual.Pin != pinActual) return false;
            if (string.IsNullOrWhiteSpace(pinNuevo)) return false;

            _usuarioActual.Pin = pinNuevo;

            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                Tipo = "CambioClave",
                Monto = 0m,
                SaldoResultante = _usuarioActual.Saldo
            };
            ArchivoHelper.AgregarMovimiento(_usuarioActual.Id, movimiento);
            ArchivoHelper.GuardarUsuarios(_usuarios);
            return true;
        }
    }
}

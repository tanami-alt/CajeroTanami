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
    }
} 
using CajeroAutomatico;

class Program
{
    static void Main()
    {
        var cajero = new Cajero();
        cajero.AsegurarUsuarioDemo();

        // Inicio de sesión
        System.Console.WriteLine("=== Cajero Automático ===");
        System.Console.Write("Usuario (Id o Nombre): ");
        var user = System.Console.ReadLine() ?? string.Empty;
        System.Console.Write("PIN: ");
        var pin = LeerPin();

        if (!cajero.IniciarSesion(user, pin))
        {
            System.Console.WriteLine("\nCredenciales incorrectas.");
            return;
        }

        var actual = cajero.ObtenerUsuarioActual();
        System.Console.WriteLine($"\nBienvenido, {actual?.Nombre}.");

        // Menú principal
        while (true)
        {
            System.Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
            System.Console.WriteLine("1. Depositar");
            System.Console.WriteLine("2. Retirar");
            System.Console.WriteLine("3. Consultar saldo");
            System.Console.WriteLine("4. Últimos 5 movimientos");
            System.Console.WriteLine("5. Cambio de clave (PIN)");
            System.Console.WriteLine("6. Salir");
            System.Console.Write("Seleccione una opción: ");
            
            var opcion = System.Console.ReadLine();
            
            switch (opcion)
            {
                case "1":
                    RealizarDeposito(cajero);
                    break;
                case "2":
                    RealizarRetiro(cajero);
                    break;
                case "3":
                    RealizarConsultaSaldo(cajero);
                    break;
                case "4":
                    RealizarConsultaMovimientos(cajero);
                    break;
                case "5":
                    RealizarCambioClave(cajero);
                    break;
                case "6":
                    System.Console.WriteLine("¡Hasta luego!");
                    return;
                default:
                    System.Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    private static void RealizarDeposito(Cajero cajero)
    {
        System.Console.Write("Ingrese el monto a depositar: ");
        var input = System.Console.ReadLine();
        
        if (decimal.TryParse(input, out var monto))
        {
            if (cajero.Depositar(monto))
            {
                var usuario = cajero.ObtenerUsuarioActual();
                System.Console.WriteLine($"Depósito exitoso. Nuevo saldo: ${usuario?.Saldo:F2}");
            }
            else
            {
                System.Console.WriteLine("Error: Monto inválido o no hay sesión activa.");
            }
        }
        else
        {
            System.Console.WriteLine("Error: Ingrese un monto válido.");
        }
    }

    private static void RealizarRetiro(Cajero cajero)
    {
        System.Console.Write("Ingrese el monto a retirar: ");
        var input = System.Console.ReadLine();
        
        if (decimal.TryParse(input, out var monto))
        {
            if (cajero.Retirar(monto))
            {
                var usuario = cajero.ObtenerUsuarioActual();
                System.Console.WriteLine($"Retiro exitoso. Nuevo saldo: ${usuario?.Saldo:F2}");
            }
            else
            {
                System.Console.WriteLine("Error: Monto inválido, saldo insuficiente o no hay sesión activa.");
            }
        }
        else
        {
            System.Console.WriteLine("Error: Ingrese un monto válido.");
        }
    }

    private static void RealizarConsultaSaldo(Cajero cajero)
    {
        var usuario = cajero.ObtenerUsuarioActual();
        if (usuario == null)
        {
            System.Console.WriteLine("No hay sesión activa.");
            return;
        }
        System.Console.WriteLine($"Saldo actual: ${usuario.Saldo:F2}");
    }

    private static void RealizarConsultaMovimientos(Cajero cajero)
    {
        var movimientos = cajero.ObtenerUltimosMovimientos(5);
        if (movimientos.Count == 0)
        {
            System.Console.WriteLine("No hay movimientos registrados.");
            return;
        }

        System.Console.WriteLine("\n=== ÚLTIMOS 5 MOVIMIENTOS ===");
        foreach (var mov in movimientos)
        {
            System.Console.WriteLine($"{mov.Fecha:dd/MM/yyyy HH:mm} - {mov.Tipo} - ${mov.Monto:F2} - Saldo: ${mov.SaldoResultante:F2}");
        }
    }

    private static void RealizarCambioClave(Cajero cajero)
    {
        System.Console.Write("PIN actual: ");
        var pinActual = LeerPin();
        System.Console.Write("PIN nuevo: ");
        var pinNuevo = LeerPin();

        if (cajero.CambiarPin(pinActual, pinNuevo))
        {
            System.Console.WriteLine("PIN actualizado correctamente.");
        }
        else
        {
            System.Console.WriteLine("Error: PIN actual incorrecto o PIN nuevo inválido.");
        }
    }

    private static string LeerPin()
    {
        var pin = string.Empty;
        ConsoleKeyInfo key;
        do
        {
            key = System.Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Backspace && pin.Length > 0)
            {
                pin = pin.Substring(0, pin.Length - 1);
                System.Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                pin += key.KeyChar;
                System.Console.Write('*');
            }
        } while (key.Key != ConsoleKey.Enter);
        System.Console.WriteLine();
        return pin;
    }
}

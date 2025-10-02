using CajeroAutomatico;

class Program
{
    static void Main()
    {
        var cajero = new Cajero();
        cajero.AsegurarUsuarioDemo(); //Asegura que exista un usario demo para pruebas

        // Inicio de sesión
        System.Console.ForegroundColor = ConsoleColor.Magenta;
        System.Console.WriteLine("=== Cajero Automático ===");
        System.Console.ResetColor();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.Write("Usuario (Id o Nombre): ");
        System.Console.ResetColor();

        var user = System.Console.ReadLine() ?? string.Empty;

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write("PIN: ");
        System.Console.ResetColor();

        var pin = LeerPin();

        if (!cajero.IniciarSesion(user, pin))
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("\nCredenciales incorrectas.");
            System.Console.ResetColor();
            return;
        }
        var actual = cajero.ObtenerUsuarioActual();

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"\nBienvenido, {actual?.Nombre}.");
        System.Console.ResetColor();

        // Menú principal
        while (true)
        {
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
            System.Console.ResetColor();

            System.Console.WriteLine("1. Depositar");
            System.Console.WriteLine("2. Retirar");
            System.Console.WriteLine("3. Consultar saldo");
            System.Console.WriteLine("4. Últimos 5 movimientos");
            System.Console.WriteLine("5. Cambio de clave (PIN)");
            System.Console.WriteLine("6. Salir");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.Write("Seleccione una opción: ");
            System.Console.ResetColor();

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
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    System.Console.WriteLine("¡Hasta luego!");
                    System.Console.ResetColor();
                    return;

                default:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("Opción inválida.");
                    System.Console.ResetColor();
                    break;
            }
        }
    }

    //Método para realizar un depósito
    private static void RealizarDeposito(Cajero cajero)
    {
        System.Console.ForegroundColor = ConsoleColor.DarkMagenta;
        System.Console.Write("Ingrese el monto a depositar: ");
        System.Console.ResetColor();

        var input = System.Console.ReadLine();

        if (decimal.TryParse(input, out var monto))
        {
            //LLama al método Depositar del cajero  
            if (cajero.Depositar(monto))
            {
                var usuario = cajero.ObtenerUsuarioActual();
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"Depósito exitoso. Nuevo saldo: ${usuario?.Saldo:F2}");
                System.Console.ResetColor();
            }
            else
            {
                //Error en caso de monto inválido o sin sesión activa
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Error: Monto inválido o no hay sesión activa.");
                System.Console.ResetColor();
            }
        }
        else
        {
            //Error en caso de entrada no numérica
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Error: Ingrese un monto válido.");
            System.Console.ResetColor();
        }
    }

    //Método para realizar un retiro
    private static void RealizarRetiro(Cajero cajero)
    {
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write("Ingrese el monto a retirar: ");
        System.Console.ResetColor();

        var input = System.Console.ReadLine();

        if (decimal.TryParse(input, out var monto))
        {
            //Llama al método Retirar del cajero
            if (cajero.Retirar(monto))
            {
                var usuario = cajero.ObtenerUsuarioActual();
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"Retiro exitoso. Nuevo saldo: ${usuario?.Saldo:F2}");
                System.Console.ResetColor();
            }
            else
            {
                //Error en caso de monto inválido, saldo insuficiente o sin sesión activa
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Error: Monto inválido, saldo insuficiente o no hay sesión activa.");
                System.Console.ResetColor();
            }
        }
        else
        {
            //Error entrada no numérica
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Error: Ingrese un monto válido.");
            System.Console.ResetColor();
        }
    }

    //Método para consultar el saldo actual
    private static void RealizarConsultaSaldo(Cajero cajero)
    {
        var usuario = cajero.ObtenerUsuarioActual();

        //Verifica si hay sesión activa
        if (usuario == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("No hay sesión activa.");
            System.Console.ResetColor();
            return;
        }

        //Muestra el saldo actual
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"Saldo actual: ${usuario.Saldo:F2}");
        System.Console.ResetColor();
    }

    //Método para consultar los últimos 5 movimientos
    private static void RealizarConsultaMovimientos(Cajero cajero)
    {
        var movimientos = cajero.ObtenerUltimosMovimientos(5);

        if (movimientos.Count == 0)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("No hay movimientos registrados.");
            System.Console.ResetColor();
            return;
        }

        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("\n=== ÚLTIMOS 5 MOVIMIENTOS ===");
        System.Console.ResetColor();

        //Recorre y muestra cada movimiento
        foreach (var mov in movimientos)
        {
            System.Console.WriteLine($"{mov.Fecha:dd/MM/yyyy HH:mm} - {mov.Tipo} - ${mov.Monto:F2} - Saldo: ${mov.SaldoResultante:F2}");
        }
    }

    //Método para cambiar el PIN
    private static void RealizarCambioClave(Cajero cajero)
    {
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write("PIN actual: ");
        System.Console.ResetColor();

        var pinActual = LeerPin();

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write("PIN nuevo: ");
        System.Console.ResetColor();

        var pinNuevo = LeerPin();

        if (cajero.CambiarPin(pinActual, pinNuevo))
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("PIN actualizado correctamente.");
            System.Console.ResetColor();
        }
        else
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Error: PIN actual incorrecto o PIN nuevo inválido.");
            System.Console.ResetColor();
        }
    }
    // Método auxiliar para leer un PIN sin mostrarlo en pantalla
    private static string LeerPin()
    {
        var pinBuilder = new System.Text.StringBuilder();
        while (true)
        {
            var key = System.Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                System.Console.WriteLine();
                break;
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (pinBuilder.Length > 0)
                {
                    pinBuilder.Length--;
                    // borrar un caracter en pantalla
                    System.Console.Write("\b \b");
                }
                continue;
            }
            if (!char.IsControl(key.KeyChar))
            {
                pinBuilder.Append(key.KeyChar);
                System.Console.Write("*");
            }
        }
        return pinBuilder.ToString();
    }
}

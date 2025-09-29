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
            System.Console.WriteLine("3. Salir");
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

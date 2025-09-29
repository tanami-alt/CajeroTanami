using CajeroAutomatico;

class Program
{
    static void Main()
    {
        var cajero = new Cajero();
        cajero.AsegurarUsuarioDemo();

        System.Console.WriteLine("=== Cajero Automático - Inicio de Sesión ===");
        System.Console.Write("Usuario (Id o Nombre): ");
        var user = System.Console.ReadLine() ?? string.Empty;
        System.Console.Write("PIN: ");
        var pin = LeerPin();

        if (cajero.IniciarSesion(user, pin))
        {
            var actual = cajero.ObtenerUsuarioActual();
            System.Console.WriteLine($"\nBienvenido, {actual?.Nombre}. Inicio de sesión exitoso.");
        }
        else
        {
            System.Console.WriteLine("\nCredenciales incorrectas.");
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
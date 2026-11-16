using System.Globalization;
using System.Text;

namespace ATM2.UI
{
    /// <summary>Utilidades de entrada/salida por consola reutilizadas por todos los menús.</summary>
    public static class ConsoleHelper
    {
        private const int Width = 50;

        public static void Header(string title)
        {
            if (!Console.IsOutputRedirected) Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', Width));
            Console.WriteLine(Center(title));
            Console.WriteLine(new string('=', Width));
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void Section(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"--- {title} ---");
            Console.ResetColor();
        }

        /// <summary>Muestra opciones numeradas desde 1 y la opción 0 para salir/volver. Devuelve la elegida.</summary>
        public static int ShowMenu(IReadOnlyList<string> options, string exitLabel = "Volver")
        {
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"  {i + 1}. {options[i]}");
            Console.WriteLine($"  0. {exitLabel}");
            Console.WriteLine();
            return ReadInt("Selecciona una opción", 0, options.Count);
        }

        /// <summary>Lista elementos numerados y deja elegir uno. Devuelve null si el usuario cancela con 0.</summary>
        public static T? SelectFromList<T>(IReadOnlyList<T> items, Func<T, string> describe, string prompt) where T : class
        {
            for (int i = 0; i < items.Count; i++)
                Console.WriteLine($"  {i + 1}. {describe(items[i])}");
            Console.WriteLine("  0. Cancelar");
            Console.WriteLine();
            int choice = ReadInt(prompt, 0, items.Count);
            return choice == 0 ? null : items[choice - 1];
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(ReadLineOrExit(), out int value) && value >= min && value <= max)
                    return value;
                Error($"Ingresa un número entre {min} y {max}.");
            }
        }

        public static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string input = ReadLineOrExit().Trim();
                if (input.Length > 0) return input;
                Error("Este campo es obligatorio.");
            }
        }

        /// <summary>Lee un monto. Devuelve null si el usuario deja el campo vacío (cancelar).</summary>
        public static decimal? ReadAmount(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (Enter para cancelar): ");
                string input = ReadLineOrExit().Trim();
                if (input.Length == 0) return null;

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal value) ||
                    decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    if (value > 0) return Math.Round(value, 2);
                }
                Error("Ingresa un monto válido mayor que cero.");
            }
        }

        /// <summary>Lee contraseñas/PIN mostrando asteriscos.</summary>
        public static string ReadSecret(string prompt)
        {
            Console.Write($"{prompt}: ");
            if (Console.IsInputRedirected) return ReadLineOrExit();

            var buffer = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return buffer.ToString();
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Length--;
                        Console.Write("\b \b");
                    }
                    continue;
                }
                if (!char.IsControl(key.KeyChar))
                {
                    buffer.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
        }

        public static bool Confirm(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (s/n): ");
                string input = ReadLineOrExit().Trim().ToLowerInvariant();
                if (input is "s" or "si" or "sí") return true;
                if (input is "n" or "no") return false;
            }
        }

        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Presiona Enter para continuar...");
            ReadLineOrExit();
        }

        public static void Cancelled() => Info("Operación cancelada.");

        public static void Success(string message) => WriteColored($"✔ {message}", ConsoleColor.Green);
        public static void Error(string message) => WriteColored($"✖ {message}", ConsoleColor.Red);
        public static void Warning(string message) => WriteColored($"! {message}", ConsoleColor.Yellow);
        public static void Info(string message) => WriteColored(message, ConsoleColor.Gray);

        private static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static string Center(string text) =>
            text.Length >= Width ? text : text.PadLeft((Width + text.Length) / 2);

        /// <summary>Si la entrada se cierra (Ctrl+Z / fin de archivo) se termina la app en vez de quedar en bucle.</summary>
        private static string ReadLineOrExit()
        {
            string? line = Console.ReadLine();
            if (line is null)
            {
                Console.WriteLine();
                Console.WriteLine("Entrada finalizada. ¡Hasta pronto!");
                Environment.Exit(0);
            }
            return line;
        }
    }
}

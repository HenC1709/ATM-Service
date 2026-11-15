using ATM2.Application;
using ATM2.Core.Enums;
using ATM2.Core.Exceptions;

namespace ATM2.UI.Menus
{
    /// <summary>Pantalla inicial: registro e inicio de sesión.</summary>
    public class MainMenu
    {
        private readonly AppServices _services;

        public MainMenu(AppServices services)
        {
            _services = services;
        }

        public void Run()
        {
            while (true)
            {
                ConsoleHelper.Header("CAJERO AUTOMÁTICO ATM2");
                int option = ConsoleHelper.ShowMenu(new[] { "Registrarse", "Iniciar sesión" }, "Salir");

                switch (option)
                {
                    case 1: Register(); break;
                    case 2: Login(); break;
                    case 0:
                        ConsoleHelper.Info("Gracias por usar ATM2. ¡Hasta pronto!");
                        return;
                }
            }
        }

        private void Register()
        {
            ConsoleHelper.Header("REGISTRO DE USUARIO");
            try
            {
                string name = ConsoleHelper.ReadText("Nombre completo");
                string email = ConsoleHelper.ReadText("Correo electrónico");
                string password = ConsoleHelper.ReadSecret("Contraseña");
                string confirm = ConsoleHelper.ReadSecret("Confirmar contraseña");

                if (password != confirm)
                {
                    ConsoleHelper.Error("Las contraseñas no coinciden.");
                }
                else
                {
                    var user = _services.Auth.Register(name, email, password);
                    Console.WriteLine();
                    ConsoleHelper.Success($"¡Bienvenido(a), {user.FullName}! Se creó tu cuenta de ahorros.");
                    ConsoleHelper.Info("Inicia sesión y solicita una tarjeta para usar el cajero.");
                }
            }
            catch (AtmException ex)
            {
                ConsoleHelper.Error(ex.Message);
            }
            ConsoleHelper.Pause();
        }

        private void Login()
        {
            ConsoleHelper.Header("INICIAR SESIÓN");
            string sessionId = string.Empty;
            try
            {
                string email = ConsoleHelper.ReadText("Correo electrónico");
                string password = ConsoleHelper.ReadSecret("Contraseña");

                var session = _services.Auth.Login(email, password);
                sessionId = session.Id;
                var user = _services.Users.GetById(session.UserId)
                    ?? throw new UserNotFoundException(session.UserId);

                if (user.Role == UserRole.Admin)
                    new AdminMenu(_services, session, user).Run();
                else
                    new UserMenu(_services, session, user).Run();
            }
            catch (AtmException ex)
            {
                ConsoleHelper.Error(ex.Message);
                ConsoleHelper.Pause();
            }
            finally
            {
                if (sessionId.Length > 0) _services.Auth.Logout(sessionId);
            }
        }
    }
}

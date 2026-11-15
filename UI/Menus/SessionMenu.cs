using ATM2.Application;
using ATM2.Core.Exceptions;
using ATM2.Core.Models;

namespace ATM2.UI.Menus
{
    /// <summary>
    /// Bucle común para los menús que requieren sesión: valida la sesión antes de cada opción,
    /// muestra los errores de negocio y vuelve al menú.
    /// </summary>
    public abstract class SessionMenu
    {
        protected readonly AppServices Services;
        protected readonly Session Session;
        protected readonly User User;

        protected SessionMenu(AppServices services, Session session, User user)
        {
            Services = services;
            Session = session;
            User = user;
        }

        protected abstract string Title { get; }
        protected abstract string[] Options { get; }
        protected virtual string ExitLabel => "Cerrar sesión";

        /// <summary>true si este menú se abre dentro de otro (el error de sesión se propaga al padre).</summary>
        protected virtual bool IsNested => false;

        protected abstract void Handle(int option);
        protected virtual void BeforeMenu() { }
        protected virtual void OnExit() { }
        protected virtual bool ShouldExitOn(AtmException ex) => false;

        public void Run()
        {
            while (true)
            {
                try
                {
                    Services.Sessions.EnsureActive(Session.Id);
                    ConsoleHelper.Header(Title);
                    BeforeMenu();

                    int option = ConsoleHelper.ShowMenu(Options, ExitLabel);
                    if (option == 0)
                    {
                        OnExit();
                        return;
                    }

                    // Revalidar: el usuario pudo quedarse mucho tiempo en el menú.
                    Services.Sessions.EnsureActive(Session.Id);
                    Console.WriteLine();
                    Handle(option);
                    ConsoleHelper.Pause();
                }
                catch (InvalidSessionException ex)
                {
                    if (IsNested) throw;
                    ConsoleHelper.Error(ex.Message);
                    ConsoleHelper.Pause();
                    return;
                }
                catch (AtmException ex)
                {
                    ConsoleHelper.Error(ex.Message);
                    ConsoleHelper.Pause();
                    if (ShouldExitOn(ex)) return;
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Error($"Error inesperado: {ex.Message}");
                    ConsoleHelper.Pause();
                }
            }
        }
    }
}

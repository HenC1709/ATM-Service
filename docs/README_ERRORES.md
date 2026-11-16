# Errores encontrados — ATM2

Revisión del proyecto **antes** de los cambios (rama `refactor/allFolder`, commit `0aced4a`).
Cada punto indica **dónde** estaba, **por qué** era un problema y **cómo** se resolvió.

> Leyenda de severidad: 🔴 impedía compilar/ejecutar · 🟠 bug o riesgo de seguridad · 🟡 calidad / convención

---

## 1. Errores que impedían compilar o usar la app

| # | Sev. | Archivo | Problema | Solución |
|---|------|---------|----------|----------|
| 1.1 | 🔴 | `Program.cs` | Estaba vacío (solo el BOM). `dotnet build` fallaba con **CS5001: el programa no contiene un método `Main`**. | Se escribió el punto de entrada: arma repositorios, servicios, crea el admin inicial y abre el menú. |
| 1.2 | 🔴 | — | Había interfaces de servicios pero **ninguna implementación** (`AccountService`, `AuthService`, etc.) ni interfaz de usuario. La app no podía hacer nada. | Se creó `Application/Services/*` y `UI/*`. |
| 1.3 | 🔴 | `Infrastructure/Json/JsonService.cs` | `WriteAll` escribía en `.../Infrastructure/Data/` pero **esa carpeta nunca se creaba** → `DirectoryNotFoundException` en la primera escritura. | El constructor ahora hace `Directory.CreateDirectory(basePath)`. |
| 1.4 | 🔴 | `.vscode/launch.json` | `"console": "internalConsole"` — la consola de depuración de VS Code **no acepta `Console.ReadLine()`**, así que el menú se quedaba colgado. | Cambiado a `"integratedTerminal"`. (Nota: `.vscode/` está en `.gitignore`, ese cambio es solo local.) |

## 2. Nombres de archivos y propiedades con errores de escritura

| # | Sev. | Antes | Después | Por qué |
|---|------|-------|---------|---------|
| 2.1 | 🟡 | `IAccaountRepository.cs` | `IAccountRepository.cs` | El archivo no coincidía con la interfaz que contiene (`IAccountRepository`). |
| 2.2 | 🟡 | `IAccaountService.cs` | `IAccountService.cs` | Igual que arriba. |
| 2.3 | 🟠 | `User.PasswordHas` | `User.PasswordHash` | Error tipográfico en una propiedad que se guarda en el JSON (cambiarla luego rompe los datos). |
| 2.4 | 🟡 | `User.CreateAdt`, `Account.CreateAdt` | `CreatedAt` | Error tipográfico; además `Ticket` ya usaba `CreatedAt` (inconsistente). |
| 2.5 | 🟡 | `JsonService.ReadALL` | `ReadAll` | Convención PascalCase de .NET. |
| 2.6 | 🟡 | `TicketStatus.Inprogress` | `TicketStatus.InProgress` | Convención PascalCase. |
| 2.7 | 🟡 | `<RootNamespace>ATM_2._0</RootNamespace>` | `ATM2` | Todo el código usa `namespace ATM2...`; los archivos nuevos que cree Visual Studio/VS Code saldrían con otro namespace. |

## 3. Seguridad

| # | Sev. | Dónde | Problema | Solución |
|---|------|-------|----------|----------|
| 3.1 | 🟠 | `User.Pin` | El PIN se iba a guardar **en texto plano** en `users.json`. | El PIN ahora vive en la tarjeta (`Card.PinHash`) y se guarda hasheado. |
| 3.2 | 🟠 | `User.PasswordHas` | Existía la propiedad pero no había nada que hasheara. | `Infrastructure/Security/PasswordHasher.cs` (PBKDF2-SHA256, 100 000 iteraciones, salt aleatorio, comparación en tiempo constante). |
| 3.3 | 🟠 | — | No había límite de intentos: se podía adivinar el PIN o la contraseña sin fin. | 3 intentos de PIN → tarjeta bloqueada. 3 intentos de contraseña → usuario bloqueado. Solo un admin desbloquea. |
| 3.4 | 🟠 | `Session` | Las sesiones no expiraban. | `Session.LastActivityAt` + timeout de 5 min de inactividad (`AtmSettings.SessionTimeoutMinutes`). |

## 4. Errores de diseño en las interfaces

| # | Sev. | Dónde | Problema | Solución |
|---|------|-------|----------|----------|
| 4.1 | 🟠 | `IAuthService.IsSessionActive` | Devolvía `void`: un método llamado "¿está activa?" **no podía responder**. | Ahora devuelve `bool`. |
| 4.2 | 🟠 | `IAuthService.ValidatePin(userId, pin)` | Devolvía `void` y el PIN estaba ligado al usuario, no a una tarjeta. No existía el concepto de tarjeta. | Se movió a `ICardService.ValidatePin(cardId, pin)`, que devuelve la tarjeta o lanza `InvalidPinException` / `CardBlockedException`. |
| 4.3 | 🟠 | `IAuthService.Login` | Devolvía `Session?` (¿`null` por qué? ¿clave mala, usuario bloqueado?). | Devuelve `Session` y lanza una excepción con el motivo exacto. Se agregó `Register(...)`. |
| 4.4 | 🟠 | `IAccountService.Deposit/Withdraw` | No registraban **quién** ni **con qué tarjeta** se hizo la operación (requisito: saber quién retiró o pagó deudas). | `Transaction` ahora guarda `UserId`, `CardId` y `BalanceAfter`. |
| 4.5 | 🟠 | `ICreditService` | Se podía pedir/aprobar un crédito pero **no había forma de pagar la deuda**. | Se agregó `Pay(creditId, amount, cardId)` y `GetPendingDebts(userId)`. |
| 4.6 | 🟠 | `TransactionType.Transfer` | Una transferencia afecta dos cuentas pero solo había un tipo: la cuenta destino no veía el movimiento en su historial. | `TransferOut` / `TransferIn` (+ `DebtPayment`, `CreditDisbursement`). |
| 4.7 | 🟡 | `Credit.InterestRate` | Era `float` mezclado con montos `decimal` → errores de redondeo con dinero. | Cambiado a `decimal`. Se agregaron `TotalToPay` y `PaidAt`. |

## 5. Persistencia JSON

| # | Sev. | Dónde | Problema | Solución |
|---|------|-------|----------|----------|
| 5.1 | 🟠 | `JsonContext.BasePath` | Usaba `AppDomain.CurrentDomain.BaseDirectory` → los datos quedaban dentro de `bin/Debug/net10.0/`. Un `dotnet clean` o recompilar en Release **"perdía" todos los datos**. | Ahora es `<carpeta de trabajo>/Data/` (la raíz del proyecto con `dotnet run` o F5). |
| 5.2 | 🟠 | `JsonService.ReadALL` | Si el JSON estaba dañado, `Deserialize` lanzaba una excepción sin contexto. | Mensaje claro indicando qué archivo está dañado. No se devuelve lista vacía a propósito (el siguiente guardado borraría todo). |
| 5.3 | 🟠 | `JsonService.WriteAll` | Si la app se cerraba a mitad de la escritura el archivo quedaba corrupto. | Escritura atómica: se escribe un `.tmp` y luego se reemplaza. |
| 5.4 | 🟡 | `JsonService` | Creaba `new JsonSerializerOptions` en cada escritura (costoso) y los enums se guardaban como números (`"Type": 1`). | Opciones estáticas, `JsonStringEnumConverter` (`"Type": "Withdrawal"`) y lectura sin distinguir mayúsculas. |
| 5.5 | 🟡 | `JsonContext` | Clase normal con campos `static readonly`; no tenía archivo para tarjetas. | `static class` con `const`; se agregó `cards.json`. |
| 5.6 | 🟡 | `*Repository.cs` | Los 5 repositorios repetían el mismo código CRUD (copiar/pegar). | Clase base genérica `JsonRepository<T>`; cada repositorio solo define sus consultas propias. |
| 5.7 | 🟡 | `*Repository.Update` | Si el id no existía, **no hacía nada y no avisaba**. | Lanza `InvalidOperationException`. `Add` también valida ids duplicados. |
| 5.8 | 🟡 | `UserRepository.GetByEmail` | Comparación sensible a mayúsculas: `Juan@x.com` y `juan@x.com` serían usuarios distintos. | Comparación sin distinguir mayúsculas + el correo se normaliza al registrar. |

## 6. Limpieza menor

| # | Archivo | Problema |
|---|---------|----------|
| 6.1 | `Core/Models/User.cs` | `using System.Dynamic;` sin usar. |
| 6.2 | `Core/Exceptions/AccountLockedException.cs` | `using ATM2.Core.Models;` sin usar. El mensaje mostraba el GUID del usuario (inútil para quien lo lee). |
| 6.3 | `Core/Exceptions/*` | Heredaban de `Exception` directamente: la UI no podía distinguir un error de negocio ("fondos insuficientes") de un bug real. Ahora heredan de `AtmException`. |
| 6.4 | `Claude.md` | Archivo vacío y sin seguimiento en git. No se tocó; bórralo o rellénalo con el contexto del proyecto. |
| 6.5 | `.gitignore` | Tenías un cambio sin commit (`.vscode/`). Se mantuvo y se agregó `Data/`. |

---

## 7. Segunda ronda — 6 mejoras de calidad / convención (🟡)

> Los 12 puntos 🟡 originales (2.1, 2.2, 2.4–2.7, 4.7, 5.4–5.8) **ya estaban corregidos** en la primera ronda.
> Esta ronda corrige **6 problemas 🟡 nuevos** detectados al releer el código ya refactorizado.

| # | Sev. | Dónde | Problema | Solución |
|---|------|-------|----------|----------|
| 7.1 | 🟡 | `AccountService`, `CreditService` | La validación "el monto debe ser mayor que cero" estaba **copiada en 4 lugares** (un método privado + 2 `if` sueltos + 1 más). Si cambiaba el mensaje había que tocar todos. | Nuevo `Core/Guard.cs` con `Guard.Positive(amount)`; los servicios lo reutilizan. |
| 7.2 | 🟡 | `CreditService.ShortId`, `Display.ShortId` | La misma lógica `id[..8].ToUpperInvariant()` duplicada en dos clases (y lanzaría excepción con un id de menos de 8 caracteres). | Nuevo `Core/IdFormatter.Short(id)`, seguro con ids cortos; ambos lo usan. |
| 7.3 | 🟡 | `UserMenu`, `AtmMenu`, `AdminMenu` | "Números mágicos" en la UI: `Take(20)`, `Take(10)`, `Take(50)`, montos rápidos `{500, 1000, 2000, 5000}` y los títulos con "últimos 20/10" escritos a mano (podían quedar desincronizados del código). | Pasaron a `AtmSettings` (`HistoryRows`, `MiniStatementRows`, `AuditRows`, `QuickWithdrawalAmounts`) y los títulos se generan con esos valores. |
| 7.4 | 🟡 | Menús | El texto `"Operación cancelada."` estaba repetido **9 veces**. | `ConsoleHelper.Cancelled()` en un solo sitio. |
| 7.5 | 🟡 | `AdminMenu.ShowUsers` | El rol se mostraba en inglés crudo (`Client`/`Admin`) mientras el resto de la UI está en español y usa `Display.Name(...)`. | Nuevo `Display.Name(UserRole)` ("Cliente" / "Admin") usado en la lista de usuarios. |
| 7.6 | 🟡 | `DataSeeder` | Las credenciales del admin eran constantes públicas (`AdminEmail`, `AdminPassword`) sin forma de cambiarlas sin recompilar. | Se leen de las variables de entorno `ATM_ADMIN_EMAIL` / `ATM_ADMIN_PASSWORD`; si no existen se usan los valores de desarrollo (`DefaultAdminEmail` / `DefaultAdminPassword`). El correo se normaliza a minúsculas. |

**Verificación:** `dotnet build` → 0 errores, 0 advertencias. Prueba de ejecución: crédito, retiro cancelado (`Cancelled()`), "ÚLTIMOS 10 MOVIMIENTOS", "HISTORIAL (últimos 20)" y lista de usuarios con rol en español. ✅

**Pendientes 🟡 detectados y no tocados** (para una tercera ronda): `<AssemblyName>` sigue siendo `ATM_2.0` (cambiarlo obliga a actualizar `launch.json`); `Program.cs` arma las dependencias con `new` en vez de `Microsoft.Extensions.DependencyInjection`; cada consulta del repositorio relee el JSON completo.

---

## Limitaciones conocidas (pendientes, no son bugs bloqueantes)

- **Sin transacciones reales:** una transferencia hace dos escrituras en `accounts.json`. Si la app se cierra justo entre ambas, el dinero quedaría descontado de una cuenta y no sumado a la otra. Con JSON es aceptable; con una base de datos se resolvería con una transacción.
- **Rendimiento:** cada consulta lee el archivo JSON completo. Bien para clase/pruebas; con miles de registros convendría caché o una BD (SQLite/EF Core).
- **Credenciales del admin** por defecto en `Infrastructure/Seed/DataSeeder.cs` (`admin@atm.com` / `admin123`); se pueden sobrescribir con las variables de entorno `ATM_ADMIN_EMAIL` y `ATM_ADMIN_PASSWORD` antes de la primera ejecución.
- **No hay pruebas unitarias.** Recomendado: un proyecto xUnit para `AccountService`, `CardService` y `CreditService`.
- **Git:** `IAccaount*.cs` se renombraron con el explorador de archivos (no con `git mv`). Al hacer `git add -A` git detecta el renombrado automáticamente.

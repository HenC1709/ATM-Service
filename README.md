# 🏧 ATM2 — Simulador de Cajero Automático (.NET 10)

Aplicación de consola interactiva en **C# / .NET 10** que simula el funcionamiento completo de un cajero automático (ATM) y una banca en línea básica, utilizando persistencia local en archivos **JSON** y arquitectura en capas.

---

## ✨ Características Principales

### 👤 Clientes (Banca y Cajero)
- **Registro e Inicio de Sesión:** Creación automática de cuenta de ahorros al registrarse.
- **Emisión de Tarjetas (Simulada):** Generación de tarjetas de débito de 16 dígitos validadas con el **algoritmo de Luhn** y vigencia de 4 años.
- **Seguridad con PIN y Contraseña:**
  - Entrada oculta en consola (`****`).
  - Hashing criptográfico con **PBKDF2 (SHA-256)**, *salt* aleatorio de 16 bytes y 100,000 iteraciones.
  - Bloqueo automático de tarjeta o cuenta tras **3 intentos fallidos**.
  - Expiración de sesión tras **5 minutos de inactividad**.
- **Operaciones en Cajero (requiere insertar tarjeta + PIN):**
  - **Consulta de saldo** en tiempo real.
  - **Retiro de efectivo:** Montos rápidos (\$500, \$1,000, \$2,000, \$5,000) u otro monto en múltiplos de \$100 (máx. \$10,000 por operación).
  - **Depósito de efectivo:** Hasta \$50,000 por operación.
  - **Pago de deudas:** Pago total o abono parcial a créditos activos descontando de la cuenta.
  - **Transferencias:** Envío de dinero a otros usuarios registrados mediante su correo electrónico.
  - **Cambio de PIN** y **emisión de comprobantes** en consola por operación.
- **Créditos y Deudas:** Solicitud de préstamos con 10% de interés (aprobación y desembolso automático hasta \$10,000; montos mayores requieren aprobación del administrador).
- **Soporte:** Creación y seguimiento de tickets de atención al cliente.

### 🛡️ Panel de Administración
- **Auditoría de Transacciones:** Historial global filtrable por tipo (*Todos*, *Retiros*, *Pagos de deuda*, *Depósitos*) que detalla qué usuario realizó la operación y con qué tarjeta.
- **Gestión de Usuarios:** Listado general y bloqueo/desbloqueo de cuentas de clientes.
- **Gestión de Créditos:** Aprobación (con desembolso inmediato) o rechazo de solicitudes mayores a \$10,000.
- **Gestión de Tarjetas:** Bloqueo y desbloqueo de tarjetas de débito.
- **Gestión de Tickets:** Revisión y cambio de estado (*Abierto*, *En progreso*, *Resuelto*, *Cerrado*).

---

## 🧱 Arquitectura y Estructura del Proyecto

El proyecto está organizado siguiendo principios de separación de responsabilidades en 4 capas:

```text
ATM2/
├── Program.cs                        # Punto de entrada y composición de dependencias
├── ATM_2.0.csproj                    # Configuración del proyecto (.NET 10)
├── Core/                             # Dominio (sin dependencias externas)
│   ├── AtmSettings.cs                # Reglas y límites de negocio centralizados
│   ├── Enums/                        # AccountType, CardStatus, CreditStatus, TicketStatus, TransactionType, UserRole
│   ├── Exceptions/                   # AtmException y excepciones de negocio específicas
│   ├── Interfaces/                   # Contratos de Repositorios, Servicios, Seguridad e IEntity
│   └── Models/                       # User, Account, Card, Credit, Transaction, Ticket, Session
├── Application/                      # Casos de uso y reglas de negocio
│   ├── AppServices.cs                # Contenedor de servicios de aplicación
│   └── Services/                     # AuthService, AccountService, CardService, CreditService, etc.
├── Infrastructure/                   # Persistencia y servicios técnicos
│   ├── Json/                         # JsonContext y JsonService (escritura atómica .tmp)
│   ├── Repositories/                 # JsonRepository<T> base y repositorios por entidad
│   ├── Security/                     # PasswordHasher (PBKDF2-SHA256)
│   └── Seed/                         # DataSeeder (creación del administrador por defecto)
├── UI/                               # Interfaz de usuario en consola
│   ├── ConsoleHelper.cs              # Lectura validada, máscaras de PIN/clave, menús y colores
│   ├── Display.cs                    # Formato de moneda, fechas, comprobantes y máscaras de tarjeta
│   └── Menus/                        # MainMenu, UserMenu, AtmMenu, AdminMenu, SessionMenu
├── Data/                             # Archivos JSON autogenerados (ignorados en Git)
└── docs/                             # Documentación de revisión y cambios
    ├── README_CAMBIOS.md
    └── README_ERRORES.md
```

---

## 🚀 Requisitos y Ejecución

### Requisitos previos
- [.NET 10 SDK](https://dotnet.microsoft.com/download) instalado.

### Ejecutar el proyecto
En la raíz del repositorio, ejecuta:

```powershell
dotnet run
```

La carpeta `Data/` y los archivos `.json` (`users.json`, `accounts.json`, `cards.json`, `transactions.json`, `credits.json`, `tickets.json`) se crean automáticamente al iniciar la aplicación.

---

## 🔑 Credenciales por Defecto (Administrador)

En la primera ejecución, `DataSeeder` registra automáticamente una cuenta de administrador:

| Campo | Valor |
| :--- | :--- |
| **Correo** | `admin@atm.com` |
| **Contraseña** | `admin123` |

---

## ⚙️ Reglas de Negocio Configurables (`AtmSettings.cs`)

| Parámetro | Valor por defecto | Descripción |
| :--- | :--- | :--- |
| `PinLength` | `4` | Cantidad de dígitos numéricos del PIN de tarjeta |
| `MinPasswordLength` | `6` | Longitud mínima de la contraseña de usuario |
| `MaxPinAttempts` / `MaxLoginAttempts` | `3` | Intentos fallidos antes de bloquear tarjeta o usuario |
| `SessionTimeoutMinutes` | `5` | Minutos de inactividad antes de expirar la sesión |
| `WithdrawalMultiple` | `$100` | Denominación mínima permitida para retiros |
| `MaxWithdrawalPerTransaction` | `$10,000` | Monto máximo por retiro en cajero |
| `MaxDepositPerTransaction` | `$50,000` | Monto máximo por depósito en cajero |
| `CreditInterestRate` | `10%` | Tasa de interés fija aplicada a créditos |
| `CreditAutoApprovalLimit` | `$10,000` | Tope para aprobación y desembolso automático |
| `MaxCreditAmount` | `$100,000` | Monto máximo solicitable en un crédito |

---

## 📚 Documentación Adicional

- [Detalle de cambios implementados](docs/README_CAMBIOS.md)
- [Reporte de errores corregidos](docs/README_ERRORES.md)

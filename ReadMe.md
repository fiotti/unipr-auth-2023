Esempio Autenticazione in ASP.NET Core
======================================

## Frontend

Il focus di questo corso è sui microservizi (backend), il frontend in questo
esempio viene generato automaticamente e serve solo per autenticare l'utente.

### Step 1

Creare un nuovo progetto di tipo ASP.NET Core Web App selezionando
"account individuali" come tipo di autenticazione.

https://learn.microsoft.com/aspnet/core/getting-started/

Verrà generato un nuovo progetto con un frontend per registrare ed autenticare
gli utenti tramite username + password.

Lanciare il progetto per verificare che tutto funzioni correttamente,
generare le tabelle su database applicando le migration quando richiesto.

Se il percorso di default del database non dovesse essere corretto, è possibile
modificarlo dal file _appsettings.json_.

### Step 2 (opzionale)

Generare scaffold con click destro sul progetto e selezionando "aggiungi" e poi
"nuovo oggetto scaffold", ed infine "identity"; selezionare poi tutte le
opzioni desiderate (per esempio tutte le checkbox).

https://learn.microsoft.com/aspnet/core/security/authentication/scaffold-identity

Grazie allo scaffold è possibile generare dei file per le varie pagine
del frontend; questi file si trovano nella cartella _Areas/Identity/Pages_
e possono essere personalizzati a piacere.

### Step 3

Nel main, aggiungere `builder.Services.AddControllers();`
e `app.MapControllers();`.

Creare un controller con tre action:
- Public
- Logged
- Admin

Aggiungere all'action "Logged" attributo `[Authorize]`.

Aggiungere all'action "Admin" attributo `[Authorize(Roles = "Administrator")]`.

### Step 4 (opzionale)

Aggiungere Swagger.

### Step 5

Nel main, aggiungere `.AddRoles<IdentityRole>()` sotto ad
`AddDefaultIdentity<IdentityUser>(…)`.

Aggiungere il role su database nella tabella "AspNetRoles", e dopo aver creato
un nuovo utente tramite frontend, aggiungerlo ad "AspNetUserRoles".

> Su Windows è possibile generare un nuovo GUID da PowerShell con `New-Guid`;
> su Linux o MacOS è possibile farlo con `uuidgen`.

### Step 6

Provare a chiamare questi tre endpoint.

"Public" può essere chiamato da tutti.

"Logged" può essere chiamato solo dagli utenti loggati.

"Admin" può essere chiamato solo dagli utenti con ruolo "Administrator".


## Microservizio 1

Il primo microservizio sarà chiamabile dagli utenti autenticati
tramite il frontend.

## Step 1

Creare un nuovo progetto di tipo ASP.NET Core Web API all'interno della
solution, selezionando "none" come tipo di autenticazione.

In Visual Studio, impostare avvio multiplo di progetti e selezionare
sia il frontend che questo microservizio.

Lanciare i progetti per verificare che tutto funzioni correttamente.

## Step 2

Configurare l'autenticazione su questo microservizio.

Copiare il file _Data/ApplicationDbContext.cs_ dal frontend a questo
microservizio e correggere il namespace.

> Potrebbe essere necessario installare i pacchetti NuGet
> "Microsoft.AspNetCore.Identity.EntityFrameworkCore" e
> "Microsoft.EntityFrameworkCore.SqlServer" per risolvere tutti gli errori
> nel file.

Nel main, aggiungere il codice seguente:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

Aggiungere la connection-string nel file _appsettings.json_.

## Step 3

Aggiungere il seguente codice sia al main frontend che a questo:

```csharp
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"../../.data/ProtectionKeys"))
    .SetApplicationName("SharedCookieApp");
```

https://learn.microsoft.com/aspnet/core/security/cookie-sharing

## Step 4

Aggiungere l'attributo `[Authorize]` alla action del controller
e verificare che sia chiamabile solo quando l'utente è loggato.


## Microservizio 2

Creare un secondo microservizio configurato come il primo.

## Step 1

Dopo aver creato un altro progetto di tipo ASP.NET Core Web API, configurare
database, autenticazione e chiavi come per il precedente.

Aggiungerlo alla lista di avvio multiplo di progetti.

## Step 2

Dall'endpoint di questo microservizio, fare una chiamata all'altro.

> Per la comunicazione tra microservizi, la Microsoft consiglia di utilizzare
> "bearer token", per esempio tramite OAuth 2.0 oppure OpenID Connect.
> 
> https://learn.microsoft.com/dotnet/architecture/microservices/secure-net-microservices-web-applications/#authenticate-with-bearer-tokens
> 
> Una implementazione di questo tipo può essere fatta affidandosi a
> servizi di autenticazione di terze parti, come Microsoft, Google, Twitter
> o Facebook.
> 
> In alternativa, è possibile costruire un proprio identity provider basandosi
> per esempio su IdentityServer4 (obsoleto) oppure OpenIddict.
> 
> Questa implementazione non è banale, e per questioni di tempo è lasciata
> come esercizio per il lettore.
> Segue ora un'alternativa più semplice, ma meno elegante, basata sui cookie.

Costruire un `HttpClient` ed utilizzare i cookie dell'utente autenticato
per fare una chiamata all'altro microservizio, a nome dell'utente.

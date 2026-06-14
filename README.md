# UserManager

Aplikacja webowa do zarządzania wpisami użytkowników (CRUD) z przyjaznym
interfejsem. Pozwala dodawać, edytować, usuwać i przeszukiwać rekordy
użytkowników zapisywane w bazie danych.

Zbudowana w **Blazor Server (.NET 8)** z **Entity Framework Core** i bazą
**SQLite**.

---

## Wymagania

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (wersja 8.0.x)
- Opcjonalnie: Visual Studio 2022 (17.8+), VS Code lub JetBrains Rider

Nie trzeba instalować żadnego serwera bazy danych — SQLite to plik
(`usermanager.db`) tworzony automatycznie przy pierwszym uruchomieniu.

---

## Uruchomienie

### Wariant A — wiersz poleceń

```bash
cd UserManager
dotnet restore
dotnet run
```

Po starcie aplikacja wypisze adres: https://localhost:5040. Otwórz go w przeglądarce.

### Wariant B — Visual Studio

1. Otwórz `UserManager.csproj` (lub folder rozwiązania).
2. Naciśnij **F5** (uruchomienie z debugowaniem) lub **Ctrl+F5**.

### Baza danych i dane startowe

Przy pierwszym uruchomieniu aplikacja:

1. zakłada bazę i stosuje migracje (`Database.MigrateAsync()`),
2. wypełnia ją trzema przykładowymi użytkownikami (seed), jeśli tabela
   jest pusta.

Migracje są w katalogu `Migrations/`. Aby utworzyć/odtworzyć bazę ręcznie:

```bash
dotnet ef database update
```

(wymaga narzędzia: `dotnet tool install --global dotnet-ef`)

---

## Funkcje

- **Pulpit** — liczba wszystkich / aktywnych / nieaktywnych użytkowników
  oraz lista ostatnio dodanych.
- **Lista użytkowników** — tabela z wyszukiwaniem na żywo (imię, nazwisko,
  e-mail, dział), statusem aktywności i akcjami.
- **Dodawanie / edycja** — wspólny formularz z walidacją po stronie
  serwera (m.in. unikalny e-mail).
- **Usuwanie** — z oknem potwierdzenia.
- **Powiadomienia** — komunikaty toast po zapisie/usunięciu.

---

## Architektura

Warstwowy podział odpowiedzialności:

```
Components/        Warstwa UI (Blazor, komponenty .razor)
  Pages/           Strony routowalne: Home, Users, UserForm
  Layout/          Układ strony (MainLayout)
  Shared/          Komponenty współdzielone (UserAvatar)
Services/          Logika aplikacji (IUserService / UserService)
Data/              Dostęp do danych (AppDbContext, seed, migracje)
Models/            Model domenowy (User, enum Department)
Migrations/        Migracje EF Core
wwwroot/           Zasoby statyczne (app.css)
```

Zasady:

- **UI nie rozmawia bezpośrednio z bazą.** Strony korzystają z
  `IUserService`, który jako jedyny używa `AppDbContext`.
- **Kontekst per operacja.** W Blazor Server komponenty żyją długo, więc
  zamiast jednego `DbContext` używany jest `IDbContextFactory<AppDbContext>`
  — każda operacja dostaje świeży, krótko żyjący kontekst. To eliminuje
  klasyczne problemy współbieżności `DbContext` w Blazorze.
- **Walidacja po stronie serwera.** `EditForm` + `DataAnnotations` w UI to
  wygoda dla użytkownika, ale unikalność e-maila jest egzekwowana w
  serwisie i dodatkowo przez `UNIQUE INDEX` w bazie.
- **Enum jako tekst.** Pole `Department` zapisywane jest w bazie jako
  czytelny string (`HasConversion<string>`), a nie liczba.

---

## Stos technologiczny

| Element            | Technologia                          |
|--------------------|--------------------------------------|
| Framework UI       | Blazor Server (.NET 8)               |
| Dostęp do danych   | Entity Framework Core 8              |
| Baza danych        | SQLite                               |
| Walidacja          | DataAnnotations + reguły w serwisie  |
| Styl               | Własny system stylów (`wwwroot/app.css`) |

---

## Dlaczego Blazor Server + EF Core?

Spośród dozwolonych frameworków (Blazor, ASP.NET, WPF, .NET MAUI,
Entity Framework Core) wybór padł na **Blazor Server + EF Core**, bo:

- **jeden język (C#) dla UI i logiki** — brak osobnej warstwy JavaScript,
- **interaktywny UI** (wyszukiwanie na żywo, modale, toasty) bez pisania
  własnego front-endu,
- **EF Core** daje typowany, migrowalny dostęp do danych,
- **SQLite** nie wymaga instalacji serwera — projekt uruchamia się "od razu",
- **wieloplatformowość** — działa na Windows, Linux i macOS.

EF Core nie jest tu konkurencją dla Blazora, tylko jego uzupełnieniem:
to warstwa danych używana *wewnątrz* aplikacji Blazor.


dotnet run → http://localhost:5040

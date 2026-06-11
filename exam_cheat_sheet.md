

## 1. Rozwiązywanie problemów z PATH / wersją .NET (Serwery/Komputery PJATK)

Jeśli terminal w Riderze zgłasza błąd typu `The term 'dotnet' is not recognized` lub błędy z `hostfxr.dll` (brak SDK .NET):

### Szybka naprawa w aktualnej sesji terminala (PowerShell)
Uruchom to polecenie (wpłynie tylko na aktualnie otwartą zakładkę terminala):
```powershell
$env:DOTNET_ROOT="$HOME\.dotnet"; $env:PATH="$HOME\.dotnet;" + $env:PATH
```
*(Użycie `$HOME` automatycznie podstawi ścieżkę profilu aktualnego użytkownika, np. `C:\Users\s12345`).*

### Stała naprawa (wymaga ponownego uruchomienia Ridera)
Uruchom to polecenie, a następnie zrestartuj Ridera:
```powershell
[Environment]::SetEnvironmentVariable("DOTNET_ROOT", "$HOME\.dotnet", "User"); $oldPath = [Environment]::GetEnvironmentVariable("Path", "User"); if ($oldPath -notlike "*$HOME\.dotnet*") { [Environment]::SetEnvironmentVariable("Path", "$HOME\.dotnet;" + $oldPath, "User") }
```

---

## 2. Nawigacja w terminalu

Zawsze upewnij się, że polecenia uruchamiasz w katalogu zawierającym plik projektu `.csproj` (a nie w folderze solucji `.sln`).

- **Wejście do folderu projektu:**
  ```powershell
  cd NazwaFolderuProjektu
  ```
  *(np. `cd CodeFirstPoprawa`)*
- **Powrót do folderu wyżej:**
  ```powershell
  cd ..
  ```
- **Wyświetlenie zawartości folderu:**
  ```powershell
  dir
  ```

---

## 3. Kompilacja projektu

Przed wygenerowaniem migracji zawsze upewnij się, czy projekt kompiluje się bez błędów:
```powershell
dotnet build
```

---

## 4. Generowanie i usuwanie migracji

- **Tworzenie nowej migracji w folderze `Migrations`:**
  ```powershell
  dotnet ef migrations add InitialCreate --output-dir Migrations
  ```
  *(Zamiast `InitialCreate` możesz wpisać dowolną nazwę).*

- **Cofnięcie ostatnio wygenerowanej migracji (jeśli popełniłeś błąd i jeszcze nie zaktualizowałeś bazy):**
  ```powershell
  dotnet ef migrations remove
  ```

---

## 5. Operacje na bazie danych

- **Aktualizacja bazy danych (zaaplikowanie migracji i utworzenie tabel):**
  ```powershell
  dotnet ef database update
  ```

- **Cofnięcie wszystkich migracji w bazie danych (czyszczenie tabel):**
  ```powershell
  dotnet ef database update 0
  ```

- **Całkowite usunięcie (drop) bazy danych z serwera (przydatne przy ponownym tworzeniu bazy):**
  ```powershell
  dotnet ef database drop -f
  ```

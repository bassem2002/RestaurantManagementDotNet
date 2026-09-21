# ?? Instructions Migration EF Core

## Problème : MSB3027 - Fichier verrouillé

Le fichier `Models.dll` est verrouillé par Visual Studio. C'est normal après les modifications.

## Solution

### 1?? Arrêter le débogueur
- Appuyez sur **Shift+F5** dans Visual Studio
- Attendez 5 secondes

### 2?? Relancer la compilation
- Appuyez sur **Ctrl+Maj+B** pour rebuilder la solution

### 3?? Créer la migration EF Core
Ouvrez une console PowerShell et exécutez :

```powershell
cd Backend
dotnet ef migrations add AddCityPostalCodeToApplicationUser
dotnet ef database update
```

## ?? Résumé des changements

### Backend - Controllers
? **ClientsController** - Ajout des endpoints :
- `GET /api/Clients/profile` - Récupère le profil utilisateur
- `PUT /api/Clients/profile` - Met à jour le profil utilisateur

? **AccountController** - Suppression des endpoints profile (ils sont maintenant dans Clients)

### Backend - Models
? **ApplicationUser** - Ajout des propriétés :
- `City` (string?, MaxLength 100)
- `PostalCode` (string?, MaxLength 10)

### Backend - Repository
? **IClientRepository** - Ajout des méthodes :
- `GetUserByIdAsync(userId)`
- `UpdateUserAsync(user)`

? **ClientRepository** - Implémentation des méthodes

### Backend - DTOs
? **UpdateProfileDTO** - Nouveau DTO pour les mises à jour de profil

### Frontend - Services
? **UserProfileService** - URLs mises à jour :
- De `/api/Account/profile` ? `/api/Clients/profile`

---

## ?? Test

Après la migration, testez :
```
GET /api/Clients/profile
PUT /api/Clients/profile
```

Ces endpoints nécessitent un **JWT token** valide dans le header `Authorization: Bearer <token>`

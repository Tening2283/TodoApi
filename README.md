# TodoApi — ASP.NET Core + EF InMemory + JWT

API RESTful basée sur un contrôleur, construite en suivant le tutoriel Microsoft :
**"Créer une API web basée sur un contrôleur avec ASP.NET Core"**

Fonctionnalités :
- Base de données **Entity Framework Core InMemory**
- **DTO** pour empêcher la sur-publication (champ `Secret` jamais exposé)
- **Authentification JWT** + **autorisation par rôles** (Admin / User)
- **Docker** pour le déploiement
- **GitLab CI/CD** pour l'intégration et le déploiement continu

---

## Endpoints

| Méthode | URL | Description | Rôle requis |
|---------|-----|-------------|-------------|
| POST | `/api/auth/register` | Créer un compte | Public |
| POST | `/api/auth/login` | Se connecter → JWT | Public |
| GET | `/api/todoitems` | Lister tous les todos | Admin, User |
| GET | `/api/todoitems/{id}` | Obtenir un todo par ID | Admin, User |
| POST | `/api/todoitems` | Créer un todo | **Admin uniquement** |
| PUT | `/api/todoitems/{id}` | Modifier un todo | **Admin uniquement** |
| DELETE | `/api/todoitems/{id}` | Supprimer un todo | **Admin uniquement** |

---

## Exécution avec Docker (recommandé)

```bash
# 1. Cloner le dépôt
git clone https://github.com/Tening2283/TodoApi
cd TodoApi

# 2. Démarrer le container
docker compose up --build
```

L'API est disponible sur **http://localhost:8080**
Swagger UI disponible à la **racine** : http://localhost:8080

---

## Exécution locale (sans Docker)

### Prérequis
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

```bash
cd TodoApi
dotnet restore
dotnet run
```

L'API démarre sur `https://localhost:7xxx` (port affiché dans le terminal).

---

## Guide d'utilisation

### 1. Créer un compte Admin

```bash
POST /api/auth/register
{
  "username": "admin1",
  "password": "Admin@123",
  "role": "Admin"
}
```

### 2. Créer un compte User (lecture seule)

```bash
POST /api/auth/register
{
  "username": "user1",
  "password": "User@123",
  "role": "User"
}
```

### 3. Se connecter et récupérer le token JWT

```bash
POST /api/auth/login
{
  "username": "admin1",
  "password": "Admin@123"
}
```

Réponse :
```json
{
  "token": "eyJhbGci...",
  "username": "admin1",
  "role": "Admin"
}
```

### 4. Utiliser l'API avec le token

Ajouter le header dans chaque requête :
```
Authorization: Bearer eyJhbGci...
```

### Tester avec Swagger UI

1. Ouvrir http://localhost:8080
2. `POST /api/auth/register` → créer un compte
3. `POST /api/auth/login` → récupérer le token
4. Cliquer **Authorize** → entrer `Bearer <token>`
5. Tester les endpoints

---

## Structure du projet

```
TodoApi/
├── Controllers/
│   ├── AuthController.cs        # Register + Login (JWT)
│   └── TodoItemsController.cs   # CRUD avec rôles (code du tutoriel + sécurité)
├── Models/
│   ├── TodoItem.cs              # Modèle EF (avec champ Secret)
│   ├── TodoItemDTO.cs           # DTO public (sans Secret = anti over-posting)
│   ├── TodoContext.cs           # DbContext EF InMemory (tutoriel)
│   ├── AppDbContext.cs          # DbContext utilisateurs
│   └── AppUser.cs               # Modèles d'authentification
├── Program.cs                   # Configuration (code tutoriel + JWT)
├── appsettings.json
├── Dockerfile
├── docker-compose.yml
└── .gitlab-ci.yml
```

---

## Déploiement GitLab CI/CD

Le fichier `.gitlab-ci.yml` automatise :
1. **Build** du projet .NET
2. **Build & push** de l'image Docker sur GitLab Container Registry
3. **Déploiement** sur serveur SSH (déclenchement manuel)

Variables GitLab à configurer (`Settings > CI/CD > Variables`) :
| Variable | Description |
|---|---|
| `JWT_SECRET` | Clé secrète JWT de production |
| `SSH_PRIVATE_KEY` | Clé SSH pour le déploiement |
| `DEPLOY_HOST` | Adresse IP du serveur |
| `DEPLOY_USER` | Utilisateur SSH |

---

*TP .NET — ESP UCAD — Année 2025/2026*

# RecipeNestAPI

A RESTful Web API for a recipe management platform, built with **C# / ASP.NET Core 8**, **Entity Framework Core**, and **SQLite**. The API powers a React single-page frontend ([recipenest-frontend](https://github.com/Edwinbio16/recipenest-frontend)) and was built as a full-stack portfolio project.

---

## Features

- **RESTful CRUD API** for Chefs and Recipes (GET, POST, PUT, DELETE)
- **Authentication** with BCrypt password hashing and constant-time verification
- **Data Transfer Objects (DTOs)** to keep password hashes out of API responses
- **Relational data model** with a one-to-many Chef → Recipe relationship and cascade delete
- **Unique email constraint** to prevent duplicate accounts
- **Validation attributes** on all incoming data (StringLength, EmailAddress, Url, Required)
- **CORS** configured for a React frontend running on `localhost:3000`
- **Seeded demo data**: 3 chefs and 5 recipes, generated on first run
- **xUnit test project** with 42 tests covering controllers and the auth flow

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 8) |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | SQLite (file-based) |
| Auth | BCrypt.Net-Next (password hashing) |
| Testing | xUnit + EF Core InMemory provider |

---

## Project Structure

```
RecipeNestAPI/
├── Controllers/
│   ├── AuthController.cs        # POST /api/auth/login
│   ├── ChefsController.cs       # CRUD /api/chefs
│   └── RecipesController.cs     # CRUD /api/recipes
├── Data/
│   └── AppDbContext.cs          # EF Core DbContext + seed data
├── Models/
│   ├── Chef.cs                  # Chef entity with validation
│   ├── Recipe.cs                # Recipe entity
│   └── Dtos/
│       ├── ChefDto.cs           # Chef without password hash
│       ├── ChefUpdateDto.cs     # Update payload
│       └── LoginDto.cs          # Login request body
├── Properties/
│   └── launchSettings.json
├── Program.cs                   # Entry point, DI, CORS, pipeline
├── RecipeNestAPI.csproj
└── appsettings.json

RecipeNestAPI.Tests/
├── AuthControllerTests.cs       # 9 tests
├── ChefsControllerTests.cs      # 19 tests
├── RecipesControllerTests.cs    # 14 tests
└── TestDbContextFactory.cs      # InMemory test database setup
```

---

## API Endpoints

### Auth
| Method | Route | Description |
|---|---|---|
| POST | `/api/auth/login` | Authenticate a chef by email and password |

### Chefs
| Method | Route | Description |
|---|---|---|
| GET | `/api/chefs` | List all chefs |
| GET | `/api/chefs/{id}` | Get one chef by ID |
| POST | `/api/chefs` | Create a chef (password is hashed before saving) |
| PUT | `/api/chefs/{id}` | Update a chef |
| DELETE | `/api/chefs/{id}` | Delete a chef (cascades to their recipes) |

### Recipes
| Method | Route | Description |
|---|---|---|
| GET | `/api/recipes` | List all recipes |
| GET | `/api/recipes/{id}` | Get one recipe by ID |
| POST | `/api/recipes` | Create a recipe |
| PUT | `/api/recipes/{id}` | Update a recipe |
| DELETE | `/api/recipes/{id}` | Delete a recipe |

---

## Running Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Steps

```bash
# Clone the repo
git clone https://github.com/Edwinbio16/RecipeNestAPI.git
cd RecipeNestAPI/RecipeNestAPI

# Restore dependencies
dotnet restore

# Run the API
dotnet run
```

The API starts on `http://localhost:5236` by default. The SQLite database (`RecipeNest.db`) is created automatically on first run and seeded with demo data.

### Demo Credentials

The seeded chefs can be used to log in via `POST /api/auth/login`:

| Email | Password |
|---|---|
| marco@recipenest.com | marco_pass_2026 |
| amina@recipenest.com | amina_pass_2026 |
| hiro@recipenest.com | hiro_pass_2026 |

---

## Running the Tests

```bash
cd RecipeNestAPI.Tests
dotnet test
```

The test project uses EF Core's InMemory provider so no real database is required. Each test gets a fresh isolated context.

---

## Design Notes

- **Password storage**: Plaintext passwords are never stored. The `Password` column on `Chef` holds a BCrypt hash (cost factor 11). Login verifies via `BCrypt.Verify`, which performs a constant-time comparison.
- **Account enumeration**: The login endpoint returns the same generic 401 response whether the email is unknown or the password is wrong, so attackers can't enumerate valid accounts.
- **DTOs**: API responses use `ChefDto` to strip out the password hash. The hash never leaves the server.
- **Database initialisation**: `EnsureCreated()` is used instead of EF migrations so the project runs out of the box without extra tooling. Deleting `RecipeNest.db` resets the database.
- **CORS**: Locked to `http://localhost:3000` for development — would be tightened for production.

---

## Related Repositories

- **[recipenest-frontend](https://github.com/Edwinbio16/recipenest-frontend)** — React single-page frontend that consumes this API

---

## Author

Built by **Edwin Owusu** as part of an Information Technology BSc at the University of Bedfordshire.

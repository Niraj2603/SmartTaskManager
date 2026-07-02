# Smart Task Management System

Full-stack task manager with automatic priority scoring and rule-based productivity recommendations.

Stack: React (Vite) + ASP.NET Core 8 Web API + MySQL (EF Core / Pomelo) + JWT auth.

## Folder structure
```
SmartTaskManager/
  backend/SmartTaskManager.API/   ASP.NET Core Web API
  frontend/                       React app
```

## 1. Backend setup

Prerequisites: .NET 8 SDK, MySQL Server running locally, `dotnet-ef` CLI tool.

```bash
cd backend/SmartTaskManager.API

# Restore packages
dotnet restore

# Install the EF CLI tool if you don't have it
dotnet tool install --global dotnet-ef

# Edit appsettings.json -> ConnectionStrings:DefaultConnection
# Set your MySQL username/password and create a database named smarttaskdb (or change the name there)

# Create the initial migration
dotnet ef migrations add InitialCreate

# Run the API (Program.cs auto-applies migrations on startup)
dotnet run
```

The API runs at `http://localhost:5000` with Swagger UI at `http://localhost:5000/swagger`.

### Key backend pieces
- `Services/PriorityService.cs` — implements the scoring formula `(Importance x 50) + (Urgency x 30) + (PendingDays x 20)`, each factor normalized 0-1 before weighting.
- `Services/RecommendationService.cs` — rule-based recommendation engine (overdue tasks, top-priority task, high-priority backlog, quick wins, due-today, stalled tasks).
- `Controllers/TasksController.cs` — CRUD + `/api/tasks/dashboard` for stats + search/filter query params (`search`, `status`, `priority`, `categoryId`, `dueDate`).
- JWT auth via `Controllers/AuthController.cs`, tokens issued on register/login, validated in `Program.cs`.

## 2. Frontend setup

Prerequisites: Node.js 18+.

```bash
cd frontend
npm install
npm run dev
```

App runs at `http://localhost:5173`. It expects the API at `http://localhost:5000/api` (see `src/api/axios.js` — change `baseURL` if your API runs elsewhere).

### Pages
- `/login`, `/register` — auth
- `/dashboard` — stat cards, pie/bar charts (recharts), recommendations list
- `/tasks` — search + filter (status/priority/category) task list, inline status change, edit/delete
- `/tasks/new`, `/tasks/:id/edit` — task form
- `/profile` — update name / password

## Notes
- Notifications (due-soon/overdue reminders) were intentionally left out of this build per scope.
- Default categories (Personal, Study, Work, Health) are seeded via EF Core `HasData`.
- Passwords are hashed with BCrypt; never stored in plain text.
- CORS is pre-configured for `http://localhost:5173` and `:3000`.

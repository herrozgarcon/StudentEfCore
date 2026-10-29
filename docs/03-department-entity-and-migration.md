# 03 — Department Entity and Initial Migration

## 1. Objective

This phase introduces the `Department` entity, configures its database mapping using EF Core Fluent API, and creates the initial database migration.

**Technology stack**
- .NET 10 Console Application
- Entity Framework Core 10
- SQL Server Express
- Code First approach

## 2. Department Model

**File:** `Models/Department.cs`

```csharp
namespace StudentCourseEFCore.Models;

public class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;
}
```

### Property explanations

| Property | Purpose |
|---|---|
| `DepartmentId` | Unique identifier for each department |
| `DepartmentName` | Stores the department name |
| `Location` | Stores the department location; optional |
| `IsActive` | Indicates whether the department is active; defaults to `true` in the model |

## 3. Configure the Entity Using Fluent API

**File:** `Data/AppDbContext.cs`

The `OnModelCreating()` method defines how EF Core maps the `Department` entity to the database.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Department>(entity =>
    {
        entity.HasKey(d => d.DepartmentId);

        entity.Property(d => d.DepartmentName)
            .IsRequired()
            .HasMaxLength(100);

        entity.HasIndex(d => d.DepartmentName)
            .IsUnique();

        entity.Property(d => d.Location)
            .HasMaxLength(100);

        entity.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    });
}
```

### Configuration explained

- `HasKey()` configures the primary key.
- `IsRequired()` makes the database column non-nullable.
- `HasMaxLength(100)` limits the column length to 100 characters.
- `HasIndex().IsUnique()` creates a unique index on `DepartmentName`.
- `HasDefaultValue(true)` configures the database default for `IsActive`.

**Important:** The unique index prevents multiple departments from having the same name. SQL Server enforces this constraint.

## 4. Configure Database Connectivity

The application uses SQL Server Express.

The connection is configured in `AppDbContext.cs` using `OnConfiguring()`:

```csharp
protected override void OnConfiguring(
    DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=StudentCourseEFCoreDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
```

The `IsConfigured` check allows the context to use externally supplied options later, when dependency injection is introduced.

> This connection configuration is suitable for the current learning setup. Production applications should use appropriate configuration and secret-management practices.

## 5. What Is a Migration?

A migration records changes to the database schema based on the EF Core model configuration.

The Code First workflow is:

1. Define the entity.
2. Configure the entity using Fluent API.
3. Generate a migration.
4. Apply the migration to the database.
5. Verify the resulting schema.

## 6. Create the Initial Migration

Run these commands in the project directory.

### Generate the migration

```bash
dotnet ef migrations add InitialCreate
```

This creates migration files under the `Migrations` folder.

### Apply the migration

```bash
dotnet ef database update
```

EF Core applies the migration to `StudentCourseEFCoreDB`.

## 7. Expected Database Structure

The `Departments` table contains the following columns:

| Column | Expected configuration |
|---|---|
| `DepartmentId` | Integer primary key |
| `DepartmentName` | Required, maximum 100 characters, unique index |
| `Location` | Nullable, maximum 100 characters |
| `IsActive` | Required, database default of `true` |

## 8. Verify the Migration

Run:

```bash
dotnet ef migrations list
```

Confirm that `InitialCreate` appears in the migration list.

You can also inspect the `Departments` table in SQL Server Management Studio.

## 9. Git Workflow

Review the changes:

```bash
git status
git diff
```

Stage the relevant files:

```bash
git add Models/Department.cs Data/AppDbContext.cs Migrations
```

Commit the implementation:

```bash
git commit -m "feat: configure department entity and initial migration"
```

Push the commit:

```bash
git push origin main
```

*These Git commands document the original implementation workflow. Do not repeat the commit if it has already been completed.*

## 10. Key Takeaways

- Entities represent the data EF Core works with.
- Fluent API provides explicit control over database mapping.
- Migrations track schema changes.
- `dotnet ef database update` applies migrations.
- Unique indexes enforce uniqueness at the database level.
- The current project uses Code First with SQL Server Express.

**Next phase:** Implement Department CRUD operations and document validation and error handling.
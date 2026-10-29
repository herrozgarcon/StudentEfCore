# 04 — Department CRUD and Validation

## 1. Objective

In this phase, we implement complete CRUD operations for the Department entity using Entity Framework Core in a .NET Console Application.

We also introduce input validation and database exception handling to make the application more reliable.

### Technologies Used
- .NET 10 Console Application
- Entity Framework Core 10
- SQL Server Express
- C#
- Asynchronous programming

---

## 2. Project Structure

```text
StudentCourseEFCore/
├── Data/
│   └── AppDbContext.cs
├── Models/
│   └── Department.cs
├── Repositories/
│   └── DepartmentRepository.cs
├── Migrations/
├── docs/
│   ├── 02-dbcontext.md
│   ├── 03-department-entity-and-migration.md
│   └── 04-department-crud-and-validation.md
└── Program.cs
```

### Responsibilities

| Component | Responsibility |
|---|---|
| `Department.cs` | Represents department data |
| `AppDbContext.cs` | Connects EF Core entities to the database |
| `DepartmentRepository.cs` | Implements department database operations |
| `Program.cs` | Handles user input and menu navigation |
| `Migrations/` | Stores database schema changes |

---

## 3. Understanding the Repository Pattern

The repository separates database operations from the application's user interaction logic.

Instead of writing EF Core queries directly inside `Program.cs`, we place department-related database operations inside `DepartmentRepository.cs`.

For example:

```csharp
var repository = new DepartmentRepository();

int id = await repository.AddAsync(department);
```

Here, `Program.cs` requests an operation from the repository without needing to know how the database insert is implemented.

### Benefits

- Separates responsibilities.
- Keeps `Program.cs` easier to understand.
- Groups related database operations in one class.
- Makes database operations reusable.

**Current implementation:** The repository creates an `AppDbContext` when each operation is called. Dependency injection will be introduced in a later phase.

---

## 4. Implementing Department CRUD Operations

CRUD stands for:

- **Create** — insert a department.
- **Read** — retrieve department information.
- **Update** — modify department details.
- **Delete** — remove a department.

### 4.1 Create — Add a Department

Repository method:

```csharp
public async Task<int> AddAsync(Department department)
{
    await using var db = new AppDbContext();

    db.Departments.Add(department);

    await db.SaveChangesAsync();

    return department.DepartmentId;
}
```

#### Explanation

1. Create an `AppDbContext`.
2. Add the department to the context.
3. Call `SaveChangesAsync()` to persist the insert.
4. Return the generated department ID.

**Important:** `Add()` marks the entity for insertion. The database operation is performed when `SaveChangesAsync()` executes.

### 4.2 Read — Retrieve All Departments

```csharp
public async Task<List<Department>> GetAllAsync()
{
    await using var db = new AppDbContext();

    return await db.Departments
        .OrderBy(d => d.DepartmentId)
        .ToListAsync();
}
```

#### Explanation

- `db.Departments` accesses the department collection.
- `OrderBy()` sorts departments by ID.
- `ToListAsync()` executes the query and returns the results.

The console application displays a message when no departments are found.

### 4.3 Read — Retrieve a Department by ID

```csharp
public async Task<Department?> GetByIdAsync(int id)
{
    await using var db = new AppDbContext();

    return await db.Departments.FindAsync(id);
}
```

`FindAsync()` searches for an entity using its primary key.

If the department does not exist, the method returns `null`.

### 4.4 Update — Modify a Department

```csharp
public async Task<bool> UpdateAsync(
    int id,
    string departmentName,
    string? location)
{
    await using var db = new AppDbContext();

    var department = await db.Departments.FindAsync(id);

    if (department is null)
        return false;

    department.DepartmentName = departmentName;
    department.Location = location;

    await db.SaveChangesAsync();

    return true;
}
```

#### Explanation

1. Retrieve the department using its ID.
2. Return `false` if it does not exist.
3. Modify the department's properties.
4. Call `SaveChangesAsync()` to persist the changes.
5. Return `true` when the update succeeds.

EF Core tracks the retrieved entity. It detects property changes and generates the required SQL `UPDATE` statement when changes are saved.

### 4.5 Delete — Remove a Department

```csharp
public async Task<bool> DeleteAsync(int id)
{
    await using var db = new AppDbContext();

    var department = await db.Departments.FindAsync(id);

    if (department is null)
        return false;

    db.Departments.Remove(department);

    await db.SaveChangesAsync();

    return true;
}
```

#### Explanation

- `FindAsync()` retrieves the department.
- `Remove()` marks the entity for deletion.
- `SaveChangesAsync()` sends the deletion to the database.
- The method returns `false` if the department does not exist.

The console application also requests confirmation before deleting a department.

---

## 5. Understanding Asynchronous Database Operations

The repository methods use `async` and `await`.

Examples include:

```csharp
await db.SaveChangesAsync();
```

```csharp
await db.Departments.ToListAsync();
```

### Why use asynchronous operations?

Database operations can involve waiting for SQL Server to execute a query or save changes.

Asynchronous methods allow the application to await those operations without synchronously blocking the executing thread.

Common EF Core asynchronous methods include:

| Method | Purpose |
|---|---|
| `ToListAsync()` | Retrieves query results as a list |
| `FindAsync()` | Retrieves an entity by primary key |
| `SaveChangesAsync()` | Saves tracked changes to the database |
| `AnyAsync()` | Checks whether matching records exist |
| `FirstOrDefaultAsync()` | Retrieves the first matching result or a default value |

These methods are useful throughout real-world .NET backend development.

---

## 6. Input Validation

Input validation prevents invalid user input from proceeding to database operations.

The console application validates the menu choice and department IDs using `int.TryParse()`.

Example:

```csharp
if (!int.TryParse(input, out int choice))
{
    Console.WriteLine("Invalid input. Enter a number.");
    continue;
}
```

### Required Department Name

A reusable helper method validates the department name:

```csharp
static string ReadRequiredInput(string message)
{
    while (true)
    {
        Console.Write(message);

        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine(
            "Input cannot be empty. Please try again.");
    }
}
```

### Concepts Used

- `string.IsNullOrWhiteSpace()` checks whether input is empty or contains only whitespace.
- `Trim()` removes leading and trailing whitespace.
- `while (true)` repeats the prompt until valid input is provided.
- A reusable method avoids duplicating the same validation logic.

### Optional Location

The department location is optional:

```csharp
Location = string.IsNullOrWhiteSpace(location)
    ? null
    : location.Trim()
```

If the user enters nothing or only whitespace, the location is stored as `null`.

Otherwise, the trimmed input is assigned to the property.

---

## 7. Database Constraints and Duplicate Names

The Department entity is configured with a unique index on `DepartmentName`.

```csharp
entity.HasIndex(d => d.DepartmentName)
    .IsUnique();
```

This configuration prevents SQL Server from accepting duplicate department names.

For example, if a department named `Computer Science` already exists, another department with that name cannot be inserted.

### Why use a database constraint?

Application-level validation alone is insufficient to guarantee uniqueness. A database constraint enforces the rule at the database level, including when other applications attempt to insert duplicate values.

---

## 8. Handling Database Exceptions

EF Core can throw `DbUpdateException` when a database update fails.

For duplicate department names, the repository examines the underlying SQL Server exception.

```csharp
catch (DbUpdateException ex)
    when (ex.InnerException is SqlException sqlEx
          && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
{
    throw new InvalidOperationException(
        "A department with this name already exists.", ex);
}
```

### Explanation

- `DbUpdateException` represents an error while saving database changes.
- `InnerException` can contain the underlying SQL Server error.
- SQL Server error `2601` is associated with duplicate keys in a unique index.
- SQL Server error `2627` is associated with a violation of a unique constraint or primary key.
- `InvalidOperationException` carries a friendlier message to the calling code.

The console application catches the exception:

```csharp
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
}
catch (DbUpdateException)
{
    Console.WriteLine(
        "A database error occurred while adding the department.");
}
```

This allows the application to display a message and return to the menu instead of terminating because of the handled error.

The same duplicate-name handling is implemented for department updates.

**Note:** These examples handle the expected duplicate-name errors. Other database failures may still occur, and a production application should handle anticipated failure scenarios appropriately.

---

## 9. Testing Scenarios

Test each operation using the console menu.

| Operation | Test case | Expected result |
|---|---|---|
| Add | Enter a valid, unique department name | Department is added |
| Add | Enter an empty department name | Application requests valid input |
| Add | Enter an existing department name | Duplicate-name message |
| View All | Retrieve departments | Displays available departments |
| View All | Database contains no departments | Displays an appropriate message |
| View By ID | Enter an existing ID | Displays department details |
| View By ID | Enter a nonexistent ID | Displays `Department not found` |
| Update | Change a department to a unique name | Update succeeds |
| Update | Change a department to an existing name | Duplicate-name message |
| Delete | Delete an existing department | Requests confirmation |
| Delete | Cancel deletion | Displays a cancellation message |
| Delete | Enter a nonexistent ID | Displays `Department not found` |

---

## 10. Git Workflow

After implementing and testing the feature, review the changes.

```bash
dotnet build
git status
git diff
```

Stage the modified files:

```bash
git add Program.cs Repositories/DepartmentRepository.cs
```

Commit the feature:

```bash
git commit -m "feat: add department validation and error handling"
```

Push the commit:

```bash
git push origin main
```

The commit records the completed Department validation and error-handling feature.

Documentation is committed separately:

```bash
git add docs/04-department-crud-and-validation.md
git commit -m "docs: document department CRUD and validation"
git push origin main
```

If either commit has already been created, do not repeat it. Only commit changes that remain uncommitted.

---

## 11. Key Takeaways

After completing this phase, you should understand:

- How to implement CRUD operations with EF Core.
- Why database operations belong in a repository in this project.
- How `Add()`, `FindAsync()`, `Remove()`, and `SaveChangesAsync()` work together.
- How asynchronous database operations are used.
- How to validate console input.
- How unique indexes enforce database constraints.
- How to handle duplicate-key errors.
- How to test and commit a feature using Git.

## 12. Next Phase: Student–Department Relationship

The next phase introduces the `Student` entity and its relationship with `Department`.

Planned topics:

1. Create the `Student` model.
2. Understand primary keys and foreign keys.
3. Learn navigation properties.
4. Configure the one-to-many relationship using Fluent API.
5. Add the `Students` DbSet to `AppDbContext`.
6. Create and apply a migration.
7. Implement Student CRUD operations.
8. Retrieve students together with department details.

**Goal:** Extend the existing application while preserving the working Department functionality.
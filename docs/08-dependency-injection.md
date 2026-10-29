# Phase 8: Dependency Injection and Application Refactoring

## 1. Objective

Refactor the Student Course Management console application to use the built-in .NET Dependency Injection (DI) container.

The objective is to remove manual object creation from `Program.cs` and allow the DI container to create and connect the application's dependencies.

## 2. Previous Approach: Manual Dependency Creation

Before implementing DI, dependencies were created manually in `Program.cs`.

```csharp
var studentRepository = new StudentRepository();

var studentService =
    new StudentService(studentRepository);

var studentMenu =
    new StudentMenu(studentService);
```

In this approach, `Program.cs` is responsible for creating each object and connecting its dependencies.

As the application grows, this can make the entry point more difficult to maintain.

## 3. Dependency Injection

Dependency Injection is a technique in which a class receives the objects it depends on rather than creating those objects itself.

For example, `StudentService` requires `StudentRepository`.

```csharp
public class StudentService
{
    private readonly StudentRepository _repository;

    public StudentService(StudentRepository repository)
    {
        _repository = repository;
    }
}
```

The repository is supplied through the constructor. This is called **constructor injection**.

### Benefits

- Reduces manual object creation.
- Separates object creation from business logic.
- Improves maintainability.
- Supports unit testing.
- Makes dependencies explicit.

## 4. Registering AppDbContext

The application registers `AppDbContext` with the DI container.

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        "Server=.\\SQLEXPRESS;Database=StudentCourseEFCoreDB;Trusted_Connection=True;TrustServerCertificate=True;"));
```

### Explanation

- `AddDbContext<AppDbContext>()` registers the EF Core context.
- `UseSqlServer()` configures SQL Server as the database provider.
- The connection string identifies the SQL Server instance and database.

`AddDbContext()` registers the context with a scoped lifetime by default.

The existing `DbSet` properties and Fluent API configurations remain in `AppDbContext`.

The connection string is hardcoded in this implementation for learning purposes. A future improvement is to move it into configuration, such as `appsettings.json`.

## 5. Registering Repositories

The repositories are registered with the DI container.

```csharp
builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<EnrollmentRepository>();
```

These registrations allow the DI container to create the repository instances and provide their constructor dependencies.

### What is AddScoped()?

`AddScoped()` creates one instance of a registered service per dependency-injection scope.

This lifetime is appropriate for many EF Core applications because `DbContext` is also scoped by default.

When repositories are resolved within the same scope, they can share the same `AppDbContext` instance through constructor injection.

## 6. Registering Services

The application registers its service classes.

```csharp
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();
```

Services coordinate application operations and use repositories to perform database operations.

For example, the dependency chain for student management is:

```text
StudentMenu
    |
    v
StudentService
    |
    v
StudentRepository
    |
    v
AppDbContext
    |
    v
SQL Server
```

The DI container constructs this dependency chain based on the registered services and their constructors.

## 7. Registering Menus

The menu classes are also registered.

```csharp
builder.Services.AddScoped<DepartmentMenu>();
builder.Services.AddScoped<StudentMenu>();
builder.Services.AddScoped<CourseMenu>();
builder.Services.AddScoped<EnrollmentMenu>();
```

For example:

```csharp
public class DepartmentMenu
{
    private readonly DepartmentService _service;

    public DepartmentMenu(DepartmentService service)
    {
        _service = service;
    }
}
```

The menu receives `DepartmentService` through its constructor instead of creating it manually.

The existing menu methods and console interactions remain unchanged.

## 8. Configuring the Generic Host

The application uses the .NET Generic Host to configure and build the DI container.

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Register application services.

using var host = builder.Build();
```

The host manages the application's registered services and exposes them through `host.Services`.

A scope can be created to resolve the application dependencies.

```csharp
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

var departmentMenu =
    services.GetRequiredService<DepartmentMenu>();
```

`GetRequiredService<T>()` retrieves the requested registered service. It throws an exception if the service cannot be resolved.

## 9. Refactoring Program.cs

The updated `Program.cs` performs the following operations:

1. Creates the application builder.
2. Registers `AppDbContext`.
3. Registers repositories.
4. Registers services.
5. Registers menu classes.
6. Builds the host.
7. Creates a service scope.
8. Resolves the menu instances.
9. Runs the existing console menu loop.

This reduces manual object creation and keeps the application's entry point focused on application startup and navigation.

## 10. Application Architecture

The application follows this structure:

```text
Program.cs
    |
    v
.NET DI Container
    |
    v
Menus
    |
    v
Services
    |
    v
Repositories
    |
    v
AppDbContext
    |
    v
SQL Server
```

### Layer Responsibilities

| Layer | Responsibility |
|---|---|
| Menus | Handle console input and output |
| Services | Coordinate application operations and business logic |
| Repositories | Perform database operations |
| AppDbContext | Manage EF Core database access and entity tracking |
| SQL Server | Store application data |
| DI Container | Create objects and supply their dependencies |

## 11. Testing and Verification

After implementing Dependency Injection, the application was built and run successfully.

The existing modules were available through the console application:

- Department Management
- Student Management
- Course Management
- Enrollment Management

The application was tested by running it successfully.

Existing entities, relationships, Fluent API configurations, and database migrations were retained.

## 12. Key Learnings

The following concepts were implemented or practiced:

- Dependency Injection
- Constructor Injection
- Service Registration
- `AddDbContext()`
- `AddScoped()`
- `GetRequiredService<T>()`
- Generic Host configuration
- Dependency resolution
- Application refactoring
- EF Core integration with DI

## 13. Future Improvements

The current implementation registers concrete classes directly.

For example:

```csharp
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<StudentService>();
```

A future improvement is to introduce interfaces such as `IStudentRepository` and `IStudentService` and register their implementations.

The connection string can also be moved into configuration.

These improvements are not required for the current phase.

## 14. Phase Completion Checklist

- [x] Register `AppDbContext` through DI.
- [x] Register repositories.
- [x] Register services.
- [x] Register menus.
- [x] Use constructor injection.
- [x] Refactor `Program.cs`.
- [x] Build and run the application successfully.
- [ ] Save the phase documentation.
- [ ] Commit and push the changes to GitHub.

**Phase 8 status:** Implementation and runtime verification completed. Documentation and Git commit/push must be completed to close the phase.
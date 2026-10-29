# EF Core — DbContext

## 1. What is DbContext?
`DbContext` is the primary class EF Core uses to coordinate interactions between application entities and the database.

It supports querying, inserting, updating, deleting, and tracking entities.

## 2. What is DbSet<T>?
`DbSet<T>` represents a collection of entities that can be queried and managed through a DbContext.

Example:
```csharp
public DbSet<Department> Departments { get; set; }
```

## 3. Why inherit from DbContext?
Our AppDbContext inherits from DbContext to use EF Core's database interaction and change-tracking functionality.

## 4. Why use DbContextOptions?
```csharp
public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
{
}
```

The options configure the context, including its database provider and connection settings.

## 5. Does DbSet create a database table?
No. Declaring a DbSet does not immediately create a SQL Server table. We will use EF Core migrations to create the database schema.

## 6. C# Accessibility Rule
A public property cannot expose a type that is less accessible than the property.

Our Department entity must therefore be declared as:

public class Department
{

}


## 7. Files created
- Models/Department.cs
- Data/AppDbContext.cs
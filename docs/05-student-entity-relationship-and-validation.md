# Phase 5: Student Entity, Department Relationship and Validation

## 1. Objective

The objective of this phase is to integrate the Student entity into the Student Course Management System, establish its relationship with Department, implement Student CRUD operations, and enforce database-level validation constraints using Entity Framework Core.

### Topics covered
- Student entity configuration
- One-to-many relationship between Department and Student
- Fluent API configuration
- Student CRUD operations using a repository
- Database validation constraints
- EF Core migrations
- Testing and verification

## 2. Student Entity

The `Student` entity represents students registered in the system.

### Properties

| Property | Type | Description |
|---|---|---|
| StudentId | int | Primary key; generated automatically |
| Name | string | Required; maximum 100 characters |
| Email | string | Required; maximum 450 characters; unique |
| Age | int | Student's age; must be between 17 and 60 |
| DepartmentId | int | Foreign key referencing Department |
| Department | Department? | Navigation property for the associated department |

The `DepartmentId` property connects each student to a department.

## 3. Student–Department Relationship

The application implements a **one-to-many relationship**.

- One department can contain multiple students.
- Each student belongs to one department.
- `DepartmentId` is the foreign key in the Students table.
- The `Department` navigation property provides access to the associated department.
- The `Students` collection in Department provides access to its related students.

### Fluent API configuration

```csharp
modelBuilder.Entity<Department>()
    .HasMany(d => d.Students)
    .WithOne(s => s.Department)
    .HasForeignKey(s => s.DepartmentId);
```

### Explanation

| Method | Purpose |
|---|---|
| `HasMany()` | A department can have multiple students |
| `WithOne()` | Each student has one associated department |
| `HasForeignKey()` | Specifies the foreign key property |

The relationship is required because `DepartmentId` is a non-nullable integer.

The configured delete behavior is cascade. Deleting a department therefore also deletes its related students unless this behavior is changed.

## 4. Fluent API Configuration

Entity configuration is maintained in `Data/AppDbContext.cs`.

### Student configuration

```csharp
modelBuilder.Entity<Student>(entity =>
{
    entity.HasKey(s => s.StudentId);

    entity.Property(s => s.Name)
        .IsRequired()
        .HasMaxLength(100);

    entity.Property(s => s.Email)
        .IsRequired()
        .HasMaxLength(450);

    entity.HasIndex(s => s.Email)
        .IsUnique();

    entity.ToTable("Students", table =>
    {
        table.HasCheckConstraint(
            "CK_Students_Age",
            "[Age] BETWEEN 17 AND 60");
    });
});
```

### Configuration summary

| Configuration | Purpose |
|---|---|
| `HasKey()` | Defines the primary key |
| `IsRequired()` | Marks a property as required |
| `HasMaxLength(100)` | Limits the Name column to 100 characters |
| `HasMaxLength(450)` | Limits the Email column to 450 characters |
| `HasIndex().IsUnique()` | Prevents duplicate email addresses |
| `HasCheckConstraint()` | Enforces the age range at database level |
| `ToTable("Students")` | Explicitly specifies the table name |

## 5. Student Repository and CRUD Operations

The `Repositories/StudentRepository.cs` file contains the database operations for Student.

The repository uses `AppDbContext` to communicate with SQL Server through EF Core.

### Operations implemented

| Operation | Description |
|---|---|
| AddAsync | Adds a new student |
| GetAllAsync | Retrieves all students with department information |
| GetByIdAsync | Retrieves a student by ID |
| UpdateAsync | Updates an existing student |
| DeleteAsync | Deletes a student |
| Department validation | Checks whether the specified department exists before updating a student |

### Loading related data

The repository uses:

```csharp
.Include(s => s.Department)
```

The `Include()` method loads the associated Department data along with the Student.

This allows the application to display department information when retrieving students.

## 6. Database Validation Constraints

The following rules are enforced by the database.

### 6.1 Required properties

The Name and Email columns are required.

The DepartmentId column is also required because the Student–Department relationship is mandatory.

### 6.2 Name length

The Name column is configured as:

```sql
nvarchar(100)
```

This limits student names to 100 characters.

### 6.3 Email length

The Email column is configured as:

```sql
nvarchar(450)
```

This limits email addresses to 450 characters.

### 6.4 Unique email index

The unique index is named:

`IX_Students_Email`

It prevents multiple students from being registered with the same email address.

### 6.5 Age check constraint

The database constraint is named:

`CK_Students_Age`

Its SQL definition is:

```sql
[Age] BETWEEN 17 AND 60
```

SQL Server rejects insert or update operations that attempt to store an age outside the permitted range.

**Important:** Database constraints protect data integrity. The application should also handle database exceptions appropriately and display meaningful error messages.

## 7. EF Core Migration

The migration created for this phase is:

`AddStudentValidationConstraints`

### Changes applied

1. Changed Name from `nvarchar(max)` to `nvarchar(100)`.
2. Changed Email from `nvarchar(max)` to `nvarchar(450)`.
3. Created the unique index on Email.
4. Added the age check constraint.

### Create the migration

```powershell
dotnet ef migrations add AddStudentValidationConstraints
```

### Apply the migration

```powershell
dotnet ef database update
```

### Migration issue and resolution

During development, the Students table was renamed manually in SQL Server, while the original EF Core migration still referred to it as `Student`.

This caused EF Core to generate migration operations that attempted to rename the table again.

The migration was inspected and corrected to target the existing `Students` table without renaming it or unnecessarily dropping its primary key and Department foreign key.

This experience highlights the importance of keeping the following consistent:

- Current entity model
- EF Core model snapshot
- Migration operations
- Actual database schema

Always inspect generated migrations before applying them, especially when database changes have been made manually.

## 8. Testing and Verification

The following functionality and database rules were tested during this phase.

| Test | Expected result |
|---|---|
| Register a student with valid information | Student is saved successfully |
| Retrieve all students | Student records are returned |
| Retrieve a student by ID | Matching student is returned |
| Update a student | Student information is updated |
| Delete a student | Student is removed |
| Retrieve students with department information | Associated department information is available |
| Register a duplicate email | Database rejects the duplicate |
| Register a student younger than 17 | Database rejects the operation |
| Register a student older than 60 | Database rejects the operation |

The database schema was also verified to confirm the column lengths, unique email index, and age constraint.

## 9. Key Learnings

After completing this phase, the following concepts have been covered:

- One-to-many relationships in EF Core
- Foreign keys and navigation properties
- Fluent API configuration
- Required properties and maximum column lengths
- Unique indexes
- Database check constraints
- Loading related entities with `Include()`
- Repository-based CRUD operations
- EF Core migration generation and application
- Model snapshot consistency
- Database-level data integrity

## 10. Phase Completion

**Phase 5: Student Entity, Department Relationship and Validation — Completed**

The Student entity is integrated with Department, Student CRUD operations are working, and the database validation constraints have been applied and verified.

### Files involved

- `Models/Student.cs`
- `Models/Department.cs`
- `Data/AppDbContext.cs`
- `Repositories/StudentRepository.cs`
- `Program.cs`
- `Migrations/`
- `docs/05-student-entity-relationship-and-validation.md`

### Next phase

**Phase 6: Course Management**

The next phase will introduce the Course entity and its CRUD operations, following the same workflow:

**Concept → Model → Fluent API → Migration → Repository → Testing → Documentation → Git commit**
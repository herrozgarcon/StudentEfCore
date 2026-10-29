# Phase 7: Enrollment Management Using EF Core

## 1. Objective

The objective of this phase is to implement enrollment management in the Student Course Management console application using Entity Framework Core.

This phase introduces a many-to-many relationship between Students and Courses through an Enrollment entity.

By completing this phase, we learn how to:
- Model many-to-many relationships using an explicit entity.
- Configure foreign keys and relationships using Fluent API.
- Create and apply EF Core migrations.
- Implement enrollment operations using the Repository and Service layers.
- Integrate enrollment functionality into the console application.

## 2. Project Requirements

The Enrollment module supports the following operations:

1. Enroll a student in a course.
2. Retrieve all enrollments.
3. Retrieve enrollments by Student ID.
4. Retrieve an enrollment by Enrollment ID.
5. Cancel an enrollment.

Each enrollment has its own unique Enrollment ID.

## 3. Understanding the Many-to-Many Relationship

A student can enroll in multiple courses.

A course can have multiple students enrolled in it.

This is a many-to-many relationship.

Instead of directly configuring a many-to-many relationship between Students and Courses, we introduce an explicit Enrollment entity.

The relationship becomes:

- One Student can have many Enrollments.
- One Course can have many Enrollments.
- Each Enrollment belongs to one Student and one Course.

### Relationship Diagram

```text
Student
   |
   | One-to-Many
   |
Enrollment
   |
   | Many-to-One
   |
Course
```

The Enrollment entity acts as the linking entity between Students and Courses.

It also stores enrollment-specific information, such as the enrollment date.

## 4. Enrollment Entity

File: `Models/Enrollment.cs`

```csharp
namespace StudentCourseEFCore.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime EnrolledAt { get; set; }

    public Student? Student { get; set; }

    public Course? Course { get; set; }
}
```

### Property Explanation

| Property | Purpose |
|---|---|
| EnrollmentId | Primary key identifying an enrollment |
| StudentId | Foreign key referencing a student |
| CourseId | Foreign key referencing a course |
| EnrolledAt | Date and time when the enrollment was created |
| Student | Navigation property referencing the related student |
| Course | Navigation property referencing the related course |

`EnrollmentId` uniquely identifies each enrollment record.

This allows multiple enrollment records to reference the same student and course when repeated enrollments are permitted.

## 5. Navigation Properties

### Student Model

The Student entity contains a collection of enrollments:

```csharp
public ICollection<Enrollment> Enrollments { get; set; }
    = new List<Enrollment>();
```

This allows one student to be associated with multiple enrollment records.

### Course Model

The Course entity also contains a collection of enrollments:

```csharp
public ICollection<Enrollment> Enrollments { get; set; }
    = new List<Enrollment>();
```

This allows one course to be associated with multiple enrollment records.

Navigation properties allow EF Core to represent and navigate relationships between entities.

## 6. DbContext and Fluent API Configuration

File: `Data/AppDbContext.cs`

Register the Enrollment entity through a DbSet:

```csharp
public DbSet<Enrollment> Enrollments => Set<Enrollment>();
```

Configure the entity inside `OnModelCreating()`:

```csharp
modelBuilder.Entity<Enrollment>(entity =>
{
    entity.ToTable("Enrollments");

    entity.HasKey(e => e.EnrollmentId);

    entity.Property(e => e.EnrollmentId)
        .ValueGeneratedOnAdd();

    entity.Property(e => e.EnrolledAt)
        .IsRequired()
        .HasDefaultValueSql("SYSDATETIME()");

    entity.HasOne(e => e.Student)
        .WithMany(s => s.Enrollments)
        .HasForeignKey(e => e.StudentId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Course)
        .WithMany(c => c.Enrollments)
        .HasForeignKey(e => e.CourseId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(e => e.StudentId);

    entity.HasIndex(e => e.CourseId);
});
```

### Configuration Explanation

**Primary key**

`EnrollmentId` uniquely identifies each enrollment.

**Generated ID**

`ValueGeneratedOnAdd()` configures the ID to be generated when a new enrollment is inserted.

**Database-generated timestamp**

`HasDefaultValueSql("SYSDATETIME()")` configures SQL Server to provide the current server date and time when an enrollment is inserted without an explicit `EnrolledAt` value.

**Student relationship
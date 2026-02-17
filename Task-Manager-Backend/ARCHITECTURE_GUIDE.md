# 3-Tier Architecture Guide - Task Manager API

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    API LAYER (Presentation)                  │
│  ✓ HTTP Request/Response                                     │
│  ✓ JWT Authentication & Authorization                        │
│  ✓ Extract Claims (UserId, RoleId) from JWT Token          │
│  ✓ Route to correct Service                                  │
│  ✓ Return HTTP Status Codes                                  │
│                                                               │
│  Controllers/                                                 │
│  - AuthController.cs                                         │
│  - TaskController.cs                                         │
└───────────────────────────┬───────────────────────────────────┘
                            │ Calls Service
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   BUSINESS LAYER (Logic)                     │
│  ✓ Business Rules & Validation                              │
│  ✓ Data Transformation (DTO ↔ Entity)                       │
│  ✓ Orchestration Logic                                       │
│  ✓ Authorization Logic (e.g., admin can't assign to self)   │
│  ✓ NO Database Calls (use Repository)                       │
│                                                               │
│  Services/                                                    │
│  - AuthService.cs                                            │
│  - TaskService.cs                                            │
│                                                               │
│  Helpers/                                                     │
│  - JwtTokenGenerator.cs                                      │
└───────────────────────────┬───────────────────────────────────┘
                            │ Calls Repository
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    DATA LAYER (Access)                       │
│  ✓ Database Operations ONLY (CRUD)                          │
│  ✓ EF Core Queries                                           │
│  ✓ NO Business Logic                                         │
│  ✓ NO Authorization Checks                                   │
│  ✓ Simple Data Retrieval/Persistence                        │
│                                                               │
│  Repositories/                                                │
│  - AuthRepository.cs                                         │
│  - TaskRepository.cs                                         │
│  - TaskManagerDbContext.cs                                   │
└───────────────────────────┬───────────────────────────────────┘
                            │ Interacts with
                            ↓
                    ┌───────────────┐
                    │   DATABASE    │
                    │  SQL Server   │
                    └───────────────┘
```

---

## 📋 Layer Responsibilities - WHAT GOES WHERE

### **1. API LAYER (Controllers)** - `TaskManager.Api/Controllers/`

#### **Responsibilities:**
- ✅ Handle HTTP requests and responses
- ✅ Apply `[Authorize]` attribute for JWT authentication
- ✅ Extract JWT claims: `UserId`, `RoleId`, `Email` from token
- ✅ Check role-based authorization (Admin vs User)
- ✅ Validate request format (not business rules)
- ✅ Call appropriate Service method
- ✅ Return correct HTTP status code

#### **What GOES in Controller:**
```csharp
// ✅ JWT Token Claims Extraction
var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;

// ✅ Role-Based Authorization Check
if (roleId != (int)eRole.Admin)
{
    return Unauthorized("Only admins can create tasks");
}

// ✅ Basic Request Validation
if (request == null || string.IsNullOrWhiteSpace(request.TaskTitle))
{
    return BadRequest("Invalid request");
}

// ✅ Call Service
var response = await _taskService.CreateTaskAsync(request, userId);

// ✅ Return HTTP Status Code
return StatusCode(response.statusCode, response);
```

#### **What DOES NOT GO in Controller:**
```csharp
// ❌ NO Database calls
var user = await _context.Users.FindAsync(userId); // WRONG!

// ❌ NO Business logic
if (task.AssignedUserId == adminUserId) // WRONG! Business logic
{
    return BadRequest("Cannot assign to self");
}

// ❌ NO Direct Entity Mapping
var task = new Task { /* ... */ }; // WRONG! That's Business Layer

// ❌ NO Password Hashing/Encryption
var hashedPassword = BCrypt.HashPassword(password); // WRONG! Business Layer
```

---

### **2. BUSINESS LAYER (Services)** - `TaskManager.Business/Services/`

#### **Responsibilities:**
- ✅ Implement ALL business rules and validation
- ✅ Transform DTOs to Entities (and vice versa)
- ✅ Orchestrate multiple repository calls if needed
- ✅ Apply business logic (e.g., "admin can't assign task to themselves")
- ✅ Set audit fields (CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)
- ✅ Handle complex workflows

#### **What GOES in Service:**
```csharp
// ✅ Business Rule Validation
if (string.IsNullOrWhiteSpace(dto.TaskTitle))
{
    return ResponseMessage.BadRequest("Task title is required");
}

// ✅ Business Logic
if (dto.AssignedUserId == adminUserId)
{
    return ResponseMessage.BadRequest("Admin cannot assign task to themselves");
}

// ✅ Check if Related Entity Exists (via Repository)
var userResponse = await _userRepository.GetByIdAsync(dto.AssignedUserId);
if (!userResponse.success)
{
    return ResponseMessage.NotFound("User not found");
}

// ✅ DTO to Entity Mapping
var task = new Task
{
    TaskTitle = dto.TaskTitle.Trim(),
    Description = dto.Description?.Trim(),
    UserId = dto.AssignedUserId,
    CreatedOn = DateTime.UtcNow,
    CreatedBy = adminUserId,  // From JWT claim
    IsActive = true
};

// ✅ Call Repository
var response = await _taskRepository.CreateAsync(task);
return response;
```

#### **What DOES NOT GO in Service:**
```csharp
// ❌ NO Direct EF Core Database Calls
var user = await _context.Users.FindAsync(userId); // WRONG! Use Repository

// ❌ NO HTTP Status Code Handling
return StatusCode(200, data); // WRONG! That's Controller's job

// ❌ NO JWT Token Claims Extraction
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // WRONG! Controller does this

// ❌ NO [Authorize] Attributes or HTTP concerns
[Authorize] // WRONG! That's Controller
```

---

### **3. DATA LAYER (Repositories)** - `TaskManager.Data/Repositories/`

#### **Responsibilities:**
- ✅ Perform CRUD operations on database
- ✅ Use Entity Framework Core for queries
- ✅ Return data as-is from database
- ✅ Simple, focused database operations
- ✅ NO business logic or validation

#### **What GOES in Repository:**
```csharp
// ✅ Simple Database Queries
public async Task<ResponseMessage> CreateAsync(Task task)
{
    try
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return ResponseMessage.Created(task, "Task created");
    }
    catch (Exception ex)
    {
        return ResponseMessage.Error(ex.Message);
    }
}

// ✅ EF Core Queries
public async Task<ResponseMessage> GetByIdAsync(int taskId)
{
    var task = await _context.Tasks
        .FirstOrDefaultAsync(t => t.TaskId == taskId && t.IsActive == true);
    
    if (task == null)
        return ResponseMessage.NotFound("Task not found");
    
    return ResponseMessage.Ok(task);
}

// ✅ Include Related Data
public async Task<ResponseMessage> GetTaskWithUserAsync(int taskId)
{
    var task = await _context.Tasks
        .Include(t => t.User)  // Join with User table
        .FirstOrDefaultAsync(t => t.TaskId == taskId);
    
    return ResponseMessage.Ok(task);
}
```

#### **What DOES NOT GO in Repository:**
```csharp
// ❌ NO Business Logic
if (task.UserId == adminUserId) // WRONG! That's Service
{
    return ResponseMessage.BadRequest("Cannot assign to self");
}

// ❌ NO DTO to Entity Mapping
public async Task<ResponseMessage> CreateAsync(CreateTaskRequestDTO dto) // WRONG!
{
    var task = new Task { 
        TaskTitle = dto.TaskTitle  // WRONG! Service should map DTO → Entity
    };
}

// ❌ NO Complex Validation
if (string.IsNullOrWhiteSpace(task.TaskTitle)) // WRONG! Service validates
{
    return ResponseMessage.BadRequest("Title required");
}

// ❌ NO Audit Field Logic
task.CreatedOn = DateTime.UtcNow;  // WRONG! Service sets audit fields
task.CreatedBy = adminUserId;      // WRONG! Repository doesn't know userId
```

---

## 🔄 Complete Flow Example: Create Task API

### **Step 1: API Layer (Controller)**
```csharp
[HttpPost("create")]
[Authorize] // JWT required
public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDTO request)
{
    // STEP 1: Extract JWT Claims
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;
    
    int currentUserId = int.Parse(userIdClaim);
    int currentRoleId = int.Parse(roleIdClaim);
    
    // STEP 2: Authorization Check (Role-based)
    if (currentRoleId != (int)eRole.Admin)
    {
        return Unauthorized("Only admins can create tasks");
    }
    
    // STEP 3: Basic Request Validation
    if (request == null || string.IsNullOrWhiteSpace(request.TaskTitle))
    {
        return BadRequest("Invalid request");
    }
    
    // STEP 4: Call Service (pass userId from JWT)
    var response = await _taskService.CreateTaskAsync(request, currentUserId);
    
    // STEP 5: Return HTTP Response
    return StatusCode(response.statusCode, response);
}
```

### **Step 2: Business Layer (Service)**
```csharp
public async Task<ResponseMessage> CreateTaskAsync(CreateTaskRequestDTO dto, int adminUserId)
{
    try
    {
        // BUSINESS RULE 1: Validate input
        if (string.IsNullOrWhiteSpace(dto.TaskTitle))
            return ResponseMessage.BadRequest("Task title required");
        
        if (dto.AssignedUserId <= 0)
            return ResponseMessage.BadRequest("Valid user ID required");
        
        // BUSINESS RULE 2: Admin cannot assign to themselves
        if (dto.AssignedUserId == adminUserId)
            return ResponseMessage.BadRequest("Admin cannot assign task to themselves");
        
        // BUSINESS RULE 3: Verify assigned user exists
        var userResponse = await _userRepository.GetByIdAsync(dto.AssignedUserId);
        if (!userResponse.success)
            return ResponseMessage.NotFound("Assigned user not found");
        
        // BUSINESS TRANSFORMATION: DTO → Entity
        var task = new Task
        {
            TaskTitle = dto.TaskTitle.Trim(),
            Description = dto.Description?.Trim(),
            TaskStatusId = dto.TaskStatusId,
            TaskPriorityId = dto.TaskPriorityId,
            DueDate = dto.DueDate,
            UserId = dto.AssignedUserId,
            Tags = dto.Tags?.Trim(),
            // AUDIT FIELDS (Business Layer responsibility)
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = adminUserId  // From JWT
        };
        
        // CALL REPOSITORY: Save to database
        var response = await _taskRepository.CreateAsync(task);
        return response;
    }
    catch (Exception ex)
    {
        return ResponseMessage.Error(ex.Message);
    }
}
```

### **Step 3: Data Layer (Repository)**
```csharp
public async Task<ResponseMessage> CreateAsync(Task task)
{
    try
    {
        // SIMPLE DATABASE OPERATION: Add and Save
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        return ResponseMessage.Created(task, "Task created successfully");
    }
    catch (Exception ex)
    {
        return ResponseMessage.Error(ex.Message);
    }
}

public async Task<ResponseMessage> GetByIdAsync(int userId)
{
    try
    {
        // SIMPLE DATABASE QUERY
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive == true);
        
        if (user == null)
            return ResponseMessage.NotFound("User not found");
        
        return ResponseMessage.Ok(user);
    }
    catch (Exception ex)
    {
        return ResponseMessage.Error(ex.Message);
    }
}
```

---

## 🎯 Quick Decision Tree: Where Does This Code Go?

### **Is it about HTTP, JWT, or Routes?**
→ **API LAYER (Controller)**

### **Is it business logic or validation?**
→ **BUSINESS LAYER (Service)**

### **Is it a database query?**
→ **DATA LAYER (Repository)**

---

## 📝 Common Patterns

### **Pattern 1: Simple CRUD**
```
Controller → Service → Repository → Database
   (Auth)   (Validate) (Query)      (Data)
```

### **Pattern 2: Complex Business Logic**
```
Controller → Service → Multiple Repositories → Database
   (Auth)   (Orchestrate)  (Different Entities)
```

### **Pattern 3: User-Specific Data**
```
Controller → Extract UserId from JWT → Service → Repository
   (Auth)       (Claims)                (Filter)   (Query WHERE UserId)
```

---

## ✅ Checklist for Creating New API

### **1. Define DTO (Model Layer)**
- [ ] Create `XxxRequestDTO.cs` in `TaskManager.Model/Model/BusinessLayer/RequestDTO/`
- [ ] Add required properties
- [ ] Add validation attributes if needed

### **2. Create Repository Interface & Implementation (Data Layer)**
- [ ] Create `IXxxRepository.cs` interface
- [ ] Define method signatures (e.g., `Task<ResponseMessage> CreateAsync(Entity entity)`)
- [ ] Implement in `XxxRepository.cs`
- [ ] Add simple CRUD operations only
- [ ] Register in `DataLayer.cs`

### **3. Create Service Interface & Implementation (Business Layer)**
- [ ] Create `IXxxService.cs` interface
- [ ] Define method signatures (e.g., `Task<ResponseMessage> CreateXxxAsync(DTO dto, int userId)`)
- [ ] Implement in `XxxService.cs`
- [ ] Add all business logic and validation
- [ ] Map DTO → Entity
- [ ] Set audit fields (CreatedBy, CreatedOn)
- [ ] Call repository method(s)
- [ ] Register in `BusinessLayer.cs`

### **4. Create Controller (API Layer)**
- [ ] Create `XxxController.cs` in `TaskManager.Api/Controllers/`
- [ ] Add `[Authorize]` attribute if authentication required
- [ ] Extract JWT claims (UserId, RoleId)
- [ ] Check role-based authorization
- [ ] Validate request format
- [ ] Call service method
- [ ] Return status code from ResponseMessage

### **5. Test**
- [ ] Run application
- [ ] Test with Swagger
- [ ] Verify JWT token required
- [ ] Verify role authorization works
- [ ] Test success and error scenarios

---

## 🚫 Common Mistakes to Avoid

### ❌ Mistake 1: Business Logic in Repository
```csharp
// WRONG! Repository doing business logic
public async Task<ResponseMessage> CreateTask(CreateTaskRequestDTO dto)
{
    if (dto.AssignedUserId == adminUserId)  // Business logic!
        return ResponseMessage.BadRequest("Cannot assign to self");
}
```

**✅ CORRECT:** Service does this, Repository just saves

### ❌ Mistake 2: Database Calls in Service
```csharp
// WRONG! Service doing direct database call
public async Task<ResponseMessage> CreateTask(CreateTaskRequestDTO dto)
{
    var user = await _context.Users.FindAsync(dto.UserId);  // Direct DB call!
}
```

**✅ CORRECT:** Service calls Repository method

### ❌ Mistake 3: JWT Claims in Service
```csharp
// WRONG! Service extracting JWT claims
public async Task<ResponseMessage> CreateTask(CreateTaskRequestDTO dto)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // HTTP concern!
}
```

**✅ CORRECT:** Controller extracts claims, passes to Service

### ❌ Mistake 4: DTOs in Repository
```csharp
// WRONG! Repository accepting DTO
public async Task<ResponseMessage> CreateAsync(CreateTaskRequestDTO dto)
{
    // Repository should only work with Entities!
}
```

**✅ CORRECT:** Repository accepts Entity, Service maps DTO → Entity

---

## 📚 Summary

| Layer | Responsibility | Examples |
|-------|---------------|----------|
| **API (Controller)** | HTTP, Auth, Claims | `[Authorize]`, Extract UserId, Role checks, Return status codes |
| **Business (Service)** | Logic, Validation, Transformation | Business rules, DTO↔Entity mapping, Set audit fields |
| **Data (Repository)** | Database Operations | EF Core queries, CRUD operations, Simple data access |

**Remember:** Each layer has ONE job. Keep them separated for maintainable, testable code!

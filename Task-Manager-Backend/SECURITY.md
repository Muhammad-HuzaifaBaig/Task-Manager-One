# Security Configuration Guide

## 🔒 Connection String & Secrets Management

### ⚠️ Security Issue Fixed
Previously, sensitive credentials (database password, JWT secret) were stored in `appsettings.json`, which gets committed to Git. This exposed credentials in:
- Git history
- Public repositories
- Team member access
- CI/CD logs

### ✅ Current Secure Setup

#### **Local Development (User Secrets)**
Secrets are stored in your **local machine** outside the project folder, never committed to Git.

**Location:** `%APPDATA%\Microsoft\UserSecrets\0e863e0f-39ca-4841-b22f-9800a9f054d4\secrets.json`

**Stored Secrets:**
- Database connection string (with password)
- JWT Secret Key

**View Your Secrets:**
```bash
cd TaskManager.Api
dotnet user-secrets list
```

**Add/Update Secrets:**
```bash
# Connection String
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TaskManager;User ID=sa;Password=YourPassword;TrustServerCertificate=True;"

# JWT Secret
dotnet user-secrets set "JwtSettings:SecretKey" "YourJWTSecretKeyHere"
```

**Remove Secrets:**
```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

**Clear All Secrets:**
```bash
dotnet user-secrets clear
```

---

#### **Production Deployment**

**Option 1: Environment Variables (Recommended)**
Set environment variables on your production server:

```bash
# Windows Server
setx ConnectionStrings__DefaultConnection "Server=prod-server;Database=TaskManager;User ID=prod_user;Password=SecurePassword123!;TrustServerCertificate=True;"
setx JwtSettings__SecretKey "ProductionJWTSecretKey256Bits!"

# Linux/Docker
export ConnectionStrings__DefaultConnection="Server=prod-server;..."
export JwtSettings__SecretKey="ProductionJWTSecretKey256Bits!"
```

**Option 2: Azure App Settings**
If deploying to Azure:
1. Go to Azure Portal → Your App Service
2. Configuration → Application Settings
3. Add:
   - `ConnectionStrings:DefaultConnection`
   - `JwtSettings:SecretKey`

**Option 3: AWS Secrets Manager / Azure Key Vault**
For enterprise applications, use cloud secret management:
- AWS Secrets Manager
- Azure Key Vault
- HashiCorp Vault

---

### 📁 What's Safe to Commit

**✅ Safe to commit (in appsettings.json):**
- Non-sensitive configuration (Issuer, Audience, ExpirationMinutes)
- Logging levels
- Feature flags
- API endpoints (without credentials)

**❌ NEVER commit:**
- Passwords
- Connection strings with credentials
- JWT secret keys
- API keys
- Certificates/private keys

---

### 🔐 Current Configuration

**appsettings.json (Safe - Committed to Git):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManager;Integrated Security=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "",
    "Issuer": "TaskManagerAPI",
    "Audience": "TaskManagerClient",
    "ExpirationMinutes": 60
  }
}
```

**User Secrets (Secure - Local Only):**
```json
{
  "ConnectionStrings:DefaultConnection": "Server=localhost;Database=TaskManager;User ID=sa;Password=Xotillweoverdose1*;TrustServerCertificate=True;",
  "JwtSettings:SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMinimum32Characters!"
}
```

---

### 🛡️ Best Practices

1. **Rotate Secrets Regularly**
   - Change JWT secret every 90 days
   - Change database passwords quarterly

2. **Use Strong Secrets**
   - JWT Secret: Minimum 256 bits (32+ characters)
   - Database Password: 16+ characters, mixed case, numbers, symbols

3. **Separate Environments**
   - Development secrets ≠ Production secrets
   - Never use production credentials locally

4. **Audit Access**
   - Track who has access to production secrets
   - Use role-based access control (RBAC)

5. **Backup Secrets Securely**
   - Store production secrets in password manager (1Password, LastPass)
   - Encrypt backup files

---

### 🚨 If Secrets Are Compromised

1. **Immediately rotate all secrets**
2. **Check Git history** for exposed credentials
3. **Use BFG Repo-Cleaner** to remove from history:
   ```bash
   bfg --replace-text passwords.txt
   git push --force
   ```
4. **Notify team members**
5. **Monitor for unauthorized access**

---

### 📚 Additional Resources

- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Safe Storage of App Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/)

# Authentication Database Separation (DM02)

## Overview
The authentication system has been separated into a dedicated database (DM02) for enhanced security. This isolates user authentication data from the main application data (DM01).

## Database Structure

### DM01 (Main Application Database)
- Blocks
- HarvestPlans
- HarvestContractors
- Pools
- Commodities
- ScoutReportsWithBlock

### DM02 (Authentication Database)
- Users
- UserSessions
- PasswordResetTokens
- AuditLogs

## Configuration Changes

### Connection Strings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=RDGW-CF;Database=DM01;User ID=CobbleAPI;password=[1Qa-2Ws];TrustServerCertificate=True;MultipleActiveResultSets=True",
    "AuthConnection": "Server=RDGW-CF;Database=DM02;User ID=CobbleAPI;password=[1Qa-2Ws];TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

### DbContexts
- **AppDbContext**: Handles main application data (DM01)
- **AuthDbContext**: Handles authentication data (DM02)

## Setup Instructions

1. **Create DM02 Database** (if it doesn't exist)
2. **Run the SQL Script**: Execute `CreateAuthDatabase.sql` in DM02
3. **Update Connection Strings**: Ensure both connection strings are correct
4. **Test Authentication**: Verify login/logout functionality

## Security Benefits

1. **Data Isolation**: Authentication data is completely separate from business data
2. **Access Control**: Different database permissions can be applied
3. **Backup Strategy**: Authentication data can be backed up separately
4. **Compliance**: Easier to meet security compliance requirements
5. **Audit Trail**: All authentication events are logged in the secure database

## API Endpoints

All authentication endpoints remain the same:
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/refresh`
- `GET /api/v1/auth/verify`
- `GET /api/v1/users`
- `POST /api/v1/users`
- etc.

## Default Users

The script creates two default users:
- **admin** / admin123
- **chrism** / 1835

Both users have admin privileges.

## Migration Notes

- No data migration needed for existing users (new setup)
- All authentication controllers now use AuthDbContext
- Main application controllers continue to use AppDbContext
- JWT tokens and sessions are stored in DM02

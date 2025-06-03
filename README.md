### Customer Management Minimal API & Notification System

This repository contains two main projects and a shared model library:

1. **CustomerApp_MinimalApi**  
   - A Minimal API built with .NET 8 and ASP.NET Core  
   - Uses Azure Cosmos DB (Emulator or Azure) to store customer and salesperson data  
   - Provides Swagger UI to test endpoints for CRUD operations and search  

2. **CustomerNotificationFunction**  
   - A .NET 8 isolated‐worker Azure Function triggered by Cosmos DB change feed  
   - Sends email notifications via SendGrid whenever a customer is created or updated  

3. **Shared_Model**  
   - A class library containing the `Customer` model shared by both projects  

---

##  Quick Start

### 1. Clone & Pull

```bash
git clone https://github.com/YOUR_USERNAME/CustomerApp-MinimalApi.git
cd CustomerApp-MinimalApi
git pull

CustomerApp-MinimalApi/           # Root folder
├─ CustomerApp_MinimalApi/        # Minimal API project  
│   ├─ Data/                       # DTOs, Entities, Interfaces, Repos  
│   ├─ Endpoints/                  # CustomerEndpoints, SalespersonEndpoints, EmailTestEndpoint  
│   ├─ Program.cs                  # WebApplication builder  
│   ├─ appsettings.json            # Cosmos DB connection settings  
│
├─ CustomerNotificationFunction/   # Azure Function (isolated-worker) project  
│   ├─ Handler/                    # CustomerCreatedHandler.cs  
│   ├─ Program.cs                  # FunctionsWorker host setup  
│   ├─ local.settings.json         # Function configuration (Cosmos + SendGrid)  
│   
│
├─ Shared_Model/                   # Class library for shared models  
│   ├─ Customer.cs                 # Customer entity (Id, Name, Title, Telephone, Email, Address, SalespersonId)   
│
└─ README.md                       # This file  

---

### Prerequisites
1. .NET 8 SDK
2. Azure Cosmos DB Emulator (for local development)
3. Visual Studio 2022 (or later) with ASP.NET and Azure development workloads
4. SendGrid Account: Obtain an API key for sending emails.

---

### Configuration & Credentials

## CustomerApp_MinimalApi/appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "CosmosDb": {
    "ConnectionString": "<YOUR_COSMOS_CONNECTION_STRING>",
    "DatabaseName": "CustomerDB",
    "CustomerContainer": "Customers",
    "SalespersonContainer": "Salespeople"
  }
}

## CustomerNotificationFunction/local.settings.json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "cosmosconn": "<YOUR_COSMOS_CONNECTION_STRING>",
    "SendGridApiKey": "<YOUR_SENDGRID_API_KEY>",
    "SendGridFromEmail": "<YOUR_FROM_EMAIL>",
    "SalespersonEmailFallback": "<FALLBACK_EMAIL_FOR_TESTING>"
  }
}

---

### Run Azure Function (CustomerNotificationFunction)

Ensure local.settings.json is in place with correct values.

{
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "cosmosconn": "<YOUR_COSMOS_CONNECTION_STRING>",
    "SendGridApiKey": "<YOUR_SENDGRID_KEY>",
    "SendGridFromEmail": "<YOUR_VERIFIED_EMAIL>",
    "SalespersonEmailFallback": "<FALLBACK_EMAIL>"
  }
}

---

### Shared_Model

# Customer.cs with [JsonProperty("id")] to match Cosmos DB document schema.

---

### Email Notification Flow
1. User calls POST /customers or PUT /customers/{id} in the Minimal API.

2. The API writes/updates a document in Cosmos DB CustomerDB → Customers.

3. Azure Function triggers on the Cosmos DB change feed (container: Customers).

4. The function reads the changed Customer document(s) and calls SendGridEmailService.

5. SendGrid processes the email request and returns HTTP status code 202 Accepted.

---

### Contributing

1. Fork this repository.

2. Create a new feature branch: git checkout -b feature/YourFeatureName

3. Commit your changes and push: git push origin feature/YourFeatureName

4. Open a Pull Request describing your changes.

---

### Contact
## Mairaj Siddiqui
# https://www.linkedin.com/in/mairaj-siddiqui-134596248/

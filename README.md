# ppsr-batch-registration
Batch create motor vehicle PPSR registrations

# Architecture


┌─────────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                          PPRS Batch Registration                                    │
├─────────────────────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                                     │
│  ┌─────────────────────┐    ┌─────────────────────┐    ┌────────────────────────────────────────┐  │
│  │      React UI       │    │    SignalR Client   │    │          REST API Consumption          │  │
│  └─────────┬───────────┘    └─────────┬──────────┘     └─────────┬──────────────────────────────┘  │
│            │                          │                          │                                 │
│            │                          │                          │                                 │
│            ▼                          ▼                          ▼                                 │
│  ┌─────────────────────┐    ┌─────────────────────┐    ┌────────────────────────────────────────┐  │
│  │  UI Components      │    │  SignalR Hub        │    │  API Controllers                       │  │
│  │  - Batch List       │    │  - Notification Hub │    │  - BatchController                     │  │
│  │  - CsvUploader
      - Registration 
	  Validator          │    │  					     │  - RegistrationController              │  │
│  └─────────────────────┘    └─────────────────────┘    └────────────────────────────────────────┘  │
│                                                                                                     │
├─────────────────────────────────────────────────────────────────────────────────────────────────────┤
│                                    Backend System (Clean Architecture)                              │
├─────────────────────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                                     │
│  ┌──────────────────────────────────────────────────────────────────────────────────────────────┐  │
│  │                                       API Layer                                              │  │
│  │  ┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────────────────┐  │  │
│  │  │   Controllers       │    │     SignalR Hub     │    │       API Models (DTOs)         │  │  │
│  │  └─────────┬───────────┘    └─────────┬──────────┘     └─────────────────────────────────┘  │  │
│  │            │                          │                                                    │  │
│  └────────────┼──────────────────────────┼────────────────────────────────────────────────────┘  │
│               │                          │                                                       │
│               ▼                          ▼                                                       │
│  ┌──────────────────────────────────────────────────────────────────────────────────────────────┐  │
│  │                                    Application Layer                                         │  │
│  │  ┌─────────────────────┐    ┌─────────────────────┐                                          │  │
│  │  │   Command Handlers  │    │     Event Handlers  │                                          │  │
│  │  │  - ProcessBatch     │    │  - BatchProcessed   │                                          │
│  │  └─────────┬───────────┘    └─────────┬──────────┘                                           │  │
│  │            │                          │                                                    │  │
│  └────────────┼──────────────────────────┼────────────────────────────────────────────────────┘  │
│               │                          │                                                       │
│               ▼                          ▼                                                       │
│  ┌──────────────────────────────────────────────────────────────────────────────────────────────┐  │
│  │                                     Domain Layer                                            │  │
│  │  ┌─────────────────────┐    ┌─────────────────────┐      
│  │  │     Entities        │    │     Domain Events   │      
│  │  │  - Batch            │    │                     │      
│  │  │  - Registration     │    │  - BatchCompleted   │      
│  │  │  - Grantor          │    └─────────────────────┘      
│  │  │  - SPG              │                                                                    │  │
│  │  └─────────────────────┘                                                                    │  │
│  └────────────┬────────────────────────────────────────────────────────────────────────────────┘  │
│               │                                                                                   │
│               ▼                                                                                   │
│  ┌──────────────────────────────────────────────────────────────────────────────────────────────┐  │
│  │                                  Infrastructure Layer                                       │  │
│  │  ┌─────────────────────┐    ┌─────────────────────┐   
│  │  │   EF Core          │    │     SignalR          │    
│  │  │  - DbContext       │    │  - HubContext        │    
│  │  │  - Repositories    │    │  - Client Manager    │    
│  │  └─────────────────────┘    └─────────────────────┘                                         │  │
│  │                                                                                             │  │
│  │  
│                                                                                                     │
│  ┌─────────────────────┐    
│  │   Database          │    
│  │  - SQL Server       │    
│  │  - Tables           │    
│  └─────────────────────┘  

# Build Instructions

# Frontend
	 Navigat to ppsr-batch-registration\frontend
	 docker-compose down -v --remove-orphans   ( To Remove orphans)
	 docker-compse up (To build and run the application)
	 Access the running application  at http://localhost:3000

# Backend
	 Navigat to ppsr-batch-registration\backend
	 docker-compose down -v --remove-orphans   ( To Remove orphans)
	 docker-compose build --no-cache (To build the application)
	 docker-compose up (To run the application and db)
	 Access the running application  at http://localhost:8080/swagger/index.html

# To Run updates to DB through migrations
	Navigate to ppsr-batch-registrations\backend\Infrastructure
	dotnet ef migrations add InitialMigration --startup-project ../Api --output-dir Persistence/Migrations
	dotnet ef database update --startup-project ../Api

# Screenshots




	 


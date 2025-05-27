# ppsr-batch-registration
Batch create motor vehicle PPSR registrations

# Architecture




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




	 


# ppsr-batch-registration
Batch create motor vehicle PPSR registrations

# Architecture

![Screenshot 2025-05-27 131641](https://github.com/user-attachments/assets/d0f83398-6148-4ed6-9db5-35fe0a819b61)
![Screenshot 2025-05-27 131707](https://github.com/user-attachments/assets/5267a38a-fe71-4937-9f72-e37c8229c289)

# Build Instructions

# Frontend
	 Navigat to ppsr-batch-registration\frontend
	 docker compose down -v --remove-orphans   ( To Remove orphans)
	 docker compose up (To build and run the application)
	 Access the running application  at http://localhost:3000

# Backend
	 Navigat to ppsr-batch-registration\backend
	 docker compose down -v --remove-orphans   ( To Remove orphans)
	 docker compose build --no-cache (To build the application)
	 docker compose up (To run the application and db)
	 Access the running application  at http://localhost:8080/swagger/index.html

# To Run updates to DB through migrations
	Navigate to ppsr-batch-registrations\backend\Infrastructure
	dotnet ef migrations add InitialMigration --startup-project ../Api --output-dir Persistence/Migrations
	dotnet ef database update --startup-project ../Api

# Screenshots

![Screenshot 2025-05-27 124620](https://github.com/user-attachments/assets/cde85323-9f78-423a-afa3-1242663f7db8)
![Screenshot 2025-05-27 124532](https://github.com/user-attachments/assets/c0942bae-69b9-4488-a02b-5a8105710d2d)
![Screenshot 2025-05-27 123015](https://github.com/user-attachments/assets/3883d73e-c29c-4fcd-bbe5-6cf946e50831)
![Screenshot 2025-05-27 124327](https://github.com/user-attachments/assets/779faec5-9a87-4d36-84af-91a9068667de)
![Screenshot 2025-05-27 124418](https://github.com/user-attachments/assets/87f5d5f8-03b0-4b64-99de-2b1a81edb461)



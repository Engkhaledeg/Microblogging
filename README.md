# Microblogging
Microblogging application
This project is a full-stack MicroBlog web application built using ASP.NET Core MVC with .NET 8 and SQL Server. It allows users to create posts, upload images, and view posts on a timeline.
Table of Contents
1.	Prerequisites
2.	Setting Up the Application
3.	Running the Application
4.	Docker Setup
5.	Folder Structure
6.	Troubleshooting
Prerequisites
Before setting up and running the application, ensure that you have the following installed:
1	.NET 8 SDK (Download from here)
2	SQL Server (Express or higher version, or use Docker)
3	Visual Studio 2022 or later (or any IDE that supports .NET)
4	Hangfire Dashboard (for managing background jobs)

Optional:
=========
1	Docker (for containerized deployment)
2	Azure Account (for using Azure Blob Storage)
Setting Up the Application
1. Clone the Repository
Download the Repository and Extract Zip File
2. Configure the Database
You need to set up the SQL Server database for the application.
•	SQL Server Setup:
o	Ensure SQL Server is installed and running.
o	You can use SQL Server Express or any edition.
o	If you're using a local SQL Server instance, make sure the connection string in appsettings.json is configured properly.
3. Configure Azure Blob Storage (Optional)
If you're using Azure Blob Storage for image storage, update the appsettings.json with your Azure connection details:
"AzureBlobStorage": {
  "ConnectionString": "your-azure-connection-string",
  "ContainerName": "your-container-name"
}
If you're using a local file system or other storage, update accordingly.
4. Set Up Configuration Values in appsettings.json
Update the following settings in your appsettings.json file:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MicroBlogDb;Trusted_Connection=True;"
  },
  "AzureBlobStorage": {
    "ConnectionString": "your-azure-connection-string",
    "ContainerName": "your-container-name"
  },
  "AppSettings": {
    "SiteName": "MicroBlog"
  }
}
5. Run EF Core Migrations
Run the following commands to apply the database migrations:
dotnet ef migrations add InitialCreate
dotnet ef database update
This will create the necessary tables in your SQL Server database.
Running the Application
1. Running with Visual Studio (Windows)
•	Open the solution in Visual Studio 2022 or later.
•	Build the solution.
•	Press F5 or click Run to start the application.
2. Running via Command Line
If you prefer running via the command line, run the following command to start the web application:
dotnet run --project MicroBlog.Web
This will start the application on http://localhost:5000 (by default).
3. Running the Worker (Background Jobs)
To process background jobs like image resizing and conversion, you need to run the MicroBlog.Worker project.
In the MicroBlog.Worker folder, run:
dotnet run
This will start the Hangfire server, which will process background jobs.
4. Running the Application with Docker
If you're using Docker, you can build and run the application in containers. Ensure Docker is installed on your machine, then follow these steps:
1. Build the Docker Image
docker build -t microblog .
2. Run the Containers
docker-compose up
This will spin up your web application, worker, and database (if you have a database container configured) in Docker containers.
Important:
•	Ensure the SQL Server Docker container is configured if you're using a containerized database. Refer to the docker-compose.yml for configurations related to the database container.
Folder Structure
Here's a brief overview of the folder structure:
/MicroBlog
|-- /MicroBlog.Api (REST API project)
|-- /MicroBlog.Web (MVC application project)
|-- /MicroBlog.Worker (Hangfire background job worker)
|-- /MicroBlog.Application (Application services and logic)
|-- /MicroBlog.Domain (Entities and Data Models)
|-- /MicroBlog.Infrastructure (Data access layer, repository, EF Core)
|-- /Dockerfile
|-- /docker-compose.yml
Troubleshooting
If you encounter issues, here are some common troubleshooting steps:
1.	Database Connection Issues:
o	Double-check your appsettings.json connection string and ensure SQL Server is running.
o	Make sure the database exists, or run the migration commands to create it.
2.	Hangfire Jobs Not Running:
o	Ensure that Hangfire is correctly set up in your Program.cs.
o	Check the Hangfire dashboard (usually accessible at http://localhost:5000/hangfire) for job status and errors.
3.	Azure Blob Storage Issues:
o	Ensure the connection string for Azure Blob Storage is correct in appsettings.json.
o	Check your Azure storage account for any issues related to containers or permissions.


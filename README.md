# MReminders - A .NET MAUI Reminders App

## 1. Requirements:
	- [x] Docker Desktop
	- [x] .NET 9.0
	- [x] .NET MAUI 
		For iOS builds:
		- [x] MacOS + XCode + NET 9.0

## 2. BackEnd Setup:
	
	- After meet the requirements, clone the repository. Then go to the root of repository folder and run:

	```bash
	
	docker network create -d bridge app-network

	```

	That will create the network for the use in app and sqlserver containers.

	- Then run: 

	```bash
	
	docker-compose up --build

	```

	That will start the app and sql containers and the app run the migrations on the startup

## 3. FrontEnd Setup:
	
	- Set the backend external IP Address on appsettings.json file on MReminders.Mobile.Client project

	- Run the app
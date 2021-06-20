# QuartzServiceWithDBLogs

A demo project running Quartz as a service with NLog logging to a SQL database

# Installation

1. Clone the repo to your local machine
2. Create a Database called QuartzScheduler on localhost
3. Build and run the program
	- The migration runs automatically to setup the database
	- Logs are stored in dbo.ApplicationLog
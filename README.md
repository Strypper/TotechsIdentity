# TotechsIdentity
This is a micro-service in Totech architecture responsible for generating token and authentication, all Totechs authorized projects must take the security standard and token from this project. The goal is to have one single Authentication services and users information storage to provide all project.
### Like AAD B2C we support similar authentication feature to all across Totechs app
- Token
- QR Code
## Table of contents

- [Setups](#Setups)
- [Features](#Features)
- [Status](#Status)

# Setups
## Get project
The project only provide the skeleton how things handle in our REST API, 
1. Clone our project from this repository.
2. Contact Totechs authorized members to get the appsettings.json file for this project
3. Include the given appsettings.json file inside the API project
## Generate your local database for Totechs Identity
The project include migrations folder to help generate the database please follow these steps below
1. Rebuild project to check if is there's any problem (If there are problems when you doing this action, please pull the project again to make sure you are on the latest version and if that doesn't help we recommend contacting the Totechs members for investigation)
2. Once the project build success navigate to TotechsIdentity.Entities and run this command
       #### Project Location
       ```TotechsIdentity
          TotechsIdentity/ 
              ── Entities
       ```
       #### Run command 
       ```
       script-migration
       ```
3. Apply the generated SQL Script to your database
4. Change the connection string inside the appsettings.json to match with your database local connection string
5. Run the project

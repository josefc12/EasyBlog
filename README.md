# 📝 EasyBlog

A primitive blogging app I creteated for ~~best~~ practice.

The Solution includes two project, one for the API backend (standard Controller-Service-Repo based backend) and a Blazor WASM front-end.

These have to be ran separately on two different ports.

### My prefered setup (Api project)
1. Create a postgres database with a name that is defined in the appsettings or set your own.
2. Create a new user for this database (this user needs to be able to write and read from the public chema of the database and yes also needs to match whatever the appsettings are).
3. Change the password for the admin account in the Project.cs file
4. Make sure all nugets are installed.
5. Build & run. Migration should automatically happen when you run the project. (If not, migrate with dotnet ef database update)
6. If there's a problem building the API, see if it's because it's trying to create an admin account into a table that doesn't exist yet. If so, comment that part out and run the database update without it first.
7. Once you run the API project for the first time, an admin account is created.
8. This Api project authenticates the users using the Client project using Jwt tokens. The Secret to this token is stored in the appsettings and I highly recommend you implement your own way of storing this secret elswhere based on your platform and prefference.
9. You can test if the Api project is running correctly on its own with swagger (the.address-you.set/swagger)
10. Make sure that the CORS are set correctly (in Program.cs) for whatever setup you're running.

### My prefered setup (Client project)
1. Build & run.
2. Register your own personal account.
3. Login with the admin account, go to "/settings" and set an admin role for your personal account.
4. Log out of the admin account, log in with your personal account.
5. You should be able to edit the homepage and create new articles (big pink button on the left)

### Notes
 - Only admin users can edit the homepage.
 - Only admin users can create new articles.
 - Only admin users can set other users as admins.
 - Admin users can only edit their own articles.
 - The authToken is stored in the local storage of the Client app.
 - The refreshToken is stored in the cookies of the Client app.
 - The authToken is refreshed with each login, in 7 days or if authorization fails for any reason (someone tempered the token)
 - Passwords are hashed with the Argon algorithm.
 - Developed and tested only on Linux (Ubuntu).
 - I admit the return messages aren't very user-friendly.

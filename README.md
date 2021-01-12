# Goals
The overall goal of this project is to establish a simple proof-of-concept for a larger planned project. A centralized identity provider (IdP) will serve to log users in and issue JWTs with the OpenIdConnect protocol. Clients will use this token to call a web API with the main data of the application. The identity server will handle deciding whether users have authorization to perform something. Clients will be able to receive and cache permissions information so they know how to display their dynamic forms and controls. Once the API call is made, the web API will verify that the user has access and, if they do, do what they need to do.

The multitenancy aspect is handled almost entirely by the clients. They implement their own portal where a user selects from their lists of available tenants. Handling this in the client as opposed to setting this on the identity side means that it is easier and faster for the user to work with multiple tenants, accessing data in one and then quickly moving to another. The client will use the selected tenant's GUID when making calls to the API, although a shortened identifier could be used as well.
## The main problem - Updating Permissions
Permissions is really the largest problem here. When it boils down to it, there are really three possible ways to handle updating of permissions.
#### 1 - Eventual update via token refresh
The easiest of the 3 methods, the basis of this is to put permissions information (or, at least, roles) into the JWT and have a very short refresh timer (< 10 minutes) so that when a user's access is changed, it will be updated *eventually*. The downside if storing the tenantId as a claim (like Azure or other companies do) is that you will have to log your user in and out if they switch tenants. I don't believe there is built-in
functionality to "soft-relog" a user currently, so while it would sound great to simply re-issue the token without prompting the user when they select a tenant to change, I don't believe it can be done, at least not simply.
#### 2 - Force a token refresh
This is how Auth0 does it. Regardless of the arguments against tracking jwts, many companies do it for revocation anyway. The downside is a lot of custom code server and client-side that goes against a standard that may have major security implications. Basically, a lot of work and kind of hacky (though pragmatic).
#### 3 - Permissions server (policy server)
This is how some companies do it, using services like OPA (basically json XACML that's very fast). Basically you have a remote server authorize requests that are protected. This is the approach I chose. It isn't for everyone but I wanted to try this.
Pros
1. Immediate updating of permissions.
2. Easy for users to switch tenants quickly, since the client is storing the tenantId locally and sending it per-request.
Cons
1. Network traffic/latency, partially mitigated by using gRPC calls.
2. Coupling between the Idp and the API, however it is quite workable from a developer standpoint because gRPC protos provide a very nice contractual understanding between servers.

## Permissions themselves
There are tons of different ways to approach authorization whether it be claims, roles, or policies including both. I chose to map tables to two enum values representing Permissions and PermissionCategories. On migration, they are updated from the enums in code. There are many different approaches. Using an enum decorated with the [Flags] attribute is the most performant way if you want to model finely-grained permissions and you need less than 64 (the max if using long).

## Multitenancy (Database-per-tenant)
This is another topic with tons of different ways to do it. As a naive approach, I am simply using the tenant id as the connection string and creating a database if it isn't already. As far as creating databases that are made from non-existing tenant ids or bogus values, each request is already guaranteed to have an existing user tenant combination so there is no possibility of a request getting to the database context with a non-real tenant id. There are obviously tons of approaches with their own pros and cons. It's all based off of what the business requirements are, and each has maintanability/scalability concerns. Applying migrations to each database via ef migrations actually is not as bad as it sounds, but nonetheless requires at least some form of script or dotnet tool.

# Setup
#### Email Configuration (Dummy values may be used if email activity undesired)
Email configuration is stored in user secrets and injected via the IOptions<TOptions> interface.
To set the email configuration, sign up for a free account with SendGrid and enter your credentials into user secrets:
```
$ cd src/CoreMultiTenancy.Identity
$ dotnet user-secrets set "Email:SendGridUser" "<your_username>"
$ dotnet user-secrets set "Email:SendGridKey" "<your_key>"
```
#### Migrations and databases
Migrations are tracked and do not need to be generated. Note that MySQL is currently used, so
appsettings.json will need to be changed in the Identity and API projects to use either a trusted connection
or a user/pass setup based on your local environment (In 5.0 SQLite supports migrations almost fully, but this is still pre 5.0).
```
$ cd src/CoreMultiTenancy.Identity
$ dotnet ef database update
```

You do not need to specifically call ```database update``` for the API project, it creates tenant databases
at runtime and uses a static design time factory for migrations.

#### Startup
the tracked .vscode folder contains json files to launch all 3 projects at once. Navigate to the
debug tab in vscode and select the "Api, Idp, and Mvc" selection, then click on the Run button.
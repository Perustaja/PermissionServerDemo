# Goals
This project has three main goals in order of least to greatest difficulty
#### 1 - To showcase how a client, API, and identity provider can work together despite being entirely separate projects
#### 2 - To have a multi-tenant structure across this architecture with tenant management handled by the identity provider
#### 3 - To have an authorization system built on permissions that have immediate affect on the users when changed (no refresh delay such as with JWT), alongside user-created roles with global defaults

## 1 - To showcase how a client, API, and identity provider can be distinct and work together

This is relatively straightforward. Using IdentityServer, the three projects are separated cleanly with JWTs issued by the identity provider. These JWTs are used solely as authentication and not authorization in the expected way authentication JWTs are used. 

## 2 - To have a multi-tenant structure across this architecture with tenant management handled by the identity provider

Initially I wanted to try a database-per-tenant approach as this is the more difficult method. I was able to get this working in this version of EF Core, however after further research and experimentation I decided this approach isn't worth it. Specifically, the management of tenant creation becomes unwieldy as asynchronous migration and database creation must take place and then users updated when this is done. Among other reasons, this was scrapped in favor of simpler data segregation via a tenant id in each record of the db. This is achieved very simply by EF Core QueryFilters and is not complex at all.

The multitenancy aspect is handled almost entirely by the client. It implements its own portal where a user selects from their lists of available tenants. Handling this in the client as opposed to setting this on the identity side means that it is easier and faster for the user to work with multiple tenants, accessing data in one and then quickly moving to another. The client uses the selected tenant's GUID when making calls to the API. This means no state is held by the API and there is no conflict of refreshing when the tenant id is stored in the JWT.

## 3 - To have an authorization system built on permissions that have immediate affect on the users when changed (no refresh delay such as with JWT)

#### The largest problem - Updating Permissions
Permissions is really the largest problem here. When it boils down to it, there are really three possible ways to handle updating of permissions.
#### 1 - Eventual update via token refresh
The easiest of the 3 methods, the basis of this is to put permissions information (or, at least, roles) into the JWT and have a very short refresh timer (< 10 minutes) so that when a user's access is changed, it will be updated *eventually*. Similarly I have seen systems where the tenant id is stored in the JWT. The downside of storing the tenantId as a claim (like Azure or other companies do, although their systems fit their requirements) is that you will have to log your user in and out if they switch tenants. I don't believe there is built-in
functionality to "soft-relog" a user currently, so while it would sound great to simply re-issue the token without prompting the user when they select a tenant to change, I don't believe it can be done, at least not without lots of custom complex code.

#### 2 - Force a token refresh
This is how Auth0 does it. Regardless of the arguments against tracking jwts, many companies do it for revocation anyway. The downside is a lot of custom code server and client-side that goes against a standard that may have major security implications. Basically, a lot of work and kind of hacky (though pragmatic).

#### 3 - Permissions server (My choice)
This is how some companies do it, using services like OPA (basically json XACML that's very fast for those with prior knowledge). You have a remote server authorize requests that are protected. The API holds the application logic and when the user calls the api, if that endpoint is protected, a network request with the necessary information is sent over to an identity server that decides whether that user is able to do something or not. This is the approach I chose. It isn't for everyone but I wanted to try this. I chose to use gRPC to make authorization validation network calls for each protected endpoint using custom attributes.<br>
Pros<br>
1. Immediate updating of permissions.
2. The API needs to know nothing about users, tenants, or anything else. Just the main application logic.<br>


Cons<br>
1. Network traffic/latency, partially mitigated by using gRPC calls. There is simply no way around this and IMO it is worth it if you need security.
2. Coupling between the identity provider and the API, however it is quite workable from a developer standpoint because gRPC protos provide a very nice contractual understanding between servers and only one or two proto files are needed. Permissions, if stored as an Enum like here, can be stored in a shared library so no magical strings are used and compile-time safety is kept.

#### Permissions themselves
There are tons of different ways to approach authorization whether it be claims, roles, or policies including both. I chose to map tables to two enum values representing Permissions and PermissionCategories (used for sorting on the front end). On migration, they are updated from the enums in code. There are many different approaches. Using an enum decorated with the [Flags] attribute is the most performant way if you want to model finely-grained permissions and you need less than 64 (the max if using long). I chose not to do this because if you need less than 64, this method is very well understood and many examples exist of how to make such a system. This is a bit harder but also allows for a larger amount of permissions.

Some people also go about making a CRUD-based system with these flags. Something like Aircraft - 0101 where each bit is a letter of CRUD. My problem with this is that realistically not all protected things are resources. Not all operations fall under CRUD. However, realistically this section is what needs to be customized most. As the creators of PolicyServer and IdentityServer have pointed out, authentication is easy to make for everyone, authorization is a very case-by-case basis where a custom system is needed and the requirements may widely vary. 

Regardless, using the existing authorization within ASP.NET Core and gRPC leads to a fast, reusable and low-code solution.

#### User-defined roles
The last feature of this project is the ability for admins to make their own roles and assign them to their users. Global roles are provided as suggested defaults. This is likely overkill for most projects but is another interesting feature for more complicated designs.

# Setup
#### Email Configuration (Dummy values may be used if email activity undesired)
Email configuration is stored in user secrets and injected via the IOptions<TOptions> interface.
To set the email configuration, sign up for a free account with SendGrid and enter your credentials into user secrets:
```
$ cd src/CoreMultiTenancy.Identity
$ dotnet user-secrets set "Email:SendGridUser" "<your_username>"
$ dotnet user-secrets set "Email:SendGridKey" "<your_key>"
```

#### Demo databases

Each database needs to be updated prior to launch. Demo seed data is applied through migrations when the database is made.
```
$ cd src/CoreMultiTenancy.Identity
$ dotnet ef database update

$ cd src/CoreMultiTenancy.Api
$ dotnet ef database update
```

The database has a seeded default user with the following email & password combination

admin@mydomain.com
password

#### Startup
the tracked .vscode folder contains json files to launch all 3 projects at once. Navigate to the
debug tab in vscode and select the "Api, Idp, and Client" selection, then click on the Run button. For TS debugging of the Angular client, select the localhost option to launch an attached browser that allows debugging of the Angular application in vscode.
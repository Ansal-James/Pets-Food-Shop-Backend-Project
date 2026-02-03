I have built the backend of a Pet Food Shop E-commerce application using ASP.NET Core Web API and Clean Architecture.

The project is divided into four layers:

API layer for controllers and middleware,

Application layer for DTOs, interfaces, and custom exceptions,

Domain layer for core entities and enums,

Infrastructure layer for database and service implementations.

This separation makes the project clean, maintainable, and scalable.

Currently, I have implemented the Authentication module.
It supports User Registration, User Login, and Admin Login.

In the Domain layer, I created a User entity using GUID as the primary key, and a UserRole enum for User and Admin.
The User entity stores username, email, phone number, password hash, role, block status, and refresh token information.

In the Application layer, I defined DTOs like SignUpDto, LoginDto, LoginResponseDto, and RefreshTokenDto, and interfaces for AuthService and TokenService.
I also added custom exceptions to handle errors like user already exists, unauthorized access, and invalid input.

In the Infrastructure layer, I implemented AuthService and TokenService.
AuthService handles registration, login, admin login, password hashing, user blocking check, and refresh token generation.
TokenService generates JWT access tokens and refresh tokens.

For security, I implemented JWT authentication with refresh token support.
When a user logs in, the API returns both an access token and a refresh token.
When the access token expires, the refresh token endpoint generates a new access token without forcing the user to log in again.

In the API layer, I created UserController and AdminController.
UserController has endpoints for signup, login, and refresh token.
AdminController has a separate login endpoint for admin users.

I also implemented a global Exception Middleware, which catches all exceptions and converts them into proper HTTP status codes with clean JSON responses.

Swagger is configured with JWT authorization, so I can test secured endpoints directly using the Authorize button.

Currently, only the authentication module is completed.
Next, I will implement Category, Product, Cart, Wishlist, Order, and Admin management modules.

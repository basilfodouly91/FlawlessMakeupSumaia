# Flawless Makeup Sumaia

A full-stack e-commerce website for premium beauty products, built with ASP.NET Core 8.0 Web API backend and Angular 20 frontend.

## Features

### Frontend (Angular 20)
- **Modern UI/UX**: Pink-themed design with responsive layout
- **Multi-language Support**: Arabic (RTL) and English with dynamic language switching
- **Product Catalog**: Browse products by categories with filtering and search
- **Shopping Cart**: Add/remove items, quantity management
- **User Authentication**: Login/Register with JWT tokens
- **Admin Panel**: Complete CRUD operations for products and categories
- **Responsive Design**: Mobile-first approach with Bootstrap 5

### Backend (ASP.NET Core 8.0)
- **RESTful API**: Clean API design with proper HTTP methods
- **Entity Framework Core**: SQLite database with code-first migrations
- **JWT Authentication**: Secure token-based authentication
- **Identity Management**: ASP.NET Core Identity for user management
- **Service Layer**: Clean architecture with dependency injection
- **CORS Support**: Cross-origin resource sharing for frontend integration

## Technology Stack

### Backend
- ASP.NET Core 8.0 Web API
- Entity Framework Core
- SQLite Database
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger/OpenAPI

### Frontend
- Angular 20
- Bootstrap 5
- RxJS
- @ngx-translate (i18n)
- TypeScript

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- Visual Studio 2022 (recommended) or Visual Studio Code

### Option 1: Visual Studio 2022 (Recommended)

1. **Clone the repository**:
   ```bash
   git clone https://github.com/basilfodouly91/FlawlessMakeupSumaia.git
   ```

2. **Open the solution**:
   - Open `FlawlessMakeupSumaia.sln` in Visual Studio 2022
   - This will load the entire project structure

3. **Set up the database**:
   - Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
   - Run: `Update-Database`
   - This creates the SQLite database and seeds it with sample data

4. **Run the backend**:
   - Set `FlawlessMakeupSumaia.API` as the startup project
   - Press **F5** or click **"Start Debugging"**
   - API will run on `http://localhost:5001`

5. **Run the frontend**:
   - Open **Terminal** in Visual Studio (View → Terminal)
   - Navigate to client: `cd FlawlessMakeupSumaia.Client`
   - Install dependencies: `npm install`
   - Start development server: `npm start`
   - Frontend will run on `http://localhost:4200`

### Option 2: Command Line Setup

### Backend Setup

1. Navigate to the API directory:
```bash
cd FlawlessMakeupSumaia.API
```

2. Restore packages:
```bash
dotnet restore
```

3. Update database:
```bash
dotnet ef database update
```

4. Run the API:
```bash
dotnet run --urls "http://localhost:5001"
```

The API will be available at `http://localhost:5001`

### Frontend Setup

1. Navigate to the client directory:
```bash
cd FlawlessMakeupSumaia.Client
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm start
```

The application will be available at `http://localhost:4200`

## Default Admin Credentials

- **Username**: admin
- **Email**: admin@flawlessmakeup.com
- **Password**: admin

## Project Structure

```
FlawlessMakeupSumaia/
├── FlawlessMakeupSumaia.API/          # Backend API
│   ├── Controllers/                    # API Controllers
│   ├── Data/                          # Database context and seeding
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Models/                        # Entity models
│   └── Services/                      # Business logic services
├── FlawlessMakeupSumaia.Client/       # Frontend Angular app
│   ├── src/
│   │   ├── app/
│   │   │   ├── admin/                 # Admin panel components
│   │   │   ├── components/            # Reusable components
│   │   │   ├── pages/                 # Page components
│   │   │   ├── services/              # Angular services
│   │   │   └── models/                # TypeScript models
│   │   └── assets/
│   │       └── i18n/                  # Translation files
│   └── dist/                          # Build output
└── README.md
```

## Features Overview

### Product Management
- View all products with pagination
- Filter by category, brand, price range
- Search functionality
- Product details with image gallery
- Stock management

### Category Management
- Browse products by category
- Category-based filtering
- Admin category management

### Shopping Cart
- Add/remove products
- Quantity adjustment
- Price calculation
- Persistent cart state

### Admin Panel
- Product CRUD operations
- Category management
- User management
- Order management
- Dashboard with analytics

### Internationalization
- Arabic (RTL) and English support
- Dynamic language switching
- Localized content
- RTL layout support

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product (Admin)
- `PUT /api/products/{id}` - Update product (Admin)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create category (Admin)
- `PUT /api/categories/{id}` - Update category (Admin)
- `DELETE /api/categories/{id}` - Delete category (Admin)

### Cart
- `GET /api/cart` - Get user cart
- `POST /api/cart/add` - Add item to cart
- `PUT /api/cart/update` - Update cart item
- `DELETE /api/cart/remove/{id}` - Remove item from cart

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Contact

For questions or support, please contact the development team.

---

**Flawless Makeup Sumaia** - Premium Beauty Products E-commerce Platform
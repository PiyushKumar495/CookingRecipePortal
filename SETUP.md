# Recipe Portal Setup Guide

## Quick Start

1. **Run the startup script:**
   ```bash
   start-app.bat
   ```

2. **Access the application:**
   - Frontend: http://localhost:4200
   - Backend API: https://localhost:7297

## Manual Setup

### Backend Setup
1. Navigate to backend directory:
   ```bash
   cd CookingRecipePortal
   ```

2. Update database:
   ```bash
   dotnet ef database update
   ```

3. Run backend:
   ```bash
   dotnet run
   ```

### Frontend Setup
1. Navigate to frontend directory:
   ```bash
   cd recipe-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Run frontend:
   ```bash
   ng serve
   ```

## Demo Credentials
- **Email:** rajesh@example.com
- **Password:** User123!

## Features Fixed

### Backend-Frontend Integration
- ✅ Fixed API base URL to match backend port (7297)
- ✅ Updated models to match backend DTOs exactly
- ✅ Fixed authentication response handling
- ✅ Corrected recipe and feedback data structures
- ✅ Fixed profile API endpoints

### Model Alignment
- ✅ Recipe model matches RecipeDetailsDto
- ✅ User model matches UserDto
- ✅ Feedback model matches FeedbackDetailsDto
- ✅ Register request matches RegisterDto
- ✅ Login response matches LoginResponse

### Component Fixes
- ✅ Home component displays recipes correctly
- ✅ Search component works with backend filter endpoint
- ✅ Profile component uses correct API methods
- ✅ Recipe detail component handles feedback properly
- ✅ Register component includes all required fields

### UI/UX Improvements
- ✅ Material Design components properly imported
- ✅ Responsive design for all screen sizes
- ✅ Proper error handling and validation
- ✅ Loading states and user feedback
- ✅ Beautiful food images from Unsplash

## Application Structure

### Backend (.NET 8)
- Controllers: Auth, Recipe, Feedback, Profile
- Models: User, Recipe, Feedback
- DTOs: Login, Register, Recipe, Feedback
- Repository pattern with Entity Framework

### Frontend (Angular 18)
- Components: Home, Search, Create Recipe, Profile, Login, Register
- Services: Auth, API
- Material Design UI with responsive layout
- Standalone components architecture

## Database Schema
- Users: Authentication and profile data
- Recipes: Recipe information with user relationships
- Feedback: Ratings and comments for recipes

The application is now fully functional with proper backend-frontend integration!
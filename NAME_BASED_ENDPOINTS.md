# Recipe API - Name-Based Endpoints

## Summary
I've successfully converted your Recipe API to use name-based endpoints exclusively. All ID-based endpoints have been removed since people naturally remember recipe names, not IDs.

## Available Endpoints

### 1. GET Recipe by Name
**Endpoint:** `GET /api/Recipe/{recipeName}`
- **Description:** Get a specific recipe by its name
- **Access:** Public (AllowAnonymous)
- **Example:** `GET /api/Recipe/Chicken Curry`

### 2. UPDATE Recipe by Name
**Endpoint:** `PUT /api/Recipe/{recipeName}`
- **Description:** Update a recipe using its name (only owner can update)
- **Access:** Authenticated users only
- **Body:** RecipeDto object
- **Example:** `PUT /api/Recipe/Chicken Curry`

### 3. DELETE Recipe by Name
**Endpoint:** `DELETE /api/Recipe/{recipeName}`
- **Description:** Delete a recipe using its name (only owner can delete)
- **Access:** Authenticated users only
- **Example:** `DELETE /api/Recipe/Chicken Curry`

### 4. DELETE Recipe by Name (Admin)
**Endpoint:** `DELETE /api/Recipe/admin/{recipeName}`
- **Description:** Admin can delete any recipe by name
- **Access:** Admin role only
- **Example:** `DELETE /api/Recipe/admin/Chicken Curry`

### 5. GET Recipes by User Name
**Endpoint:** `GET /api/Recipe/user/{userName}`
- **Description:** Get all recipes created by a specific user (by username)
- **Access:** Authenticated users only
- **Example:** `GET /api/Recipe/user/john_doe`

### 6. Other Endpoints (Unchanged)
- `POST /api/Recipe` - Create new recipe
- `GET /api/Recipe` - Get all recipes (public)
- `GET /api/Recipe/filter` - Filter recipes by name, category, region

## Key Features

### Case-Insensitive Search
All name-based searches are case-insensitive, so:
- `chicken curry` = `Chicken Curry` = `CHICKEN CURRY`

### User Ownership Validation
- Users can only update/delete their own recipes when using name-based endpoints
- Admin can delete any recipe regardless of ownership

### Error Handling
- Returns `404 Not Found` if recipe doesn't exist
- Returns `404 Not Found` if user tries to access/modify someone else's recipe
- Returns `500 Internal Server Error` for database/server issues

## Usage Examples

### Get Recipe by Name
```http
GET /api/Recipe/Butter Chicken
Authorization: Bearer {your-jwt-token}
```

### Update Recipe by Name
```http
PUT /api/Recipe/Butter Chicken
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "title": "Butter Chicken Deluxe",
  "description": "Updated description",
  "ingredients": "Updated ingredients",
  "instructions": "Updated instructions",
  "isVegetarian": false,
  "region": "North Indian",
  "imageUrl": "https://example.com/image.jpg"
}
```

### Delete Recipe by Name
```http
DELETE /api/Recipe/Butter Chicken
Authorization: Bearer {your-jwt-token}
```

### Get All Recipes by User Name
```http
GET /api/Recipe/user/chef_john
Authorization: Bearer {your-jwt-token}
```

## Notes
- Recipe names should be URL-encoded if they contain special characters or spaces
- The system will match the first recipe found with the given name
- For better uniqueness, consider using a combination of name and user in your application logic
export interface Recipe {
  recipeId: number;
  title: string;
  description?: string;
  ingredients: string;
  instructions: string;
  imageUrl?: string;
  isVegetarian: boolean;
  region?: string;
  createdAt?: Date;
  userId: number;
  user?: UserDto;
  feedbacks?: FeedbackDetailsDto[];
}

export interface UserDto {
  userId: number;
  username: string;
  email?: string;
}

export interface User {
  userId: number;
  name: string;
  email: string;
  role: string;
  bio?: string;
  createdAt?: Date;
}

export interface Feedback {
  feedbackId: number;
  rating?: number;
  comment?: string;
  createdAt?: Date;
  userId: number;
  recipeId: number;
  user?: User;
  recipe?: Recipe;
}

export interface FeedbackDetailsDto {
  feedbackId: number;
  rating?: number;
  comment?: string;
  createdAt?: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  name: string;
  role: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
  bio?: string;
}

export interface CreateRecipeRequest {
  title: string;
  description?: string;
  ingredients: string;
  instructions: string;
  imageUrl?: string;
  isVegetarian: boolean;
  region?: string;
}

export interface FeedbackRequest {
  recipeName: string;
  userName: string;
  rating: number;
  comment?: string;
}
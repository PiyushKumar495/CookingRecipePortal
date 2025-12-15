import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recipe, LoginRequest, LoginResponse, Feedback } from '../models/recipe.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5141/api';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }

  // Auth
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/Auth/login`, credentials);
  }

  // Recipes
  getAllRecipes(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.baseUrl}/Recipe`);
  }

  getRecipeByName(recipeName: string): Observable<Recipe> {
    return this.http.get<Recipe>(`${this.baseUrl}/Recipe/${encodeURIComponent(recipeName)}`);
  }

  getUserRecipes(userName: string): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.baseUrl}/Recipe/user/${encodeURIComponent(userName)}`, { headers: this.getHeaders() });
  }

  createRecipe(recipe: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Recipe`, recipe, { headers: this.getHeaders() });
  }

  // Feedback
  getFeedbackForRecipe(recipeName: string): Observable<Feedback[]> {
    return this.http.get<Feedback[]>(`${this.baseUrl}/Feedback/recipe/${recipeName}`);
  }

  addFeedback(feedback: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Feedback`, feedback, { headers: this.getHeaders() });
  }

  updateFeedback(feedbackId: number, feedback: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Feedback/${feedbackId}`, feedback, { headers: this.getHeaders() });
  }

  deleteFeedback(feedbackId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Feedback/${feedbackId}`, { headers: this.getHeaders() });
  }

  getUserFeedbacks(userName: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Feedback/user/${encodeURIComponent(userName)}`, { headers: this.getHeaders() });
  }

  // Search
  searchRecipes(query: string): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.baseUrl}/Recipe/filter?name=${encodeURIComponent(query)}`);
  }

  // Profile
  getUserProfile(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Profile`, { headers: this.getHeaders() });
  }

  updateProfile(profile: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Profile`, profile, { headers: this.getHeaders() });
  }

  // Admin
  deleteFeedbackAsAdmin(feedbackId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Feedback/admin/${feedbackId}`, { headers: this.getHeaders() });
  }

  deleteRecipeAsAdmin(recipeName: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Recipe/admin/${encodeURIComponent(recipeName)}`, { headers: this.getHeaders() });
  }

  updateRecipe(recipeName: string, recipe: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Recipe/${encodeURIComponent(recipeName)}`, recipe, { headers: this.getHeaders() });
  }

  deleteRecipe(recipeName: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Recipe/${encodeURIComponent(recipeName)}`, { headers: this.getHeaders() });
  }
}
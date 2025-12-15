import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { LoginComponent } from '../login/login.component';
import { RecipeDetailComponent } from '../recipe-detail/recipe-detail.component';
import { Recipe, Feedback } from '../../models/recipe.model';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  recipes: Recipe[] = [];
  filteredRecipes: Recipe[] = [];
  selectedFilter = 'all';
  isLoading = false;
  isLoggedIn = false;

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = !!user;
    });
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.isLoading = true;
    this.apiService.getAllRecipes().subscribe({
      next: (recipes) => {
        this.recipes = recipes;
        this.filteredRecipes = recipes;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading recipes:', error);
        this.isLoading = false;
      }
    });
  }

  filterRecipes(filter: string): void {
    this.selectedFilter = filter;
    
    switch (filter) {
      case 'vegetarian':
        this.filteredRecipes = this.recipes.filter(r => r.isVegetarian);
        break;
      case 'north-indian':
        this.filteredRecipes = this.recipes.filter(r => r.region?.toLowerCase().includes('north'));
        break;
      case 'south-indian':
        this.filteredRecipes = this.recipes.filter(r => r.region?.toLowerCase().includes('south'));
        break;
      default:
        this.filteredRecipes = this.recipes;
    }
  }

  openLoginDialog(): void {
    this.dialog.open(LoginComponent, {
      width: '400px'
    });
  }

  openRecipeDetail(recipe: Recipe): void {
    this.dialog.open(RecipeDetailComponent, {
      width: '95vw',
      maxWidth: '1200px',
      height: '90vh',
      maxHeight: '800px',
      data: recipe
    });
  }

  getDefaultImage(recipe: Recipe): string {
    const foodImages = [
      'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=400&h=300&fit=crop',
      'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400&h=300&fit=crop',
      'https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=400&h=300&fit=crop',
      'https://images.unsplash.com/photo-1574484284002-952d92456975?w=400&h=300&fit=crop',
      'https://images.unsplash.com/photo-1563379091339-03246963d51a?w=400&h=300&fit=crop'
    ];
    return foodImages[recipe.recipeId % foodImages.length];
  }

  getAverageRating(feedbacks: any[]): number {
    if (!feedbacks || feedbacks.length === 0) return 0;
    const sum = feedbacks.reduce((acc, feedback) => acc + (feedback.rating || 0), 0);
    return Math.round((sum / feedbacks.length) * 10) / 10;
  }
}
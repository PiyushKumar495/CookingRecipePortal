import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatTabsModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  allRecipes: any[] = [];
  allFeedbacks: any[] = [];
  isLoading = false;
  displayedColumns: string[] = ['recipe', 'user', 'rating', 'comment', 'date', 'actions'];
  recipeColumns: string[] = ['title', 'author', 'region', 'vegetarian', 'date', 'actions'];

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAdmin()) {
      this.router.navigate(['/home']);
      return;
    }
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.apiService.getAllRecipes().subscribe({
      next: (recipes) => {
        this.allRecipes = recipes;
        this.extractFeedbacks();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading data:', error);
        this.isLoading = false;
      }
    });
  }

  extractFeedbacks(): void {
    this.allFeedbacks = [];
    this.allRecipes.forEach(recipe => {
      if (recipe.feedbacks && recipe.feedbacks.length > 0) {
        recipe.feedbacks.forEach((feedback: any) => {
          this.allFeedbacks.push({
            ...feedback,
            recipeName: recipe.title,
            recipeUser: recipe.user?.username || 'Unknown'
          });
        });
      }
    });
  }

  deleteFeedback(feedbackId: number): void {
    if (confirm('Are you sure you want to delete this feedback?')) {
      this.apiService.deleteFeedbackAsAdmin(feedbackId).subscribe({
        next: () => {
          this.loadData();
        },
        error: (error) => {
          console.error('Error deleting feedback:', error);
        }
      });
    }
  }

  deleteRecipe(recipeName: string): void {
    if (confirm('Are you sure you want to delete this recipe? This will also delete all associated feedbacks.')) {
      this.apiService.deleteRecipeAsAdmin(recipeName).subscribe({
        next: () => {
          this.loadData();
        },
        error: (error) => {
          console.error('Error deleting recipe:', error);
        }
      });
    }
  }

  getUniqueUsersCount(): number {
    const uniqueUsers = new Set();
    this.allRecipes.forEach(recipe => {
      if (recipe.user?.username) {
        uniqueUsers.add(recipe.user.username);
      }
    });
    return uniqueUsers.size;
  }
}
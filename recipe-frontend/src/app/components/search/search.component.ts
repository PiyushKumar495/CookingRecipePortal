import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApiService } from '../../services/api.service';
import { RecipeDetailComponent } from '../recipe-detail/recipe-detail.component';
import { Recipe, Feedback } from '../../models/recipe.model';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent {
  searchQuery = '';
  searchResults: Recipe[] = [];
  isLoading = false;
  hasSearched = false;

  constructor(
    private apiService: ApiService,
    private dialog: MatDialog
  ) {}

  searchRecipes(): void {
    if (!this.searchQuery.trim()) return;
    
    this.isLoading = true;
    this.hasSearched = true;
    
    this.apiService.searchRecipes(this.searchQuery).subscribe({
      next: (recipes) => {
        this.searchResults = recipes;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Search error:', error);
        this.searchResults = [];
        this.isLoading = false;
      }
    });
  }

  searchWithQuery(query: string): void {
    this.searchQuery = query;
    this.searchRecipes();
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
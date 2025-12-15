import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule, MatTabGroup } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { RecipeDetailComponent } from '../recipe-detail/recipe-detail.component';
import { Recipe, User, LoginResponse, Feedback } from '../../models/recipe.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    FormsModule,
    MatSelectModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  @ViewChild(MatTabGroup) tabGroup!: MatTabGroup;
  currentUser: LoginResponse | null = null;
  userProfile: User | null = null;
  userRecipes: Recipe[] = [];
  profileForm: FormGroup;
  isLoading = false;
  isUpdating = false;
  updateMessage = '';
  updateSuccess = false;
  totalRatings = 0;
  averageRating = 0;
  isAdmin = false;
  userFeedbacks: any[] = [];
  editingFeedback: any = null;

  constructor(
    private authService: AuthService,
    private apiService: ApiService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private router: Router
  ) {
    this.profileForm = this.fb.group({
      name: ['', [Validators.required]],
      email: [''],
      bio: ['']
    });
  }

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/home']);
      return;
    }
    
    this.isAdmin = this.authService.isAdmin();
    this.loadUserProfile();
    if (!this.isAdmin) {
      this.loadUserRecipes();
      this.loadUserFeedbacks();
    }
  }

  loadUserProfile(): void {
    if (!this.currentUser) return;
    
    this.isLoading = true;
    this.apiService.getUserProfile().subscribe({
      next: (profile) => {
        this.userProfile = profile;
        this.profileForm.patchValue({
          name: profile.name,
          email: profile.email,
          bio: profile.bio || ''
        });
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading profile:', error);
        this.isLoading = false;
      }
    });
  }

  loadUserRecipes(): void {
    if (!this.currentUser) return;
    
    this.apiService.getUserRecipes(this.currentUser.name).subscribe({
      next: (recipes) => {
        this.userRecipes = recipes;
        this.calculateStats();
      },
      error: (error) => {
        console.error('Error loading user recipes:', error);
        this.userRecipes = [];
        this.calculateStats();
      }
    });
  }

  calculateStats(): void {
    let totalRatings = 0;
    let totalScore = 0;
    
    this.userRecipes.forEach(recipe => {
      if (recipe.feedbacks && recipe.feedbacks.length > 0) {
        totalRatings += recipe.feedbacks.length;
        totalScore += recipe.feedbacks.reduce((sum: number, feedback: any) => sum + (feedback.rating || 0), 0);
      }
    });
    
    this.totalRatings = totalRatings;
    this.averageRating = totalRatings > 0 ? Math.round((totalScore / totalRatings) * 10) / 10 : 0;
  }

  editProfile(): void {
    if (this.tabGroup) {
      this.tabGroup.selectedIndex = 1; // Switch to Settings tab (index 1)
    }
  }

  updateProfile(): void {
    if (this.profileForm.valid && this.currentUser) {
      this.isUpdating = true;
      this.updateMessage = '';
      
      const profileData = {
        name: this.profileForm.value.name,
        bio: this.profileForm.value.bio
      };
      
      this.apiService.updateProfile(profileData).subscribe({
        next: (updatedProfile) => {
          this.userProfile = updatedProfile;
          this.isUpdating = false;
          this.updateMessage = 'Profile updated successfully!';
          this.updateSuccess = true;
          
          setTimeout(() => {
            this.updateMessage = '';
          }, 3000);
        },
        error: (error) => {
          this.isUpdating = false;
          this.updateMessage = error.error?.message || 'Failed to update profile. Please try again.';
          this.updateSuccess = false;
        }
      });
    }
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

  editRecipe(recipe: Recipe): void {
    // Navigate to edit recipe page or open edit dialog
    this.router.navigate(['/create-recipe'], { queryParams: { edit: recipe.title } });
  }

  deleteRecipe(recipe: Recipe): void {
    if (confirm(`Are you sure you want to delete "${recipe.title}"? This action cannot be undone.`)) {
      this.apiService.deleteRecipe(recipe.title).subscribe({
        next: () => {
          this.loadUserRecipes(); // Reload recipes
        },
        error: (error) => {
          console.error('Error deleting recipe:', error);
        }
      });
    }
  }

  loadUserFeedbacks(): void {
    if (!this.currentUser) return;
    
    this.apiService.getUserFeedbacks(this.currentUser.name).subscribe({
      next: (feedbacks) => {
        this.userFeedbacks = feedbacks;
      },
      error: (error) => {
        console.error('Error loading user feedbacks:', error);
        this.userFeedbacks = [];
      }
    });
  }

  editFeedback(feedback: any): void {
    this.editingFeedback = { ...feedback };
  }

  updateFeedback(): void {
    if (this.editingFeedback) {
      const updateData = {
        recipeName: this.editingFeedback.recipeTitle,
        rating: this.editingFeedback.rating,
        comment: this.editingFeedback.comment
      };
      
      this.apiService.updateFeedback(this.editingFeedback.feedbackId, updateData).subscribe({
        next: () => {
          this.loadUserFeedbacks();
          this.editingFeedback = null;
        },
        error: (error) => {
          console.error('Error updating feedback:', error);
        }
      });
    }
  }

  deleteFeedback(feedback: any): void {
    if (confirm(`Are you sure you want to delete your review for "${feedback.recipeTitle}"?`)) {
      this.apiService.deleteFeedback(feedback.feedbackId).subscribe({
        next: () => {
          this.loadUserFeedbacks();
        },
        error: (error) => {
          console.error('Error deleting feedback:', error);
        }
      });
    }
  }

  cancelEdit(): void {
    this.editingFeedback = null;
  }
}
import { Component, OnInit, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Recipe, Feedback } from '../../models/recipe.model';
import { LoginComponent } from '../login/login.component';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './recipe-detail.component.html',
  styleUrls: ['./recipe-detail.component.scss']
})
export class RecipeDetailComponent implements OnInit {
  recipe: Recipe;
  feedbacks: Feedback[] = [];
  feedbackForm: FormGroup;
  selectedRating = 0;
  isLoggedIn = false;

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<RecipeDetailComponent>,
    private apiService: ApiService,
    @Inject(MAT_DIALOG_DATA) public data: Recipe
  ) {
    this.recipe = data;
    this.feedbackForm = this.fb.group({
      comment: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  ngOnInit() {
    this.checkLoginStatus();
    this.loadSampleFeedbacks();
  }

  checkLoginStatus() {
    this.isLoggedIn = !!localStorage.getItem('token');
  }

  loadSampleFeedbacks() {
    this.apiService.getFeedbackForRecipe(this.recipe.title).subscribe({
      next: (feedbacks) => {
        this.feedbacks = feedbacks;
      },
      error: (error) => {
        console.error('Error loading feedbacks:', error);
        this.feedbacks = [];
      }
    });
  }

  getIngredientsList(): string[] {
    return this.recipe.ingredients.split(',').map(item => item.trim());
  }

  getInstructionsList(): string[] {
    return this.recipe.instructions.split(/\d+\./).filter(item => item.trim()).map(item => item.trim());
  }

  setRating(rating: number) {
    this.selectedRating = rating;
  }

  submitFeedback() {
    if (!this.isLoggedIn) {
      this.openLogin();
      return;
    }
    
    if (this.feedbackForm.valid && this.selectedRating > 0) {
      const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
      const token = localStorage.getItem('token');
      
      if (!token || !user.name) {
        console.error('User not properly authenticated');
        this.openLogin();
        return;
      }
      
      const feedbackData = {
        recipeName: this.recipe.title,
        rating: this.selectedRating,
        comment: this.feedbackForm.value.comment
      };
      
      console.log('Submitting feedback:', feedbackData);
      console.log('Token:', token);
      
      this.apiService.addFeedback(feedbackData).subscribe({
        next: () => {
          this.feedbackForm.reset();
          this.selectedRating = 0;
          this.loadSampleFeedbacks(); // Reload feedbacks from backend
        },
        error: (error) => {
          console.error('Error submitting feedback:', error);
          console.error('Full error:', error);
        }
      });
    }
  }

  openLogin() {
    const loginDialog = this.dialog.open(LoginComponent, {
      width: '450px',
      disableClose: false
    });

    loginDialog.afterClosed().subscribe(result => {
      if (result) {
        this.checkLoginStatus();
      }
    });
  }

  goBack() {
    this.dialogRef.close();
  }
}
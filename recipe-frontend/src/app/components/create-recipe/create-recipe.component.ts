import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { CreateRecipeRequest } from '../../models/recipe.model';

@Component({
  selector: 'app-create-recipe',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './create-recipe.component.html',
  styleUrls: ['./create-recipe.component.scss']
})
export class CreateRecipeComponent implements OnInit {
  recipeForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  originalRecipeName = '';

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.recipeForm = this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      region: [''],
      isVegetarian: [false],
      imageUrl: [''],
      ingredients: ['', [Validators.required]],
      instructions: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
      return;
    }
    
    this.route.queryParams.subscribe(params => {
      if (params['edit']) {
        this.isEditMode = true;
        this.originalRecipeName = params['edit'];
        this.loadRecipeForEdit(params['edit']);
      }
    });
  }

  onSubmit(): void {
    if (this.recipeForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';
      
      const recipeData: CreateRecipeRequest = this.recipeForm.value;
      
      if (this.isEditMode) {
        this.apiService.updateRecipe(this.originalRecipeName, recipeData).subscribe({
          next: (response) => {
            this.isLoading = false;
            this.successMessage = 'Recipe updated successfully!';
            setTimeout(() => {
              this.router.navigate(['/profile']);
            }, 2000);
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.error?.message || 'Failed to update recipe. Please try again.';
          }
        });
      } else {
        this.apiService.createRecipe(recipeData).subscribe({
          next: (response) => {
            this.isLoading = false;
            this.successMessage = 'Recipe created successfully!';
            setTimeout(() => {
              this.router.navigate(['/home']);
            }, 2000);
          },
          error: (error) => {
            this.isLoading = false;
            this.errorMessage = error.error?.message || 'Failed to create recipe. Please try again.';
          }
        });
      }
    }
  }

  loadRecipeForEdit(recipeName: string): void {
    this.isLoading = true;
    this.apiService.getRecipeByName(recipeName).subscribe({
      next: (recipe) => {
        this.recipeForm.patchValue({
          title: recipe.title,
          description: recipe.description,
          region: recipe.region,
          isVegetarian: recipe.isVegetarian,
          imageUrl: recipe.imageUrl,
          ingredients: recipe.ingredients,
          instructions: recipe.instructions
        });
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load recipe for editing.';
        console.error('Error loading recipe:', error);
      }
    });
  }
}
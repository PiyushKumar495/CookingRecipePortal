import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { CreateRecipeComponent } from './components/create-recipe/create-recipe.component';
import { SearchComponent } from './components/search/search.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AdminComponent } from './components/admin/admin.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'create-recipe', component: CreateRecipeComponent },
  { path: 'search', component: SearchComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'admin', component: AdminComponent },
  { path: '**', redirectTo: '/home' }
];
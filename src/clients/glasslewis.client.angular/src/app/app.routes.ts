// app.routes.ts
import { Routes } from '@angular/router';
import { MsalGuard, MsalRedirectComponent } from '@azure/msal-angular';
import { CompaniesListComponent } from './components/companies-list/companies-list.component';
import { GetCompanyComponent } from './components/get-company/get-company.component';
import { GetCompanyByIsinComponent } from './components/get-company-by-isin/get-company-by-isin.component';
import { CreateCompanyComponent } from './components/create-company/create-company.component';
import { UpdateCompanyComponent } from './components/update-company/update-company.component';
import { PatchCompanyComponent } from './components/patch-company/patch-company.component';

export const routes: Routes = [
  {
    path: '',
    component: CompaniesListComponent,
    canActivate: [MsalGuard],
  },
  {
    path: 'auth',
    component: MsalRedirectComponent,
  },
  {
    path: 'companies',
    component: CompaniesListComponent,
    canActivate: [MsalGuard],
  },
  { 
    path: 'company/create', 
    component: CreateCompanyComponent,
    canActivate: [MsalGuard],
  },
  { 
    path: 'company/update/:id',
    component: UpdateCompanyComponent,
    canActivate: [MsalGuard],
  },
  { 
    path: 'company/patch/:id',
    component: PatchCompanyComponent,
    canActivate: [MsalGuard],
  },
  { 
    path: 'company/:id', 
    component: GetCompanyComponent,
    canActivate: [MsalGuard],
  },
  {
    path: 'company/isin/:isin',
    component: GetCompanyByIsinComponent,
    canActivate: [MsalGuard],
  },
  {
    path: '**',
    component: CompaniesListComponent,
  },
];

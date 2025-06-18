import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CreateCompanyService } from '../../services/company/create-company.service';
import { CreateCompanyRequestModel } from '../../models/request/create-company-request.model';
import { CreateCompanyResponseModel } from '../../models/response/create-company-response.model';
import { Location } from '@angular/common';
import { isinValidator, urlValidator } from '../../utils/validation-utils';
@Component({
  imports: [ReactiveFormsModule, CommonModule],
  selector: 'app-create-company',
  templateUrl: './create-company.component.html',
  styleUrls: ['./create-company.component.css']
})
export class CreateCompanyComponent {
  createCompanyForm: FormGroup;
  isLoading = false;
  isSuccess = false;
  errorMessage = '';
  successMessage = '';

  fb = inject(FormBuilder);
  createCompanyService = inject(CreateCompanyService);
  location = inject(Location);

  constructor() {
    this.createCompanyForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      stockTicker: ['', [Validators.required, Validators.maxLength(10), Validators.pattern(/^[A-Z0-9.]+$/)]],
      exchange: ['', [Validators.required, Validators.maxLength(100)]],
      isin: ['', [Validators.required, isinValidator]],
      website: ['', [Validators.maxLength(500), urlValidator]]
    });
  }

  onSubmit(): void {
    if (this.createCompanyForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const createCompanyRequest: CreateCompanyRequestModel = {
        name: this.createCompanyForm.get('name')?.value,
        stockTicker: this.createCompanyForm.get('stockTicker')?.value?.toUpperCase(),
        exchange: this.createCompanyForm.get('exchange')?.value,
        isin: this.createCompanyForm.get('isin')?.value?.toUpperCase(),
        website: this.createCompanyForm.get('website')?.value || undefined
      };

      this.createCompanyService.createCompany(createCompanyRequest).subscribe({
        next: (response: CreateCompanyResponseModel) => {
          this.isLoading = false;
          this.isSuccess = true;
          this.successMessage = 'Company created successfully!';
          this.createCompanyForm.reset();
          console.log('Company created:', response);
        },
        error: (error) => {
          this.isLoading = false;
          this.isSuccess = false;
          this.errorMessage = error.error?.message || 'An error occurred while creating the company. Please try again.';
          console.error('Error creating company:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.createCompanyForm.controls).forEach(key => {
      const control = this.createCompanyForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.createCompanyForm.get(fieldName);
    if (field?.touched && field?.errors) {
      if (field.errors['required']) {
        return `${this.getFieldDisplayName(fieldName)} is required`;
      }
      if (field.errors['maxlength']) {
        return `${this.getFieldDisplayName(fieldName)} cannot exceed ${field.errors['maxlength'].requiredLength} characters`;
      }
      if (field.errors['pattern']) {
        if (fieldName === 'stockTicker') {
          return 'Stock ticker must contain only uppercase letters, numbers, and periods';
        }
      }
      if (field.errors['invalidIsin']) {
        return 'Please enter a valid ISIN code (12 characters, starting with 2 letters)';
      }
      if (field.errors['invalidUrl']) {
        return 'Please enter a valid URL (e.g., https://example.com)';
      }
    }
    return '';
  }

  private getFieldDisplayName(fieldName: string): string {
    const displayNames: Record<string, string> = {
      'name': 'Company name',
      'stockTicker': 'Stock ticker',
      'exchange': 'Exchange',
      'isin': 'ISIN',
      'website': 'Website'
    };
    return displayNames[fieldName] || fieldName;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.createCompanyForm.get(fieldName);
    return !!(field?.touched && field?.errors);
  }

  goBack(): void {
    this.location.back();
  }

  resetForm(): void {
    this.createCompanyForm.reset();
    this.errorMessage = '';
    this.successMessage = '';
    this.isSuccess = false;
  }
}
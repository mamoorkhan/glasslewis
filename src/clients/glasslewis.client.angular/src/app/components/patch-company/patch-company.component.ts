import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef, OnDestroy, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { GetCompanyService } from '../../services/company/get-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';
import { PatchCompanyService } from '../../services/company/patch-company.service';
import { PatchCompanyRequestModel } from '../../models/request/patch-company-request.model';
import { PatchCompanyResponseModel } from '../../models/response/patch-company-response.model';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { isinValidator, urlValidator } from '../../utils/validation-utils';

@Component({
  imports: [CommonModule, ReactiveFormsModule],
  selector: 'app-patch-company',
  templateUrl: './patch-company.component.html',
  styleUrls: ['./patch-company.component.css']
})
export class PatchCompanyComponent implements OnInit, OnDestroy {
  company: GetCompanyResponseModel | null = null;
  patchCompanyForm: FormGroup;
  originalFormData: Partial<GetCompanyResponseModel> = {};
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  private destroy$ = new Subject<void>();

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private getCompanyService = inject(GetCompanyService);
  private patchCompanyService = inject(PatchCompanyService);
  private cdr = inject(ChangeDetectorRef);

  constructor() { 
    this.patchCompanyForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadCompany();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Custom validator for non-empty, non-whitespace values
  private noWhitespaceValidator(control: AbstractControl): ValidationErrors | null {
    if (control.value && typeof control.value === 'string') {
      const trimmedValue = control.value.trim();
      if (trimmedValue.length === 0) {
        return { whitespace: true };
      }
    }
    return null;
  }

  // Custom validator that combines required and no-whitespace validation
  private requiredNoWhitespace(control: AbstractControl): ValidationErrors | null {
    if (!control.value || (typeof control.value === 'string' && control.value.trim().length === 0)) {
      return { requiredNoWhitespace: true };
    }
    return null;
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [this.requiredNoWhitespace, Validators.maxLength(200)]],
      stockTicker: ['', [this.requiredNoWhitespace, Validators.maxLength(10)]],
      exchange: ['', [this.requiredNoWhitespace, Validators.maxLength(100)]],
      isin: ['', [this.requiredNoWhitespace, isinValidator]],
      website: ['', [Validators.maxLength(500), urlValidator]] // Website remains optional
    });
  }

  private populateForm(data: GetCompanyResponseModel): void {
    console.log('Populating form with data:', data);
    
    const formData = {
      name: data.name || '',
      stockTicker: data.stockTicker || '',
      exchange: data.exchange || '',
      isin: data.isin || '',
      website: data.website || ''
    };

    this.patchCompanyForm.patchValue(formData);
    
    // Store original data for comparison
    this.originalFormData = { ...formData };
    
    // Mark as pristine to avoid showing validation errors for pre-filled data
    this.patchCompanyForm.markAsPristine();
    
    // Force change detection
    this.cdr.detectChanges();
    
    console.log('Form value after population:', this.patchCompanyForm.value);
  }

  private loadCompany(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const id = params['id'];

        if (!id) {
          this.errorMessage = 'Invalid company ID provided';
          return;
        }

        this.isLoading = true;
        this.errorMessage = '';
        this.company = null;

        this.getCompanyService.getCompany(id)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: (response) => {
              console.log('Company data received:', response);
              this.company = response;
              this.isLoading = false;
              this.populateForm(response);
            },
            error: (error) => {
              console.error('Error loading company:', error);
              this.isLoading = false;

              if (error.status === 404) {
                this.errorMessage = 'Company not found';
              } else if (error.status === 0) {
                this.errorMessage = 'Unable to connect to the server. Please check your connection.';
              } else {
                this.errorMessage = `Error loading company details: ${error.message || 'Unknown error'}`;
              }
            }
          });
      });
  }

  // Get only the changed fields for PATCH request
  private getChangedFields(): PatchCompanyRequestModel {
    const currentValues = this.patchCompanyForm.value;
    const changedFields: PatchCompanyRequestModel = {};

    // Compare each field with original data
    Object.keys(currentValues).forEach(key => {
      const currentValue = currentValues[key]?.trim() || '';
      const originalValue = this.originalFormData[key] || '';

      // If the value has changed from the original
      if (currentValue !== originalValue) {
        // For all fields, include the current value (even if empty to clear)
        // Send empty string to clear the field, or the new value
        changedFields[key] = currentValue || null; // Use null to explicitly clear
      }
    });

    return changedFields;
  }

  // Check if any fields have been modified
  hasChanges(): boolean {
    const changedFields = this.getChangedFields();
    return Object.keys(changedFields).length > 0;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.patchCompanyForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit(): void {
    if (!this.hasChanges()) {
      this.errorMessage = 'No changes detected. Please modify at least one field.';
      return;
    }

    if (this.patchCompanyForm.valid && this.company?.id) {
      this.isSubmitting = true;
      this.successMessage = '';
      this.errorMessage = '';

      const changedFields = this.getChangedFields();
      console.log('Patching company with changes:', changedFields);

      this.patchCompanyService.patchCompany(this.company.id, changedFields)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response: PatchCompanyResponseModel) => {
            this.isSubmitting = false;
            this.successMessage = 'Company updated successfully!';
            console.log('Company patched:', response);
            
            // Update the original data with the response
            if (response) {
              this.originalFormData = { ...this.patchCompanyForm.value };
              this.patchCompanyForm.markAsPristine();
            }
          },
          error: (error) => {
            this.isSubmitting = false;
            this.errorMessage = error.error?.message || 'An error occurred while updating the company. Please try again.';
            console.error('Error patching company:', error);
          }
        });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.patchCompanyForm.controls).forEach(key => {
        this.patchCompanyForm.get(key)?.markAsTouched();
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/companies']); // Adjust navigation as needed
  }

  onReset(): void {
    this.patchCompanyForm.patchValue(this.originalFormData);
    this.patchCompanyForm.markAsPristine();
    this.successMessage = '';
    this.errorMessage = '';
  }

  // Clear a specific field
  clearField(fieldName: string): void {
    this.patchCompanyForm.get(fieldName)?.setValue('');
    this.patchCompanyForm.get(fieldName)?.markAsDirty();
  }

  // Restore original value for a specific field
  restoreField(fieldName: string): void {
    const originalValue = this.originalFormData[fieldName] || '';
    this.patchCompanyForm.get(fieldName)?.setValue(originalValue);
    this.patchCompanyForm.get(fieldName)?.markAsPristine();
  }
}
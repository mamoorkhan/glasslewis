import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { GetCompanyService } from '../../services/company/get-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';
import { UpdateCompanyService } from '../../services/company/update-company.service';
import { UpdateCompanyRequestModel } from '../../models/request/update-company-request.model';
import { UpdateCompanyResponseModel } from '../../models/response/update-company-response.model';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { isinValidator, urlValidator } from '../../utils/validation-utils';

@Component({
  imports: [CommonModule, ReactiveFormsModule],
  selector: 'app-update-company',
  templateUrl: './update-company.component.html',
  styleUrls: ['./update-company.component.css']
})
export class UpdateCompanyComponent implements OnInit {
  company: GetCompanyResponseModel | null = null;
  updateCompanyForm: FormGroup;
  isLoading = false; // For data loading
  isSubmitting = false; // For form submission
  isSuccess = false;
  errorMessage = '';
  successMessage = '';
  private destroy$ = new Subject<void>();

  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private getCompanyService = inject(GetCompanyService);
  private updateCompanyService = inject(UpdateCompanyService);
  private location = inject(Location);

  ngOnInit(): void {
    this.updateCompanyForm = this.createForm();
    this.loadCompany();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      stockTicker: ['', [Validators.required, Validators.maxLength(10)]],
      exchange: ['', [Validators.required, Validators.maxLength(100)]],
      isin: ['', [Validators.required, isinValidator]],
      website: ['', [Validators.maxLength(500), urlValidator]]
    });
  }

  private populateForm(data: GetCompanyResponseModel): void {
    this.updateCompanyForm.patchValue({
        name: data.name,
        stockTicker: data.stockTicker,
        exchange: data.exchange,
        isin: data.isin,
        website: data.website || ''
      });
    }

  private loadCompany(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const id = params['id'];

        if (!id) {
          this.errorMessage = 'Invalid company ID provided';
          return; // Exit early if invalid ID
        }

        // Move the API call inside the params subscription
        this.isLoading = true;
        this.errorMessage = '';
        this.company = null;

        this.getCompanyService.getCompany(id)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: (response) => {
              this.company = response;
              this.isLoading = false;
              setTimeout(() => {
                this.populateForm(response);
              }, 0);
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

  isFieldInvalid(fieldName: string): boolean {
    const field = this.updateCompanyForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit(): void {
  if (this.updateCompanyForm.valid && this.company?.id) {
    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    const formValue = this.updateCompanyForm.value;
    const requestData: UpdateCompanyRequestModel = {
      name: formValue.name,
      stockTicker: formValue.stockTicker,
      exchange: formValue.exchange,
      isin: formValue.isin,
      website: formValue.website || undefined
    };
    this.updateCompanyService.editCompany(this.company?.id, requestData).subscribe({
      next: (response: UpdateCompanyResponseModel) => {
        this.isSubmitting = false;
        this.successMessage = 'Company updated successfully!';
        console.log('Company updated:', response);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'An error occurred while updating the company. Please try again.';
        console.error('Error updating company:', error);
      }
    });
  } else {
    Object.keys(this.updateCompanyForm.controls).forEach(key => {
      this.updateCompanyForm.get(key)?.markAsTouched();
    });
  }
}

  goBack(): void {
    this.location.back();
  }

  onReset(): void {
    this.updateCompanyForm.reset();
    this.successMessage = '';
    this.errorMessage = '';
    this.loadCompany();
  }
}